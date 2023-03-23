using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphContainer : MonoBehaviour
{
    // Start is called before the first frame update

    public MorphedCylinder cylinder;
    void Start()
    {
        
    }

    public void Reset() {
        cylinder.Reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SizeUp() {
        cylinder.sizeDown = false;
        cylinder.sizeUp = true;
    }

    public void SizeDown() {
        cylinder.sizeDown = true;
        cylinder.sizeUp = false;
    }
}
