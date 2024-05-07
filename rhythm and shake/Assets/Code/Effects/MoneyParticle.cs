using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    Vector3 dest;
    Transform t;
    float velocity;

    void Start()
    {
        t = transform;
        velocity = Random.Range(5f, 8f);
        dest = GameObject.FindGameObjectsWithTag("scoreIcon")[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        t.position = t.position + (Vector3)((Vector2)dest-(Vector2)t.position).normalized*Time.deltaTime*velocity;
        if (t.position.y <= dest.y) Destroy(gameObject); 
    }
}
