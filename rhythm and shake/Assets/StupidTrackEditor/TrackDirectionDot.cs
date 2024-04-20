using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackDirectionDot : MonoBehaviour
{
    ArrowDirection[] li = { ArrowDirection.no, ArrowDirection.up, ArrowDirection.down, ArrowDirection.left, ArrowDirection.right };
    [SerializeField] Sprite[] spriteStates;
    int[] rotations = { 0, 0, 180, 90, -90 };
    trackDot t;
    SpriteRenderer sr;
    Transform tr;
    int state = 0;
    void Start()
    {
        tr = transform;
        sr = GetComponent<SpriteRenderer>();
        t = transform.parent.GetComponent<trackDot>();
        state = 0;
        t.dir = li[state];
        tr.rotation = Quaternion.Euler(new Vector3(0, 0, rotations[state]));
        sr.sprite = spriteStates[state];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        ++state;
        if (state == li.Length)
        {
            state = 0;
        }
        t.dir = li[state];
        tr.rotation = Quaternion.Euler(new Vector3(0, 0, rotations[state]));
        sr.sprite = spriteStates[state];
    }
}
