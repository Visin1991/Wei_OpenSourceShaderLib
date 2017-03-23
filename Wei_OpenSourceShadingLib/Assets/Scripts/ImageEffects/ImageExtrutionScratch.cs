using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageExtrutionScratch : MonoBehaviour {

    public Shader shader;
    Material mat;

    private int convertUVY = 0;

    public Texture2D headTexture;
    public Texture2D ScratchEffectTex;

    int isHit = 0;
    int indexHit = 0;

    int useHeat = 0;
    int heatIndex = 0;

    float ScratchTensity = 0;

    public Material Mat
    {
        get
        {
            return mat;
        }
    }

    // Use this for initialization
    void Start () {
        mat = new Material(shader);
        mat.SetTexture("_HeatTex", headTexture);
        mat.SetTexture("_ScratchEffectTex", ScratchEffectTex);
        mat.SetInt("convertUVY", convertUVY);
	}
	
	// Update is called once per frame
	void Update () {

        ScratchTensity -= Time.deltaTime * 1 / 3;
        if (ScratchTensity < 0.0f)
        {
            ScratchTensity = 0.0f;
        }
    }

    public void Hit()
    {
        isHit = indexHit % 2;
        indexHit++;
        mat.SetInt("isHit", isHit);
    }

    public void Heat()
    {
        useHeat = heatIndex % 2;
        heatIndex++;
        mat.SetInt("useHeat", useHeat);
    }

    public void Scratch()
    {
        ScratchTensity = 1.0f;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        mat.SetFloat("scratchTensity", ScratchTensity);
        Graphics.Blit(source, destination, mat);
    }

    /// <summary>
    ///     When this script is disable. We set up the value of convertUVY
    /// So when this script is enable, Start function will get call then the value of convertUVY
    /// will be set.
    /// </summary>
    /// <param name="statu"></param>
    public void ConvertUVY(int statu)
    {
        convertUVY = statu;
        if (mat != null)
        {
            mat.SetInt("convertUVY", convertUVY);
        }
    }
}
