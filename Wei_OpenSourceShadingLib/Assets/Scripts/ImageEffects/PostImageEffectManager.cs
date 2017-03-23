using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostImageEffectManager : MonoBehaviour {

    ImageExtrutionScratch imageExtrution;
    PostOutLineEffect outlineEffect;

    // Use this for initialization
    void Start () {
        imageExtrution = GetComponent<ImageExtrutionScratch>();
        imageExtrution.enabled = false;
        outlineEffect = GetComponent<PostOutLineEffect>();
        outlineEffect.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            outlineEffect.enabled = !outlineEffect.enabled;
            if (outlineEffect.enabled)
            {
                imageExtrution.ConvertUVY(1);
            }
            else
            {
                imageExtrution.ConvertUVY(0);

            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            imageExtrution.enabled = !imageExtrution.enabled;

            bool statu = imageExtrution.enabled;
            Debug.LogFormat("The ImageExtrutionScratch effet is {0}", statu);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (imageExtrution.enabled) { imageExtrution.Hit(); }
            else Debug.Log("To use the hit splash effect you need to enable the imageExturionScrath effect first. Enable it press 1");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (imageExtrution.enabled) { imageExtrution.Heat(); }
            else Debug.Log("To use the Heat splash effect you need to enable the imageExturionScrath effect first. Enable it press 1");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (imageExtrution.enabled) { imageExtrution.Scratch(); }
            else Debug.Log("To use the Scratch splash effect you need to enable the imageExturionScrath effect first. Enable it press 1");
        }
    }
}
