using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public enum NoteType {simple, accNote}

public class GamePlayCode : MonoBehaviour
{



    const float MAX_NOTE_EFFECT_DISTANCE = 10.0f;
    const float MAX_ACC_EFFECT_DISTANCE = 10.0f;

    [SerializeField]
    SceneTransition sTransition;

    Transform myTransform;

    [SerializeField]
    TMPro.TMP_Text te;

    [SerializeField]
    TransportedData transportedData;

    [SerializeField]
    Transform tab1Transform;
    [SerializeField]
    Transform tab2Transform;

    TabScript tabScript1;
    TabScript tabScript2;

    [SerializeField]
    TabScript tabScriptAcc;


    [SerializeField]
    GameObject canvas;


    [SerializeField]
    GameObject textPrefab;

    public bool isMain = false;


    SongGenerator songGenerator;

    enum HState { straight, left, right };
    enum VState { straight, up, down };

    HState hState;
    VState vState;

    [SerializeField]
    Color colorUP;
    [SerializeField]
    Color colorDOWN;
    [SerializeField]
    Color colorLEFT;
    [SerializeField]
    Color colorRIGHT;
    [SerializeField]
    Color colorDefault;

    [SerializeField]
    float maxVelocity;

    [SerializeField]
    Transform scoreIndicatorT;
    [SerializeField]
    Transform scoreSladerT;
    [SerializeField]
    Transform maxScoreNowIndicatorT;
    [SerializeField]
    Transform maxScoreNowSladerT;

    [SerializeField]
    TMPro.TMP_Text hisScoreText;
    [SerializeField]
    Transform hisScoreIndicatorT;
    [SerializeField]
    Transform hisScoreSliderT;


    int score = 0;
    int maxScoreNow = 0;
    public static int maxScore;
    //change later!!!!!!!!!!!!!!!!!!
    //change later!!!!!!!!!!!!!!!!!!
    //change later!!!!!!!!!!!!!!!!!!
    //change later!!!!!!!!!!!!!!!!!!
    int bestScore = 0;
    public int combo = 0;
    public int maxComboNow = 0;

    [SerializeField]
    float inSideAccuracy = 0.85f;
    [SerializeField]
    float inStraightAccuracy = 0.5f;

    [SerializeField]
    bool inMultiplayer = false;


    bool useHyro = true;

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

    [SerializeField]
    GameObject directionIndicator1;
    [SerializeField]
    GameObject directionIndicator2;

    /// <summary>
    /// /////////////////////////////GYRO STUFF///////////////////////
    /// /////////////////////////////GYRO STUFF///////////////////////
    /// /////////////////////////////GYRO STUFF///////////////////////
    /// /////////////////////////////GYRO STUFF///////////////////////
    /// /////////////////////////////GYRO STUFF///////////////////////
    /// </summary>

    public bool _supported;

    private Quaternion _off;
    private Vector3 _offEuler;
    int _activeSemaphore = 0;
    private float _degreesForFullTilt = 10;

    public Vector2 _lastTilt;

    public void Init()
    {
        _off = Quaternion.identity;
        _supported = SystemInfo.supportsGyroscope;
    }

    public bool Activate(bool isActivated)
    {
        if (isActivated) _activeSemaphore++;
        else _activeSemaphore--;

        _activeSemaphore = Mathf.Max(_activeSemaphore, 0);

        if (_activeSemaphore > 0)
        {
            if (_supported)
            {
                Input.gyro.enabled = true;
            }
            else
            {
                return false; //everything not ok; you requested gyro but can't have it!
            }
        }
        else
        {
            if (_supported)
            {
                Input.gyro.enabled = false;
            }
        }
        return true; //everything ok;

    }

    public void Deactivate()
    {
        _activeSemaphore = 0;
    }

    public void SetCurrentReadingAsFlat()
    {
        _off = Input.gyro.attitude;
        _offEuler = _off.eulerAngles;
    }

    public Vector3 GetReading()
    {
        if (_supported)
        {
            return (Quaternion.Inverse(_off) * Input.gyro.attitude).eulerAngles;
        }
        else
        {
            Debug.LogError("Tried to get gyroscope reading on a device which didn't have one.");
            return Vector3.zero;
        }
    }

