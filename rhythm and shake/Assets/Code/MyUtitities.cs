using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;



public class MyUtitities
{
    public static string scoreSaveFlag = "score__";
    public static string lastRewardFlag = "lastRevard__";


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
        LoadSongFromPath(ref song, filePath + "/" + fileName);
    }
    public static void LoadSongFromPath(ref Song song, string totalPath)
    {
        string filePath = Application.persistentDataPath;
        string jsonSongText = System.IO.File.ReadAllText(totalPath);
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

    public static Texture2D LoadTexture(string FilePath)
    {
        Texture2D Tex2D;
        byte[] FileData;
        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);
            if (Tex2D.LoadImage(FileData))
                return Tex2D; 
        }
        return null;                     
    }

    public static Sprite LoadNewSprite(string FilePath, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        Texture2D SpriteTexture = LoadTexture(FilePath);
        if (SpriteTexture == null) 
            return null;
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), Mathf.Max(SpriteTexture.width, SpriteTexture.height), 0, spriteType);
        return NewSprite;
    }

    public static byte[] IntToByteArray(int number)
    {
        return new byte[4] { (byte)number, (byte)(number >> 8), (byte)(number >> 16), (byte)(number >> 24) };
    }

    //IMPORTANT: VERY MUCH NOT OPTIMAL
    public static int ByteArrayToInt(byte[] array, int initialIndex)
    {
        return (int)array[initialIndex] + ((int)array[initialIndex+1] << 8) + ((int)array[initialIndex+2] << 16) + ((int)array[initialIndex+3] << 24);
    }

    public static void LogByteArray(byte[] array, int size)
    {
        string s = "";
        for (int i = 0; i < size; ++i)
        {
            s += ((char)array[i]).ToString();
        }
        Debug.Log(s);
    }


}
