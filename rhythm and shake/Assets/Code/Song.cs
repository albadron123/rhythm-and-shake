using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Song", menuName = "ScriptableObjects/Song", order = 1)]
public class Song : ScriptableObject
{
    public string songName;
    public string authorName;
    public string songRecord;
    public Sprite songPicture;
    public List<float> track1 = new List<float>();
    public List<float> track2 = new List<float>();
    public List<AccItem> trackAcc = new List<AccItem>();
}
