using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using UnityEngine.Networking;

public class BDController : MonoBehaviour
{

    public static BDController Instance = null;
    public UnityEvent Changed;
    private string lastSeq = "0";
    public float lastRequest;
    public string lastUrl;
    public UnityEvent Loaded;
    public string ErrorString = "";
    private CouchResponse response;
    private CouchResponseDoc infoDoc = null;
    public float refreshTime = 2.0f;
    private bool initialLoad = true;
    private bool keepLoadng = true;

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
        DontDestroyOnLoad(gameObject);

        if (Loaded == null)
        {
            Loaded = new UnityEvent();
        }
        if (Changed == null)
        {
            Changed = new UnityEvent();
        }
    }

    void Start()
    {
        StartCoroutine(GetChanges());
    }

    public bool AutoRotate()
    {
        if (infoDoc != null)
        {
            return infoDoc.doc.values.auto_rotate;
        }
        return false;
    }

    public float RotateFactor()
    {
        if (infoDoc != null)
        {
            return infoDoc.doc.values.rot_factor;
        }
        return 0.1f;
    }

    public float FinalTime()
    {
        if (infoDoc != null)
        {
            return infoDoc.doc.values.final_time;
        }
        return 5.0f;
    }

    public float Timeout()
    {
        if (infoDoc != null)
        {
            return infoDoc.doc.values.timeout;
        }
        return 10.0f;
    }
    
     public float ZOffset()
    {
        if (infoDoc != null)
        {
            return infoDoc.doc.values.z_offset;
        }
        return 0.0f;
    }

        public float YOffset()
    {
        if (infoDoc != null)
        {
            return infoDoc.doc.values.y_offset;
        }
        return 0.0f;
    }

         public int PhidgetSerialNumber()
    {
        if (infoDoc != null)
        {
            return infoDoc.doc.values.phidgetSerialNumber;
        }
        return 0;
    }


    public KeyValuePair<string, string> Translation(string id)
    {
        if(infoDoc == null) {
            return new KeyValuePair<string, string>("","");    
        }

        switch (id)
        {
            case "intro_title": return new KeyValuePair<string, string>(infoDoc.doc.values.intro_title_pt, infoDoc.doc.values.intro_title_en); 
            case "welcome_1": return new KeyValuePair<string, string>(infoDoc.doc.values.welcome_1_pt, infoDoc.doc.values.welcome_1_en); 
            case "welcome_2": return new KeyValuePair<string, string>(infoDoc.doc.values.welcome_2_pt, infoDoc.doc.values.welcome_2_en); 
            case "intro_t_1": return new KeyValuePair<string, string>(infoDoc.doc.values.intro_t_1_pt, infoDoc.doc.values.intro_t_1_en); 
            case "intro_d_1": return new KeyValuePair<string, string>(infoDoc.doc.values.intro_d_1_pt, infoDoc.doc.values.intro_d_1_en); 
            case "intro_t_2": return new KeyValuePair<string, string>(infoDoc.doc.values.intro_t_2_pt, infoDoc.doc.values.intro_t_2_en); 
            case "intro_d_2": return new KeyValuePair<string, string>(infoDoc.doc.values.intro_d_2_pt, infoDoc.doc.values.intro_d_2_en); 
            case "intro_t_3": return new KeyValuePair<string, string>(infoDoc.doc.values.intro_t_3_pt, infoDoc.doc.values.intro_t_3_en); 
            case "intro_d_3": return new KeyValuePair<string, string>(infoDoc.doc.values.intro_d_3_pt, infoDoc.doc.values.intro_d_3_en); 
            case "intro_t_4": return new KeyValuePair<string, string>(infoDoc.doc.values.intro_t_4_pt, infoDoc.doc.values.intro_t_4_en); 
            case "intro_d_4": return new KeyValuePair<string, string>(infoDoc.doc.values.intro_d_4_pt, infoDoc.doc.values.intro_d_4_en); 
            case "intro_b_1": return new KeyValuePair<string, string>(infoDoc.doc.values.intro_b_1_pt, infoDoc.doc.values.intro_b_1_en); 
            
            
            case "intro2_t_1": return new KeyValuePair<string, string>(infoDoc.doc.values.intro2_t_1_pt, infoDoc.doc.values.intro2_t_1_en); 
            case "intro2_d_1": return new KeyValuePair<string, string>(infoDoc.doc.values.intro2_d_1_pt, infoDoc.doc.values.intro2_d_1_en); 
            case "intro2_t_2": return new KeyValuePair<string, string>(infoDoc.doc.values.intro2_t_2_pt, infoDoc.doc.values.intro2_t_2_en); 
            case "intro2_d_2": return new KeyValuePair<string, string>(infoDoc.doc.values.intro2_d_2_pt, infoDoc.doc.values.intro2_d_2_en); 
            /*
            case "ingame_d_1": return new KeyValuePair<string, string>(infoDoc.doc.values.ingame_d_1_pt, infoDoc.doc.values.ingame_d_1_en); 
            case "ingame_d_2": return new KeyValuePair<string, string>(infoDoc.doc.values.ingame_d_2_pt, infoDoc.doc.values.ingame_d_2_en); 
            case "ingame_d_3": return new KeyValuePair<string, string>(infoDoc.doc.values.ingame_d_3_pt, infoDoc.doc.values.ingame_d_3_en); 
            case "ingame_d_4": return new KeyValuePair<string, string>(infoDoc.doc.values.ingame_d_4_pt, infoDoc.doc.values.ingame_d_4_en); 
            */
            case "ingame_b_1": return new KeyValuePair<string, string>(infoDoc.doc.values.ingame_b_1_pt, infoDoc.doc.values.ingame_b_1_en); 
            case "ingame_b_2": return new KeyValuePair<string, string>(infoDoc.doc.values.ingame_b_2_pt, infoDoc.doc.values.ingame_b_2_en); 
            case "ingame_b_3": return new KeyValuePair<string, string>(infoDoc.doc.values.ingame_b_3_pt, infoDoc.doc.values.ingame_b_3_en); 
            case "finish_d": return new KeyValuePair<string, string>(infoDoc.doc.values.finish_d_pt, infoDoc.doc.values.finish_d_en); 
            default: Debug.Log("Missing " + id);break;
        }

        return new KeyValuePair<string, string>("","");
    }

    // CouchDB Find Changes
    IEnumerator GetChanges()
    {
        string couchIp = SettingsManager.Instance.CouchDBUrl;


        if (couchIp != "")
        {
            //            Debug.Log("Loading info from " + couchIp);
            float time = Time.realtimeSinceStartup;
            string url = couchIp + "/_changes?include_docs=true&timeout=240000&since=" + lastSeq;

            lastUrl = url;
            lastRequest = Time.timeSinceLevelLoad;
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    ErrorString += "\n" + "Error" + www.error;
                }
                else
                {
                    byte[] results = www.downloadHandler.data;
                    string str = System.Text.Encoding.UTF8.GetString(results);
                    response = JsonUtility.FromJson<CouchResponse>(str);

                    if (lastSeq != response.last_seq)
                    {
                        lastSeq = response.last_seq;

                        List<CouchResponseDoc> _infos = new List<CouchResponseDoc>();
                        //CouchDocValues mySettings = null;

                        foreach (CouchResponseDoc cDoc in response.results)
                        {
                            if (cDoc.doc.type != "mesaEntity")
                            {
                                continue;
                            }

                            Debug.Log("Found a doc");
                            infoDoc = cDoc;
                            Loaded.Invoke();
                            break;
                        }
                    }
                }
            }
            yield return new WaitForSeconds(refreshTime);
            if (keepLoadng)
            {
                StartCoroutine(GetChanges());
            }
        }
        else
        {
            yield return new WaitForSeconds(refreshTime);
            StartCoroutine(GetChanges());
        }
    }
}
