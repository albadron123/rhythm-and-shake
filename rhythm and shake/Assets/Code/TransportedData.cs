using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioInfo
{
    public AudioClip clip;
    public string name;
}

public class JsonInfo
{
    public string json;
    public string name;
}

[CreateAssetMenu(fileName = "TransportedData", menuName = "ScriptableObjects/TransportedData", order = 1)]
public class TransportedData : ScriptableObject
{
    public Song currentSong;
    public int currentMenuIndex;

    public List<AudioInfo> deletctedAudioFiles;
    public List<JsonInfo> detectedJsonFiles;
}
