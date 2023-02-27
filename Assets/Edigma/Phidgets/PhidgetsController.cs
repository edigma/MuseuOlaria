using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Phidget22;
using Phidget22.Events;

public class PhidgetsController : MonoBehaviour
{
    public static PhidgetsController Instance = null;

    private Encoder encoder;   

    float levelValue = 2.0f;   
    bool levelSelectionEnabled = false;    
    bool phidgetsReady = false;  
    int m_phidgetSerialNumber = 538745; //661125;  
    public CanvasGroup lockedCanvas;
    int m_position = 0;

    public float GetLevelValue
    {
        get { return levelValue; }
    }

    private static void Encoder_PositionChange(object sender, Phidget22.Events.EncoderPositionChangeEventArgs e) {

		// Access event source via the sender object
		Encoder ch = (Encoder)sender;

		// Access event data via the EventArgs
		int positionChange = e.PositionChange;
		double timeChange = e.TimeChange;
		bool indexTriggered = e.IndexTriggered;
        Instance.m_position = positionChange;
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

        encoder = new Encoder();       

        try
        {
            encoder.DeviceSerialNumber = m_phidgetSerialNumber;            
            encoder.Open(250);
            encoder.PositionChange += Encoder_PositionChange;
            phidgetsReady = true;

        }
        catch (PhidgetException ex)
        {
            Debug.Log("Failure: " + ex.Message);
            phidgetsReady = false;
        }
    }

    public float animTime = 0;
    public float startAnimL = 0;

    private void Update()
    {
        Debug.Log("ENCODER STATUS: " + m_position);
    }

}
