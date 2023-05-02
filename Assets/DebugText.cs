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
        yield return new WaitForSeconds(3.0f);
        text.text = "";
    }

    public void SetText(string s)
    {

        text.text += s + "\n";

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
