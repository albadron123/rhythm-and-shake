using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class So : MonoBehaviour
{
    int dir = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += new Vector3(1,1,1) * Time.deltaTime * dir;
    }

    private void OnMouseDown()
    {
        dir = 1;
    }

    private void OnMouseUp()
    {
        dir = 0;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
}
