using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{

    [SerializeField]
    GameObject plarticle;

    public float velocity;

    Transform t;

    bool inJundgementZone = false;

    TabScript myTabScript = null;

    SpriteRenderer sr;

    public ArrowDirection dir;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        t = transform;
    }

    void Update()
    {
        t.position += Vector3.right * Time.deltaTime * velocity;
        if (t.position.x > 10)
        {
            Destroy(gameObject);
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
                GyroChecker gc = GameObject.Find("Main Camera").GetComponent<GyroChecker>();
                gc.PrintNote("Too late!", t.position);
                gc.combo = 0;
            }
        }
    }
}
