using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
//using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class GyroChecker : MonoBehaviour
{

    [SerializeField]
    Text te;


    [SerializeField]
    GameObject textPrefab;



    [SerializeField]
    SpriteRenderer tab1;
    [SerializeField]
    SpriteRenderer tab2;


    [SerializeField]
    GameObject canvas;

    [SerializeField]
    Transform tab1Transform;
    [SerializeField]
    Transform tab2Transform;

    TabScript tabScript1;
    TabScript tabScript2;

    [SerializeField]
    TabScript tabScriptAcc;

    [SerializeField]
    Color tab1Col;
    [SerializeField]
    Color tab2Col;

    [SerializeField]
    GameObject playSongButton;
    [SerializeField]
    GameObject BestScoreTextObject;

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


    bool inMenu = false;

    void Start()
    {
        songGenerator = GetComponent<SongGenerator>();

        tabScript1 = tab1Transform.GetComponent<TabScript>();
        tabScript2 = tab2Transform.GetComponent<TabScript>();

        if (!Input.gyro.enabled)
        {
            Input.gyro.enabled = true;
        }

        Screen.orientation = ScreenOrientation.Portrait;

        te.text = "score: 0";

        Vibration.Init();

        EnterMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inMenu)
        {

            Vector3 acc = Input.acceleration;
            //t.text = Input.acceleration.ToString();
            //t.text = gyroData.ToString();

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
                        te.text = "score: " + score.ToString();
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
                        te.text = "score: " + score.ToString();
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
                        te.text = "score: " + score.ToString();
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
                        te.text = "score: " + score.ToString();
                    }
                }
            }


            if (Input.touchCount > 0)
            {
                Touch[] touches = Input.touches;
                foreach (Touch t in touches)
                {
                    if (t.phase == TouchPhase.Began)
                    {
                        Vector2 pos = Camera.main.ScreenToWorldPoint(t.position);
                        if (pos.x < (tab1Transform.position.x + (3.5 / 2f)) && pos.x > (tab1Transform.position.x - (3.5 / 2f)) &&
                            pos.y < (tab1Transform.position.y + (3.5 / 2f)) && pos.y > (tab1Transform.position.y - (3.5 / 2f)))
                        {
                            if (tabScript1.notesInJudgementZone.Count != 0)
                            {
                                tabScript1.TapIncrease(true);
                                List<GameObject> candidatesSorted = tabScript1.notesInJudgementZone.OrderBy(x => x.transform.position.x).ToList();
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
                                te.text = "score: " + score.ToString();
                            }
                            else
                            {
                                tabScript1.TapIncrease(false);
                                PrintNote("Missed!", pos);
                                combo = 0;
                            }
                        }
                        if (pos.x < (tab2Transform.position.x + (3.5 / 2f)) && pos.x > (tab2Transform.position.x - (3.5 / 2f)) &&
                            pos.y < (tab2Transform.position.y + (3.5 / 2f)) && pos.y > (tab2Transform.position.y - (3.5 / 2f)))
                        {

                            if (tabScript2.notesInJudgementZone.Count != 0)
                            {
                                tabScript2.TapIncrease(true);
                                List<GameObject> candidatesSorted = tabScript2.notesInJudgementZone.OrderBy(x => x.transform.position.x).ToList();
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
                                te.text = "score: " + score.ToString();
                            }
                            else
                            {
                                tabScript2.TapIncrease(false);
                                PrintNote("Missed!", pos);
                                combo = 0;
                            }
                        }
                    }
                }
            }
            ///end of gameplay part

        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch[] touches = Input.touches;
                foreach (Touch t in touches)
                {
                    if (t.phase == TouchPhase.Began)
                    {
                        Vector2 pos = Camera.main.ScreenToWorldPoint(t.position);
                        if (pos.x < (tab1Transform.position.x + (3.5 / 2f)) && pos.x > (tab1Transform.position.x - (3.5 / 2f)) &&
                            pos.y < (tab1Transform.position.y + (3.5 / 2f)) && pos.y > (tab1Transform.position.y - (3.5 / 2f)))
                        {
                            tabScript1.TapIncrease(true);
                            ExitMenu();
                            songGenerator.StartSong();
                        }
                    }
                }
            }
        }
    }


    public void PrintNote(string content, Vector2 pos)
    {
        GameObject textObject = Instantiate(textPrefab, pos, Quaternion.identity, canvas.transform);
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
        textObject.GetComponent<RectTransform>().anchoredPosition = screenPoint - canvas.GetComponent<RectTransform>().sizeDelta / 2f;
        textObject.GetComponent<Text>().text = content;
    }
    void ExitMenu()
    {
        playSongButton.SetActive(false);
        BestScoreTextObject.SetActive(false);
        te.gameObject.SetActive(true);
        inMenu = false;
        score = 0;
        combo = 0;
        te.text = "score:" + score;
    }

    public void EnterMenu()
    {
        playSongButton.SetActive(true);
        BestScoreTextObject.SetActive(true);
        te.gameObject.SetActive(false);
        if (score > bestScore)
        {
            bestScore = score;
        }
        if(bestScore >= 491) 
            BestScoreTextObject.GetComponent<Text>().text = "Best score:\n" + bestScore + " (A+)";
        else if (bestScore > 400)
            BestScoreTextObject.GetComponent<Text>().text = "Best score:\n" + bestScore + " (A)";
        else if (bestScore > 350)
            BestScoreTextObject.GetComponent<Text>().text = "Best score:\n" + bestScore + "(B)";
        else if (bestScore > 300)
            BestScoreTextObject.GetComponent<Text>().text = "Best score:\n" + bestScore + "(C)";
        else
            BestScoreTextObject.GetComponent<Text>().text = "Best score:\n" + bestScore;
        inMenu = true;
    }

}
