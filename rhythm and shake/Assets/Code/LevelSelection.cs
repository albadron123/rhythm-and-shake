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
    int currentSongIndex = 1;


    [SerializeField]
    TMPro.TMP_Text SongNameText;
    [SerializeField]
    TMPro.TMP_Text SongAuthorText;
    [SerializeField]
    TMPro.TMP_Text LastTryText;
    [SerializeField]
    TMPro.TMP_Text RecordText;



    [SerializeField]
    GameObject textPrefab;

    Transform myTransform;

    [SerializeField]
    SpriteRenderer tab1;



    [SerializeField]
    GameObject canvas;

    [SerializeField]
    Transform tab1Transform;

    TabScript tabScript1;


    [SerializeField]
    GameObject playSongButton;

    AudioSource audioSource;


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

        tabScript1 = tab1Transform.GetComponent<TabScript>();
        audioSource = GetComponent<AudioSource>();

        if (!Input.gyro.enabled)
        {
            Input.gyro.enabled = true;
        }

        Screen.orientation = ScreenOrientation.Portrait;


        Vibration.Init();

        //Later should expand it
        EnterMenu();
        //ViewSongData();
    }

    // Update is called once per frame
    void Update()
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

        tab1Transform.rotation = Quaternion.Euler(-acc.y * 25f, -acc.x * 25f, 0);

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
                    Vector2 pos = -Camera.main.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, -9));
                    Vector3 tab1Scale = tab1Transform.localScale;
                    if (pos.x < (tab1Transform.position.x + (tab1Scale.x / 2f)) && pos.x > (tab1Transform.position.x - (tab1Scale.x / 2f)) &&
                        pos.y < (tab1Transform.position.y + (tab1Scale.y / 2f)) && pos.y > (tab1Transform.position.y - (tab1Scale.y / 2f)))
                    {
                        tabScript1.TapIncrease(true);
                        //here we will turn song tab around
                    }
                }
            }
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

    public void EnterMenu()
    {
        sTransition.FadeIn();
        currentSongIndex = transportedData.currentMenuIndex;
        //set camera on needed position
        myTransform.position = new Vector3(cameraShift * (currentSongIndex - 1), myTransform.position.y, myTransform.position.z);
        SongAuthorText.text = transportedData.currentSong.authorName;
        SongNameText.text = transportedData.currentSong.songName;
        if (transportedData.currentSong.lastTryScore > transportedData.currentSong.songRecord)
        {
            transportedData.currentSong.songRecord = transportedData.currentSong.lastTryScore;
        }
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
        LastTryText.text = "Last try: " + songs[currentSongIndex].lastTryScore.ToString();
        RecordText.text = "Record: " + songs[currentSongIndex].songRecord.ToString();
        audioSource.clip = songs[currentSongIndex].audio;
        audioSource.Play();
        //later we need to load record from here and to here
        //we also need to play song in this place (may be)
    }

}
