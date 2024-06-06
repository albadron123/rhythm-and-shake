using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkSupport : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject pauseBack;

    public Button pauseButton;
    public Button startButton;
    public TMPro.TMP_Text statusText;

    //inPause
    public Image IamReadyIndicator;
    public TMPro.TMP_Text IamReadyText;
    public Image heIsReadyIndicator;
    public TMPro.TMP_Text heIsReadyText;
    public TMPro.TMP_Text te;
}