    public Vector2 Get2DTilt()
    {
        Vector3 reading = GetReading();

        Vector2 tilt = new Vector2(
            -Mathf.DeltaAngle(reading.y, 0),
            Mathf.DeltaAngle(reading.x, 0)
        );

        //can't go over max
        tilt.x = Mathf.InverseLerp(-_degreesForFullTilt, _degreesForFullTilt, tilt.x) * 2 - 1;
        tilt.y = Mathf.InverseLerp(-_degreesForFullTilt, _degreesForFullTilt, tilt.y) * 2 - 1;

        //get phase
        tilt.x = Mathf.Clamp(tilt.x, -1, 1);
        tilt.y = Mathf.Clamp(tilt.y, -1, 1);

        _lastTilt = tilt;

        return tilt;
    }

    public string GetExplanation()
    {
        Vector3 reading = GetReading();

        string msg = "";

        msg += "OFF: " + _offEuler + "\n";

        Vector2 tilt = new Vector2(
            -Mathf.DeltaAngle(reading.y, 0),
            Mathf.DeltaAngle(reading.x, 0)
        );

        msg += "DELTA: " + tilt + "\n";

        //can't go over max
        tilt.x = Mathf.InverseLerp(-_degreesForFullTilt, _degreesForFullTilt, tilt.x) * 2 - 1;
        tilt.y = Mathf.InverseLerp(-_degreesForFullTilt, _degreesForFullTilt, tilt.y) * 2 - 1;

        msg += "LERPED: " + tilt + "\n";

        //get phase
        tilt.x = Mathf.Clamp(tilt.x, -1, 1);
        tilt.y = Mathf.Clamp(tilt.y, -1, 1);

        msg += "CLAMPED: " + tilt + "\n";

        return msg;

    }

    public void SetDegreesForFullTilt(float degrees)
    {
        _degreesForFullTilt = degrees;
    }

    /// <summary>
    /// /////////////////////////////GYRO STUFF///////////////////////
    /// /////////////////////////////GYRO STUFF///////////////////////
    /// /////////////////////////////GYRO STUFF///////////////////////
    /// /////////////////////////////GYRO STUFF///////////////////////
    /// /////////////////////////////GYRO STUFF///////////////////////
    /// </summary>

    private void Awake()
    {
        maxScore = 1000;
    }


    void Start()
    {
        myTransform = transform;

        songGenerator = GetComponent<SongGenerator>();

        tabScript1 = tab1Transform.GetComponent<TabScript>();
        tabScript2 = tab2Transform.GetComponent<TabScript>();

        currentTouches = new List<TouchInfo>();


        if (SystemInfo.supportsGyroscope)
        {
            Input.simulateMouseWithTouches = true;
            Init();
            Activate(true);
            SetDegreesForFullTilt(85);
            SetCurrentReadingAsFlat();
        }
        else
        {
            useHyro = false;
        }

        Screen.orientation = ScreenOrientation.Portrait;

        te.text = "0";

        Vibration.Init();

        sTransition.FadeIn();

    }

