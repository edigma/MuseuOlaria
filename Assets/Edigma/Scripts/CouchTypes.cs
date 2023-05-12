using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CouchResponseDoc
{
    public string id;
    public bool deleted;
    public CouchDoc doc;
}


//list of all changes
[System.Serializable]
public class CouchResponse
{
    public CouchResponseDoc[] results;
    public string last_seq;
}


[System.Serializable]
public class CouchDoc
{
    public string _id;
    public string _rev;
    public string type;
    public float time;
    public string _attachments;
    public CouchBlocks[] blocks;
    public CouchDocValues values;

}


//Edigma's block [list of values]
[System.Serializable]
public class CouchBlocks
{
    public CouchDocValues values;
}

//common file
[System.Serializable]
public class CouchDocValues
{
    public string intro_title_pt = "";
    public string intro_title_en = "";
    public string welcome_1_pt = "";
    public string welcome_1_en = "";
    public string welcome_2_pt = "";
    public string welcome_2_en = "";
    public string intro_t_1_pt = "";
    public string intro_t_1_en = "";
    public string intro_d_1_pt = "";
    public string intro_d_1_en = "";
    public string intro_t_2_pt = "";
    public string intro_t_2_en = "";
    public string intro_d_2_pt = "";
    public string intro_d_2_en = "";
    public string intro_t_3_pt = "";
    public string intro_t_3_en = "";
    public string intro_d_3_pt = "";
    public string intro_d_3_en = "";
    public string intro_t_4_pt = "";
    public string intro_t_4_en = "";
    public string intro_d_4_pt = "";
    public string intro_d_4_en = "";
    public string intro_b_1_pt = "";
    public string intro_b_1_en = "";
    /*
    public string ingame_d_1_pt;
    public string ingame_d_1_en;
    public string ingame_d_2_pt;
    public string ingame_d_2_en;
    public string ingame_d_3_pt;
    public string ingame_d_3_en;
    public string ingame_d_4_pt;
    public string ingame_d_4_en;
    */
    public string ingame_b_1_pt = "";
    public string ingame_b_1_en = "";
    public string ingame_b_2_pt = "";
    public string ingame_b_2_en = "";
    public string ingame_b_3_pt = "";
    public string ingame_b_3_en = "";
    public string intro2_t_1_pt = "";
    public string intro2_t_1_en = "";
    public string intro2_d_1_pt = "";
    public string intro2_d_1_en = "";
    public string intro2_t_2_pt = "";
    public string intro2_t_2_en = "";
    public string intro2_d_2_pt = "";
    public string intro2_d_2_en = "";
    public string finish_d_pt  = "";
    public string finish_d_en  = "";
    public bool auto_rotate = true;
    public float rot_factor = 0.1f;
    public float final_time = 5.0f;
    public float timeout = 10.0f;
    public float z_offset = 0.0f;
    public int phidgetSerialNumber = 0;
}

