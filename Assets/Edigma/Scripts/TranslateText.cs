using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TranslateText : MonoBehaviour
{
    // Start is called before the first frame update
    public string text_id;
    public string enText;
    public string ptText;
    bool wasPT = true;
    Text text;
    void Start()
    {
        text = GetComponent<Text>();
        text.text = ptText;

        BDController.Instance.Loaded.AddListener(BDUpdate);
    }


    void BDUpdate()
    {
        if(text_id == "") {
            return;
        }
        KeyValuePair<string, string> tt = BDController.Instance.Translation(text_id);
        if (tt.Key != "")
        {
            ptText = tt.Key;
            if (wasPT)
            {
                text.text = ptText;
            }
        }
        if (tt.Value != "")
        {
            enText = tt.Value;
            if (!wasPT)
            {
                text.text = enText;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (UIController.Instance.LanguageEN && wasPT)
        {
            wasPT = false;
            text.text = enText;
        }

        if (!UIController.Instance.LanguageEN && !wasPT)
        {
            wasPT = true;
            text.text = ptText;
        }
    }
}
