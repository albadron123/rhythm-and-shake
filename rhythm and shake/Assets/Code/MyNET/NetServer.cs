using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public class NetServer : MonoBehaviour
{
    [SerializeField]
    LevelSelection lvlSelection;

    [SerializeField]
    TMPro.TMP_Text serverText;

    [SerializeField]
    GameObject ipButtonPrefab;
    [SerializeField]
    Transform canvasTransform;

    List<GameObject> ipButtonInstances = new List<GameObject>();

    string ip;
    const int port = 8080;

    bool isRunning = false;
    bool isConnected = false;

    IPEndPoint tcpEndPoint;
    Socket tcpSocket;
    Socket listener;
    Thread networkThread = null;

    string jsonAddress = "";
    string audioAddress = "";



    static List<string> GetLocalIPAddresses()
    {
        List<string> Ips = new List<string>();
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                //Debug.Log(ip.ToString());
                Ips.Add(ip.ToString());
            }
        }
        return Ips;
    }

    


    void Start()
    {
        //find my ip
        //ip = 
    }

    void Update()
    {
    
    }

    public void ConnectDisconnect()
    {
        if (isRunning)
        {
            StopServer();
        }
        else
        {
            //IT's TEMPORALY HERE
            //JUST NOT TO CONFUSE LOCAL AND NETWORK CODE
            NetworkStatus.isActive = true;


            List<string> ips = GetLocalIPAddresses();
            if (ips.Count < 1)
            {
                serverText.text = "Вероятно, вы не подключены к интернетету :^(";
                return;
            }
            if (ips.Count > 1)
            {
                if (ipButtonInstances.Count > 0)
                {
                    //WE DISCONNECT BEFORE CONNECTION HERE
                    serverText.text = " ";
                    NetworkStatus.isActive = false;
                    foreach (GameObject g in ipButtonInstances)
                    {
                        Destroy(g);
                    }
                    ipButtonInstances.Clear();
                }
                else
                {
                    serverText.text = "Похоже, у вас много ip-адресов. Выберите тот, с которого хотите транслировать игру...";
                    Debug.Log(ips.Count);
                    for (int i = 0; i < ips.Count; ++i)
                    {
                        Debug.Log(ips[i]);
                        GameObject inst = Instantiate(ipButtonPrefab, Vector3.zero, Quaternion.identity, canvasTransform);
                        inst.transform.localPosition = new Vector3(0, -500, 0) - new Vector3(0, (i + 1) * 75, 0);
                        Debug.Log(inst.transform.position);
                        inst.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = ips[i];
                        var temp = i;
                        inst.GetComponent<Button>().onClick.AddListener(delegate { SelectIPAddressAndConnect(ips[temp]); });
                        ipButtonInstances.Add(inst);
                    }
                }
            }
            else
            {
                ip = ips[0];
                StartServer();
            }
        }
    }

    public void SelectIPAddressAndConnect(string ip)
    {
        this.ip = ip;
        foreach (GameObject g in ipButtonInstances)
        {
            Destroy(g);
        }
        ipButtonInstances.Clear();
        StartServer();
    }

    private void StartServer()
    {
        try
        {
            isRunning = true;
            tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.Bind(tcpEndPoint);
            tcpSocket.Listen(1);
            networkThread = new Thread(RunServer);
            networkThread.Start();
            serverText.text = "Вы запустились с ip адреса " + ip;
            NetworkStatus.networkState = NetState.start;
            NetworkStatus.networkEnd = NetworkEnd.server;
            jsonAddress = lvlSelection.GetCurrentSongJSONAddress();
            audioAddress = lvlSelection.GetCurrentSongAudioAddress();
            NetworkStatus.song = lvlSelection.GetCurrentSong();
        }
        catch (Exception e)
        {
            serverText.text = "Не получается создать LAN сервер.\nОшибка: " + e.Message + "\n";
        }
    }

    private void StopServer()
    {
        isRunning = false;
        isConnected = false;
        serverText.text = "Нажмите, чтобы играть с другом";
        try
        {
            tcpSocket.Shutdown(SocketShutdown.Both);
        }
        catch(Exception e)
        {
            Debug.Log("Socket was already disconnected");
        }
        if(tcpSocket != null) tcpSocket.Close();
        tcpSocket = null;
        try
        {
            listener.Shutdown(SocketShutdown.Both);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        if (listener != null) listener.Close();
        listener = null;
        if (networkThread != null)
        {
            networkThread.Abort();
        }
        networkThread = null;
        NetworkStatus.ReInit();
    }

    void RunServer()
    {
        try
        {
            while (isRunning)
            {
                try
                {
                    listener = tcpSocket.Accept();
                    NetworkStatus.status = "К вам кто-то подключился!";
                    NetworkStatus.coroutineScene = "Lobby";
                }
                catch (Exception e)
                {
                    NetworkStatus.status = "Не получилось устоновить соединение: " + e.Message;
                    continue;
                }
                isConnected = true;

                while (isConnected)
                {

                    List<byte> msg = new List<byte>();
                    byte[] buffer = new byte[256];
                    int bytesInBuffer = 0;


                    //try to get message from client
                    try
                    {
                        do
                        {
                            bytesInBuffer = listener.Receive(buffer);
                            if (bytesInBuffer == 256)
                            {
                                msg.AddRange(buffer);
                            }
                            else
                            {
                                for (int i = 0; i < bytesInBuffer; ++i)
                                {
                                    msg.Add(buffer[i]);
                                }
                            }
                        }
                        while (listener.Available > 0);
                    }
                    catch (Exception e)
                    {
                        NetworkStatus.status = "Клиент не отвечает. " + e.Message;
                        isConnected = false;
                        break;
                    }

#if __DEBUG
                string messageLog = "codes: ";
                for (int i = 0; i < msg.Count; ++i)
                {
                    messageLog += ((int)msg[i]).ToString() + " ";
                }
                Debug.Log(messageLog);
                NetworkStatus.status = messageLog;
#endif


                    //Analyze message
                    byte[] outMessage;

                    //timing story
                    if (msg[0] == Requests.ACK)
                    {
                        double currentTime = NetworkStatus.sw.Elapsed.TotalMilliseconds;
                        double thatTime = BitConverter.ToDouble(msg.ToArray(), 1);
                        double precision = (currentTime - NetworkStatus.lastSentTime) / 2;
                        if (precision < NetworkStatus.precision)
                        {
                            NetworkStatus.precision = precision;
                            NetworkStatus.globalDelta = (currentTime - precision) - thatTime;
#if __DEBUG
                        Debug.Log(NetworkStatus.globalDelta + "::" + precision);
#endif
                        }
                    }

                    if (NetworkStatus.networkState == NetState.start)
                    {
                        if (msg[0] == Requests.LOAD_REQ)
                        {
                            //prepare song data for CLIENT  
                            //IMPORTANT:
                            //SONG DATA STRUCTURE:
                            //4 bytes -- size of audio
                            //4 bytes -- size of json
                            //audio file
                            //json file
                            NetworkStatus.networkState = NetState.load;
                            NetworkStatus.status = "Передаем уровень другому игроку...";
                            //Right now song params is 8 bit-flags with such meanings (from least significant bit)
                            //0 -- audio type (0 -- wav, 1 -- mpeg)
                            //1 -- reserved for later
                            //2 -- reserved for later
                            //3 -- reserved for later
                            //4 -- reserved for later
                            //5 -- reserved for later
                            //6 -- reserved for later
                            //7 -- reserved for later
                            byte songParams = 0;
                            if (audioAddress.Contains(".mp3"))
                            {
                                songParams |= 1;
                            }
                            byte[] audioArray = File.ReadAllBytes(audioAddress);
                            byte[] jsonArray = File.ReadAllBytes(jsonAddress);
                            byte[] audioArraySize = MyUtitities.IntToByteArray(audioArray.Length);
                            byte[] jsonArraySize = MyUtitities.IntToByteArray(jsonArray.Length);
                            outMessage = new byte[audioArray.Length + jsonArray.Length + 9];
                            outMessage[0] = songParams;
                            Array.Copy(audioArraySize, 0, outMessage, 1, 4);
                            Array.Copy(jsonArraySize, 0, outMessage, 5, 4);
                            Array.Copy(audioArray, 0, outMessage, 9, audioArray.Length);
                            Array.Copy(jsonArray, 0, outMessage, audioArray.Length + 9, jsonArray.Length);
                        }
                        else
                        {
                            NetworkStatus.lastSentTime = NetworkStatus.sw.Elapsed.TotalMilliseconds;
                            byte[] timeBytes = BitConverter.GetBytes(NetworkStatus.lastSentTime);
                            outMessage = new byte[] { Requests.ACK, timeBytes[0], timeBytes[1], timeBytes[2], timeBytes[3], timeBytes[4], timeBytes[5], timeBytes[6], timeBytes[7] };
                        }
                    }
                    else if (NetworkStatus.networkState == NetState.load)
                    {
                        if (msg[0] == Requests.ACK)
                        {
                            NetworkStatus.networkState = NetState.pause;
                            //LATER MAY BE TARGET SCENE HERE
                        }
                        NetworkStatus.lastSentTime = NetworkStatus.sw.Elapsed.TotalMilliseconds;
                        byte[] timeBytes = BitConverter.GetBytes(NetworkStatus.lastSentTime);
                        outMessage = new byte[] { Requests.ACK, timeBytes[0], timeBytes[1], timeBytes[2], timeBytes[3], timeBytes[4], timeBytes[5], timeBytes[6], timeBytes[7] };
                    }
                    else if (NetworkStatus.networkState == NetState.pause)
                    {
                        if (msg[0] == Requests.START_REQ)
                        {
                            NetworkStatus.hisReq = true;
                            if (NetworkStatus.myReq == true)
                            {
                                //start or unset pause
                                NetworkStatus.networkState = NetState.run;
                                NetworkStatus.myReq = false;
                                NetworkStatus.hisReq = false;
                                NetworkStatus.backInterruptionFlags |= Requests.BACK_REQ_RUN;

                                NetworkStatus.plannedTimeToStart = BitConverter.ToDouble(msg.ToArray(), 1) + NetworkStatus.globalDelta;
                                NetworkStatus.isRunning = false;
                            }
                            else
                            {
                                NetworkStatus.backInterruptionFlags |= Requests.BACK_REQ_SHOW_2D_READY;
                            }
                            NetworkStatus.lastSentTime = NetworkStatus.sw.Elapsed.TotalMilliseconds;
                            byte[] timeBytes = BitConverter.GetBytes(NetworkStatus.lastSentTime);
                            outMessage = new byte[] { Requests.ACK, timeBytes[0], timeBytes[1], timeBytes[2], timeBytes[3], timeBytes[4], timeBytes[5], timeBytes[6], timeBytes[7] };
                        }
                        else if (msg[0] == Requests.SCORE_REQ)
                        {
                            //let everybody know about score change!
                            NetworkStatus.hisScore = MyUtitities.ByteArrayToInt(msg.ToArray(), 1);
                            NetworkStatus.backInterruptionFlags |= Requests.BACK_REQ_SCORE;
                            NetworkStatus.lastSentTime = NetworkStatus.sw.Elapsed.TotalMilliseconds;
                            byte[] timeBytes = BitConverter.GetBytes(NetworkStatus.lastSentTime);
                            outMessage = new byte[] { Requests.ACK, timeBytes[0], timeBytes[1], timeBytes[2], timeBytes[3], timeBytes[4], timeBytes[5], timeBytes[6], timeBytes[7] };
                        }
                        else if (NetworkStatus.GetReqInterruption() == true)
                        {
                            NetworkStatus.SetReqInterruption(false);
                            NetworkStatus.myReq = true;
                            if (NetworkStatus.hisReq == true)
                            {
                                //start or unset pause
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
                                outMessage = new byte[1] { Requests.START_REQ };
                            }
                        }
                        else if (NetworkStatus.GetScoreInterruption() == true)
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
                        if (msg[0] == Requests.PAUSE_REQ)
                        {
                            NetworkStatus.networkState = NetState.pause;
                            //IMPORTANT: isRunning ALWAYS after setting networkState
                            NetworkStatus.isRunning = false;
                            //decyphering pause sync value
                            NetworkStatus.pauseTime = BitConverter.ToSingle(msg.ToArray(), 1);
                            NetworkStatus.backInterruptionFlags |= Requests.BACK_REQ_PAUSE;
                            //forming ACC answer
                            NetworkStatus.lastSentTime = NetworkStatus.sw.Elapsed.TotalMilliseconds;
                            byte[] timeBytes = BitConverter.GetBytes(NetworkStatus.lastSentTime);
                            outMessage = new byte[] { Requests.ACK, timeBytes[0], timeBytes[1], timeBytes[2], timeBytes[3], timeBytes[4], timeBytes[5], timeBytes[6], timeBytes[7] };
                        }
                        else if (msg[0] == Requests.SCORE_REQ)
                        {
                            NetworkStatus.hisScore = MyUtitities.ByteArrayToInt(msg.ToArray(), 1);
                            NetworkStatus.backInterruptionFlags |= Requests.BACK_REQ_SCORE;
                            NetworkStatus.lastSentTime = NetworkStatus.sw.Elapsed.TotalMilliseconds;
                            byte[] timeBytes = BitConverter.GetBytes(NetworkStatus.lastSentTime);
                            outMessage = new byte[] { Requests.ACK, timeBytes[0], timeBytes[1], timeBytes[2], timeBytes[3], timeBytes[4], timeBytes[5], timeBytes[6], timeBytes[7] };
                        }
                        else if (NetworkStatus.GetScoreInterruption() == true)
                        {
                            NetworkStatus.SetScoreInterruption(false);
                            byte[] scores = MyUtitities.IntToByteArray(NetworkStatus.myScore);
                            outMessage = new byte[5] { Requests.SCORE_REQ, scores[0], scores[1], scores[2], scores[3] };
                        }
                        else if (NetworkStatus.GetReqInterruption() == true)
                        {
                            NetworkStatus.SetReqInterruption(false);
                            NetworkStatus.networkState = NetState.pause;
                            //IMPORTANT: isRunning ALWAYS after setting networkState
                            NetworkStatus.isRunning = false;
                            //sending synced pause time
                            byte[] pauseTimeBytes = BitConverter.GetBytes(NetworkStatus.pauseTime);
                            outMessage = new byte[] { Requests.PAUSE_REQ, pauseTimeBytes[0], pauseTimeBytes[1], pauseTimeBytes[2], pauseTimeBytes[3] };
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
                    //LogByteArray(msg.ToArray(), msg.Count);

                    try
                    {
                        listener.Send(outMessage);
                    }
                    catch (Exception e)
                    {
                        NetworkStatus.status = "Не получилось связаться с клиентом: " + e.Message;
                        isConnected = false;
                        break;
                    }

                }

                try
                {
                    listener.Shutdown(SocketShutdown.Both);
                    listener.Close();
                    NetworkStatus.status = "Упс... от вас отключились.";
                }
                catch (Exception e)
                {

                    NetworkStatus.status = "Упс... от вас отключились.";
                }
            }
        }
        catch (ThreadAbortException exp)
        {
            Debug.Log("Thread Aborted!\n" + exp.Message);
        }
        finally 
        {
            //Thread.ResetAbort();
        }
    }

}
