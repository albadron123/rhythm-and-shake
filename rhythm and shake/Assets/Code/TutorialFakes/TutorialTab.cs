using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTab : MonoBehaviour
{

    Transform t;
    TabScript tab;

    [SerializeField]
    BasicDialing basicDialing;

    public static int successCount = 0;


    void Start()
    {
        Input.simulateMouseWithTouches = true;
        t = transform;
        tab = GetComponent<TabScript>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (MyUtitities.TouchOver(mousePosition, t))
            {
                switch (TutorialEvents.tutorialStage)
                {
                    //Just tap the tab
                    case 0:
                        {
                            tab.TapIncrease(true, Color.blue);
                            basicDialing.SkipTo(35);
                            ++TutorialEvents.tutorialStage;
                            break;
                        }
                    //Tap the tab when note is in the judgement zone
                    case 1:
                        {
                            if (tab.notesInJudgementZone.Count != 0)
                            {
                                tab.notesInJudgementZone[0].GetComponent<Note>().Anihilate();
                                basicDialing.SkipTo(39);
                                ++TutorialEvents.tutorialStage;
                                successCount = 0;
                            }
                            else
                            {
                                basicDialing.SkipTo(38);
                            }
                            tab.TapIncrease(true, Color.blue);
                            break;
                        }
                    //On mistap start all over again
                    case 2:
                        {
                            if (tab.notesInJudgementZone.Count != 0)
                            {
                                tab.notesInJudgementZone[0].GetComponent<Note>().Anihilate();
                                ++successCount;
                                if (successCount == 3)
                                {
                                    ++TutorialEvents.tutorialStage;
                                    successCount = 0;
                                    //to success
                                    basicDialing.SkipTo(43);
                                }
                            }
                            else
                            {
                                successCount = 0;
                                BasicDialing.DestroyAllNotesOnFail();
                                //failedDialogue3
                                basicDialing.SkipTo(42);
                                basicDialing.PlayThreeNotes();
                            }
                            tab.TapIncrease(true, Color.blue);
                            break;
                        }
                }
                

            }
        }
    }
}
