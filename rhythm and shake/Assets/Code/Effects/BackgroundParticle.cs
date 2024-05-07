using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParticle : MonoBehaviour
{
    SpriteRenderer sr;
    Transform t;

    Color initialColor;
    float colorPortion;

    float initialRotVelocity;

    public float rotationalVelocity;
    public int rotationalDirection;

    float colorSeed;

    Vector3 initalPosition;

    Vector3 direction;
    bool movingToDir = false;

    bool scaling = false;
    float initialScale;
    int scaleDirection;
    float scaleVelocity = 1.5f;
    float maxScale;
    float scale;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        t = transform;
        initalPosition = t.position;

        colorSeed = Random.Range(0, 0.1f);
        float val = Random.Range(0.2f, 0.4f);
        initialColor = new Color(val, val, val);
        sr.color = initialColor;
        colorPortion = 1;

        initialRotVelocity = Random.Range(6, 12);
        rotationalVelocity = initialRotVelocity;
        rotationalDirection = Random.Range(0, 2) == 1 ? 1 : -1;

        movingToDir = false;
        scaling = false;
        colorSeed = Random.Range(0, 0.1f);


        scale = transform.localScale.x;
        maxScale = scale * 1.2f;
        initialScale = scale;
    }

    void Update()
    {
        t.rotation = Quaternion.Euler(t.eulerAngles + new Vector3(0, 0, rotationalDirection * rotationalVelocity) * Time.deltaTime);

        if (rotationalVelocity > initialRotVelocity)
        {
            rotationalVelocity -= Time.deltaTime;
        }

        //A little bit better (but still should be more optimized)
        if (movingToDir)
        {
            Vector3 newPos = Vector3.MoveTowards(t.position, direction, 5 * Time.deltaTime);
            if (newPos == t.position)
                movingToDir = false;
            t.position = newPos;
        }
        else
        {
            t.position = Vector3.MoveTowards(t.position, initalPosition, Time.deltaTime);
        }
        /*
        if (Mathf.Abs(sr.color.r - sr.color.g) > 0.1f || Mathf.Abs(sr.color.b - sr.color.g) > 0.1f)
        {
            sr.color -= new Color(1, 1, 1) * 2 * colorSeed * Time.deltaTime + new Color(Mathf.Sign(sr.color.r - sr.color.b), Mathf.Sign(sr.color.g - sr.color.b), 0) * Time.deltaTime * (colorSeed+0.1f)*2;
        }
        else
        {
            colorSeed = Random.Range(0.08f, 0.1f);
        }
        */
        if (sr.color != initialColor)
        {
            colorPortion -= 0.8f*Time.deltaTime;
            if (colorPortion < 0) colorPortion = 0;
            sr.color = Color.Lerp(initialColor, sr.color, colorPortion);
        }

        if (scaling)
        {
            if (scaleDirection == 1)
            {
                scale += scaleVelocity * scaleVelocity * Time.deltaTime;
                if (scale > maxScale)
                {
                    scaleDirection = -1;
                }
                t.localScale = new Vector3(scale, scale, 1);
            }
            else
            {
                scale -= scaleVelocity * scaleVelocity * Time.deltaTime;
                if (scale < initialScale)
                {
                    scaling = false;
                }
                t.localScale = new Vector3(scale, scale, 1);
            }
        }

    }

    public void SetColor(Color col)
    {
        colorPortion = 1;
        sr.color = col;
        float val = Random.Range(-0.05f, 0.05f);
        initialColor = new Color(sr.color.r*Random.Range(0.05f,0.2f)+val, sr.color.r * Random.Range(0.05f, 0.2f)+val, sr.color.r * Random.Range(0.05f, 0.2f)+val);
    }

    public void MoveTo(Vector2 direction)
    {
        movingToDir = true;
        this.direction = initalPosition + (Vector3)direction;
    }

    public void ScaleWave()
    {
        if (scaling)
        {
            scale = initialScale;
            t.localScale = new Vector3(scale, scale, 1);
        }
        scaling = true;
        scaleDirection = 1;
    }

}
