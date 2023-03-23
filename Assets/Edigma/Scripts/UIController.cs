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
    float lastInteraction = 0f;
    float doneTime = 0.0f;
    LeapButton lastR = null;
    LeapButton lastL = null;

    public GameObject introScreen;
    public GameObject inGame;
    public MorphContainer mContainer;

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
    }


    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        int layerMask = 1 << 5;
        LeapButton lR = null;
        LeapButton lL = null;

        if (!handL.gameObject.activeInHierarchy && !handR.gameObject.activeInHierarchy)
        {
            lastInteraction += Time.deltaTime;
            if (lastInteraction >= timeout)
            {
                RestartMesh();
                Home();
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

        lastInteraction = 0.0f;

        if (handL.gameObject.activeInHierarchy)
        {
            Vector3 dir = handL.transform.position - Camera.main.transform.position;
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
            Vector3 dir = handR.transform.position - Camera.main.transform.position;
            if (Physics.Raycast(Camera.main.transform.position, dir, out hit, Mathf.Infinity, layerMask))
            {
                lR = hit.transform.gameObject.GetComponent<LeapButton>();
            }
        }
        else
        {
            if (lastR)
            {
                lastL.Off();
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

    public void Startinteraction()
    {
        mContainer.gameObject.SetActive(true);
        mContainer.Reset();
        introScreen.SetActive(false);
        inGame.SetActive(true);
    }

    public void RestartMesh()
    {
        mContainer.Reset();
    }

    public void Home()
    {
        mContainer.gameObject.SetActive(false);
        introScreen.SetActive(true);
        inGame.SetActive(false);
    }

    public void Language()
    {
        LanguageEN = !LanguageEN;
    }

}
