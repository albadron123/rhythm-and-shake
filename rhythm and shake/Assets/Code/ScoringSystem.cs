using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringSystem : MonoBehaviour
{
    static TMPro.TMP_Text te;


    private static int score;

    const string scoreKey = "SCORE";

    public void Start()
    {
        te = GameObject.FindGameObjectsWithTag("scoreText")[0].GetComponent<TMPro.TMP_Text>();
        LoadScore();
    }


    public static void LoadScore()
    {
        if (PlayerPrefs.HasKey(scoreKey))
        {
            score = PlayerPrefs.GetInt(scoreKey, 0);
        }
        else
        {
            score = 0;
            PlayerPrefs.SetInt(scoreKey, 0);
        }
        te.text = ": " + score.ToString();
    }

    public static void SaveScore()
    {
        PlayerPrefs.SetInt(scoreKey, score);
    }

    public static int GetScore()
    {
        return score;
    }

    public static void SetScore(int difference)
    {
        score += difference;
        te.text = ": " + score.ToString();
        SaveScore();
    }
}
