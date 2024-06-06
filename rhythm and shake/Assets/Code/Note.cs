using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Note : MonoBehaviour
{
    public NoteType type;

    [SerializeField]
    GameObject plarticle;

    public float velocity = 1;

    Transform t;

    bool inJundgementZone = false;

    TabScript myTabScript = null;

    SpriteRenderer sr;

    public ArrowDirection dir;

    public bool hasDir = false;

    GamePlayCode gameplay;

    bool inTutorial = false;

    void Start()
    {
        gameplay = GameObject.Find("Main Camera").GetComponent<GamePlayCode>();
        sr = GetComponent<SpriteRenderer>();
        t = transform;
        if (GameObject.Find("Main Camera").GetComponent<BasicDialing>() != null) inTutorial = true;
        else inTutorial = false;
    }

    void Update()
    {
        if (inTutorial)
        {
            t.position += Vector3.right * Time.deltaTime * velocity * 0.75f;
            if (t.position.x > 2.5f)
            {
                if (TutorialEvents.tutorialStage > 1)
                {
                    TutorialTab.successCount = 0;
                    BasicDialing.DestroyAllNotesOnFail();
                    //failedDialogue3
                    //YOU SHOULD PUT FAIL MESSAGE HERE!!!!!!!!!!!!!!!!!!!!!
                    //YOU SHOULD PUT FAIL MESSAGE HERE!!!!!!!!!!!!!!!!!!!!!
                    //YOU SHOULD PUT FAIL MESSAGE HERE!!!!!!!!!!!!!!!!!!!!!
                    //YOU SHOULD PUT FAIL MESSAGE HERE!!!!!!!!!!!!!!!!!!!!!                                         TUTORIAL MISTAKE
                    //YOU SHOULD PUT FAIL MESSAGE HERE!!!!!!!!!!!!!!!!!!!!!
                    //YOU SHOULD PUT FAIL MESSAGE HERE!!!!!!!!!!!!!!!!!!!!!
                    //YOU SHOULD PUT FAIL MESSAGE HERE!!!!!!!!!!!!!!!!!!!!!
                    //YOU SHOULD PUT FAIL MESSAGE HERE!!!!!!!!!!!!!!!!!!!!!
                    //YOU SHOULD PUT FAIL MESSAGE HERE!!!!!!!!!!!!!!!!!!!!!
                    //YOU SHOULD PUT FAIL MESSAGE HERE!!!!!!!!!!!!!!!!!!!!!
                    //YOU SHOULD PUT FAIL MESSAGE HERE!!!!!!!!!!!!!!!!!!!!!
                    BasicDialing db = GameObject.Find("Main Camera").GetComponent<BasicDialing>();
                    db.PlayThreeNotes();
                    db.SkipTo(42);
                }
                else if(TutorialEvents.tutorialStage == 1) 
                    GameObject.Find("Main Camera").GetComponent<SongGenerator>().GenerateNote(new Vector3(0, transform.position.y, transform.position.z) - new Vector3(3, 0, 0), ArrowDirection.no);
                Destroy(gameObject);
            }
        }
        else
        {
            if (-Mathf.Sign((float)TransportedData.handMode - 0.5f) * t.position.x > 10)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Anihilate()
    {
        int amount = Random.Range(15, 25);
        for (int i = 0; i < amount; ++i)
        {
            GameObject particleInstance = Instantiate(plarticle, t.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))));
            if (sr != null)
            {
                particleInstance.GetComponent<SpriteRenderer>().color = sr.color;
                particleInstance.transform.localScale = new Vector3(Random.Range(0.05f, 0.15f), Random.Range(0.05f, 0.15f), 1);
            }
            else
            {
                particleInstance.GetComponent<SpriteRenderer>().color = Color.white;
                particleInstance.transform.localScale = new Vector3(Random.Range(0.15f, 0.35f), Random.Range(0.15f, 0.35f), 1);
            }
        }
        if (inJundgementZone)
        {
            myTabScript.notesInJudgementZone.Remove(gameObject);
            inJundgementZone = false; 
        }
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "tab")
        {
            myTabScript = collision.gameObject.GetComponent<TabScript>();
            myTabScript.notesInJudgementZone.Add(gameObject);
            inJundgementZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "tab")
        {
            if (inJundgementZone)
            {
                myTabScript.notesInJudgementZone.Remove(gameObject);
                inJundgementZone = false;
                gameplay.PrintNote("Too late!", t.position, sr.color);
                gameplay.combo = 0;
                gameplay.ChangeMaxScoreNow(type);
            }
        }
    }

    
}
