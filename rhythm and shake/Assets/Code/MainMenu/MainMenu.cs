using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Components of main menu
    [SerializeField]
    TMPro.TMP_Text pressToPlayText;
    [SerializeField]
    TMPro.TMP_Text shopkeeperDialogueText;
    [SerializeField]
    TMPro.TMP_Text editorText;
    [SerializeField]
    TMPro.TMP_Text multiplayerText;
    [SerializeField]
    GameObject shopkeeperDialogueBox;
    //End of components of main menu
    string[] shopkeeperLines = new string[]{ "Магазин не работает! Я тут сплю, не видишь!", "Пластинки закончились заходи завтра!", "Меня здесь нет. Закрыто до завтра." };

    [SerializeField]
    Shopkeeper shopkeeper;

    LevelSelection lvlSelection;

    [SerializeField]
    Transform cubeT;


    float targetRot;
    float rotY;

    [SerializeField]
    float rotVelocity;

    float realRotVelocity;

    int dir;

    bool rotating = false;

    int state = 0;

    bool inMainMenu = true;

    struct TouchInfo
    {
        public Touch t;
        public Vector2 pos;
        public int tabIndex;
        public TouchInfo(Touch t, Vector2 pos, int tabIndex)
        {
            this.t = t;
            this.pos = pos;
            this.tabIndex = tabIndex;
        }
    }
    List<TouchInfo> currentTouches;

    void Start()
    {
        currentTouches = new List<TouchInfo>();
        realRotVelocity = rotVelocity;
        rotY = -50;
        targetRot = 0;
        dir = 1;
        rotating = true;
        realRotVelocity = 0.5f * rotVelocity;
        lvlSelection = GetComponent<LevelSelection>();
    }



    float timer = 0;
    void Update()
    {
        if (inMainMenu)
        {
            timer += Time.deltaTime / 2;
            cubeT.position = new Vector3(cubeT.position.x, 1 + Mathf.Sin(timer) / 4, cubeT.position.z);
            cubeT.rotation = Quaternion.Euler(Mathf.Sin(timer * 2 / 3) * 5, cubeT.rotation.eulerAngles.y, Mathf.Cos(timer * 2 / 3) * 5);

            if (Input.touchCount > 0)
            {
                Touch[] touches = Input.touches;
                foreach (Touch t in touches)
                {
                    if (t.phase == TouchPhase.Began)
                    {
                        Vector3 pos = Camera.main.ScreenToWorldPoint(t.position);
                        pos = new Vector3(pos.x, pos.y, -5f);
                        currentTouches.Add(new TouchInfo(t, t.position, 1));
                    }
                    if (t.phase == TouchPhase.Ended)
                    {
                        if (currentTouches.Exists(x => (x.t.fingerId == t.fingerId)))
                        {
                            TouchInfo ti = currentTouches.Find(x => (x.t.fingerId == t.fingerId));
                            Vector2 delta = Camera.main.ScreenToWorldPoint(t.position) - Camera.main.ScreenToWorldPoint(ti.pos);
                            Debug.Log(t.position + " " + ti.pos + " " + delta);
                            if (Mathf.Abs(delta.x) > 1.25)
                            {
                                if (delta.x < 0)
                                {
                                    if (rotating)
                                    {
                                        realRotVelocity = 2 * rotVelocity;
                                    }
                                    targetRot += 90;
                                    dir = 1;
                                    rotating = true;
                                    DeactivateAll();
                                    state += dir;
                                    if (state > 3) state = 0;
                                    if (state < 0) state = 3;
                                }
                                else
                                {
                                    if (rotating)
                                    {
                                        realRotVelocity = 2 * rotVelocity;
                                    }
                                    targetRot -= 90;
                                    dir = -1;
                                    rotating = true;
                                    DeactivateAll();
                                    state += dir;
                                    if (state > 3) state = 0;
                                    if (state < 0) state = 3;
                                }
                                currentTouches.Remove(ti);
                            }
                            else
                            {
                                if (!rotating)
                                {
                                    switch (state)
                                    {
                                        case 0: OpenLevelSelect(); break;
                                        case 3: SceneManager.LoadScene("TrackGenerator"); break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (rotating)
                {
                    realRotVelocity = 2 * rotVelocity;
                }
                targetRot -= 90;
                dir = -1;
                rotating = true;
                DeactivateAll();
                state += dir;
                if (state > 3) state = 0;
                if (state < 0) state = 3;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (rotating)
                {
                    realRotVelocity = 2 * rotVelocity;
                }
                targetRot += 90;
                dir = 1;
                rotating = true;
                DeactivateAll();
                state += dir;
                if (state > 3) state = 0;
                if (state < 0) state = 3;
            }

            if (rotating)
            {
                if ((dir == 1 && rotY > targetRot) || (dir == -1 && rotY < targetRot))
                {
                    rotY = targetRot;
                    cubeT.rotation = Quaternion.Euler(cubeT.rotation.eulerAngles.x, rotY, cubeT.rotation.eulerAngles.z);
                    realRotVelocity = rotVelocity;
                    rotating = false;

                    switch (state)
                    {
                        case 0: ActiveMainState(); break;
                        case 1: ActivateShop(); break;
                        case 3: ActivateLvlEditor(); break;
                        case 4: ActivateOnlineScreen(); break;
                    }
                }
                else
                {
                    rotY += dir * Time.deltaTime * realRotVelocity;
                    cubeT.rotation = Quaternion.Euler(cubeT.rotation.eulerAngles.x, rotY, cubeT.rotation.eulerAngles.z);
                }

            }

        }


    }


    public void ActiveMainState()
    {
        pressToPlayText.gameObject.SetActive(true);
    }
    public void ActivateShop()
    {
        shopkeeperDialogueBox.SetActive(true);
        shopkeeper.OpenShopWithCurrentState();
        //shopkeeperDialogueText.text = shopkeeperLines[Random.Range(0, shopkeeperLines.Length)];
    }
    public void ActivateLvlEditor()
    {
        editorText.gameObject.SetActive(true);
    }
    public void ActivateOnlineScreen()
    {
        multiplayerText.gameObject.SetActive(true);
    }

    public void DeactivateAll()
    {
        pressToPlayText.gameObject.SetActive(false);
        shopkeeperDialogueBox.SetActive(false);
        multiplayerText.gameObject.SetActive(false);
        editorText.gameObject.SetActive(false);
        shopkeeper.CloseAllScreenObjects();
    }

    public void OpenLevelSelect()
    {
        DeactivateAll();
        cubeT.gameObject.SetActive(false);
        lvlSelection.PrepareMenu();
        inMainMenu = false;

    }

}
