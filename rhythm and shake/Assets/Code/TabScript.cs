using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class TabScript : MonoBehaviour
{
    public List<GameObject> notesInJudgementZone = new List<GameObject>();

    [SerializeField]
    Color tabCol;

    SpriteRenderer sr;

    Transform t;


    bool increasing = false;
    bool decreacing = false;

    [SerializeField]
    float maxSize;
    [SerializeField]
    float minSize;

    [SerializeField]
    float increaseVelocity;
    [SerializeField]
    float decreaseVelocity;

    void Start()
    {
        t = transform;
        sr = GetComponent<SpriteRenderer>(); 
    }

    void Update()
    {
        if (increasing)
        {
            t.localScale += new Vector3(1, 1, 0) * increaseVelocity * increaseVelocity * Time.deltaTime;
            if (t.localScale.x > maxSize)
            {
                increasing = false;
                decreacing = true;
                t.localScale = new Vector3(maxSize, maxSize, 1);
            }
        }
        if (decreacing)
        {
            t.localScale -= new Vector3(1,1,0) * decreaseVelocity * decreaseVelocity * Time.deltaTime;
            if (t.localScale.x <= minSize)
            {
                decreacing = false;
                sr.color = Color.white;
                t.localScale = new Vector3(minSize, minSize, 1);
            }
        }

    }

    public void TapIncrease(bool success)
    {
        if (success)
        {
            sr.color = tabCol;
        }
        t.localScale = new Vector3(minSize, minSize, 1);
        increasing = true; 
    }


}
