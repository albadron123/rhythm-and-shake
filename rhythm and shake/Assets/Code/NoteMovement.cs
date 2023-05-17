using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMovement : MonoBehaviour
{
    [SerializeField]
    float velocity;

    RectTransform rt;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        Destroy(gameObject, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        rt.position += Vector3.up * Time.deltaTime * velocity;
    }

    
}
