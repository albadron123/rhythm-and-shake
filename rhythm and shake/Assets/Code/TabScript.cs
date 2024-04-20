using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class TabScript : MonoBehaviour
{
    public List<GameObject> notesInJudgementZone = new List<GameObject>();

    SpriteRenderer sr;

    Transform t;


    bool increasing = false;
    bool decreacing = false;

    bool rotating = false;
    int rotDir = 0;

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
        if (rotating)
        {
            float delta = Time.deltaTime * 1000 * rotDir;
            if (rotDir > 0 && Mathf.Abs(t.rotation.eulerAngles.y - 180) < Mathf.Abs(delta))
            {
                rotating = false;
                t.rotation = Quaternion.Euler(0, 179.99f, 0);
            }
            else if (rotDir < 0 && Mathf.Abs(t.rotation.eulerAngles.y - 0) < Mathf.Abs(delta))
            {
                rotating = false;
                t.rotation = Quaternion.identity;
            }
            else
            {
                t.rotation = Quaternion.Euler(0, t.rotation.eulerAngles.y + delta, 0);
            }

        }

    }

    public void TapIncrease(bool success, Color c)
    {
        if (success)
        {
            sr.color = c;
        }
        t.localScale = new Vector3(minSize, minSize, 1);
        increasing = true; 
    }

    public void TabRotate(int dir)
    {
        rotDir = dir;
        rotating = true;
    }


}
