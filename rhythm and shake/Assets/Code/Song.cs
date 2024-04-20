using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Song", menuName = "ScriptableObjects/Song", order = 1)]
public class Song : ScriptableObject
{
    public string songName;
    public string authorName;
    public int songRecord;
    public int lastTryScore;
    public Sprite songPicture;
    public List<SongItem> track1 = new List<SongItem>();
    public List<SongItem> track2 = new List<SongItem>();
    public List<SongItem> trackAcc = new List<SongItem>();
    public AudioClip audio;
}

[System.Serializable]
public class JsonSong
{
    public string songName;
    public string authorName;
    public List<SongItem> track1 = new List<SongItem>();
    public List<SongItem> track2 = new List<SongItem>();
    public List<SongItem> trackAcc = new List<SongItem>();
}


