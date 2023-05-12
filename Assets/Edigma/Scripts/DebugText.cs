using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    public Text text;
    public static DebugText Instance = null;
    // Start is called before the first frame update

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


    IEnumerator WaitClear()
    {
        yield return new WaitForSeconds(10.0f);
        text.text = "";
    }

    public void SetText(string s)
    {
        if(!SettingsManager.Instance) {
            return;
        }

        if (!SettingsManager.Instance.DoDebug)
        {
            return;
        }
        
        text.text = s + "\n" + text.text;
        StopAllCoroutines();
        StartCoroutine(WaitClear());
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
