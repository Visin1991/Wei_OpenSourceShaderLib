using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基本原理：
///     1.先把整个Scene 渲染出来，并且把它存为一个Texture buffer (render target)
///      其实就是OpenGL 和 DirectX 中所说的 render target.
///      
///     2.渲染特定的某个Layer 并且存为另外的一个texture buffer
/// </summary>
public class PostOutLineEffect : MonoBehaviour {

    Camera mainCamera;
    Camera tempCam;
    public Shader post_Outline;
    public Shader DrawFlatShap;
    Material post_Mat;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        tempCam = new GameObject().AddComponent<Camera>();
        tempCam.enabled = false;
        post_Mat = new Material(post_Outline);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        tempCam.CopyFrom(mainCamera);
        tempCam.clearFlags = CameraClearFlags.Color;
        tempCam.backgroundColor = Color.black;

        //选定特定的 Layer 去Render
        tempCam.cullingMask = 1 << LayerMask.NameToLayer("Outline"); //把需要 增加Outline 的物体的 Layer 设置成 Outline 
        RenderTexture tempRT = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.R8);


        //把这个texture buffer 储存到 GPU 内存中
        tempRT.Create();

        tempCam.targetTexture = tempRT;

        post_Mat.SetTexture("_SceneTex", source);

        tempCam.RenderWithShader(DrawFlatShap, "");

        Graphics.Blit(tempRT, destination,post_Mat);

        tempRT.Release();


    }
}
