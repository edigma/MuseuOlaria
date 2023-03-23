using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TranslateText : MonoBehaviour
{
    // Start is called before the first frame update
    public string enText;
    public string ptText;
    bool wasPT = true;
    Text text;
    void Start()
    {
        text = GetComponent<Text>();
        text.text = ptText;
    }


    // Update is called once per frame
    void Update()
    {
        if(UIController.Instance.LanguageEN && wasPT) {
            wasPT = false;
            text.text = enText;
        }

        if(!UIController.Instance.LanguageEN && !wasPT) {
            wasPT = true;
            text.text = ptText;
        }
    }
}