    void Update()
    {
        if (SongGenerator.isPlaying)
        {


            HState prevHState;
            VState prevVstate;

            if (!useHyro)
            {
                Vector3 acc = Input.acceleration;
                prevHState = hState;
                prevVstate = vState;
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
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    SetCurrentReadingAsFlat();
                }

                prevHState = hState;
                prevVstate = vState;

                Vector2 hyro = Get2DTilt();
                if (hyro.x < inStraightAccuracy && hState == HState.right)
                {
                    hState = HState.straight;
                }
                else if (hyro.x > -inStraightAccuracy && hState == HState.left)
                {
                    hState = HState.straight;
                }
                else if (hyro.x > inSideAccuracy && hState == HState.straight)
                {
                    hState = HState.right;
                }
                else if (hyro.x < -inSideAccuracy && hState == HState.straight)
                {
                    hState = HState.left;
                }

                if (hyro.y < inStraightAccuracy && vState == VState.up)
                {
                    vState = VState.straight;
                }
                else if (hyro.y > -inStraightAccuracy && vState == VState.down)
                {
                    vState = VState.straight;
                }
                else if (hyro.y > inSideAccuracy && vState == VState.straight)
                {
                    vState = VState.up;
                }
                else if (hyro.y < -inSideAccuracy && vState == VState.straight)
                {
                    vState = VState.down;
                }
            }


            if ((prevVstate == VState.straight && vState == VState.up) || Input.GetKeyDown(KeyCode.I))
            {
                //trigger up
                if (tabScriptAcc.notesInJudgementZone.Count != 0)
                {
                    List<GameObject> candidatesSorted = tabScriptAcc.notesInJudgementZone.OrderBy(x => -Mathf.Sign((float)TransportedData.handMode - 0.5f) * x.transform.position.x).ToList();
                    Note n = candidatesSorted[candidatesSorted.Count - 1].GetComponent<Note>();
                    if (n.dir == ArrowDirection.up)
                    {
                        n.Anihilate();
                        Vibration.VibratePeek();
                        EffectOnBackGround(Vector3.zero, colorUP, MAX_ACC_EFFECT_DISTANCE, 0.2f);
                        EffectTilt(Vector3.zero, Vector2.up * 0.5f, MAX_ACC_EFFECT_DISTANCE);

                        ++combo;
                        if (combo < 10)
                            ChangeScore(score + 2);
                        else if (combo < 50)
                            ChangeScore(score + 5);
                        else
                            ChangeScore(score + 10);

                        ChangeMaxScoreNow(NoteType.accNote);

                        if (combo >= 10)
                        {
                            PrintNote("Combo x" + combo, new Vector3(0, 0, -5f), colorUP);
                        }
                        else
                        {
                            PrintNote("GREAT UP (+2)!", new Vector3(0, 0, -5f), colorUP);
                        }
                    }
                }
            }
            if ((prevVstate == VState.straight && vState == VState.down) || Input.GetKeyDown(KeyCode.K))
            {
                //trigger down
                if (tabScriptAcc.notesInJudgementZone.Count != 0)
                {
                    List<GameObject> candidatesSorted = tabScriptAcc.notesInJudgementZone.OrderBy(x => -Mathf.Sign((float)TransportedData.handMode - 0.5f) * x.transform.position.x).ToList();
                    Note n = candidatesSorted[candidatesSorted.Count - 1].GetComponent<Note>();
                    if (n.dir == ArrowDirection.down)
                    {
                        n.Anihilate();
                        Vibration.VibratePeek();
                        EffectOnBackGround(Vector3.zero, colorDOWN, MAX_ACC_EFFECT_DISTANCE, 0.2f);
                        EffectTilt(Vector3.zero, Vector2.down * 0.5f, MAX_ACC_EFFECT_DISTANCE);

                        ++combo;
                        if (combo < 10)
                            ChangeScore(score + 2);
                        else if (combo < 50)
                            ChangeScore(score + 5);
                        else
                            ChangeScore(score + 10);

                        ChangeMaxScoreNow(NoteType.accNote);

                        if (combo >= 10)
                        {
                            PrintNote("Combo x" + combo, new Vector3(0, 0, -5f), colorDOWN);
                        }
                        else
                        {
                            PrintNote("GREAT DOWN (+2)!", new Vector3(0, 0, -5f), colorDOWN);
                        }
                    }
                }
            }
            if ((prevHState == HState.straight && hState == HState.left) || Input.GetKeyDown(KeyCode.J))
            {
                if (tabScriptAcc.notesInJudgementZone.Count != 0)
                {
                    List<GameObject> candidatesSorted = tabScriptAcc.notesInJudgementZone.OrderBy(x => -Mathf.Sign((float)TransportedData.handMode - 0.5f) * x.transform.position.x).ToList();
                    Note n = candidatesSorted[candidatesSorted.Count - 1].GetComponent<Note>();
                    if (n.dir == ArrowDirection.left)
                    {
                        n.Anihilate();
                        Vibration.VibratePeek();
                        EffectOnBackGround(Vector3.zero, colorLEFT, MAX_ACC_EFFECT_DISTANCE, 0.2f);
                        EffectTilt(Vector3.zero, Vector2.left * 0.5f, MAX_ACC_EFFECT_DISTANCE);


                        ++combo;
                        if (combo < 10)
                            ChangeScore(score + 2);
                        else if (combo < 50)
                            ChangeScore(score + 5);
                        else
                            ChangeScore(score + 10);

                        ChangeMaxScoreNow(NoteType.accNote);

                        if (combo >= 10)
                        {
                            PrintNote("Combo x" + combo, new Vector3(0, 0, -5f), colorLEFT);
                        }
                        else
                        {
                            PrintNote("GREAT LEFT (+2)!", new Vector3(0, 0, -5f), colorLEFT);
                        }
                    }
                }
            }
            if ((prevHState == HState.straight && hState == HState.right) || Input.GetKeyDown(KeyCode.L))
            {
                if (tabScriptAcc.notesInJudgementZone.Count != 0)
                {
                    List<GameObject> candidatesSorted = tabScriptAcc.notesInJudgementZone.OrderBy(x => -Mathf.Sign((float)TransportedData.handMode - 0.5f) * x.transform.position.x).ToList();
                    Note n = candidatesSorted[candidatesSorted.Count - 1].GetComponent<Note>();
                    if (n.dir == ArrowDirection.right)
                    {
                        n.Anihilate();
                        Vibration.VibratePeek();
                        EffectOnBackGround(Vector3.zero, colorRIGHT, MAX_ACC_EFFECT_DISTANCE, 0.2f);
                        EffectTilt(Vector3.zero, Vector2.right * 0.5f, MAX_ACC_EFFECT_DISTANCE);

                        ++combo;
                        if (combo < 10)
                            ChangeScore(score + 2);
                        else if (combo < 50)
                            ChangeScore(score + 5);
                        else
                            ChangeScore(score + 10);

                        ChangeMaxScoreNow(NoteType.accNote);

                        if (combo >= 10)
                        {
                            PrintNote("Combo x" + combo, new Vector3(0, 0, -5f), colorRIGHT);
                        }
                        else
                        {
                            PrintNote("GREAT RIGHT (+2)!", new Vector3(0, 0, -5f), colorRIGHT);
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.W))
                TouchTab(tabScript1, directionIndicator1, new Vector3(0, 0, -2), ArrowDirection.up);
            if (Input.GetKeyDown(KeyCode.S))
                TouchTab(tabScript1, directionIndicator1, new Vector3(0, 0, -2), ArrowDirection.down);
            if (Input.GetKeyDown(KeyCode.A))
                TouchTab(tabScript1, directionIndicator1, new Vector3(0, 0, -2), ArrowDirection.left);
            if (Input.GetKeyDown(KeyCode.D))
                TouchTab(tabScript1, directionIndicator1, new Vector3(0, 0, -2), ArrowDirection.right);
            if (Input.GetKeyDown(KeyCode.UpArrow))
                TouchTab(tabScript2, directionIndicator2, new Vector3(0, 0, -2), ArrowDirection.up);
            if (Input.GetKeyDown(KeyCode.DownArrow))
                TouchTab(tabScript2, directionIndicator2, new Vector3(0, 0, -2), ArrowDirection.down);
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                TouchTab(tabScript2, directionIndicator2, new Vector3(0, 0, -2), ArrowDirection.left);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                TouchTab(tabScript2, directionIndicator2, new Vector3(0, 0, -2), ArrowDirection.right);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //EffectOnBackGround(Vector3.zero, colorRIGHT, MAX_ACC_EFFECT_DISTANCE, 0.2f);
                EffectTilt(Vector3.zero, Vector2.right * 0.5f, MAX_ACC_EFFECT_DISTANCE);
                ScaleBackground();
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
                        Vector3 tab1Scale = tab1Transform.localScale;
                        Vector3 tab2Scale = tab2Transform.localScale;
                        if (pos.x < (tab1Transform.position.x + (tab1Scale.x / 2f)) && pos.x > (tab1Transform.position.x - (tab1Scale.x / 2f)) &&
                            pos.y < (tab1Transform.position.y + (tab1Scale.y / 2f)) && pos.y > (tab1Transform.position.y - (tab1Scale.y / 2f)))
                        {
                            currentTouches.Add(new TouchInfo(t, t.position, 1));
                            //TouchTab(tabScript1, pos);
                            //EffectOnBackGround(pos, col1);
                        }
                        if (pos.x < (tab2Transform.position.x + (tab2Scale.x / 2f)) && pos.x > (tab2Transform.position.x - (tab2Scale.x / 2f)) &&
                            pos.y < (tab2Transform.position.y + (tab2Scale.y / 2f)) && pos.y > (tab2Transform.position.y - (tab2Scale.y / 2f)))
                        {
                            //TouchTab(tabScript2, pos);
                            //EffectOnBackGround(pos, col2);
                            currentTouches.Add(new TouchInfo(t, t.position, 2));
                        }
                    }
                    if (t.phase == TouchPhase.Ended)
                    {
                        if (currentTouches.Exists(x => (x.t.fingerId == t.fingerId)))
                        {
                            TouchInfo ti = currentTouches.Find(x => (x.t.fingerId == t.fingerId));
                            Vector2 delta = t.position - ti.pos;
                            Debug.Log(delta);
                            ArrowDirection dir;
                            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                            {
                                if (delta.x > 0)
                                    dir = ArrowDirection.right;
                                else
                                    dir = ArrowDirection.left;
                            }
                            else
                            {
                                if (delta.y > 0)
                                    dir = ArrowDirection.up;
                                else
                                    dir = ArrowDirection.down;
                            }
                            if (ti.tabIndex == 1)
                            {
                                TouchTab(tabScript1, directionIndicator1, ti.pos, dir);
                            }
                            else
                            {
                                TouchTab(tabScript2, directionIndicator2, ti.pos, dir);
                            }
                            currentTouches.Remove(ti);
                        }
                    }
                }
            }

        }

    }

    private void HideDirectionIndicator1(){ directionIndicator1.SetActive(false); }
    private void HideDirectionIndicator2(){ directionIndicator2.SetActive(false); }

    public void PrintNote(string content, Vector3 pos, Color col)
    {
        GameObject textObject = Instantiate(textPrefab, pos, Quaternion.identity);
        textObject.transform.parent = canvas.transform;
        textObject.transform.localScale = new Vector3(1, 1, 1);
        TMPro.TMP_Text te = textObject.GetComponent<TMPro.TMP_Text>();
        te.text = content;
        te.color = col;
    }

    private void EffectOnBackGround(Vector3 pos, Color col, float maxDist, float intencity)
    {
        foreach (GameObject bp in GetComponent<GenerateGrid>().backgroundParticles)
        {
            float dist = Vector2.Distance(bp.transform.position, pos);
            if (dist < maxDist)
            {

                BackgroundParticle bpScript = bp.GetComponent<BackgroundParticle>();
                bpScript.rotationalVelocity = maxVelocity * (maxDist - dist);
                bpScript.rotationalDirection = -bpScript.rotationalDirection;
                bpScript.SetColor((maxDist - intencity * dist) / maxDist * col + 
                                  (dist / maxDist) * bp.GetComponent<SpriteRenderer>().color + 
                                  new Color(Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f)));

            }
        }
    }

    private void ScaleBackground()
    {
        foreach (GameObject bp in GetComponent<GenerateGrid>().backgroundParticles)
        {
            BackgroundParticle bpScript = bp.GetComponent<BackgroundParticle>();
            bpScript.ScaleWave();
        }
    }

    private void EffectTilt(Vector3 pos, Vector2 dir, float maxDist)
    {
        foreach (GameObject bp in GetComponent<GenerateGrid>().backgroundParticles)
        {
            float dist = Vector2.Distance(bp.transform.position, pos);
            if (dist < maxDist)
            {
                BackgroundParticle bpScript = bp.GetComponent<BackgroundParticle>();
                bpScript.MoveTo(dir * Random.Range(0.9f, 1.1f) * (maxDist - dist) / maxDist);
                bpScript.rotationalDirection = -bpScript.rotationalDirection;
            }
        }
    }

    public void TouchTab(TabScript tabScript, GameObject directionIndicator, Vector3 pos, ArrowDirection dir)
    {
        ScaleBackground();
        Vector3 effectPos = Camera.main.ScreenToWorldPoint(pos);
        effectPos = new Vector3(effectPos.x, effectPos.y, -5f);
        if (tabScript.notesInJudgementZone.Count != 0)
        {
            List<GameObject> candidatesSorted = tabScript.notesInJudgementZone.OrderBy(x => -Mathf.Sign((float)TransportedData.handMode - 0.5f) * x.transform.position.x).ToList();
            Note note = candidatesSorted[candidatesSorted.Count - 1].GetComponent<Note>();
            Color col = colorDefault;
            if (note.dir == ArrowDirection.no || note.dir == dir)
            {
                
                if (note.dir == dir)
                {
                    Vibration.VibratePop();
                    directionIndicator.SetActive(true);
                    if (directionIndicator == directionIndicator1)
                    {
                        Invoke("HideDirectionIndicator1", 0.15f);
                    }
                    else
                    {
                        Invoke("HideDirectionIndicator2", 0.15f);
                    }
                    switch (dir)
                    {
                        case ArrowDirection.up: 
                            directionIndicator.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
                            tabScript.TapIncrease(true, colorUP);
                            EffectOnBackGround(effectPos, colorUP, MAX_NOTE_EFFECT_DISTANCE, 1f);
                            col = colorUP;
                            break;
                        case ArrowDirection.down: 
                            directionIndicator.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
                            tabScript.TapIncrease(true, colorDOWN);
                            EffectOnBackGround(effectPos, colorDOWN, MAX_NOTE_EFFECT_DISTANCE, 1f);
                            col = colorDOWN;
                            break;
                        case ArrowDirection.left: directionIndicator.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -180));
                            tabScript.TapIncrease(true, colorLEFT);
                            EffectOnBackGround(effectPos, colorLEFT, MAX_NOTE_EFFECT_DISTANCE, 1f);
                            col = colorLEFT;
                            break;
                        default: directionIndicator.transform.localRotation = Quaternion.identity;
                            tabScript.TapIncrease(true, colorRIGHT);
                            EffectOnBackGround(effectPos, colorRIGHT, MAX_NOTE_EFFECT_DISTANCE, 1f);
                            col = colorRIGHT;
                            break;
                    }
                }
                else
                {
                    tabScript.TapIncrease(true, colorDefault);
                    EffectOnBackGround(effectPos, colorDefault, MAX_NOTE_EFFECT_DISTANCE, 1f);
                }

                note.Anihilate();

                ++combo;
                if (combo < 10)
                    ChangeScore(score + 1);
                else if (combo < 50)
                    ChangeScore(score + 2);
                else
                    ChangeScore(score + 3);

                ChangeMaxScoreNow(NoteType.simple);

                if (combo >= 10)
                {
                    PrintNote("Combo x" + combo.ToString(), effectPos, col);
                }
                else
                {
                    PrintNote("Amazing!", effectPos, col);
                }

            }
            else
            {
                //REFACTOR!
                tabScript.TapIncrease(false, Color.white);
                PrintNote("Missed!", effectPos, Color.white);
                EffectOnBackGround(effectPos, new Color(0.8f,0.8f,0.8f), MAX_NOTE_EFFECT_DISTANCE, 1f);
                combo = 0;
            }
        }
        else
        {
            tabScript.TapIncrease(false, Color.white);
            PrintNote("Missed!", effectPos, Color.white);
            EffectOnBackGround(effectPos, new Color(0.8f, 0.8f, 0.8f), MAX_NOTE_EFFECT_DISTANCE, 1f);
            combo = 0;
        }
    }

    public void SaveRecords()
    {
        if (score > transportedData.currentSong.songRecord)
        {
            TransportedData.scoreDelta = score - transportedData.currentSong.songRecord;
        }
        transportedData.currentSong.lastTryScore = score;
        PlayerPrefs.SetInt(MyUtitities.scoreSaveFlag + "lastTry__" + transportedData.currentSong.songName, transportedData.currentSong.lastTryScore);
        //DONT DO IT EVERY TIME!!
        PlayerPrefs.SetInt(MyUtitities.scoreSaveFlag + "max__" + transportedData.currentSong.songName, maxScore);
        if (transportedData.currentSong.lastTryScore > transportedData.currentSong.songRecord)
        {
            transportedData.currentSong.songRecord = transportedData.currentSong.lastTryScore;
            PlayerPrefs.SetInt(MyUtitities.scoreSaveFlag + transportedData.currentSong.songName, transportedData.currentSong.songRecord);
        }
    }

    public void ExitGamePlay()
    {
        sTransition.FadeOut();
        SaveRecords();
        Invoke("MoveToMenu", 1);
    }

    void MoveToMenu()
    {
        if (inMultiplayer)
        {
            if (score > NetworkStatus.hisScore)
            {
                NetworkStatus.isActive = false;
                SceneManager.LoadScene("LANResults");
            }
            else if (score < NetworkStatus.hisScore)
            {
                NetworkStatus.isActive = false;
                SceneManager.LoadScene("LANResults1");
            }
            else
            {
                NetworkStatus.isActive = false;
                //Ничья
                SceneManager.LoadScene("LANResults2");
            }
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    void ChangeScore(int value)
    {
        score = value;
        te.text = score.ToString();
        scoreIndicatorT.localPosition = new Vector3((float)score/maxScore, scoreIndicatorT.localPosition.y, scoreIndicatorT.localPosition.z);
        scoreSladerT.localScale = new Vector3((float)score / maxScore, 1, 1);
        if (inMultiplayer)
        {
            NetworkStatus.myScore = score;
            NetworkStatus.SetScoreInterruption(true);

            float myZ = -1;
            float hisZ = -1;
            if (NetworkStatus.hisScore > score)
                myZ = -2;
            else
                hisZ = -2;
            hisScoreSliderT.transform.position = new Vector3(hisScoreSliderT.transform.position.x, hisScoreSliderT.transform.position.y, hisZ);
            scoreSladerT.transform.position = new Vector3(scoreSladerT.transform.position.x, scoreSladerT.transform.position.y, myZ);
        }
    }


    public void NET_Change2dPlayerScore()
    {
        if (!inMultiplayer)
        {
            Debug.LogError("Called function NET_Change2dPlayerScore outside of multiplayer.");
            return;
        }
        
        
        hisScoreText.text = NetworkStatus.hisScore.ToString();
        hisScoreIndicatorT.localPosition = new Vector3((float)NetworkStatus.hisScore / maxScore, hisScoreIndicatorT.localPosition.y, hisScoreIndicatorT.localPosition.z);
        hisScoreSliderT.localScale = new Vector3((float)NetworkStatus.hisScore / maxScore, 1, 1);

        float myZ = -1;
        float hisZ = -1;
        if (NetworkStatus.hisScore > score)
            myZ = -2;
        else
            hisZ = -2;
        hisScoreSliderT.transform.position = new Vector3(hisScoreSliderT.transform.position.x, hisScoreSliderT.transform.position.y, hisZ);
        scoreSladerT.transform.position = new Vector3(scoreSladerT.transform.position.x, scoreSladerT.transform.position.y, myZ);
    }

    public int GetScore()
    {
        return score;
    }

    public void ChangeMaxScoreNow(NoteType type)
    {
        ++maxComboNow;
        if (type == NoteType.simple)
        {
            if (maxComboNow < 10)
                ++maxScoreNow;
            else if (maxComboNow < 50)
                maxScoreNow+=2;
            else
                maxScoreNow+=3;
        }
        if (type == NoteType.accNote)
        {
            if (maxComboNow < 10)
                maxScoreNow += 2;
            else if (maxComboNow < 50)
                maxScoreNow += 5;
            else
                maxScoreNow += 10;
        }
        Debug.Log(maxScoreNow/maxScore);
        Debug.Log(maxScore);
        maxScoreNowIndicatorT.localPosition = new Vector3((float)maxScoreNow / maxScore, maxScoreNowIndicatorT.localPosition.y, maxScoreNowIndicatorT.localPosition.z);
        maxScoreNowSladerT.localScale = new Vector3((float)maxScoreNow / maxScore, 1, 1);
    }

    public void IsMain()
    { 
        isMain =true;
    }

    public void IsNotMain()
    {
        isMain = false;
    }

}
