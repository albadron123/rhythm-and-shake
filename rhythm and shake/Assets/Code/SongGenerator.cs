using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ArrowDirection {up, down, left, right};

[System.Serializable]
public struct AccItem { public float time; public ArrowDirection dir; }

public class SongGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject notePrefab;
    [SerializeField]
    GameObject arrowPrefab;

    [SerializeField]
    List<float> track1 = new List<float>();

    [SerializeField]
    List<float> track2 = new List<float>();

    [SerializeField]
    List<AccItem> trackAcc = new List<AccItem>();

    [SerializeField]
    Color track1Color;
    [SerializeField]
    Color track2Color;

    [SerializeField]
    float trackVelocity;

    int track1Index = 0;
    int track2Index = 0;
    int trackAccIndex = 0;
    float track1Time;
    float track2Time;
    float trackAccTime;
    float timer;

    bool track1Playing;
    bool track2Playing;
    bool trackAccPlaying;

    [SerializeField]
    AudioSource song;

    [SerializeField]
    float delay = 1f;

    bool isPlaying = false;

    bool inGame = false;

    GyroChecker gc;

    void Start()
    {
        gc = GetComponent<GyroChecker>();
    }

    void FixedUpdate()
    {
        if (inGame)
        {
            timer += Time.deltaTime;

            if (timer >= delay && !isPlaying)
            {
                song.Play();
                isPlaying = true;
            }

            if (track1Playing && timer >= track1Time)
            {
                GenerateNote(new Vector3(-3f, 0.5f, 0), track1Color);
                ++track1Index;
                if (track1Index >= track1.Count)
                {
                    track1Playing = false;
                }
                else
                {
                    track1Time = track1[track1Index];
                }
            }
            if (track2Playing && timer >= track2Time)
            {
                GenerateNote(new Vector3(-3f, -3f, 0), track2Color);
                ++track2Index;
                if (track2Index >= track2.Count)
                {
                    track2Playing = false;
                }
                else
                {
                    track2Time = track2[track2Index];
                }
            }
            if (trackAccPlaying && timer >= trackAccTime)
            {
                GenerateArrow(new Vector3(-3f, 3.25f, 0), trackAcc[trackAccIndex].dir);
                ++trackAccIndex;
                if (trackAccIndex >= trackAcc.Count)
                {
                    trackAccPlaying = false;
                }
                else
                {
                    trackAccTime = trackAcc[trackAccIndex].time;
                }
            }


            if (!track1Playing && !track2Playing && !trackAccPlaying)
            {
                inGame = false;
                //later here we will finish tracks
                Invoke("StopSongAndOpenMenu", delay + 0.5f);
            }
        }
    }


    void StopSongAndOpenMenu()
    {
        isPlaying = false;
        song.Stop();
        Invoke("OpenMenu", 1f);
    }

    void OpenMenu()
    {
        gc.EnterMenu();
    }

    public void StartSong() 
    {
        track1Time = track1[0];
        track2Time = track2[0];
        trackAccTime = trackAcc[0].time;
        track1Index = 0;
        track2Index = 0;
        trackAccIndex = 0;
        track1Playing = true;
        track2Playing = true;
        trackAccPlaying = true;
        timer = 0;
        inGame = true;
    }


    void GenerateNote(Vector2 position, Color col)
    {
        GameObject noteInstance = Instantiate(notePrefab, position, Quaternion.identity);
        noteInstance.GetComponent<Note>().velocity = trackVelocity;
        noteInstance.GetComponent<SpriteRenderer>().color = col;
    }

    void GenerateArrow(Vector2 position, ArrowDirection dir)
    {
        GameObject arrowInstance = Instantiate(arrowPrefab, position, Quaternion.identity);
        switch (dir)
        {
            case ArrowDirection.up: arrowInstance.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90)); break;
            case ArrowDirection.down: arrowInstance.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90)); break;
            case ArrowDirection.left: arrowInstance.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -180)); break;
            default: arrowInstance.transform.localRotation = Quaternion.identity; break;
        }
        Note n = arrowInstance.GetComponent<Note>();
        n.velocity = trackVelocity;
        n.dir = dir;
    }
}
