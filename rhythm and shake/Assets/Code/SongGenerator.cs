using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ArrowDirection {no, up, down, left, right};

public enum HandMode {left, right};

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
    Transform SongLayountT;

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
    public static bool inTutorial = false;

    [SerializeField]
    TransportedData td;


    int maxScoreNow;
    int maxComboNow;

    double trackTime = 0;
    double timer = 0;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("HAND"))
        {
            TransportedData.handMode = (HandMode)PlayerPrefs.GetInt("HAND");
        }

    }

    void Start()
    {
        
                
        SongLayountT.position = Vector3.zero;

        if (GetComponent<BasicDialing>() != null)
            inTutorial = true;

        gameplay = GetComponent<GamePlayCode>();
        if (inMultiplayer)
        {
            //
        }
        else if (inTutorial)
        { 
            //
        }
        else
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

    public void PauseSongWithButton()
    {
        PauseSong();
    }

    public float PauseSong()
    {
        isPlaying = false;
        song.Pause();
        if(!inMultiplayer)
            pauseMenu.SetActive(true);
        return song.time;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && !inMultiplayer)
        {
            PauseSong();
        }
    }

    public void ResumeSong()
    {
        isPlaying = true;
        song.UnPause();
        if(!inMultiplayer)
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

    public void NET_InitSong()
    {
        song.clip = NetworkStatus.song.audio;
        track1 = NetworkStatus.song.track1;
        track2 = NetworkStatus.song.track2;
        trackAcc = NetworkStatus.song.trackAcc;

    }
    public void NET_CorrectSong(float time)
    {
        //do corrections after pause
        song.time = time;
        SongLayountT.position = new Vector3(-Mathf.Sign((float)TransportedData.handMode - 0.5f) * song.time * trackVelocity, 0, 0);
    }


    private void Update()
    {
        if (isPlaying)
        {
            SongLayountT.position = new Vector3(-Mathf.Sign((float)TransportedData.handMode - 0.5f) * song.time * trackVelocity, 0, 0);
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
            Vector3 notePosition = new Vector3(Mathf.Sign((float)TransportedData.handMode - 0.5f) * (-0.1f + (track1[i].time) * trackVelocity), 0, -1);
            if (TransportedData.handMode == HandMode.right)
            {
                if (track1[i].dir == ArrowDirection.left)
                    GenerateNote(notePosition, ArrowDirection.right);
                else if (track1[i].dir == ArrowDirection.right)
                    GenerateNote(notePosition, ArrowDirection.left);
                else
                    GenerateNote(notePosition, track1[i].dir);
            }
            else
            {
                GenerateNote(notePosition, track1[i].dir);
            }
            NoteTimeAndType n = new NoteTimeAndType();
            n.time = track1[i].time;
            n.type = NoteType.simple;
            scoreFlags.Add(n);
        }
        for (int i = 0; i < track2.Count; ++i)
        {
            Vector3 notePosition = new Vector3(0, -3.2f, 0) + new Vector3(Mathf.Sign((float)TransportedData.handMode - 0.5f) * (-0.1f + (track2[i].time) * trackVelocity), 0, -1);
            if (TransportedData.handMode == HandMode.right)
            {
                if (track2[i].dir == ArrowDirection.left)
                    GenerateNote(notePosition, ArrowDirection.right);
                else if (track2[i].dir == ArrowDirection.right)
                    GenerateNote(notePosition, ArrowDirection.left);
                else
                    GenerateNote(notePosition, track2[i].dir);
            }
            else
            {
                GenerateNote(notePosition, track2[i].dir);
            }
            NoteTimeAndType n = new NoteTimeAndType();
            n.time = track2[i].time;
            n.type = NoteType.simple;
            scoreFlags.Add(n);
        }
        for (int i = 0; i < trackAcc.Count; ++i)
        {
            Vector3 notePosition = new Vector3(0, 2.7f, 0) + new Vector3(Mathf.Sign((float)TransportedData.handMode - 0.5f) * (-0.1f + (trackAcc[i].time) * trackVelocity), 0, -1);
            if (TransportedData.handMode == HandMode.right)
            {
                if (trackAcc[i].dir == ArrowDirection.left)
                    GenerateArrow(notePosition, ArrowDirection.right);
                else if (trackAcc[i].dir == ArrowDirection.right)
                    GenerateArrow(notePosition, ArrowDirection.left);
                else
                    GenerateArrow(notePosition, trackAcc[i].dir);
            }
            else
            {
                GenerateArrow(notePosition, trackAcc[i].dir);
            }
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


    public void GenerateNote(Vector2 position, ArrowDirection dir)
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
        GameObject noteInstance = Instantiate(notePrefab, position, Quaternion.identity, SongLayountT);
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
        GameObject arrowInstance = Instantiate(arrowPrefab, position, arrowPrefab.transform.rotation, SongLayountT);
        Note n = arrowInstance.GetComponent<Note>();
        n.velocity = trackVelocity;
    }
}
