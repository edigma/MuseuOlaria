using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance = null;
    // Start is called before the first frame update

    public bool LanguageEN = false;
    public Morpher handR;
    public Morpher handL;
    public float selectiontime = 3.0f;
    public float timeout = 10.0f;
    public float finish_time = 10.0f;

    public float z_int_offset = .0f;
    float lastInteraction = 0f;
    float doneTime = 0.0f;
    LeapButton lastR = null;
    LeapButton lastL = null;

    public Transform mainOffset;

    public MySmoothCanvas introScreen;
    public MySmoothCanvas helpScreen;
    public MySmoothCanvas inGame;
    public MySmoothCanvas finish;
    public MySmoothCanvas callToAction;
    public GameObject introText;

    public SpritePlayer video1;
    public SpritePlayer video2;

    public SpritePlayer video3;
    public SpritePlayer video4;
    public GameObject inGameVideos;

    public MorphContainer mContainer;

    bool locked = false;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        inGame.Show(false);
        finish.Show(false);
        introText.SetActive(true);
        introScreen.Show(false);
        callToAction.Show(true);
        helpScreen.Show(false);

        BDController.Instance.Loaded.AddListener(CouchChanged);
    }

    public void CouchChanged()
    {
        timeout = BDController.Instance.Timeout();
        finish_time = BDController.Instance.FinalTime();
    }


    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        int layerMask = 1 << 5;
        LeapButton lR = null;
        LeapButton lL = null;

        mainOffset.position = new Vector3(0.0f, 0.0f, BDController.Instance.ZOffset());

        if (!handL.gameObject.activeInHierarchy && !handR.gameObject.activeInHierarchy)
        {
            lastInteraction += Time.deltaTime;
            if (lastInteraction >= timeout && !locked)
            {
                Lock();
                locked = true;
                Debug.Log("LOCKED");
            }
            if (lastL)
            {
                lastL.Off();
            }
            if (lastR)
            {
                lastR.Off();
            }

            return;
        }
        else if (locked || callToAction.Showing)
        {
            locked = false;
            Home();
            Debug.Log("UNLOCKED");
        }

        lastInteraction = 0.0f;

        if (handL.gameObject.activeInHierarchy)
        {
            Vector3 dir = (handL.transform.position + new Vector3(0, z_int_offset, 0)) - Camera.main.transform.position;
            //Debug.DrawRay(Camera.main.transform.position,dir * 100,Color.red,0.1f);
            if (Physics.Raycast(Camera.main.transform.position, dir, out hit, Mathf.Infinity, layerMask))
            {
                lL = hit.transform.gameObject.GetComponent<LeapButton>();
            }
        }
        else
        {
            if (lastL)
            {
                lastL.Off();
            }
            lL = null;
            lastL = null;
        }

        if (handR.gameObject.activeInHierarchy)
        {
            Vector3 dir = (handR.transform.position + new Vector3(0, z_int_offset, 0)) - Camera.main.transform.position;
            if (Physics.Raycast(Camera.main.transform.position, dir, out hit, Mathf.Infinity, layerMask))
            {
                lR = hit.transform.gameObject.GetComponent<LeapButton>();
            }
        }
        else
        {
            if (lastR)
            {
                lastR.Off();
            }
            lR = null;
            lastR = null;
        }

        if (lL && lR)
        {
            if (lastL)
            {
                lastL.Off();
                lastL = null;
            }

            if (lastR)
            {
                lastR.Off();
                lastL = null;
            }
            doneTime = 0.0f;
        }
        else
        {
            if (lL)
            {
                if (lL == lastL)
                {
                    doneTime += Time.deltaTime;
                }
                else
                {
                    doneTime = 0.0f;
                }
                if (lastL)
                {
                    lastL.Off();
                }
                lL.On();

                lastL = lL;
            }
            else
            {
                if (lastL)
                {
                    doneTime = 0.0f;
                    lastL.Off();
                }
                lastL = null;
            }
            if (lR)
            {
                if (lR == lastR)
                {
                    doneTime += Time.deltaTime;
                }
                else
                {
                    doneTime = 0.0f;
                }
                if (lastR)
                {
                    lastR.Off();
                }
                lR.On();
                lastR = lR;
            }
            else
            {
                if (lastR)
                {
                    doneTime = 0.0f;
                    lastR.Off();
                }
                lastR = null;
            }

            if (!lastL && !lastR)
            {
                doneTime = 0.0f;
            }

            if (lastL)
            {
                lastL.Progress(doneTime / selectiontime);
            }
            if (lastR)
            {
                lastR.Progress(doneTime / selectiontime);
            }

            if (doneTime >= selectiontime)
            {
                doneTime = 0.0f;
                if (lastL)
                {
                    lastL.DoClick(); ;
                }
                if (lastR)
                {
                    lastR.DoClick(); ;
                }
            }
        }
    }


    IEnumerator MeshRestart()
    {
        mContainer.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        mContainer.gameObject.SetActive(true);
        yield return null;
    }

    IEnumerator RestartApp()
    {
        yield return new WaitForSeconds(finish_time);
        RestartApp();
    }

    public void Finish()
    {
        mContainer.Stop();
        introScreen.Show(false);
        inGame.Show(false);
        introText.SetActive(false);
        finish.Show(true);
        helpScreen.Show(false);

        StartCoroutine(RestartApp());
    }

    public void TempHelp()
    {
        StartCoroutine(TempHelpRoutine());
    }

    IEnumerator TempHelpRoutine()
    {
        inGameVideos.SetActive(true);
        video3.Play();
        video4.Play();
        yield return new WaitForSeconds(10.0f);
        video3.Stop();
        video4.Stop();
        inGameVideos.SetActive(false);
    }

    public void Startinteraction()
    {
        mContainer.gameObject.SetActive(true);
        mContainer.Reset();
        introScreen.Show(false);
        inGame.Show(true);
        introText.SetActive(true);
        finish.Show(false);
        helpScreen.Show(false);
        video1.Stop();
        video2.Stop();
        video3.Stop();
        video4.Stop();
        inGameVideos.SetActive(false);
    }

    public void RestartMesh()
    {
        mContainer.Reset();
    }

    public void Lock()
    {
        Debug.Log("LOCK SCREEN");
        mContainer.Reset();
        mContainer.gameObject.SetActive(false);
        introScreen.Show(false);
        callToAction.Show(true);
        introText.SetActive(true);
        finish.Show(false);
        inGame.Show(false);
        helpScreen.Show(false);
        video1.Stop();
        video2.Stop();
        video3.Stop();
        video4.Stop();
        inGameVideos.SetActive(false);
    }

    public void HelpScreen()
    {
        Debug.Log("Screen");
        mContainer.Reset();
        mContainer.gameObject.SetActive(false);
        introScreen.Show(false);
        callToAction.Show(false);
        introText.SetActive(false);
        finish.Show(false);
        inGame.Show(false);
        helpScreen.Show(true);
        video1.Play();
        video2.Play();
        video3.Stop();
        video4.Stop();
        inGameVideos.SetActive(false);
    }

    public void Home()
    {
        Debug.Log("HOME");
        mContainer.Reset();
        mContainer.gameObject.SetActive(false);
        introScreen.Show(true);
        callToAction.Show(false);
        introText.SetActive(true);
        finish.Show(false);
        inGame.Show(false);
        helpScreen.Show(false);
        video1.Stop();
        video2.Stop();
        video3.Stop();
        video4.Stop();
    }

    public void Language()
    {
        LanguageEN = !LanguageEN;
    }

    public void SizeUp()
    {
        mContainer.SizeUp();
    }

    public void SizeDown()
    {
        mContainer.SizeDown();
    }

    public void NoSize()
    {

    }

}


