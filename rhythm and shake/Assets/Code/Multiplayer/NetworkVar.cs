using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Net.Sockets;

public class NetworkVar : NetworkBehaviour
{
    /// <summary>
    /// TODO: 
    /// 1. ���������� � �������� ��������� 
    /// 2. ���������� 2� ������� � ����������� �� ����� �� ����������
    /// 3. ���������� � ������� ������� � �������� ����� � ������� �� �������
    /// 4. ��� ��� �����������
    /// 5. ��� �� ����������� � host, ���������������� ����� ������ ����������
    /// 6. ��������� ��������� �������
    /// </summary>




    [SerializeField]
    TMPro.TMP_Text scoreText;

    [SerializeField]
    bool isHost = false;


    public NetworkVariable<bool> State = new NetworkVariable<bool>();
    public NetworkVariable<int> Score = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        State.OnValueChanged += OnStateChanged;
        Score.OnValueChanged += OnScoreChanged;
    }

    public override void OnNetworkDespawn()
    {
        State.OnValueChanged -= OnStateChanged;
        Score.OnValueChanged -= OnScoreChanged;
    }

    public void OnStateChanged(bool previous, bool current)
    {
        // note: `State.Value` will be equal to `current` here
        if (State.Value)
        {
            
            if(!isHost) 
                GameObject.Find("Main Camera").GetComponent<SongGenerator>().StartSong();
            // door is open:
            //  - rotate door transform
            //  - play animations, sound etc.
        }
        else
        {
            // door is closed:
            //  - rotate door transform
            //  - play animations, sound etc.
        }
    }

    public void OnScoreChanged(int previous, int current)
    {
        scoreText.text = Score.Value.ToString();
    }


    [ServerRpc(RequireOwnership = false)]
    public void ToggleServerRpc()
    {
        // this will cause a replication over the network
        // and ultimately invoke `OnValueChanged` on receivers
        State.Value = !State.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeScoreServerRpc(int delta)
    {
        Score.Value = Score.Value + delta;
    }


}
