using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowEffects : MonoBehaviour {

    Material mat;
    // Use this for initialization
    void Start()
    {
        mat = GetComponent<MeshRenderer>().sharedMaterial;
    }


    // Update is called once per frame
    void Update () {

        float x = Mathf.Clamp(Mathf.Sin((2 * Mathf.PI / 10.0f) * Time.time),-0.5f,1.0f);
        mat.SetFloat("_SnowLevel", x);
    }
}
