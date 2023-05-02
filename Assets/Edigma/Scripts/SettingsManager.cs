using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class SettingsManager : MonoBehaviour
{

    public static SettingsManager Instance = null;
    private bool m_settingsLoaded = false;
    Dictionary<string, string> jsonData = new Dictionary<string, string>();
    ASettings appSettings  = null;

    public string CouchDBUrl {
        get {
            if(m_settingsLoaded) {
                return appSettings.appSettings.bdUrl;
            } else {
                return "";
            }
        }
    }

    public bool SettginsLoaded {
        get {return m_settingsLoaded;}
    }

    [System.Serializable]
    public class AppSettings
    {
        public string bdUrl = "";
        public bool debug = false;
    }
    [System.Serializable]
    public class ASettings
    {
        public AppSettings appSettings = null;
    }

    public ASettings CurrentAppSettings
    {
        get { return appSettings; }
    }
    public bool SettingsReady
    {
        get { return m_settingsLoaded; }
    }

    public bool DoDebug {
        get {
            if(appSettings == null) {
                return true;
            }
            return appSettings.appSettings.debug;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

     
    }

    private void Start()
    {   
        LoadSettings();

        if (appSettings == null)
        {
            appSettings = new ASettings();
            appSettings.appSettings = new AppSettings();
            SaveAppSettings();
        }

        if (appSettings.appSettings == null)
        {
            appSettings.appSettings = new AppSettings();
            appSettings.appSettings.bdUrl = "";
            appSettings.appSettings.debug = false;
            SaveAppSettings();
        }
    }

    public void SaveAppSettings()
    {
        string fileName = "Settings.json";
        string path = Application.persistentDataPath + "/settings/" + fileName;

        File.WriteAllText(path, JsonUtility.ToJson(appSettings));

    }

    public void LoadSettings()
    {
        string fileName = "Settings.json";
        string path = Application.persistentDataPath + "/settings/" + fileName;
        Directory.CreateDirectory(Application.persistentDataPath + "/settings/");

        Debug.Log("Loading settings from: " + Application.persistentDataPath + "/settings/");
        
        DebugText.Instance.SetText("Loading settings from: " + Application.persistentDataPath + "/settings/");

        if (!File.Exists(path))
        {
            appSettings = new ASettings();
            appSettings.appSettings = new AppSettings();

            File.Create(path);
        }

        string json = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(path));

        if (json != null)
        {
            appSettings = JsonUtility.FromJson<ASettings>(json);
            DebugText.Instance.SetText("BD url: " + appSettings.appSettings.bdUrl);
        }

        m_settingsLoaded = true;
    }
}
