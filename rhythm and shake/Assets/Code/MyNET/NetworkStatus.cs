using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum NetworkEnd {client, server, none};
public enum BackInterruption {begin, pause, ready2dPlayer, score};

public class NetworkStatus : MonoBehaviour
{

    TMPro.TMP_Text te = null;

    [SerializeField]
    TMPro.TMP_Text clientStatusText;
    [SerializeField]
    TMPro.TMP_Text serverStatusText;

    SongGenerator sg;
    GamePlayCode gp;

    public static NetState networkState = NetState.start;
    /*
     * 0 bit -- begin gameplay
     * 1 bit -- pause gameplay
     * 2 bit -- show that second player is ready
     * 3 bit -- update second player's scores
    */
    public static byte backInterruptionFlags = 0;
    public static int myScore = 0;
    public static int hisScore = 0;

    public static bool myReq = false;
    public static bool hisReq = false;

    private static bool scoreInterruption = false;
    private static Mutex scoreInterruptionMutex = new Mutex();
    private static bool reqInterruption = false;
    private static Mutex reqInterruptionMutex = new Mutex();

    public static float pauseTime = -1;

    public static Song song = null;
    public static byte songParams;

    public static bool isActive = false;

    public static bool GetScoreInterruption()
    {
        return scoreInterruption;
    }

    public static bool GetReqInterruption()
    {
        return reqInterruption; 
    }

    public static void SetScoreInterruption(bool value)
    {
        scoreInterruptionMutex.WaitOne();
        scoreInterruption = value;
        scoreInterruptionMutex.ReleaseMutex();
    }

    public static void SetReqInterruption(bool value)
    {
        reqInterruptionMutex.WaitOne();
        reqInterruption = value;
        reqInterruptionMutex.ReleaseMutex();
    }


    public static string targetScene = "";
    public static string coroutineScene = "";
    public static string status = "";

    public static NetworkEnd networkEnd = NetworkEnd.none;

    NetworkSupport networkSupport = null;

    string multiplayerGameplaySceneName = "Lobby";

    AsyncOperation asyncGPSceneLoad;

    public static double lastSentTime = -1;
    public static double precision = 10;
    public static double globalDelta = 0;
    public static System.Diagnostics.Stopwatch sw;

    public static double plannedTimeToStart;
    public static bool isRunning = false;

    bool enteredLobby = false;
    bool startedPlaying = false;

    private void Awake()
    {
        sw = System.Diagnostics.Stopwatch.StartNew();
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        song = null;
    }

