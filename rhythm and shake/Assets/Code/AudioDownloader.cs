using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;

public class AudioDownloader : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    AudioClip clip;

    [SerializeField]
    TransportedData td;

    private int processed;


    void Start()
    {
        processed = 0;
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/songs");
        FileInfo[] info = dir.GetFiles("*.wav");
        int index = 0;
        td.deletctedAudioFiles = new List<AudioInfo>();
        td.detectedJsonFiles = new List<JsonInfo>();
        foreach (FileInfo i in info)
        {
            AudioInfo ai = new AudioInfo();
            ai.name = i.Name;
            StartCoroutine(GetAudioClip("songs/" + i.Name, index, AudioType.WAV));
            td.deletctedAudioFiles.Add(ai);
            ++index;
        }
        dir = new DirectoryInfo(Application.persistentDataPath);
        info = dir.GetFiles("*.json");
        foreach (FileInfo i in info)
        {
            JsonInfo ji = new JsonInfo();
            ji.name = i.Name;
            ji.json = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + i.Name);
            td.detectedJsonFiles.Add(ji);
        }
    }

    void Update()
    {
        if (processed == td.deletctedAudioFiles.Count)
        {
            if (!audioSource.isPlaying)
            {
                Debug.Log("We have loaded all the tracks.");
                string msg = "List of loaded tracks:\n";
                for (int i = 0; i < td.deletctedAudioFiles.Count; ++i)
                {
                    msg += td.deletctedAudioFiles[i].name + "\n";
                }
                Debug.Log(msg);
                SceneManager.LoadScene("SampleScene");
            }
        }
    }

    IEnumerator GetAudioClip(string fileName, int index, AudioType audioType)
    {
        //change to mp3
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + Application.persistentDataPath + "/" + fileName, audioType))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                td.deletctedAudioFiles[index].clip = DownloadHandlerAudioClip.GetContent(www);
                ++processed;
            }
        }
    }

    public static IEnumerator GetAndPlayAudioClip(string fileName, AudioType audioType, AudioSource source)
    {
        //change to mp3
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + Application.persistentDataPath + "/" + fileName, audioType))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                source.clip = DownloadHandlerAudioClip.GetContent(www);
                source.Play();
            }
        }
    }
}
