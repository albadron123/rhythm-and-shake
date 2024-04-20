using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using UnityEngine;



public class GameplayServer : MonoBehaviour
{
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;

    LevelSelection ls;

    bool isOnline = false;

    [SerializeField]
    TMPro.TMP_Text statusText;

    void Start()
    {
        Input.simulateMouseWithTouches = true;
        //later create different blocks for lobby and for gameplay
        ls = GetComponent<LevelSelection>();
    }

    public void StartServer()
    {
        if (!isOnline)
        {
            tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
            tcpListenerThread.IsBackground = true;
            tcpListenerThread.Start();
            isOnline = true;
            statusText.text = MyUtitities.GetLocalIPAddress() + " (server)";
        }
    }

    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            //SendMessage();
            //SendWAV();
        }
        */
    }

    private void ListenForIncomingRequests()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Any, 8052);
            tcpListener.Start();
            Debug.Log("Server is listening");
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    using (NetworkStream stream = connectedTcpClient.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            for (int i = 0; i < incomingData.Length; ++i)
                            {
                                if (incomingData[i] == 0b_0000_0001)
                                {
                                    SendWAV(ls.GetCurrentSongWAVAddress());
                                }
                            }
                            //NetworkData.otherScore = incomingData[length-1];
                            Debug.Log("client message received as: " + NetworkData.otherScore);
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }

        Debug.Log("Exiting...");
    }

    
    private void SendMessage()
    {
        ++NetworkData.myScore;

        if (connectedTcpClient == null)
        {
            return;
        }

        try
        {
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                //string serverMessage = "This is a message from your server.";
                byte[] serverMessageAsByteArray = {NetworkData.myScore};
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                Debug.Log("Server sent his message - should be received by client");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }


    private void SendWAV(string address)
    {
        Debug.Log("address:" + address);

        if (connectedTcpClient == null)
        {
            return;
        }

        try
        {
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                //string serverMessage = "This is a message from your server.";
                
                byte[] audioArray = File.ReadAllBytes(address);
                byte[] header = {0,
                                (byte)(audioArray.Length & 0b11111111),
                                (byte)((audioArray.Length>>8) & 0b11111111),
                                (byte)((audioArray.Length>>16) & 0b11111111),
                                (byte)((audioArray.Length>>24) & 0b11111111)};
                byte[] serverMessageAsByteArray = new byte[audioArray.Length + 5];
                Array.Copy(header, 0, serverMessageAsByteArray, 0, 5);
                Array.Copy(audioArray, 0, serverMessageAsByteArray, 5, audioArray.Length);
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                Debug.Log("Server sent his message - should be received by client");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}