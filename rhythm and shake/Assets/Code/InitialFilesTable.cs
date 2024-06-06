using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "InitialFilesTable", menuName = "ScriptableObjects/InitialFilesTable", order = 2)]
public class InitialFilesTable : ScriptableObject
{
    public object o;
    [SerializeField]
    public List<object> jsons = new List<object>();
    [SerializeField]
    public List<object> audioSources = new List<object>();
}
