using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using UnityEngine;


public enum ReadingGoal
{
    header, audio, json
}
public class GameplayClient : MonoBehaviour
{
    ReadingGoal rg = ReadingGoal.header;
    byte[] header = new byte[5];
    int alreadyRead = 0;
    //List<byte[]> songChunks = new List<byte[]>();
    byte[] song;

    [SerializeField]
    TMPro.TMP_Text statusText;

    [SerializeField]
    TMPro.TMP_Text ipAddressField;


    [SerializeField]
    AudioSource audioPlayer;

    bool connected = false;
    bool firstRequest = false;


    #region private members     
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    #endregion
    // Use this for initialization  
    void Start()
    {
        Input.simulateMouseWithTouches = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (connected && !firstRequest)
        {
            Debug.Log("we try");
            SendMessage(new byte[] { 0b_0000_0001 });
        }
    }
    /// <summary>   
    /// Setup socket connection.    
    /// </summary>  
    /// 

    public void Connect()
    {
        if (!connected)
        {
            Debug.Log("We are here");
            ConnectToTcpServer();
            connected = true;
            firstRequest = false;
        }
    }

    public void OnConnectedToServer()
    {
        Debug.Log("we are connected (somehow) to the server");
    }


    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
            Debug.Log("we listen");
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    

    /// <summary>   
    /// Runs in background clientReceiveThread; Listens for incoming data.  
    /// </summary>   
    private void ListenForData()
    {
        try
        {
            string txtIP = "100.88.214.232";
            string ip = (Convert.ToInt16(txtIP.Replace(",", ".").Split('.')[0])).ToString();
            ip += "." + (Convert.ToInt16(txtIP.Replace(",", ".").Split('.')[1])).ToString();
            ip += "." + (Convert.ToInt16(txtIP.Replace(",", ".").Split('.')[2])).ToString();
            ip += "." + (Convert.ToInt16(txtIP.Replace(",", ".").Split('.')[3])).ToString();
            socketConnection = new TcpClient();
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), 8052);
            socketConnection.Connect(ipEndPoint);
            Debug.Log("socketConnection:" + socketConnection.Connected);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                // Get a stream object for reading              
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incoming stream into byte array.
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incomingData = new byte[length];
                        Array.Copy(bytes, 0, incomingData, 0, length);
                        int readInPacket = 0;
                        //songChunks.Add(incomingData);
                        // Convert byte array to string message.                        
                        //NetworkData.otherScore = incomingData[length - 1];
                        if (rg == ReadingGoal.header)
                        {
                            int copyLen = Mathf.Min(5 - alreadyRead, length-readInPacket);
                            Array.Copy(incomingData, 0, header, alreadyRead, copyLen);
                            alreadyRead += copyLen;
                            readInPacket += copyLen;
                            statusText.text = "reading header (" + alreadyRead + "/5 bytes)...";
                            if (alreadyRead == 5)
                            {
                                rg = ReadingGoal.audio;
                                alreadyRead = 0;
                                int songSize = (int)header[1] + ((int)header[2] << 8) + ((int)header[3] << 16) + ((int)header[4] << 24);
                                statusText.text = "song size: " + songSize + "\n" + "full pack:" + incomingData.Length;
                                song = new byte[songSize];
                                for (int i = copyLen; i < incomingData.Length; ++i)
                                {
                                    incomingData[i - copyLen] = incomingData[i];
                                }
                                Array.Resize<byte>(ref incomingData, incomingData.Length - copyLen);
                            }
                        }
                        if (rg == ReadingGoal.audio)
                        {
                            int copyLen = Mathf.Min(song.Length - alreadyRead, length - readInPacket);
                            //statusText.text = "copyLen: " + copyLen + "\nIncomingData:" + incomingData.Length + "\nAlreadyRead:" + alreadyRead + "\nSongLen:" + song.Length;
                            Array.Copy(incomingData, 0, song, alreadyRead, copyLen);
                            alreadyRead += copyLen;
                            readInPacket += copyLen;
                            //statusText.text = "copyLen: " + copyLen + "\nIncomingData:" + incomingData.Length + "\nAlreadyRead:" + alreadyRead + "\nSongLen:" + song.Length;
                            statusText.text = "reading audio (" + ((float)(int)((float)alreadyRead/song.Length*10000)/100) + "/100)...";
                            if (alreadyRead == song.Length)
                            {
                                rg = ReadingGoal.json;
                                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/multi/songs");
                                File.WriteAllBytes(Application.persistentDataPath + "/multi/songs/transferred.wav", song);
                                StartCoroutine(AudioDownloader.GetAndPlayAudioClip("multi/songs/transferred.wav", AudioType.WAV, audioPlayer));
                            }
                        }
                        Debug.Log("server message received as: " + incomingData.Length);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }

        Debug.Log("Exiting...");
    }
    /// <summary>   
    /// Send message to server using socket connection.     
    /// </summary>  
    private void SendMessage(byte[] clientMessageAsByteArray)
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            Debug.Log("attempt to send " + clientMessageAsByteArray[0]);
            // Get a stream object for writing.             
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                //string clientMessage = "This is a message from one of your clients.";
                // Convert string message to byte array.                 
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                if (!firstRequest)
                {
                    firstRequest = true;
                }
                Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}