using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ArrowDirection {no, up, down, left, right};

[System.Serializable]
public struct SongItem { public float time; public ArrowDirection dir; }

public class NoteTimeAndType 
{
    public float time;
    public NoteType type;
}

public class SongGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;


    [SerializeField]
    GameObject classicNotePrefab;
    [SerializeField]
    GameObject leftNotePrefab;
    [SerializeField]
    GameObject rightNotePrefab;
    [SerializeField]
    GameObject upNotePrefab;
    [SerializeField]
    GameObject downNotePrefab;
    [SerializeField]
    GameObject leftArrowPrefab;
    [SerializeField]
    GameObject rightArrowPrefab;
    [SerializeField]
    GameObject upArrowPrefab;
    [SerializeField]
    GameObject downArrowPrefab;

    [SerializeField]
    List<SongItem> track1 = new List<SongItem>();

    [SerializeField]
    List<SongItem> track2 = new List<SongItem>();

    [SerializeField]
    List<SongItem> trackAcc = new List<SongItem>();

    [SerializeField]
    float trackVelocity;

    [SerializeField]
    AudioSource song;

    [SerializeField]
    float delay = 1f;

    public static bool isPlaying = false;

    GamePlayCode gameplay;

    [SerializeField]
    bool inMultiplayer = false;

    [SerializeField]
    TransportedData td;

    [SerializeField]
    Vector3 judgementPosition = Vector3.zero;

    int maxScoreNow;
    int maxComboNow;

    double trackTime = 0;
    double timer = 0;


    void Start()
    {
        gameplay = GetComponent<GamePlayCode>();
        if (!inMultiplayer)
        {
            song.clip = td.currentSong.audio;
            track1 = td.currentSong.track1;
            track2 = td.currentSong.track2;
            trackAcc = td.currentSong.trackAcc;
            StartSong();
        }
    }

    void StopSongAndOpenMenu()
    {
        isPlaying = false;
        song.Stop();
        gameplay.ExitGamePlay();
    }


    public void PauseSong()
    {
        isPlaying = false;
        song.Pause();
        pauseMenu.SetActive(true);
    }

    public void ResumeSong()
    {
        isPlaying = true;
        song.UnPause();
        pauseMenu.SetActive(false);
    }

    public void ExitToMenu()
    {
        gameplay.ExitGamePlay();
    }

    public void RetrySong()
    {
        gameplay.SaveRecords();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    

    public void StartSong()
    {
        GenerateTrack();
        song.Play();
        timer = 0;
        trackTime = 2*delay + Mathf.Max(track1[track1.Count - 1].time, track2[track2.Count - 1].time, trackAcc[trackAcc.Count - 1].time);
        isPlaying = true;
    }


    private void Update()
    {
        if (isPlaying)
        {
            timer += Time.deltaTime;
            if (timer >= trackTime)
            {
                StopSongAndOpenMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPlaying)
            {
                PauseSong();
            }
            else
            {
                ResumeSong();
            }
        }
        
    }

    void GenerateTrack()
    {
        List<NoteTimeAndType> scoreFlags = new List<NoteTimeAndType>();
        for (int i = 0; i < track1.Count; ++i)
        {
            Vector3 notePosition = new Vector3(judgementPosition.x, 0, 0) - new Vector3((track1[i].time + delay) * trackVelocity, 0, -1);
            GenerateNote(notePosition, track1[i].dir);
            NoteTimeAndType n = new NoteTimeAndType();
            n.time = track1[i].time;
            n.type = NoteType.simple;
            scoreFlags.Add(n);
        }
        for (int i = 0; i < track2.Count; ++i)
        {
            Vector3 notePosition = new Vector3(judgementPosition.x, -3.2f, 0) - new Vector3((track2[i].time + delay) * trackVelocity, 0, -1);
            GenerateNote(notePosition, track2[i].dir);
            NoteTimeAndType n = new NoteTimeAndType();
            n.time = track2[i].time;
            n.type = NoteType.simple;
            scoreFlags.Add(n);
        }
        for (int i = 0; i < trackAcc.Count; ++i)
        {
            Vector3 notePosition = new Vector3(judgementPosition.x, 2.7f, 0) - new Vector3((trackAcc[i].time + delay) * trackVelocity, 0, -1);
            GenerateArrow(notePosition, trackAcc[i].dir);
            NoteTimeAndType n = new NoteTimeAndType();
            n.time = trackAcc[i].time;
            n.type = NoteType.accNote;
            scoreFlags.Add(n);
        }
        maxComboNow = 0;
        maxScoreNow = 0;
        scoreFlags.Sort(delegate(NoteTimeAndType n1, NoteTimeAndType n2) { return (n1.time < n2.time)? -1: 1; });
        for (int i = 0; i < scoreFlags.Count; ++i)
        {
            ChangeMaxScoreNow(scoreFlags[i].type);
        }
        GamePlayCode.maxScore = maxScoreNow;
        Debug.Log(GamePlayCode.maxScore);
    }

    public void ChangeMaxScoreNow(NoteType type)
    {
        ++maxComboNow;
        if (type == NoteType.simple)
        {
            if (maxComboNow < 10)
                ++maxScoreNow;
            else if (maxComboNow < 50)
                maxScoreNow += 2;
            else
                maxScoreNow += 3;
        }
        if (type == NoteType.accNote)
        {
            if (maxComboNow < 10)
                maxScoreNow += 2;
            else if (maxComboNow < 50)
                maxScoreNow += 5;
            else
                maxScoreNow += 10;
        }
    }


    void GenerateNote(Vector2 position, ArrowDirection dir)
    {
        GameObject notePrefab;
        switch (dir)
        {
            case ArrowDirection.up: notePrefab = upNotePrefab; break;
            case ArrowDirection.down: notePrefab = downNotePrefab; break;
            case ArrowDirection.left: notePrefab = leftNotePrefab; break;
            case ArrowDirection.right: notePrefab = rightNotePrefab; break;
            default: notePrefab = classicNotePrefab; break;
        }
        GameObject noteInstance = Instantiate(notePrefab, position, Quaternion.identity);
        Note note = noteInstance.GetComponent<Note>();
        note.velocity = trackVelocity;
        note.dir = dir;
    }

    void GenerateArrow(Vector2 position, ArrowDirection dir)
    {
        GameObject arrowPrefab;
        switch (dir)
        {
            case ArrowDirection.up: arrowPrefab = upArrowPrefab; break;
            case ArrowDirection.down: arrowPrefab = downArrowPrefab; break;
            case ArrowDirection.left: arrowPrefab = leftArrowPrefab; break;
            default: arrowPrefab = rightArrowPrefab; break;
        }
        GameObject arrowInstance = Instantiate(arrowPrefab, position, arrowPrefab.transform.rotation);
        Note n = arrowInstance.GetComponent<Note>();
        n.velocity = trackVelocity;
    }
}
