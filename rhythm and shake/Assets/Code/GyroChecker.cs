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
    List<Song> songs = new List<Song>();
    int currentSongIndex = 1;


    [SerializeField]
    TMPro.TMP_Text te;

    [SerializeField]
    TMPro.TMP_Text SongNameText;
    [SerializeField]
    TMPro.TMP_Text SongAuthorText;



    [SerializeField]
    GameObject textPrefab;

    Transform myTransform;

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

    bool isCameraMoving = false;
    float destinationX;
    [SerializeField]
    float cameraShift;
    [SerializeField]
    float cameraVelocty;

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

        te.text = "score: 0";

        Vibration.Init();

        EnterMenu();
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

        if (!inMenu)
        {

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
                        Vector2 pos = -Camera.main.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, -9));
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
            ///end of gameplay part

        }
        else
        {
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
                tabScript1.TapIncrease(true);
                ExitMenu();
                songGenerator.StartSong();
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
                            ExitMenu();
                            songGenerator.StartSong();
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
    }


    public void PrintNote(string content, Vector2 pos)
    {
        GameObject textObject = Instantiate(textPrefab, pos, Quaternion.identity, canvas.transform);
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
        textObject.GetComponent<RectTransform>().anchoredPosition = screenPoint - canvas.GetComponent<RectTransform>().sizeDelta / 2f;
        textObject.GetComponent<TMPro.TMP_Text>().text = content;
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
            BestScoreTextObject.GetComponent<TMPro.TMP_Text>().text = "record: " + bestScore + " (A+)";
        else if (bestScore > 400)
            BestScoreTextObject.GetComponent<TMPro.TMP_Text>().text = "record: " + bestScore + " (A)";
        else if (bestScore > 350)
            BestScoreTextObject.GetComponent<TMPro.TMP_Text>().text = "record: " + bestScore + " (B)";
        else if (bestScore > 300)
            BestScoreTextObject.GetComponent<TMPro.TMP_Text>().text = "record: " + bestScore + " (C)";
        else
            BestScoreTextObject.GetComponent<TMPro.TMP_Text>().text = "record: " + bestScore;
        inMenu = true;

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
            te.text = "score: " + score.ToString();
        }
        else
        {
            tabScript.TapIncrease(false);
            PrintNote("Missed!", pos);
            combo = 0;
        }
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
        //later we need to load record from here and to here
        //we also need to play song in this place (may be)
    }

}
