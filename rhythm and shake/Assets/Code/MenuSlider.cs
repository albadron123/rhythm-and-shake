using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSlider : MonoBehaviour
{

    int score;
    int maxScore;
    int record;
    bool inProcess;
    [SerializeField]
    float processTime;

    [SerializeField]
    Transform s1;
    [SerializeField]
    Transform s2;
    [SerializeField]
    Transform p1;
    [SerializeField]
    Transform p2;

    float timer = 0;

    [SerializeField]
    TMPro.TMP_Text sText;
    [SerializeField]
    TMPro.TMP_Text rText;

    void Start()
    {
    }

    void Update()
    {

        if (inProcess)
        {
            if (timer + Time.deltaTime > processTime)
            {
                timer = processTime;
                inProcess = false;
            }
            else
            {
                timer += Time.deltaTime;
            }
            float progress = timer / processTime;
            s1.localScale = new Vector3(score/(float)maxScore*progress*4, s1.localScale.y, s1.localScale.z);
            s2.localScale = new Vector3(record/(float)maxScore*progress*4, s2.localScale.y, s2.localScale.z);
            p1.localPosition = new Vector3(-2+score/(float)maxScore*progress*4, p1.localPosition.y, p1.localPosition.z);
            p2.localPosition = new Vector3(-2+record/(float)maxScore*progress*4, p2.localPosition.y, p2.localPosition.z);
            sText.text = ((int)(score * progress)).ToString();
            rText.text = ((int)(record * progress)).ToString();
        }
    }

    public void Reset(int score, int record, int maxScore)
    {
        this.score = score;
        this.maxScore = maxScore;
        this.record = record;
        timer = 0;
        inProcess = true;
    }
}
