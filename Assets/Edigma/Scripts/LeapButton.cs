using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeapButton : MonoBehaviour
{
    // Start is called before the first frame update
    public Color startColor;
    public Color selectColor;
    Image image;

    bool selected = false;
    float progress = 0.0f;

    void Start()
    {
        image = GetComponent<Image>();
        image.color = startColor;
    }

    // Update is called once per frame
    void Update()
    {
        image.color = Color.Lerp(startColor,selectColor,progress);
    }

    public void Off() {
        selected = false;
        progress = 0.0f;
    }

    public void On() {
        selected = true;
    }

    public void Progress(float p) {
        progress = p;
    }
}
