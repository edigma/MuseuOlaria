using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphContainer : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3 morph_init_pos;
    Vector3 morph_init_scale;
    Quaternion morph_init_rot;
    Vector3 morph_finish_pos;
    Vector3 morph_finish_scale;
    Quaternion morph_finish_rot;
    Vector3 morph_target_pos;
    Vector3 morph_target_scale;
    Quaternion morph_target_rot;

    public float moveSpeed;
    public Transform target;

    void ResetMorph()
    {
        morph_target_pos = morph_init_pos;
        morph_target_scale = morph_init_scale;
        morph_target_rot = morph_init_rot;
    }

    void FinishMorph()
    {
        morph_target_pos = morph_finish_pos;
        morph_target_scale = morph_finish_scale;
        morph_target_rot = morph_finish_rot;
    }

    public MorphedCylinder cylinder;
    void Start()
    {
        morph_init_pos = transform.localPosition;
        morph_init_scale = transform.localScale;
        morph_init_rot = transform.localRotation;
        morph_finish_pos = target.localPosition;
        morph_finish_scale = target.localScale;
        morph_finish_rot = target.localRotation;
        morph_target_pos = morph_init_pos;
        morph_target_scale = morph_init_scale;
        morph_target_rot = morph_init_rot;
        ResetMorph();
    }

    public void Reset()
    {
        cylinder.Reset();
        ResetMorph();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, morph_target_rot, Time.deltaTime * moveSpeed);
        transform.localPosition = Vector3.Lerp(transform.localPosition, morph_target_pos, Time.deltaTime * moveSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, morph_target_scale, Time.deltaTime * moveSpeed);
    }

    public void Stop()
    {
        cylinder.Stop();
        FinishMorph();
    }

    public void SizeUp()
    {
        cylinder.sizeDown = false;
        cylinder.sizeUp = true;
    }

    public void SizeDown()
    {
        cylinder.sizeDown = true;
        cylinder.sizeUp = false;
    }
}
