using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;



public class MyUtitities
{
    public static string scoreSaveFlag = "score__";

    public static void SaveSongToAndroid(Song track, string fileName)
    {
        string filePath = Application.persistentDataPath;
        JsonSong jsonSong = new JsonSong();
        jsonSong.songName = track.songName;
        jsonSong.authorName = track.authorName;
        jsonSong.track1 = track.track1;
        jsonSong.track2 = track.track2;
        jsonSong.trackAcc = track.trackAcc;
        string data = JsonUtility.ToJson(jsonSong);
        System.IO.File.WriteAllText(filePath + "/" + fileName, data);
        Debug.Log("saved to " + filePath + "/" + fileName);
    }

    public static void LoadSongFromAndroid(ref Song song, string fileName)
    {
        string filePath = Application.persistentDataPath;
        string jsonSongText = System.IO.File.ReadAllText(filePath + "/" + fileName);
        JsonSong jsonSong = JsonUtility.FromJson<JsonSong>(jsonSongText);
        song.songName = jsonSong.songName;
        song.authorName = jsonSong.authorName;
        song.track1 = jsonSong.track1;
        song.track2 = jsonSong.track2;
        song.trackAcc = jsonSong.trackAcc;
    }

    public static void LoadSongFromJSON(ref Song song, string jsonSongText)
    {
        JsonSong jsonSong = JsonUtility.FromJson<JsonSong>(jsonSongText);
        song.songName = jsonSong.songName;
        song.authorName = jsonSong.authorName;
        song.track1 = jsonSong.track1;
        song.track2 = jsonSong.track2;
        song.trackAcc = jsonSong.trackAcc;
    }

    public static bool TouchOver(Vector3 pos, Transform t)
    {
        return (pos.x < (t.position.x + (t.localScale.x / 2f)) && pos.x > (t.position.x - (t.localScale.x / 2f)) &&
                pos.y < (t.position.y + (t.localScale.y / 2f)) && pos.y > (t.position.y - (t.localScale.y / 2f)));
    }

    public static string GetLocalIPAddress()
    {
        Debug.Log(System.Net.Dns.GetHostName());
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                Debug.Log(ip.ToString());
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }


}
