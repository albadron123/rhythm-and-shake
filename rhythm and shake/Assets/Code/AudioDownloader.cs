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
        if (!PlayerPrefs.HasKey("TUTORIAL_DONE"))
        {
            SceneManager.LoadScene("Tutorial");
            return;
        }

        processed = 0;
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/songs");
        if (!dir.Exists)
        {
            dir.Create();
            //Here we can init ourselves later
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            int index = 0;

            FileInfo[] wavFileInfos = dir.GetFiles("*.wav");
            FileInfo[] mpegFileInfos = dir.GetFiles("*.mp3");
            dir = new DirectoryInfo(Application.persistentDataPath);
            FileInfo[] jsonFileInfos = dir.GetFiles("*.json");

            td.detectedAudioFiles = new List<AudioInfo>();
            td.detectedJsonFiles = new List<JsonInfo>();
            
            foreach (FileInfo wavFileInfo in wavFileInfos)
            {
                AudioInfo ai = new AudioInfo();
                ai.name = wavFileInfo.Name;
                StartCoroutine(GetAudioClip("songs/" + wavFileInfo.Name, index, AudioType.WAV));
                td.detectedAudioFiles.Add(ai);
                ++index;
            }
            foreach (FileInfo mpegFileInfo in mpegFileInfos)
            {
                AudioInfo ai = new AudioInfo();
                ai.name = mpegFileInfo.Name;
                StartCoroutine(GetAudioClip("songs/" + mpegFileInfo.Name, index, AudioType.MPEG));
                td.detectedAudioFiles.Add(ai);
                ++index;
            }
            foreach (FileInfo i in jsonFileInfos)
            {
                JsonInfo ji = new JsonInfo();
                ji.name = i.Name;
                ji.json = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + i.Name);
                td.detectedJsonFiles.Add(ji);
            }
        }
    }

    void Update()
    {
        if (processed == td.detectedAudioFiles.Count)
        {
            if (!audioSource.isPlaying)
            {
                Debug.Log("We have loaded all the tracks.");
                string msg = "List of loaded tracks:\n";
                for (int i = 0; i < td.detectedAudioFiles.Count; ++i)
                {
                    msg += td.detectedAudioFiles[i].name + "\n";
                }
                Debug.Log(msg);
                SceneManager.LoadScene("SampleScene");
            }
        }
    }

    IEnumerator GetAudioClip(string fileName, int index, AudioType audioType)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + Application.persistentDataPath + "/" + fileName, audioType))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                td.detectedAudioFiles[index].clip = DownloadHandlerAudioClip.GetContent(www);
                ++processed;
            }
        }
    }

    public static IEnumerator NET_GetAudioClip(string fileName, AudioType audioType)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + Application.persistentDataPath + "/" + fileName, audioType))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                NetworkStatus.song.audio = DownloadHandlerAudioClip.GetContent(www);
            }
        }
    }
}
