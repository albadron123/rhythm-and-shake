using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NetState
{
    start, load, pause, run, disconnected
}

public static class Requests
{
    public const byte LOAD_REQ = 1;
    public const byte START_REQ = 2;
    public const byte PAUSE_REQ = 3;
    public const byte SCORE_REQ = 4;
    public const byte ACK = 5;
    public const byte BACK_REQ_RUN = 0b00000001;
    public const byte BACK_REQ_PAUSE = 0b00000010;
    public const byte BACK_REQ_SHOW_2D_READY = 0b00000100;
    public const byte BACK_REQ_SCORE = 0b00001000;
    public const byte BACK_REQ_UNPACK_TRACKS = 0b00010000;
}
