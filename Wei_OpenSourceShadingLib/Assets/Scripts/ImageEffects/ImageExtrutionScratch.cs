using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageExtrutionScratch : MonoBehaviour {

    public Shader shader;
    Material mat;
    RenderTexture renderTex;
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
        SetRenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight);
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

    public void ConvertUVY(int statu)
    {
        convertUVY = statu;
        if (mat != null)
        {
            mat.SetInt("convertUVY", convertUVY);
        }
    }

    void SetRenderTexture(int w, int h)
    {
        renderTex = new RenderTexture(w, h, 0, RenderTextureFormat.R8);
        //把这个texture buffer 储存到 GPU 内存中
        renderTex.Create();
    }
}
