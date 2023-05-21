using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TransportedData", menuName = "ScriptableObjects/TransportedData", order = 1)]
public class TransportedData : ScriptableObject
{
    public Song currentSong;
    public int currentMenuIndex;
}
