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

    public static bool enableMainMenu = true;
    public static HandMode handMode = HandMode.right;
    public static int scoreDelta = 0;


    //shopping constancts
    public static bool[] downloaded = new bool[3] { false, false, false };

    public List<AudioInfo> detectedAudioFiles;
    public List<JsonInfo> detectedJsonFiles;
}