    void Update()
    {
        if (status != "")
        {
            if (networkEnd == NetworkEnd.client && clientStatusText != null)
            {
                clientStatusText.text = status;
                status = "";
            }
            if (networkEnd == NetworkEnd.server && serverStatusText != null)
            {
                serverStatusText.text = status;
                status = "";
            }
        }
        if (!isActive)
        {
            //MAY BE DO THIS OUTSIDE THE SCOPE AND GET RID OF IS ACTIVE?
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName != "Lobby" && currentSceneName != "SampleScene")
            {
                Destroy(gameObject);
            }            
        }
        else
        {
            if (enteredLobby == false && networkState == NetState.pause)
            {
                if (networkEnd == NetworkEnd.server || (networkEnd == NetworkEnd.client && song != null && song.audio != null))
                {
                    ActivatePauseButtons();
                    enteredLobby = true;
                }
            }
            if (networkState == NetState.run && !isRunning && sw.ElapsedMilliseconds >= plannedTimeToStart)
            {
                ActivateRunInterface();
                //pyre test
                PlaySongSync();
            }          
            if (coroutineScene != "")
            {
                //if there will be more scenes we will do something to this code.
                StartCoroutine(LoadGameplaySceneAsync());
                targetScene = coroutineScene;
                coroutineScene = "";
            }
            if (targetScene != "" && asyncGPSceneLoad.isDone)
            {
                Debug.Log(SceneManager.GetActiveScene().name);
                if (SceneManager.GetActiveScene().name == targetScene)
                {
                    if (targetScene == multiplayerGameplaySceneName)
                    {
                        GameObject mainObj = GameObject.Find("Main Camera");
                        networkSupport = mainObj.GetComponent<NetworkSupport>();
                        sg = mainObj.GetComponent<SongGenerator>();
                        gp = mainObj.GetComponent<GamePlayCode>();
                        //change status text
                        clientStatusText = networkSupport.statusText;
                        serverStatusText = networkSupport.statusText;
                        te = networkSupport.te;
                        networkSupport.startButton.onClick.AddListener(PressReady);
                        networkSupport.pauseButton.onClick.AddListener(PressPause);
                    }
                    targetScene = "";
                }
            }
            if (te != null)
            {
                te.text = "my: " + myScore + "\this:" + hisScore + "\nnetwork status: " + networkState + "\nmy req: " + myReq + "\nhis req: " + hisReq + "\ntime:";

            }
            if (backInterruptionFlags != 0)
            {
                CheckForBackInterruptions();
            }
        }
    }

    public void CheckForBackInterruptions()
    {
        // begin gameplay
        if ((backInterruptionFlags & Requests.BACK_REQ_RUN) == Requests.BACK_REQ_RUN)
        {
            DiactivatePauseButtons();
            status = "Приготовьтесь играть...";
            backInterruptionFlags &= 0b11111110;
        }
        //pause gameplay
        if ((backInterruptionFlags & Requests.BACK_REQ_PAUSE) == Requests.BACK_REQ_PAUSE)
        {
            RecievePause();
            //sync correction
            sg.NET_CorrectSong(pauseTime);
            status = " ";
            backInterruptionFlags &= 0b11111101;
        }
        //show that second player is ready
        if ((backInterruptionFlags & Requests.BACK_REQ_SHOW_2D_READY) == Requests.BACK_REQ_SHOW_2D_READY)
        {
            RecieveReady();
            backInterruptionFlags &= 0b11111011;
        }
        //update second player's scores
        if ((backInterruptionFlags & Requests.BACK_REQ_SCORE) == Requests.BACK_REQ_SCORE)
        {
            RecieveScore();
            backInterruptionFlags &= 0b11110111;
        }
        //update second player's scores
        if ((backInterruptionFlags & Requests.BACK_REQ_UNPACK_TRACKS) == Requests.BACK_REQ_UNPACK_TRACKS)
        {
            song = new Song();
            song.audio = null;
            MyUtitities.LoadSongFromPath(ref song, Application.persistentDataPath + "/multi/transferred.json");
            if ((songParams & 0b00000001) == 1)
            {
                StartCoroutine(AudioDownloader.NET_GetAudioClip("multi/songs/transferred.mp3", AudioType.MPEG));
            }
            else
            {
                StartCoroutine(AudioDownloader.NET_GetAudioClip("multi/songs/transferred.wav", AudioType.WAV));
            }
            backInterruptionFlags &= 0b11101111;
        }
    }

    public void PressPause()
    {
        status = " ";
        ActivatePauseButtons();
        pauseTime = sg.PauseSong();
        SetReqInterruption(true);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && networkState == NetState.run)
        {
            PressPause();
        }
    }

    public void PressReady()
    {
        //may be later unpush button
        SetReqInterruption(true);
        networkSupport.IamReadyIndicator.color = Color.green;
        networkSupport.IamReadyText.text = "Вы готовы";
    }

    private void RecieveReady()
    {
        networkSupport.heIsReadyIndicator.color = Color.green;
        networkSupport.heIsReadyText.text = "Готов";
    }

    private void RecievePause()
    {
        ActivatePauseButtons();
        sg.PauseSong();
    }

    public void PressScore()
    {
        myScore += 2; 
        SetScoreInterruption(true);
        //visualize myScore
    }

    public void RecieveScore()
    {
        gp.NET_Change2dPlayerScore();
    }

    public void PlaySongSync()
    {
        isRunning = true;
        if (!startedPlaying)
        {
            sg.NET_InitSong();
            sg.StartSong();
            startedPlaying = true;
        }
        else
        {
            sg.ResumeSong();
        }
    }

    IEnumerator LoadGameplaySceneAsync()
    {
        asyncGPSceneLoad = SceneManager.LoadSceneAsync(multiplayerGameplaySceneName);
        asyncGPSceneLoad.allowSceneActivation = true;

        // Wait until the asynchronous scene fully loads
        while (!asyncGPSceneLoad.isDone)
        {

            yield return null;
        }
    }

    void ActivatePauseButtons()
    {
        networkSupport.pauseCanvas.SetActive(true);
        networkSupport.IamReadyIndicator.color = Color.grey;
        networkSupport.IamReadyText.text = "Намите, чтобы начать";
        networkSupport.heIsReadyIndicator.color = Color.grey;
        networkSupport.heIsReadyText.text = "Не готов";
        clientStatusText.gameObject.SetActive(true);
        serverStatusText.gameObject.SetActive(true);
        networkSupport.pauseButton.gameObject.SetActive(false);
        networkSupport.pauseBack.SetActive(true);
    }

    void DiactivatePauseButtons()
    {
        networkSupport.pauseCanvas.SetActive(false);
    }

    void ActivateRunInterface()
    {
        networkSupport.pauseCanvas.SetActive(false);
        clientStatusText.gameObject.SetActive(false);
        serverStatusText.gameObject.SetActive(false);
        networkSupport.pauseBack.SetActive(false);
        networkSupport.pauseButton.gameObject.SetActive(true);
    }

    public static void ReInit()
    {
        networkEnd = NetworkEnd.none;
        networkState = NetState.disconnected;
        isActive = false;
        myReq = false;
        hisReq = false;
        myScore = 0;
        hisScore = 0;
        backInterruptionFlags = 0;
        pauseTime = -1;
        targetScene = "";
        coroutineScene = "";
        status = "";
        lastSentTime = -1;
        precision = 10;
        globalDelta = 0;
    }

}
