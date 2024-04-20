using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkScan : MonoBehaviour
{
    const string ipBase = "192.168.1.";
    float PingTimeoutInSeconds = 1f;

    List<string> IPs = new List<string>();

    bool hasScannedResults = false;

    [SerializeField]
    TMPro.TMP_Text te;

    // Start is called before the first frame update
    void Start()
    {
        te.text = GetLocalIPAddress() + (GetComponent<GameplayServer>().enabled ? "  (server)" : " (client)");
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScanNet();
        }
        */
        if (hasScannedResults)
        {
            string res = "";
            for (int i = 0; i < IPs.Count; ++i)
            {
                res += IPs[i] + "\n";
            }
            Debug.Log(res);
            hasScannedResults = false;
        }
    }

    void ScanNet()
    {
        IPs.Clear();
        for (int i = 0; i < 255; ++i)
        {
            string ip = ipBase + i.ToString();
            StartCoroutine(PingAddress(ip));
        }
    }

    IEnumerator PingAddress(string ip)
    {
        Ping p = new Ping(ip);
        yield return new WaitForSeconds(1f);
        if (p.isDone)
        {
            if(!IPs.Exists(x=>x==ip))
                IPs.Add(p.ip);
            hasScannedResults = true;
        }
        else
        {
            //IPs.Add("Error");
            //hasScannedResults = true;
        }
    }

    public static string GetLocalIPAddress()
    {
        Debug.Log(Dns.GetHostName());
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                Debug.Log(ip.ToString());
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
