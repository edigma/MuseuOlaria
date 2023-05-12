using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySmoothCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;

    bool showing = false;

    public bool Showing {
        get {
            return showing;
        }
    }

    void Awake()
    {
       // anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Kill () {
        gameObject.SetActive(false);
    }

    public void Show(bool a) {
        showing = a;
        if(a) {
            gameObject.SetActive(true);
        } else {
            Invoke("Kill",.3f);
        }
        anim.SetBool("Show",a);
    }
}
