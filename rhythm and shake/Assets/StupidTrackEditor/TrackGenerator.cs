using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrackGenerator : MonoBehaviour
{
    [SerializeField]
    TransportedData td;

    [SerializeField]
    TMPro.TMP_Dropdown dropdown;

    float timer;
    bool isRecording = false;

    float deltaT = 0.15f;

    [SerializeField] AudioClip song;
    [SerializeField] AudioSource audioSource;

    [SerializeField] Transform tab1Transform;
    [SerializeField] Transform tab1UTransform;
    [SerializeField] Transform tab1DTransform;
    [SerializeField] Transform tab1LTransform;
    [SerializeField] Transform tab1RTransform;
    [SerializeField] Transform tab2Transform;
    [SerializeField] Transform tab2UTransform;
    [SerializeField] Transform tab2DTransform;
    [SerializeField] Transform tab2LTransform;
    [SerializeField] Transform tab2RTransform;

    [SerializeField] TabScript tabScript1;
    [SerializeField] TabScript tabScript2;

    [SerializeField] Song track;

    [SerializeField] 
    [TextArea(10, 10)]
    string track1Str;
    [SerializeField]
    [TextArea(10,10)]
    string track2Str;
    [SerializeField]
    [TextArea(10, 10)]
    string track3Str;

    enum HState { straight, left, right };
    enum VState { straight, up, down };

    HState hState;
    VState vState;

    [SerializeField]
    GameObject directionIndicator1;
    [SerializeField]
    GameObject directionIndicator2;
    [SerializeField]
    GameObject directionIndicatorMain;

    [SerializeField]
    string te;

    struct TouchInfo
    {
        public Touch t;
        public Vector2 pos;
        public int tabIndex;
        public TouchInfo(Touch t, Vector2 pos, int tabIndex)
        {
            this.t = t;
            this.pos = pos;
            this.tabIndex = tabIndex;
        }
    }
    List<TouchInfo> currentTouches;

    List<string> ddOptions;

    void Start()
    {
        dropdown.ClearOptions();
        ddOptions = new List<string>();
        for (int i = 0; i < td.deletctedAudioFiles.Count; ++i)
        {
            ddOptions.Add(td.deletctedAudioFiles[i].name);
        }
        dropdown.AddOptions(ddOptions);
        audioSource = GetComponent<AudioSource>();
        Screen.orientation = ScreenOrientation.Portrait;
        Vibration.Init();
        currentTouches = new List<TouchInfo>();
    }

    [SerializeField]
    float inSideAccuracy = 0.85f;
    [SerializeField]
    float inStraightAccuracy = 0.5f;

    [SerializeField]
    float swipeSensitivity = 0.1f;




    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            MyUtitities.LoadSongFromJSON(ref track, te);
        }


        if (isRecording)
        {
            timer += Time.deltaTime;




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

            if (hState == HState.straight && prevVstate == VState.straight && vState == VState.up)
            {
                Vibration.VibratePeek();
                directionIndicatorMain.SetActive(true);

                directionIndicatorMain.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
                Invoke("DisableIndicatorMain", 0.2f);

                SongItem songItem;
                songItem.dir = ArrowDirection.up;
                songItem.time = Mathf.Clamp(timer-deltaT, 0, timer);
                track.trackAcc.Add(songItem);
            }
            if (hState == HState.straight && prevVstate == VState.straight && vState == VState.down)
            {
                Vibration.VibratePeek();
                directionIndicatorMain.SetActive(true);

                directionIndicatorMain.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
                Invoke("DisableIndicatorMain", 0.2f);

                SongItem songItem;
                songItem.dir = ArrowDirection.down;
                songItem.time = Mathf.Clamp(timer - deltaT, 0, timer);
                track.trackAcc.Add(songItem);
            }
            if (vState == VState.straight && prevHState == HState.straight && hState == HState.left)
            {
                Vibration.VibratePeek();
                directionIndicatorMain.SetActive(true);

                directionIndicatorMain.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
                Invoke("DisableIndicatorMain", 0.2f);

                SongItem songItem;
                songItem.dir = ArrowDirection.left;
                songItem.time = Mathf.Clamp(timer - deltaT, 0, timer);
                track.trackAcc.Add(songItem);
            }
            if (vState == VState.straight && prevHState == HState.straight && hState == HState.right)
            {
                Vibration.VibratePeek();
                directionIndicatorMain.SetActive(true);

                directionIndicatorMain.transform.localRotation = Quaternion.identity;
                Invoke("DisableIndicatorMain", 0.2f);

                SongItem songItem;
                songItem.dir = ArrowDirection.right;
                songItem.time = Mathf.Clamp(timer - deltaT, 0, timer);
                track.trackAcc.Add(songItem);
            }

            if (Input.touchCount > 0)
            {
                Touch[] touches = Input.touches;
                foreach (Touch t in touches)
                {
                    if (t.phase == TouchPhase.Began)
                    {
                        Vector3 pos = Camera.main.ScreenToWorldPoint(t.position);
                        pos = new Vector3(pos.x, pos.y, -5f);
                        Debug.Log(pos);

                        
                        if (MyUtitities.TouchOver(pos, tab1UTransform))
                        {
                            Vibration.VibratePop();
                            directionIndicator1.SetActive(true);
                            directionIndicator1.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
                            Invoke("DisableIndicator1", 0.2f);
                            tabScript1.TapIncrease(true, Color.green);
                            SongItem songItem;
                            songItem.dir = ArrowDirection.up;
                            songItem.time = timer - deltaT;
                            track.track1.Add(songItem);
                        }
                        else if (MyUtitities.TouchOver(pos, tab1DTransform))
                        {
                            Vibration.VibratePop();
                            directionIndicator1.SetActive(true);
                            directionIndicator1.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
                            Invoke("DisableIndicator1", 0.2f);
                            tabScript1.TapIncrease(true, Color.green);
                            SongItem songItem;
                            songItem.dir = ArrowDirection.down;
                            songItem.time = timer - deltaT;
                            track.track1.Add(songItem);
                        }
                        else if (MyUtitities.TouchOver(pos, tab1LTransform))
                        {
                            Vibration.VibratePop();
                            directionIndicator1.SetActive(true);
                            directionIndicator1.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
                            Invoke("DisableIndicator1", 0.2f);
                            tabScript1.TapIncrease(true, Color.green);
                            SongItem songItem;
                            songItem.dir = ArrowDirection.left;
                            songItem.time = timer - deltaT;
                            track.track1.Add(songItem);
                        }
                        else if (MyUtitities.TouchOver(pos, tab1RTransform))
                        {
                            Vibration.VibratePop();
                            directionIndicator1.SetActive(true);
                            directionIndicator1.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                            Invoke("DisableIndicator1", 0.2f);
                            tabScript1.TapIncrease(true, Color.green);
                            SongItem songItem;
                            songItem.dir = ArrowDirection.right;
                            songItem.time = timer - deltaT;
                            track.track1.Add(songItem);
                        }
                        else if (MyUtitities.TouchOver(pos, tab1Transform))
                        {
                            Vibration.VibratePop();
                            tabScript1.TapIncrease(true, Color.green);
                            SongItem songItem;
                            songItem.dir = ArrowDirection.no;
                            songItem.time = timer - deltaT;
                            track.track1.Add(songItem);
                        }

                        if (MyUtitities.TouchOver(pos, tab2UTransform))
                        {
                            Vibration.VibratePop();
                            directionIndicator2.SetActive(true);
                            directionIndicator2.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
                            Invoke("DisableIndicator2", 0.2f);
                            tabScript2.TapIncrease(true, Color.green);
                            SongItem songItem;
                            songItem.dir = ArrowDirection.up;
                            songItem.time = timer - deltaT;
                            track.track2.Add(songItem);
                        }
                        else if (MyUtitities.TouchOver(pos, tab2DTransform))
                        {
                            Vibration.VibratePop();
                            directionIndicator2.SetActive(true);
                            directionIndicator2.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
                            Invoke("DisableIndicator2", 0.2f);
                            tabScript2.TapIncrease(true, Color.green);
                            SongItem songItem;
                            songItem.dir = ArrowDirection.down;
                            songItem.time = timer - deltaT;
                            track.track2.Add(songItem);
                        }
                        else if (MyUtitities.TouchOver(pos, tab2LTransform))
                        {
                            Vibration.VibratePop();
                            directionIndicator2.SetActive(true);
                            directionIndicator2.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
                            Invoke("DisableIndicator2", 0.2f);
                            tabScript2.TapIncrease(true, Color.green);
                            SongItem songItem;
                            songItem.dir = ArrowDirection.left;
                            songItem.time = timer - deltaT;
                            track.track2.Add(songItem);
                        }
                        else if (MyUtitities.TouchOver(pos, tab2RTransform))
                        {
                            Vibration.VibratePop();
                            directionIndicator2.SetActive(true);
                            directionIndicator2.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                            Invoke("DisableIndicator2", 0.2f);
                            tabScript2.TapIncrease(true, Color.green);
                            SongItem songItem;
                            songItem.dir = ArrowDirection.right;
                            songItem.time = timer - deltaT;
                            track.track2.Add(songItem);
                        }
                        else if (MyUtitities.TouchOver(pos, tab2Transform))
                        {
                            Vibration.VibratePop();
                            tabScript2.TapIncrease(true, Color.green);
                            SongItem songItem;
                            songItem.dir = ArrowDirection.no;
                            songItem.time = timer - deltaT;
                            track.track2.Add(songItem);
                        }

                        /*
                        if (pos.x < (tab1Transform.position.x + (tab1Scale.x / 2f)) && pos.x > (tab1Transform.position.x - (tab1Scale.x / 2f)) &&
                            pos.y < (tab1Transform.position.y + (tab1Scale.y / 2f)) && pos.y > (tab1Transform.position.y - (tab1Scale.y / 2f)))
                        {
                            currentTouches.Add(new TouchInfo(t, t.position, 1));
                            //TouchTab(tabScript1, pos);
                            //EffectOnBackGround(pos, col1);
                            Debug.Log("touch start1");
                        }

                        if (pos.x < (tab2Transform.position.x + (tab2Scale.x / 2f)) && pos.x > (tab2Transform.position.x - (tab2Scale.x / 2f)) &&
                            pos.y < (tab2Transform.position.y + (tab2Scale.y / 2f)) && pos.y > (tab2Transform.position.y - (tab2Scale.y / 2f)))
                        {
                            //TouchTab(tabScript2, pos);
                            //EffectOnBackGround(pos, col2);
                            currentTouches.Add(new TouchInfo(t, t.position, 2));
                        }
                        */
                    }
                    /*
                    if (t.phase == TouchPhase.Ended)
                    {
                        if (currentTouches.Exists(x => (x.t.fingerId == t.fingerId)))
                        {
                            Debug.Log("touch end");
                            TouchInfo ti = currentTouches.Find(x => (x.t.fingerId == t.fingerId));
                            //Vector2 delta = t.position - ti.pos;
                            //Debug.Log(delta);
                            ArrowDirection dir = ArrowDirection.no;
                            float angle = 0;
                            float maxB = 0;
                            float minB = 0;
                            if (ti.tabIndex == 1)
                            {
                                maxB = (tab1Transform.position.y + (tab1Transform.localScale.y / 2f));
                                minB = (tab1Transform.position.y - (tab1Transform.localScale.y / 2f));
                            }
                            else
                            {
                                maxB = (tab2Transform.position.y + (tab2Transform.localScale.y / 2f));
                                minB = (tab2Transform.position.y - (tab2Transform.localScale.y / 2f));
                            }

                            if (t.position.x < (tab1Transform.position.x - (tab1Transform.localScale.x / 2f)))
                            {
                                dir = ArrowDirection.left;
                                angle = 180;
                            }
                            else if (t.position.x > (tab1Transform.position.x + (tab1Transform.localScale.x / 2f)))
                            {
                                dir = ArrowDirection.right;
                                angle = 0;
                            }
                            else if (t.position.y > maxB)
                            {
                                dir = ArrowDirection.up;
                                angle = 90;
                            }
                            else if (t.position.y < minB)
                            {
                                dir = ArrowDirection.down;
                                angle = -90;
                            }

                            if (ti.tabIndex == 1)
                            {
                                Debug.Log("touch end1");
                                Vibration.VibratePop();
                                if (dir != ArrowDirection.no)
                                {
                                    directionIndicator1.SetActive(true);
                                    directionIndicator1.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                                    Invoke("DisableIndicator1", 0.2f);
                                }
                                tabScript1.TapIncrease(true, Color.green);
                                SongItem songItem;
                                songItem.dir = dir;
                                songItem.time = timer - deltaT;
                                track.track1.Add(songItem);
                            }
                            else
                            {
                                Vibration.VibratePop();
                                if (dir != ArrowDirection.no)
                                {
                                    directionIndicator2.SetActive(true);
                                    directionIndicator2.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                                    Invoke("DisableIndicator2", 0.2f);
                                }
                                tabScript2.TapIncrease(true, Color.green);
                                SongItem songItem;
                                songItem.dir = dir;
                                songItem.time = timer - deltaT;
                                track.track2.Add(songItem);
                            }
                            currentTouches.Remove(ti);
                        }
                    }
                    */
                }
            }

            /*

            if (Input.touchCount > 0)
            {
                Touch[] touches = Input.touches;
                foreach (Touch t in touches)
                {
                    if (t.phase == TouchPhase.Began)
                    {
                        Vector3 pos = Camera.main.ScreenToWorldPoint(t.position);
                        pos = new Vector3(pos.x, pos.y, -5f);
                        Debug.Log(pos);
                        Vector3 tab1Scale = tab1Transform.localScale;
                        Vector3 tab2Scale = tab2Transform.localScale;
                        if (pos.x < (tab1Transform.position.x + (tab1Scale.x / 2f)) && pos.x > (tab1Transform.position.x - (tab1Scale.x / 2f)) &&
                            pos.y < (tab1Transform.position.y + (tab1Scale.y / 2f)) && pos.y > (tab1Transform.position.y - (tab1Scale.y / 2f)))
                        {
                            Vibration.VibratePop();
                            tabScript1.TapIncrease(true, Color.green);
                            SongItem songItem;
                            songItem.dir = ArrowDirection.no;
                            songItem.time = timer-delta;
                            track.track1.Add(songItem);
                        }
                        if (pos.x < (tab2Transform.position.x + (tab2Scale.x / 2f)) && pos.x > (tab2Transform.position.x - (tab2Scale.x / 2f)) &&
                            pos.y < (tab2Transform.position.y + (tab2Scale.y / 2f)) && pos.y > (tab2Transform.position.y - (tab2Scale.y / 2f)))
                        {
                            Vibration.VibratePop();
                            tabScript2.TapIncrease(true, Color.green);
                            SongItem songItem;
                            songItem.dir = ArrowDirection.no;
                            songItem.time = timer-delta;
                            track.track2.Add(songItem);
                        }
                    }
                }
            }
            */
        }
    }


    

    public void PressButton()
    {
        if (isRecording)
        {
            StopTrackRecording();
        }
        else
        {
            StartTrackRecording();
        }
    }

    void StartTrackRecording()
    {
        track.track1.Clear();
        track.track2.Clear();
        track.trackAcc.Clear();
        timer = 0;
        isRecording = true;
        audioSource.clip = td.deletctedAudioFiles[dropdown.value].clip;
        track.songName = td.deletctedAudioFiles[dropdown.value].name.Replace(".wav", "");
        audioSource.Play();
    }

    void StopTrackRecording()
    {
        isRecording = false;
        MyUtitities.SaveSongToAndroid(track, track.songName + ".json");
        if (!td.detectedJsonFiles.Exists(x => x.name == track.songName + ".json"))
        {
            JsonInfo ji = new JsonInfo();
            ji.name = track.songName + ".json";
            ji.json = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + track.songName + ".json");
            td.detectedJsonFiles.Add(ji);
        }
        audioSource.Stop();
        SceneManager.LoadScene("SampleScene");
    }


    void DisableIndicator1()
    {
        directionIndicator1.SetActive(false);
    }

    void DisableIndicator2()
    {
        directionIndicator2.SetActive(false);
    }

    void DisableIndicatorMain()
    {
        directionIndicatorMain.SetActive(false);
    }


    
}
