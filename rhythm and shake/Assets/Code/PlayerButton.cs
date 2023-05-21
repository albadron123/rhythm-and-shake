using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerButton : MonoBehaviour
{
    public enum ButtonType { leftShift, rightShift, play};
    [SerializeField]
    ButtonType buttonType;

    [SerializeField]
    LevelSelection levelSelection;

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
            levelSelection.ForceChangeSong(-1);
        }
        if (buttonType == ButtonType.rightShift)
        {
            levelSelection.ForceChangeSong(1);
        }
        if (buttonType == ButtonType.play)
        {
            levelSelection.PlaySong();
        }
        sr.color = new Color(1, 1, 1, 0.2f);
    }

    private void OnMouseUp()
    {
        sr.color = new Color(1, 1, 1, 0); ;
    }
}
