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

    [SerializeField]
    GameplayServer gs;

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
            //probably do it later with check if change is possible
            levelSelection.RotateCurrentTabBack();
            levelSelection.ForceChangeSong(-1);
        }
        if (buttonType == ButtonType.rightShift)
        {
            //rotates it back if needed
            levelSelection.RotateCurrentTabBack();
            levelSelection.ForceChangeSong(1);
        }
        if (buttonType == ButtonType.play)
        {
            if (LevelSelection.currentSongInMultiplayer)
            {
                //start and finish server later
                //now just start if not started
                gs.StartServer();
            }
            else
            {
                levelSelection.PlaySong();
            }
        }
        sr.color = new Color(1, 1, 1, 0.2f);
    }

    private void OnMouseUp()
    {
        sr.color = new Color(1, 1, 1, 0); ;
    }
}
