using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerButton : MonoBehaviour
{
    public enum ButtonType { leftShift, rightShift, play};
    [SerializeField]
    ButtonType buttonType;

    [SerializeField]
    GyroChecker globalScript;

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();        
    }

    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (buttonType == ButtonType.leftShift)
        {
            globalScript.ForceChangeSong(-1);
        }
        if (buttonType == ButtonType.rightShift)
        {
            globalScript.ForceChangeSong(1);
        }
        sr.color = new Color(1, 1, 1, 0.2f);
    }

    private void OnMouseUp()
    {
        sr.color = new Color(1, 1, 1, 0); ;
    }
}
