using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GamePlayCode : MonoBehaviour
{
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


    SongGenerator songGenerator;

    enum HState { straight, left, right };
    enum VState { straight, up, down };

    HState hState;
    VState vState;


    int score = 0;
    int bestScore = 0;
    public int combo = 0;

    [SerializeField]
    float inSideAccuracy = 0.85f;
    [SerializeField]
    float inStraightAccuracy = 0.5f;

    void Start()
    {
        myTransform = transform;

        songGenerator = GetComponent<SongGenerator>();

        tabScript1 = tab1Transform.GetComponent<TabScript>();
        tabScript2 = tab2Transform.GetComponent<TabScript>();

        if (!Input.gyro.enabled)
        {
            Input.gyro.enabled = true;
        }

        Screen.orientation = ScreenOrientation.Portrait;

        te.text = "0";

        Vibration.Init();

        sTransition.FadeIn();
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
        if (prevVstate == VState.straight && vState == VState.up)
        {
            //trigger up
            if (tabScriptAcc.notesInJudgementZone.Count != 0)
            {
                List<GameObject> candidatesSorted = tabScriptAcc.notesInJudgementZone.OrderBy(x => x.transform.position.x).ToList();
                Note n = candidatesSorted[candidatesSorted.Count - 1].GetComponent<Note>();
                if (n.dir == ArrowDirection.up)
                {
                    n.Anihilate();
                    Vibration.VibratePop();

                    ++combo;
                    if (combo < 10)
                        score += 2;
                    else if (combo < 50)
                        score += 5;
                    else score += 10;

                    if (combo >= 10)
                    {
                        PrintNote("Combo x" + combo, new Vector3(0, 0, -5f));
                    }
                    else
                    {
                        PrintNote("GREAT UP (+5)!", new Vector3(0, 0, -5f));
                    }
                    te.text = score.ToString();
                }
            }
        }
        if (prevVstate == VState.straight && vState == VState.down)
        {
            //trigger down
            if (tabScriptAcc.notesInJudgementZone.Count != 0)
            {
                List<GameObject> candidatesSorted = tabScriptAcc.notesInJudgementZone.OrderBy(x => x.transform.position.x).ToList();
                Note n = candidatesSorted[candidatesSorted.Count - 1].GetComponent<Note>();
                if (n.dir == ArrowDirection.down)
                {
                    n.Anihilate();
                    Vibration.VibratePop();

                    ++combo;
                    if (combo < 10)
                        score += 2;
                    else if (combo < 50)
                        score += 5;
                    else score += 10;

                    if (combo >= 10)
                    {
                        PrintNote("Combo x" + combo, new Vector3(0, 0, -5f));
                    }
                    else
                    {
                        PrintNote("GREAT DOWN (+5)!", new Vector3(0, 0, -5f));
                    }
                    te.text = score.ToString();
                }
            }
        }
        if (prevHState == HState.straight && hState == HState.left)
        {
            if (tabScriptAcc.notesInJudgementZone.Count != 0)
            {
                List<GameObject> candidatesSorted = tabScriptAcc.notesInJudgementZone.OrderBy(x => x.transform.position.x).ToList();
                Note n = candidatesSorted[candidatesSorted.Count - 1].GetComponent<Note>();
                if (n.dir == ArrowDirection.left)
                {
                    n.Anihilate();
                    Vibration.VibratePop();

                    ++combo;
                    if (combo < 10)
                        score += 2;
                    else if (combo < 50)
                        score += 5;
                    else score += 10;


                    if (combo >= 10)
                    {
                        PrintNote("Combo x" + combo, new Vector3(0, 0, -5f));
                    }
                    else
                    {
                        PrintNote("GREAT LEFT (+5)!", new Vector3(0, 0, -5f));
                    }
                    te.text = score.ToString();
                }
            }
        }
        if (prevHState == HState.straight && hState == HState.right)
        {
            if (tabScriptAcc.notesInJudgementZone.Count != 0)
            {
                List<GameObject> candidatesSorted = tabScriptAcc.notesInJudgementZone.OrderBy(x => x.transform.position.x).ToList();
                Note n = candidatesSorted[candidatesSorted.Count - 1].GetComponent<Note>();
                if (n.dir == ArrowDirection.right)
                {
                    n.Anihilate();
                    Vibration.VibratePop();

                    ++combo;
                    if (combo < 10)
                        score += 2;
                    else if (combo < 50)
                        score += 5;
                    else score += 10;

                    if (combo >= 10)
                    {
                        PrintNote("Combo x" + combo, new Vector3(0, 0, -5f));
                    }
                    else
                    {
                        PrintNote("GREAT RIGHT (+5)!", new Vector3(0, 0, -5f));
                    }
                    te.text = score.ToString();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            TouchTab(tabScript1, new Vector3(0, 0, -2));
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            TouchTab(tabScript2, new Vector3(0, 0, -2));
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
                        TouchTab(tabScript1, pos);
                    }
                    if (pos.x < (tab2Transform.position.x + (tab2Scale.x / 2f)) && pos.x > (tab2Transform.position.x - (tab2Scale.x / 2f)) &&
                        pos.y < (tab2Transform.position.y + (tab2Scale.y / 2f)) && pos.y > (tab2Transform.position.y - (tab2Scale.y / 2f)))
                    {
                        TouchTab(tabScript2, pos);
                    }
                }
            }
        }

    }

    public void PrintNote(string content, Vector3 pos)
    {
        GameObject textObject = Instantiate(textPrefab, pos, Quaternion.identity);
        textObject.transform.parent = canvas.transform;
        textObject.transform.localScale = new Vector3(1, 1, 1);
        textObject.GetComponent<TMPro.TMP_Text>().text = content;
    }

    public void TouchTab(TabScript tabScript, Vector3 pos)
    {
        if (tabScript.notesInJudgementZone.Count != 0)
        {
            tabScript.TapIncrease(true);
            List<GameObject> candidatesSorted = tabScript.notesInJudgementZone.OrderBy(x => x.transform.position.x).ToList();
            candidatesSorted[candidatesSorted.Count - 1].GetComponent<Note>().Anihilate();

            ++combo;
            if (combo < 10)
                ++score;
            else if (combo < 50)
                score += 2;
            else
                score += 3;

            if (combo >= 10)
            {
                PrintNote("Combo x" + combo.ToString(), pos);
            }
            else
            {
                PrintNote("Amazing!", pos);
            }
            te.text = score.ToString();
        }
        else
        {
            tabScript.TapIncrease(false);
            PrintNote("Missed!", pos);
            combo = 0;
        }
    }

    public void ExitGamePlay()
    {
        sTransition.FadeOut();
        Invoke("MoveToMenu", 1);
    }

    void MoveToMenu()
    {
        transportedData.currentSong.lastTryScore = score;
        SceneManager.LoadScene("SampleScene");
    }

}
