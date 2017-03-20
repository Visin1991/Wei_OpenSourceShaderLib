using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTheNormal : MonoBehaviour {

    Material mat;
	// Use this for initialization
	void Start () {
        mat = GetComponent<MeshRenderer>().sharedMaterial;
	}
	
	// Update is called once per frame
	void Update () {
        //We wanna each circle 10 seconds
        //y = A sin(wX + y) +b : period T = 2PI/|w|
        //Here Time.time is x.  2PI / |w| = 10. So w = 2PI / 10.0f

        float x = Mathf.Sin((2 * Mathf.PI / 10.0f) * Time.time) * 5.0f;
        mat.SetFloat("_Intensity", x);
	}
}
