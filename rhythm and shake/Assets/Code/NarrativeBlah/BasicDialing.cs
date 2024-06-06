using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[System.Serializable]
public class Line
{
    public string text;
    public float time;
    public string beforeLineFunctionName = "";
    public string afterLineFunctionName = "";
    public bool terminate = false;
    public bool skipable = true;
    public bool interactable = true;
    public Vector2 position = new Vector2(0, 0.6f);
}

public class BasicDialing : MonoBehaviour
{
    [SerializeField]
    GameObject tab1;
    [SerializeField]
    GameObject tab2;
    [SerializeField]
    GameObject tabAcc;
    [SerializeField]
    GameObject scores;

    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    GameObject callMenu;
    [SerializeField]
    GameObject rightLeft;

    [SerializeField]
    List<Line> lines = new List<Line>();


    [SerializeField]
    List<Line> lines2Intro = new List<Line>();


    [SerializeField]
    List<SpriteRenderer> pictures;
    [SerializeField]
    List<AudioClip> clips;


    Line current;

    [SerializeField]
    TMPro.TMP_Text te;
    [SerializeField]
    RectTransform textRT;

    List<GameObject> testNotes = new List<GameObject>();

    float timer = 0;
    float time = 0;

    int currentIndex;

    bool inDialogue = true;

    void Start()
    {
        Input.simulateMouseWithTouches = true;
        textRT = te.gameObject.GetComponent<RectTransform>();

        currentIndex = 0;
        current = lines[currentIndex];
        
        StartLine();
    }

    void Update()
    {

        string stringJson = JsonUtility.ToJson(lines);
        Debug.Log(stringJson);
        if (inDialogue)
        {
            timer += Time.deltaTime;
            if (current.interactable && ((!current.skipable && timer >= time) || (current.skipable && Input.GetMouseButtonDown(0))))
            {
                FinishLine();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Tutorial2();
            }
        }
    }


    public void InitDialogue(List<Line> dialogue)
    {
        lines = dialogue;
        currentIndex = 0;
        current = lines[currentIndex];
        te.enabled = true;
        inDialogue = true;
        StartLine();
    }

    public void SkipTo(int index)
    {
        currentIndex = index;
        current = lines[currentIndex];
        StartLine();
    }

    public void ShowPicture(int index)
    {
        for (int i = 0; i < pictures.Count; ++i)
        {
            pictures[i].enabled = (i == index);
        }
    }

    public void HideAllPictures()
    {
        for (int i = 0; i < pictures.Count; ++i)
        {
            pictures[i].enabled = false;
        }
    }

    public void StartClip(int index)
    {
        audioSource.clip = clips[index];
        audioSource.Play();
    }

    public void StopClip()
    {
        audioSource.Stop();
    }

    void Tutorial2()
    {
        InitDialogue(lines2Intro);
    }


    public void StartLine()
    {
        if (current.beforeLineFunctionName != "")
            Invoke(current.beforeLineFunctionName, 0);
        te.text = current.text;
        textRT.position = current.position;
        timer = 0;
        time = current.time;
    }

    public void FinishLine()
    {
        if (current.afterLineFunctionName != "")
            Invoke(current.afterLineFunctionName, 0);
        if (!current.terminate)
        {
            ++currentIndex;
            current = lines[currentIndex];
            StartLine();
        }
        else
        {
            inDialogue = false;
            te.enabled = false;
        }
    }

    /// <summary>
    /// //////////////////////////////
    /// </summary>
    void StartTutorial()
    {
        tab1.SetActive(true);
    }

    void ActivateTab2()
    {
        tab2.SetActive(true);
    }




