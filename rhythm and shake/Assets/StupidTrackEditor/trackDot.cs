using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trackDot : MonoBehaviour
{
    public int id;
    public int lineId;
    public ArrowDirection dir;
    public bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        isActive = !isActive;
        if (isActive)
        {
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.grey;
        }
    }
}
