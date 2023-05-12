using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpritePlayer : MonoBehaviour
{

    public Sprite[] imgs;

    int currentFrame = 0;

    public bool playing = false;
    public Image sRenderer;
    // Start is called before the first frame update
    public void Play() {
        currentFrame = 0;
        playing = true;
        StartCoroutine(PlayRoutine());
        
    }

    public void Stop() {
        playing = false;
        StopCoroutine(PlayRoutine());
        
    }

    void Start()
    {
     //   sRenderer = GetComponent<Image>();

        if(playing) {
            Play();
        }
    }

    // Update is called once per frame
    IEnumerator PlayRoutine() {
        while(playing) {
            currentFrame++;
            currentFrame %= imgs.Length;
            sRenderer.sprite = imgs[currentFrame];
            yield return new WaitForSeconds(0.04f);
        }
        yield return null;
    }
}