    [SerializeField]
    GameObject classicNotePrefab;
    [SerializeField]
    GameObject leftNotePrefab;
    [SerializeField]
    GameObject rightNotePrefab;
    [SerializeField]
    GameObject upNotePrefab;
    [SerializeField]
    GameObject downNotePrefab;
    [SerializeField]
    GameObject leftArrowPrefab;
    [SerializeField]
    GameObject rightArrowPrefab;
    [SerializeField]
    GameObject upArrowPrefab;
    [SerializeField]
    GameObject downArrowPrefab;
    float trackVelocity = 1.75f;
    public void GenerateNote(Vector2 position, ArrowDirection dir)
    {
        GameObject notePrefab;
        switch (dir)
        {
            case ArrowDirection.up: notePrefab = upNotePrefab; break;
            case ArrowDirection.down: notePrefab = downNotePrefab; break;
            case ArrowDirection.left: notePrefab = leftNotePrefab; break;
            case ArrowDirection.right: notePrefab = rightNotePrefab; break;
            default: notePrefab = classicNotePrefab; break;
        }
        GameObject noteInstance = Instantiate(notePrefab, position, Quaternion.identity);
        Note note = noteInstance.GetComponent<Note>();


        note.velocity = trackVelocity;
        note.dir = dir;
    }


    void PlayOneNote()
    {
        GenerateNote(tab1.gameObject.transform.position - new Vector3(3, 0, 0), ArrowDirection.no);
    }

    public void PlayThreeNotes()
    {
        GenerateNote(tab1.gameObject.transform.position - new Vector3(3, 0, 0), ArrowDirection.no);
        GenerateNote(tab2.gameObject.transform.position - new Vector3(5, 0, 0), ArrowDirection.no);
        GenerateNote(tab2.gameObject.transform.position - new Vector3(8, 0, 0), ArrowDirection.no);
    }

    public static void DestroyAllNotesOnFail()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("NOTETAG");
        foreach(GameObject o in objs)
        {
            o.GetComponent<Note>().Anihilate();
        }
    }

    void RepeatPressTab()
    {
        SkipTo(10);
    }


    void ShowCallMenu()
    {
        HideAllPictures();
        callMenu.SetActive(true);
    }

    void HideCallMenu()
    {
        callMenu.SetActive(false);
    }

    public void RejectCall()
    {
        if (currentIndex <= 9)
        {
            if (currentIndex == 9)
            {
                HideCallMenu();
                StopClip();
            }
            SkipTo(currentIndex + 1);
        }
    }

    public void AcceptCall()
    {
        HideCallMenu();
        StopClip();
        SkipTo(11);   
    }

    public void ActivateRightLeftChoise()
    {
        rightLeft.SetActive(true);
    }

    public void LeftHandChoise()
    {
        rightLeft.SetActive(false);
        TransportedData.handMode = HandMode.left;
        PlayerPrefs.SetInt("HAND", (int)TransportedData.handMode);
        HideAllPictures();
        SkipTo(32);
    }

    public void RightHandChoise()
    {
        rightLeft.SetActive(false);
        TransportedData.handMode = HandMode.right;
        PlayerPrefs.SetInt("HAND", (int)TransportedData.handMode);
        HideAllPictures();
        SkipTo(32);   
    }


    public void Next()
    {
        SkipTo(currentIndex+1);
    }


    public void TerminateTutorial()
    {
        if (!PlayerPrefs.HasKey("TUTORIAL_DONE"))
        {
            PlayerPrefs.SetInt("TUTORIAL_DONE", 1);
        }
        SceneManager.LoadScene("Loading");
    }

    void ShowPicture0(){ShowPicture(0);}
    void ShowPicture1(){ShowPicture(1);}
    void ShowPicture2(){ShowPicture(2);}
    void ShowPicture3(){ShowPicture(3);}
    void ShowPicture4(){ShowPicture(4);}
    void ShowPicture5(){ShowPicture(5);}
    void StartClip0() { StartClip(0);}
    void StartClip1() { StartClip(1);}
    void StartClip2() { StartClip(2);}
    void StartClip3() { StartClip(3);}

    void SkipTo12() { SkipTo(12); }
}
