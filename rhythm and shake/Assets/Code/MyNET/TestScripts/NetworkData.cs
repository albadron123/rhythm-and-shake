using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkData : MonoBehaviour
{
    public static byte myScore = 0;
    public static byte otherScore = 0;

    [SerializeField]
    TMPro.TMP_Text te;

    // Start is called before the first frame update
    void Start()
    {
        int a = 12345678;
        byte[] data = {0,
                                (byte)(a & 0b11111111),
                                (byte)((a >> 8) & 0b11111111),
                                (byte)((a >> 16) & 0b11111111),
                                (byte)((a >> 24) & 0b11111111)};
        int b = (int)data[1] + ((int)data[2] << 8) + ((int)data[3] << 16) + ((int)data[4] << 24);
        Debug.Log(a + " " + b);
        Debug.Log(data[0] + "\n" + data[1] + (((int)data[2]) << 8) + "\n" + (data[3]<<16) + "\n" + ((int)data[4] << 24) + "\n");
    }

    // Update is called once per frame
    void Update()
    {
        te.text = myScore + "\t" + otherScore;
    }


}
