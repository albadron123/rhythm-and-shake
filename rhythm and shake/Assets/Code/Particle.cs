using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{ 
    float velocity;

    Vector2 dir;

    Transform t;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        t = transform; 
        velocity = Random.Range(1f, 3f);
        float angle = Random.Range(0, 360);
        dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    // Update is called once per frame
    void Update()
    {
        t.position += (Vector3)dir * Time.deltaTime * velocity;
        sr.color -= new Color(0, 0, 0, 1) * Time.deltaTime * velocity;
        if (sr.color.a <= 0) Destroy(gameObject);
    }
}
