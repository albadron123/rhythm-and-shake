using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Line
{
    public string text;
    public float time;
    public string beforeLineFunctionName = "";
    public string afterLineFunctionName = "";
    public bool terminate = false;
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
















    /// <summary>
    /// ////////////////////
    /// </summary>

    [SerializeField]
    List<Line> lines = new List<Line>();

    Line current;

    [SerializeField]
    TMPro.TMP_Text te;

    float timer = 0;
    float time = 0;

    int currentIndex;

    bool inDialogue = true;

    void Start()
    {
        currentIndex = 0;
        current = lines[currentIndex];
        StartLine();
    }

    void Update()
    {
        if (inDialogue)
        {
            timer += Time.deltaTime;
            if (timer >= time)
            {
                FinishLine();
            }
        }
    }


    void StartLine()
    {
        if (current.beforeLineFunctionName != "")
            Invoke(current.beforeLineFunctionName, 0);
        te.text = current.text;
        timer = 0;
        time = current.time;
    }

    void FinishLine()
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
        }
    }

    /// <summary>
    /// //////////////////////////////
    /// </summary>
    void StartTutorial()
    {
        te.rectTransform.position += new Vector3(0, 1.5f, 0);
        tab1.SetActive(true);
    }
}
