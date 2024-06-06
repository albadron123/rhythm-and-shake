using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerButton : MonoBehaviour
{
    public enum ButtonType { leftShift, rightShift, play, server};
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
            levelSelection.PlaySong();
        }
        if (buttonType == ButtonType.server)
        {
            NetServer server = GameObject.Find("NET").GetComponent<NetServer>();
            server.ConnectDisconnect();
            //later shutDownServer
        }
        sr.color = new Color(1, 1, 1, 0.2f);
    }

    private void OnMouseUp()
    {
        sr.color = new Color(1, 1, 1, 0); ;
    }
}
