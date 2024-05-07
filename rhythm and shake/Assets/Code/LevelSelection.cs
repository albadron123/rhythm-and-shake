using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
//using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [SerializeField]
    TransportedData transportedData;

    [SerializeField]
    SceneTransition sTransition;

    [SerializeField]
    List<Song> songs = new List<Song>();
    int currentSongIndex = 0;
    public static bool currentSongInMultiplayer = false;


    [SerializeField]
    MenuSlider mSlider;


    [SerializeField]
    TMPro.TMP_Text SongNameText;
    [SerializeField]
    TMPro.TMP_Text SongAuthorText;
    [SerializeField]
    TMPro.TMP_Text LastTryText;
    [SerializeField]
    TMPro.TMP_Text RecordText;


    const int REWARDS_COUNT = 4;
    [SerializeField]
    GameObject[] rewardPoints = new GameObject[REWARDS_COUNT];
    [SerializeField]
    int[] rewards = new int[REWARDS_COUNT];
    float[] rewardsPercents = {0.5f, 0.75f, 0.9f, 1f};



    [SerializeField]
    GameObject textPrefab;

    Transform myTransform;

    [SerializeField]
    SpriteRenderer tab1;



    [SerializeField]
    GameObject canvas;

    [SerializeField]
    GameObject divisionLine;


    [SerializeField]
    List<TabScript> tabScripts = new List<TabScript>();

    [SerializeField]
    GameObject tabPrefab;



    [SerializeField]
    GameObject playSongButton;

    AudioSource audioSource;

    bool inSelectionMenu = false;



    enum HState { straight, left, right };
    enum VState { straight, up, down };

    HState hState;
    VState vState;

    int score = 0;
    int bestScore = 0;

    [SerializeField]
    float inSideAccuracy = 0.85f;
    [SerializeField]
    float inStraightAccuracy = 0.5f;


    bool isCameraMoving = false;
    float destinationX;
    [SerializeField]
    float cameraShift;
    [SerializeField]
    float cameraVelocty;

    void Start()
    {
        myTransform = transform;

        audioSource = GetComponent<AudioSource>();

        if (!Input.gyro.enabled)
        {
            Input.gyro.enabled = true;
        }

        Screen.orientation = ScreenOrientation.Portrait;


        Vibration.Init();

        AddCustomSongs();

        //Later should expand it
        EnterMenu();
        //ViewSongData();

        PrepareMenuExit();

    }

    // Update is called once per frame
    void Update()
    {
        if(inSelectionMenu)
        { 
            Vector3 acc = Input.acceleration;

            HState prevHState = hState;
            VState prevVstate = vState;

            if (acc.x < inStraightAccuracy && hState == HState.right)
            {
                hState = HState.straight;
            }
            else if (acc.x > -inStraightAccuracy && hState == HState.left)
            {
                hState = HState.straight;
            }
            else if (acc.x > inSideAccuracy && hState == HState.straight)
            {
                hState = HState.right;
            }
            else if (acc.x < -inSideAccuracy && hState == HState.straight)
            {
                hState = HState.left;
            }

            if (acc.y < inStraightAccuracy && vState == VState.up)
            {
                vState = VState.straight;
            }
            else if (acc.y > -inStraightAccuracy && vState == VState.down)
            {
                vState = VState.straight;
            }
            else if (acc.y > inSideAccuracy && vState == VState.straight)
            {
                vState = VState.up;
            }
            else if (acc.y < -inSideAccuracy && vState == VState.straight)
            {
                vState = VState.down;
            }


            if (isCameraMoving)
            {
                myTransform.position += Vector3.right * (destinationX - myTransform.position.x) * cameraVelocty * Time.deltaTime;
                if (Mathf.Abs(myTransform.position.x - destinationX) < 0.05f)
                {
                    myTransform.position = new Vector3(destinationX, myTransform.position.y, myTransform.position.z);
                    isCameraMoving = false;
                }
            }

            //tab1Transform.rotation = Quaternion.Euler(-acc.y * 25f, -acc.x * 25f, 0);

            if (Input.GetKeyDown(KeyCode.Z))
            {
                PlaySong();
                //songGenerator.StartSong();
            }

            if (Input.touchCount > 0)
            {
                Touch[] touches = Input.touches;
                foreach (Touch t in touches)
                {
                    if (t.phase == TouchPhase.Began)
                    {
                        Vector2 pos = Camera.main.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, -9));
                        if (MyUtitities.TouchOver(pos, tabScripts[currentSongIndex].transform))
                        {
                            tabScripts[currentSongIndex].TapIncrease(true, Color.green);
                            tabScripts[currentSongIndex].TabRotate(currentSongInMultiplayer == true ? -1 : 1);
                            currentSongInMultiplayer = !currentSongInMultiplayer;
                            //here we will turn song tab around (Horray, one year passed and we are finally here!)
                            //let's do multiplayer now!
                            //but firstly  _____                
                            //     _..--'''@   @'''--.._
                            //   .'   @_/-//-\/>/>'/ @  '.
                            //  (  @  /_<//<'/----------^-)
                            //  |'._  @     //|###########|
                            //  |~  ''--..@|',|}}}}}}}}}}}|
                            //  |  ~   ~   |/ |###########|
                            //  | ~~  ~   ~|./|{{{{{{{{{{{|
                            //   '._ ~ ~ ~ |,/`````````````
                            //      ''--.~.|/
                            // a cake!!!!!!!!!!!!!!!!
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("rotating!");
                tabScripts[currentSongIndex].TabRotate(currentSongInMultiplayer == true ? -1 : 1);
                currentSongInMultiplayer = !currentSongInMultiplayer;
            }


            if (prevHState == HState.straight && hState == HState.left)
            {
                if (!isCameraMoving || Mathf.Abs(destinationX - myTransform.position.x) < (cameraShift / 4f))
                {
                    ForceChangeSong(-1);
                }
            }

            if (prevHState == HState.straight && hState == HState.right)
            {
                if (!isCameraMoving || Mathf.Abs(destinationX - myTransform.position.x) < (cameraShift / 4f))
                {
                    ForceChangeSong(1);
                }
            }
        }
    }

    public void RotateCurrentTabBack()
    {
        if (currentSongInMultiplayer == true)
        {
            tabScripts[currentSongIndex].TabRotate(-1);
            currentSongInMultiplayer = false;
        }
    }


    public void PlaySong()
    {
        sTransition.FadeOut();
        Invoke("LoadSongScene", 0.75f);
    }

    void LoadSongScene()
    {
        transportedData.currentMenuIndex = currentSongIndex;
        transportedData.currentSong = songs[currentSongIndex];
        SceneManager.LoadScene("GameplayScene");
    }

    [SerializeField] GameObject cassetePlayer;
    [SerializeField] GameObject lvlSelectionCanvas;
    public void PrepareMenu()
    {
        GameObject[] tabs = GameObject.FindGameObjectsWithTag("tab");
        for (int i = 0; i < tabs.Length; ++i)
        {
            tabs[i].GetComponent<SpriteRenderer>().enabled = true;
            //we enable all the children
            for (int childID = 0; childID < tabs[i].transform.childCount; ++childID)
            {
                tabs[i].transform.GetChild(childID).gameObject.SetActive(true);
            }
        }
        cassetePlayer.SetActive(true);
        lvlSelectionCanvas.SetActive(true);
        divisionLine.SetActive(true);
        inSelectionMenu = true;
    }
    public void PrepareMenuExit()
    {
        GameObject[] tabs = GameObject.FindGameObjectsWithTag("tab");
        for (int i = 0; i < tabs.Length; ++i)
        {
            tabs[i].GetComponent<SpriteRenderer>().enabled = false;
            //if there will be errors then they will be HERE!
            //tabs[i].SetActiveRecursively(false);
            //we disable all the children
            for (int childID = 0; childID < tabs[i].transform.childCount; ++childID)
            {
                tabs[i].transform.GetChild(childID).gameObject.SetActive(false);
            }
        }
        cassetePlayer.SetActive(false);
        lvlSelectionCanvas.SetActive(false);
        divisionLine.SetActive(false);
        inSelectionMenu = false;
    }


    public void EnterMenu()
    {
        sTransition.FadeIn();
        currentSongIndex = transportedData.currentMenuIndex;
        //set camera on needed position
        myTransform.position = new Vector3(cameraShift * currentSongIndex, myTransform.position.y, myTransform.position.z);
        SongAuthorText.text = transportedData.currentSong.authorName;
        SongNameText.text = transportedData.currentSong.songName;
        
        ViewSongData();
        //later we will move it to view song data
        /*
        if(bestScore >= 491) 
            RecordText.text = "Record: " + bestScore + " (A+)";
        else if (bestScore > 400)
            RecordText.text = "Record: " + bestScore + " (A)";
        else if (bestScore > 350)
            RecordText.text = "Record: " + bestScore + " (B)";
        else if (bestScore > 300)
            RecordText.text = "Record: " + bestScore + " (C)";
        else
            RecordText.text = "Record: " + bestScore;
        */
    }


    void CameraMove(int dir)
    {
        isCameraMoving = true;
        destinationX = myTransform.position.x + cameraShift * dir;
    }

    void PreFinishCameraMove()
    {
        isCameraMoving = false;
        myTransform.position = new Vector3(destinationX, myTransform.position.y, myTransform.position.z);
    }

    public void ForceCameraMove(int dir)
    {
        if (isCameraMoving)
        {
            PreFinishCameraMove();
        }
        CameraMove(dir);
    }

    public void ForceChangeSong(int dir)
    {

        if (currentSongIndex + dir < 0 || currentSongIndex + dir >= songs.Count)
        {
            //here we try to move somewhere we dont know...
            return;
        }
        currentSongIndex += dir;
        ViewSongData();
        ForceCameraMove(dir);
    }

    void ViewSongData()
    {
        SongNameText.text = songs[currentSongIndex].songName;
        SongAuthorText.text = songs[currentSongIndex].authorName;
        //LastTryText.text = "Last try: " + songs[currentSongIndex].lastTryScore.ToString();
        //transportedData.currentSong
        if (PlayerPrefs.HasKey(MyUtitities.scoreSaveFlag + songs[currentSongIndex].songName))
        {
            songs[currentSongIndex].songRecord = PlayerPrefs.GetInt(MyUtitities.scoreSaveFlag + songs[currentSongIndex].songName);
        }
        else
        {
            songs[currentSongIndex].songRecord = 0;
            PlayerPrefs.SetInt(MyUtitities.scoreSaveFlag + songs[currentSongIndex].songName, 0);
        }
        //REDO THIS LATER
        if (PlayerPrefs.HasKey(MyUtitities.scoreSaveFlag + "lastTry__" + songs[currentSongIndex].songName))
        {
            songs[currentSongIndex].lastTryScore = PlayerPrefs.GetInt(MyUtitities.scoreSaveFlag + "lastTry__" + songs[currentSongIndex].songName);
        }
        else
        {
            songs[currentSongIndex].lastTryScore = 0;
            PlayerPrefs.SetInt(MyUtitities.scoreSaveFlag + "lastTry__" + songs[currentSongIndex].songName, 0);
        }
        if (songs[currentSongIndex].songRecord == 0)
        {
            mSlider.Reset(0, 0, 100);
        }
        else
        {
            int maxScore = 100;
            if(PlayerPrefs.HasKey(MyUtitities.scoreSaveFlag + "max__" + songs[currentSongIndex].songName))
                maxScore = PlayerPrefs.GetInt(MyUtitities.scoreSaveFlag + "max__" + songs[currentSongIndex].songName);
            mSlider.Reset(songs[currentSongIndex].lastTryScore, songs[currentSongIndex].songRecord, maxScore);
        }

        //working with grades in here
        if (PlayerPrefs.HasKey(MyUtitities.lastRewardFlag + songs[currentSongIndex].songName))
        {
            int lastReward = PlayerPrefs.GetInt(MyUtitities.lastRewardFlag + songs[currentSongIndex].songName);
            
            for (int i = lastReward; i < REWARDS_COUNT; ++i)
            {
                rewardPoints[i].SetActive(true);
            }


        }

        RecordText.text = "Record: " + songs[currentSongIndex].songRecord.ToString();
        audioSource.clip = songs[currentSongIndex].audio;
        audioSource.Play();
        //later we need to load record from here and to here
    }

    void AddCustomSongs()
    {
        foreach (AudioInfo audioI in transportedData.deletctedAudioFiles)
        {
            string audioName = audioI.name.Replace(".wav", "");
            if (transportedData.detectedJsonFiles.Exists(x => (x.name).Replace(".json", "") == audioName))
            {
                Song s = new Song();
                MyUtitities.LoadSongFromJSON(ref s, transportedData.detectedJsonFiles.Find(x => (x.name).Replace(".json", "") == audioName).json);
                s.audio = audioI.clip;
                songs.Add(s);
                //later may be add custom album covers
                Sprite albumCover = MyUtitities.LoadNewSprite(Application.persistentDataPath + "/covers/" + audioName + ".jpg");
                if(albumCover == null)
                    albumCover = MyUtitities.LoadNewSprite(Application.persistentDataPath + "/covers/" + audioName + ".png");
                if(albumCover == null)
                    albumCover = MyUtitities.LoadNewSprite(Application.persistentDataPath + "/covers/" + audioName + ".jpeg");

                GameObject tabInstance = Instantiate(tabPrefab, new Vector3((songs.Count - 1) * 3.7f, 1.9f, -1), Quaternion.identity);

                if (albumCover != null)
                {
                    tabInstance.transform.Find("cover").GetComponent<SpriteRenderer>().sprite = albumCover;
                }

                tabScripts.Add(tabInstance.GetComponent<TabScript>());
            }
        }
    }


    public string GetCurrentSongWAVAddress()
    {
        return Application.persistentDataPath + "/songs/" + songs[currentSongIndex].songName+".wav";
    }

}
