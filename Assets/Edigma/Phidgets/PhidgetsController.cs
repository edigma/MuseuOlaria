using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Phidget22;
using Phidget22.Events;
using System;
public class PhidgetsController : MonoBehaviour
{
    public static PhidgetsController Instance = null;
    private Encoder encoder = null;
    float levelValue = 2.0f;
    bool levelSelectionEnabled = false;
    bool phidgetsReady = false;
    int m_phidgetSerialNumber = 0;//682604;
    public CanvasGroup lockedCanvas;
    int m_position = 0;

    double m_encoderTime = 0;
    public float M_Position
    {
        get
        {
            float ret = (m_position / 1440.0f) * 360f;
            return ret;
        }
    }

    public float M_DataInterval
    {
        get
        {
            try
            {
                if (encoder == null)
                {
                    return 0.1f;
                }
                return encoder.DataInterval / 1000.0f;
            }

            catch (Exception e)
            {
                return 0.1f;
            }
        }
    }
    public double M_EncoderTime
    {
        get { return m_encoderTime; }
    }

    public float GetLevelValue
    {
        get { return levelValue; }
    }

    private static void Encoder_PositionChange(object sender, Phidget22.Events.EncoderPositionChangeEventArgs e)
    {
        // Access event source via the sender object
        Encoder ch = (Encoder)sender;

        // Access event data via the EventArgs
        int positionChange = e.PositionChange;
        double timeChange = e.TimeChange;
        bool indexTriggered = e.IndexTriggered;
        Instance.m_position = positionChange;
        //Instance.m_encoderTime = timeChange
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (Instance == null)
        {
            Instance = this;
            Invoke("CreatePhidgets", 2.0f);
        }
        else
        {
            Destroy(this.gameObject);
            Debug.Log("Destroy this");
        }
    }


    private void OnDestroy()
    {

        if (Instance != this)
        {
            return;
        }
        encoder.Close();
        encoder = null;

        Debug.Log("Phidgets Destroyed!!!");
    }

    void CreatePhidgets()
    {
        Debug.Log("INIT PHIDGETS");

        m_phidgetSerialNumber = BDController.Instance.PhidgetSerialNumber();

        DebugText.Instance.SetText("INIT PHIDGETS " + m_phidgetSerialNumber);

        if(m_phidgetSerialNumber == 0) {
            DebugText.Instance.SetText("FAILED PHIDGETS RETRYING");
            Invoke("CreatePhidgets", 2.0f);
            return;
        }

        try
        {
            encoder = new Encoder();
            encoder.DeviceSerialNumber = m_phidgetSerialNumber;
            encoder.Open(250);
            encoder.DataInterval = encoder.MinDataInterval;
            encoder.PositionChange += Encoder_PositionChange;
            phidgetsReady = true;
            DebugText.Instance.SetText("INIT PHIDGETS OK");
        }
        catch (PhidgetException ex)
        {
            Debug.Log("Failure: " + ex.Message);
            DebugText.Instance.SetText("INIT PHIDGETS FAILED");
            phidgetsReady = false;
            Invoke("CreatePhidgets", 2.0f);
        }
    }
}
