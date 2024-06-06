using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public class NetClient : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text ipText;
    [SerializeField]
    TMPro.TMP_Text statusText;

    //string status;

    string ip = "";
    const int port = 8080;

    IPEndPoint tcpEndPoint;
    Socket tcpSocket;
    Thread clientThread = null;

    bool isConnected = false;

    bool batchedRegime = false;
    int batchedMessageSize = -1;

    string persistentDataPath = "";

    void Start()
    {
        persistentDataPath = Application.persistentDataPath;
    }

    void Update()
    {

    }


    public void Connect()
    {
        try
        {
            //IT's TEMPORALY HERE
            //JUST NOT TO CONFUSE LOCAL AND NETWORK CODE
            NetworkStatus.isActive = true;

            ip = ipText.text.Remove(ipText.text.Length-1);
            Debug.Log(ip);
            tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.Connect(tcpEndPoint);
            clientThread = new Thread(RunClient);
            clientThread.Start();
            NetworkStatus.networkState = NetState.start;
            NetworkStatus.networkEnd = NetworkEnd.client;
            NetworkStatus.coroutineScene = "Lobby";
            //SceneManager.LoadScene("Lobby");
        }
        catch (SocketException ex)
        {
            statusText.text = "Не удалось подключиться.\nОшибка:" + ex.Message;
            return;
        }
    }

    public void Disconnect()
    {
        if (clientThread != null)
        {
            clientThread.Abort();
        }
        isConnected = false;
        NetworkStatus.networkEnd = NetworkEnd.none;
        //zero everything else is net status 
    }

    void RunClient()
    {
        isConnected = true;
        NetworkStatus.status = "Вы подключились!";
        byte[] outMessage = new byte[0];
        byte[] inMessage = new byte[0];
        List<byte> inMessageList = new List<byte>();
        while (isConnected)
        {
            //form message
            //this is pure test
            if(!batchedRegime)
            { 
                if (NetworkStatus.networkState == NetState.start)
                {
                    outMessage = new byte[] { Requests.LOAD_REQ };
                    NetworkStatus.networkState = NetState.load;
                    batchedRegime = true;
                }
                else if (NetworkStatus.networkState == NetState.load)
                {
                    //later may be check what is coming or the size of it?
                    NetworkStatus.networkState = NetState.pause;
                    //LATER MAY BE TARGET SCENE HERE 
                    NetworkStatus.lastSentTime = NetworkStatus.sw.Elapsed.TotalMilliseconds;
                    byte[] timeBytes = BitConverter.GetBytes(NetworkStatus.lastSentTime);
                    outMessage = new byte[] { Requests.ACK, timeBytes[0], timeBytes[1], timeBytes[2], timeBytes[3], timeBytes[4], timeBytes[5], timeBytes[6], timeBytes[7] };
                }
                else if (NetworkStatus.networkState == NetState.pause)
                {
                    if (NetworkStatus.GetReqInterruption())
                    {
                        NetworkStatus.SetReqInterruption(false);
                        NetworkStatus.myReq = true;
                        if (NetworkStatus.hisReq == true)
                        {
                            NetworkStatus.networkState = NetState.run;
                            NetworkStatus.myReq = false;
                            NetworkStatus.hisReq = false;
                            //RUN
                            NetworkStatus.backInterruptionFlags |= Requests.BACK_REQ_RUN;
                            NetworkStatus.plannedTimeToStart = NetworkStatus.sw.Elapsed.TotalMilliseconds + 3000;
                            NetworkStatus.isRunning = false;
                            byte[] plannedTimeBytes = BitConverter.GetBytes(NetworkStatus.plannedTimeToStart);
                            outMessage = new byte[] { Requests.START_REQ, plannedTimeBytes[0], plannedTimeBytes[1], plannedTimeBytes[2], plannedTimeBytes[3], plannedTimeBytes[4], plannedTimeBytes[5], plannedTimeBytes[6], plannedTimeBytes[7] };
                        }
                        else
                        {
                            outMessage = new byte[] { Requests.START_REQ };
                        }
                    }
                    else if (NetworkStatus.GetScoreInterruption())
                    {
                        NetworkStatus.SetScoreInterruption(false);
                        byte[] scores = MyUtitities.IntToByteArray(NetworkStatus.myScore);
                        outMessage = new byte[] { Requests.SCORE_REQ, scores[0], scores[1], scores[2], scores[3] };
                    }
                    else
                    {
                        NetworkStatus.lastSentTime = NetworkStatus.sw.Elapsed.TotalMilliseconds;
                        byte[] timeBytes = BitConverter.GetBytes(NetworkStatus.lastSentTime);
                        outMessage = new byte[] { Requests.ACK, timeBytes[0], timeBytes[1], timeBytes[2], timeBytes[3], timeBytes[4], timeBytes[5], timeBytes[6], timeBytes[7] };
                    }
                }
                else if (NetworkStatus.networkState == NetState.run)
                {
                    if (NetworkStatus.GetReqInterruption())
                    {
                        NetworkStatus.SetReqInterruption(false);
                        NetworkStatus.networkState = NetState.pause;
                        //IMPORTANT: isRunning ALWAYS after setting networkState
                        NetworkStatus.isRunning = false;
                        //sending synced pause time
                        byte[] pauseTimeBytes = BitConverter.GetBytes(NetworkStatus.pauseTime);
                        outMessage = new byte[] { Requests.PAUSE_REQ, pauseTimeBytes[0], pauseTimeBytes[1], pauseTimeBytes[2], pauseTimeBytes[3] };
                    }
                    else if (NetworkStatus.GetScoreInterruption())
                    {
                        NetworkStatus.SetScoreInterruption(false);
                        byte[] scores = MyUtitities.IntToByteArray(NetworkStatus.myScore);
                        outMessage = new byte[] { Requests.SCORE_REQ, scores[0], scores[1], scores[2], scores[3] };
                    }
                    else
                    {
                        NetworkStatus.lastSentTime = NetworkStatus.sw.Elapsed.TotalMilliseconds;
                        byte[] timeBytes = BitConverter.GetBytes(NetworkStatus.lastSentTime);
                        outMessage = new byte[] { Requests.ACK, timeBytes[0], timeBytes[1], timeBytes[2], timeBytes[3], timeBytes[4], timeBytes[5], timeBytes[6], timeBytes[7] };
                    }
                }
                else
                {
                    NetworkStatus.lastSentTime = NetworkStatus.sw.Elapsed.TotalMilliseconds;
                    byte[] timeBytes = BitConverter.GetBytes(NetworkStatus.lastSentTime);
                    outMessage = new byte[] { Requests.ACK, timeBytes[0], timeBytes[1], timeBytes[2], timeBytes[3], timeBytes[4], timeBytes[5], timeBytes[6], timeBytes[7] };
                }

                try
                {
                    tcpSocket.Send(outMessage);
                }
                catch (Exception e)
                {
                    NetworkStatus.status = "Сервер не отвечает...";
                    isConnected = false;
                    break;
                }
                inMessageList = new List<byte>();
            }

            //RECIEVE DATA CYCLE

            byte[] buffer = new byte[256];
            int bytesInBuffer = 0;
            //try to get message from server
            try
            {
                do
                {                    
                    bytesInBuffer = tcpSocket.Receive(buffer);
                    if (bytesInBuffer == 256)
                    {
                        inMessageList.AddRange(buffer);
                    }
                    else
                    {
                        for (int i = 0; i < bytesInBuffer; ++i)
                        {
                            inMessageList.Add(buffer[i]);
                        }
                    }
                }
                while (tcpSocket.Available > 0);
            }
            catch (Exception e)
            {
                NetworkStatus.status = "Сервер не отвечает" + e.Message;
                isConnected = false;
                break;
            }



            if (batchedRegime)
            {
                //now only audio loading comes in batched regime
                //if later something else will be, it will be needed to create a seperate automata for it
                if (batchedMessageSize <= 0)
                {
                    int audioSize = (int)inMessageList[1] + ((int)inMessageList[2] << 8) + ((int)inMessageList[3] << 16) + ((int)inMessageList[4] << 24);
                    int jsonSize = (int)inMessageList[5] + ((int)inMessageList[6] << 8) + ((int)inMessageList[7] << 16) + ((int)inMessageList[8] << 24);
                    batchedMessageSize = audioSize + jsonSize + 9;
                }
                NetworkStatus.status = "Загрузка уровня: " + ((int)((float)100*inMessageList.Count/batchedMessageSize)).ToString() + " %";
                if (inMessageList.Count >= batchedMessageSize)
                {
                    NetworkStatus.status = " ";
                    batchedRegime = false;
                    batchedMessageSize = -1;
                }
            }

            if(!batchedRegime)
            {

                inMessage = inMessageList.ToArray();

                //timing story
                if (NetworkStatus.networkState != NetState.load && inMessage[0] == Requests.ACK)
                {
                    double currentTime = NetworkStatus.sw.Elapsed.TotalMilliseconds;
                    double thatTime = BitConverter.ToDouble(inMessage, 1);
                    double precision = (currentTime - NetworkStatus.lastSentTime) / 2;
                    if (precision < NetworkStatus.precision)
                    {
                        NetworkStatus.precision = precision;
                        NetworkStatus.globalDelta = (currentTime - precision) - thatTime;
                        Debug.Log(NetworkStatus.globalDelta + "::" + precision);
                    }
                }

                if (NetworkStatus.networkState == NetState.load)
                {
                    //IMPORTANT:
                    //SONG DATA STRUCTURE:
                    //4 bytes -- size of audio
                    //4 bytes -- size of json
                    //audio file
                    //json file
                    Debug.Log("we are loading!");
                    int audioSize = (int)inMessage[1] + ((int)inMessage[2] << 8) + ((int)inMessage[3] << 16) + ((int)inMessage[4] << 24);
                    int jsonSize = (int)inMessage[5] + ((int)inMessage[6] << 8) + ((int)inMessage[7] << 16) + ((int)inMessage[8] << 24);
                    byte[] audioBytes = new byte[audioSize];
                    byte[] jsonBytes = new byte[jsonSize];
                    Array.Copy(inMessage, 9, audioBytes, 0, audioSize);
                    Array.Copy(inMessage, 9 + audioSize, jsonBytes, 0, jsonSize);
                    NetworkStatus.songParams = inMessage[0];
                    System.IO.Directory.CreateDirectory(persistentDataPath + "/multi/songs");
                    if((NetworkStatus.songParams & 0b00000001) == 1)
                        File.WriteAllBytes(persistentDataPath + "/multi/songs/transferred.mp3", audioBytes);
                    else
                        File.WriteAllBytes(persistentDataPath + "/multi/songs/transferred.wav", audioBytes);
                    File.WriteAllBytes(persistentDataPath + "/multi/transferred.json", jsonBytes);
                    NetworkStatus.backInterruptionFlags |= Requests.BACK_REQ_UNPACK_TRACKS;
                }
                else if (NetworkStatus.networkState == NetState.pause)
                {
                    if (inMessage[0] == Requests.START_REQ)
                    {
                        NetworkStatus.hisReq = true;
                        if (NetworkStatus.myReq == true)
                        {
                            NetworkStatus.SetReqInterruption(false);
                            NetworkStatus.hisReq = false;
                            NetworkStatus.myReq = false;
                            NetworkStatus.networkState = NetState.run;
                            NetworkStatus.backInterruptionFlags |= Requests.BACK_REQ_RUN;

                            NetworkStatus.plannedTimeToStart = BitConverter.ToDouble(inMessage, 1) + NetworkStatus.globalDelta;
                            NetworkStatus.isRunning = false;
                        }
                        else
                        {
                            NetworkStatus.backInterruptionFlags |= Requests.BACK_REQ_SHOW_2D_READY;
                        }
                    }
                    if (inMessage[0] == Requests.SCORE_REQ)
                    {
                        NetworkStatus.hisScore = MyUtitities.ByteArrayToInt(inMessage, 1);
                        NetworkStatus.backInterruptionFlags |= Requests.BACK_REQ_SCORE;
                    }
                }
                else if (NetworkStatus.networkState == NetState.run)
                {
                    //TODO: may be also recieve req.pause in pause state to sync in situations when pause is pressed almost simultaneously
                    if (inMessage[0] == Requests.PAUSE_REQ)
                    {
                        NetworkStatus.SetReqInterruption(false);
                        NetworkStatus.hisReq = false;
                        NetworkStatus.myReq = false;
                        NetworkStatus.networkState = NetState.pause;
                        //IMPORTANT: isRunning ALWAYS after setting networkState
                        NetworkStatus.isRunning = false;
                        //decyphering pause sync value
                        NetworkStatus.pauseTime = BitConverter.ToSingle(inMessage, 1);
                        NetworkStatus.backInterruptionFlags |= Requests.BACK_REQ_PAUSE;
                    }
                    if (inMessage[0] == Requests.SCORE_REQ)
                    {
                        NetworkStatus.hisScore = MyUtitities.ByteArrayToInt(inMessage, 1);
                        NetworkStatus.backInterruptionFlags |= Requests.BACK_REQ_SCORE;
                    }
                }
            }

        }

        try
        {
            tcpSocket.Shutdown(SocketShutdown.Both);
            tcpSocket.Close();
            NetworkStatus.status = "Нет подключения";
        }
        catch (Exception e)
        {

            NetworkStatus.status = "Нет подключения";
        }

    }
}
