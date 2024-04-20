using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Editor : MonoBehaviour
{
    [SerializeField] float step;
    [SerializeField] float length;

    [SerializeField] GameObject trackDot;

    List<trackDot> track1;
    List<trackDot> track2;
    List<trackDot> trackAcc;

    [SerializeField]
    Song track;

    [SerializeField] GameObject playSoundPoint;

    void Start()
    {
        track1 = new List<trackDot>();
        track2 = new List<trackDot>();
        trackAcc = new List<trackDot>();

        int count = 0;
        for (float i = 0; i < length; i+=step)
        {
            GameObject pp = Instantiate(playSoundPoint, new Vector3(0,2,0) + new Vector3(i * 4, 0, 0), Quaternion.identity);
            pp.GetComponent<playSoundDot>().time = i;

            GameObject tr1Instance = Instantiate(trackDot, Vector3.zero + new Vector3(i * 4, 0,0), Quaternion.identity);
            trackDot t = tr1Instance.GetComponent<trackDot>();
            t.id = count;
            track1.Add(t);

            GameObject tr2Instance = Instantiate(trackDot, new Vector3(0, -1, 0) + new Vector3(i * 4, 0, 0), Quaternion.identity);
            t = tr2Instance.GetComponent<trackDot>();
            t.id = count;
            track2.Add(t);

            GameObject tr3Instance = Instantiate(trackDot, new Vector3(0, 1, 0) + new Vector3(i * 4, 0, 0), Quaternion.identity);
            t = tr3Instance.GetComponent<trackDot>();
            t.id = count;
            trackAcc.Add(t);


            ++count;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            track.track1 = new List<SongItem>();
            track.track2 = new List<SongItem>();
            track.trackAcc = new List<SongItem>();

            foreach (trackDot t in track1)
            {
                if (t.isActive)
                {
                    SongItem si = new SongItem();
                    si.time = t.id * step;
                    si.dir = t.dir;
                    track.track1.Add(si);
                }
            }
            foreach (trackDot t in track2)
            {
                if (t.isActive)
                {
                    SongItem si = new SongItem();
                    si.time = t.id * step;
                    si.dir = t.dir;
                    track.track2.Add(si);
                }
            }
            foreach (trackDot t in trackAcc)
            {
                if (t.isActive)
                {
                    SongItem si = new SongItem();
                    si.time = t.id * step;
                    si.dir = t.dir;
                    track.trackAcc.Add(si);
                }
            }
            Debug.Log("trackSaved");
        }
    }


    void Save()
    {
        track.track1 = new List<SongItem>();
        track.track2 = new List<SongItem>();
        track.trackAcc = new List<SongItem>();

        foreach (trackDot t in track1)
        {
            if (t.isActive)
            {
                SongItem si = new SongItem();
                si.time = t.id * step;
                si.dir = t.dir;
                track.track1.Add(si);
            }
        }
        foreach (trackDot t in track2)
        {
            if (t.isActive)
            {
                SongItem si = new SongItem();
                si.time = t.id * step;
                si.dir = t.dir;
                track.track2.Add(si);
            }
        }
        foreach (trackDot t in trackAcc)
        {
            if (t.isActive)
            {
                SongItem si = new SongItem();
                si.time = t.id * step;
                si.dir = t.dir;
                track.trackAcc.Add(si);
            }
        }
        Debug.Log("trackSaved");
    }

    void Load()
    {
        //???
        Debug.Log("track Loaded");
    }
}
