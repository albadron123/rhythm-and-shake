using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GyroTest : MonoBehaviour
{
    [SerializeField]
    GameObject cube;

    [SerializeField]
    GameObject b1;
    [SerializeField]
    GameObject b2;
    [SerializeField]
    GameObject b3;

    [SerializeField]
    TMPro.TMP_Text t;

    public bool _supported;

    private Quaternion _off;
    private Vector3 _offEuler;
    int _activeSemaphore = 0;
    private float _degreesForFullTilt = 10;

    public Vector2 _lastTilt;

    public void Init()
    {
        _off = Quaternion.identity;
        _supported = SystemInfo.supportsGyroscope;
    }

    public bool Activate(bool isActivated)
    {
        if (isActivated) _activeSemaphore++;
        else _activeSemaphore--;

        _activeSemaphore = Mathf.Max(_activeSemaphore, 0);

        if (_activeSemaphore > 0)
        {
            if (_supported)
            {
                Input.gyro.enabled = true;
            }
            else
            {
                return false; //everything not ok; you requested gyro but can't have it!
            }
        }
        else
        {
            if (_supported)
            {
                Input.gyro.enabled = false;
            }
        }
        return true; //everything ok;

    }

    public void Deactivate()
    {
        _activeSemaphore = 0;
    }

    public void SetCurrentReadingAsFlat()
    {
        _off = Input.gyro.attitude;
        _offEuler = _off.eulerAngles;
    }

    public Vector3 GetReading()
    {
        if (_supported)
        {
            return (Quaternion.Inverse(_off) * Input.gyro.attitude).eulerAngles;
        }
        else
        {
            Debug.LogError("Tried to get gyroscope reading on a device which didn't have one.");
            return Vector3.zero;
        }
    }

    public Vector2 Get2DTilt()
    {
        Vector3 reading = GetReading();

        Vector2 tilt = new Vector2(
            -Mathf.DeltaAngle(reading.y, 0),
            Mathf.DeltaAngle(reading.x, 0)
        );

        //can't go over max
        tilt.x = Mathf.InverseLerp(-_degreesForFullTilt, _degreesForFullTilt, tilt.x) * 2 - 1;
        tilt.y = Mathf.InverseLerp(-_degreesForFullTilt, _degreesForFullTilt, tilt.y) * 2 - 1;

        //get phase
        tilt.x = Mathf.Clamp(tilt.x, -1, 1);
        tilt.y = Mathf.Clamp(tilt.y, -1, 1);

        _lastTilt = tilt;

        return tilt;
    }

    public string GetExplanation()
    {
        Vector3 reading = GetReading();

        string msg = "";

        msg += "OFF: " + _offEuler + "\n";

        Vector2 tilt = new Vector2(
            -Mathf.DeltaAngle(reading.y, 0),
            Mathf.DeltaAngle(reading.x, 0)
        );

        msg += "DELTA: " + tilt + "\n";

        //can't go over max
        tilt.x = Mathf.InverseLerp(-_degreesForFullTilt, _degreesForFullTilt, tilt.x) * 2 - 1;
        tilt.y = Mathf.InverseLerp(-_degreesForFullTilt, _degreesForFullTilt, tilt.y) * 2 - 1;

        msg += "LERPED: " + tilt + "\n";

        //get phase
        tilt.x = Mathf.Clamp(tilt.x, -1, 1);
        tilt.y = Mathf.Clamp(tilt.y, -1, 1);

        msg += "CLAMPED: " + tilt + "\n";

        return msg;

    }

    public void SetDegreesForFullTilt(float degrees)
    {
        _degreesForFullTilt = degrees;
    }

    private void Start()
    {
        Input.simulateMouseWithTouches = true;
        Init();
        Activate(true);
        SetDegreesForFullTilt(85);
        SetCurrentReadingAsFlat();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetCurrentReadingAsFlat();
        }

        t.text = GetExplanation();
    }
}
