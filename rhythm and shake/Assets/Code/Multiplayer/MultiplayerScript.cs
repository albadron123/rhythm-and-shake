using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Net.Sockets;


public class MultiplayerScript : MonoBehaviour
{
    [SerializeField]
    NetworkVar v;

    [SerializeField]
    SongGenerator sg;

    [SerializeField]
    TMPro.TMP_Text ipAddressText;
    [SerializeField]
    TMPro.TMP_InputField ipAddressInputField;
    [SerializeField]
    string ipAddress = "0.0.0.0";

    [SerializeField]
    UnityTransport uTransport;

    [SerializeField] GameObject hostIdentifier;
    [SerializeField] GameObject clientIdentifier;

    void Start()
    {
    }

    void Update()
    {

    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        //it shows where are we hosting right now ;)
        GetLocalIPAddress();
        hostIdentifier.SetActive(true);
    }

    public void StartClient()
    {
        ipAddress = ipAddressInputField.text;
        SetIpAddress();
        bool res = NetworkManager.Singleton.StartClient();
        ipAddressText.text = ipAddress + " (" + res + ")";
        clientIdentifier.SetActive(true);
    }

    //HOST ONLY
    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                ipAddressText.text = ip.ToString();
                ipAddress = ip.ToString();
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    public void SetIpAddress()
    {
        uTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        uTransport.ConnectionData.Address = ipAddress;
    }

    public void PressReady()
    {
        v.ToggleServerRpc();
    }
}

