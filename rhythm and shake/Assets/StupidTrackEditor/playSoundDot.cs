using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playSoundDot : MonoBehaviour
{
    AudioSource a;

    public float time;
    void Start()
    {
        a = GameObject.Find("Main Camera").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        a.time = time;
        a.Play();

    }
}
