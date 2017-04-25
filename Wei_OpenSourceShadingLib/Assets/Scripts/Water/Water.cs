using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wei,Zhu
///     Using Unity 5.5
/// </summary>

public class Water : MonoBehaviour {

    public enum FlageWaterRefType
    {
        Both = 0,
        Reflection = 1,
        Refraction = 2
    }

    public bool DisablePixelLights = false;
    public LayerMask Layers = -1;
    public int TexSize = 512;
    public FlageWaterRefType refType = FlageWaterRefType.Both;
    public float reflectClipPlaneOffset = 0;
    public float RefractionAngle = 0;

    private static Camera _reflectionCamera; // CreateRenderTexAndCam
    private static Camera _refractionCamera; // CreateRenderTexAndCam

    private int _OldTexSize = 0;
    private RenderTexture _reflectionRenderTex; // CreateRenderTexAndCam
    private RenderTexture _refractionRenderTex; // CreateRenderTexAndCam

    private bool _insideRendering = false;
    private float _refType = (float)FlageWaterRefType.Both;

    /// <summary>
    ///     OnWillRenderObject is called for each camera if the object is visible
    ///     The function is not called if the MonoBehaviour is disabled
    /// </summary>
    private void OnWillRenderObject()
    {   
        Renderer renderer = GetComponent<Renderer>();
        if (!enabled || !renderer || !renderer.sharedMaterial || !renderer.enabled) return;

        Camera cam = Camera.main;
        if (!cam) return;

        Material[] materials = renderer.sharedMaterials;

        if (_insideRendering) return;
        _insideRendering = true;

        int oldPixelLightCount = QualitySettings.pixelLightCount;

        if (DisablePixelLights)
            QualitySettings.pixelLightCount = 0;

        if (refType == FlageWaterRefType.Both || refType == FlageWaterRefType.Reflection)
        {
            DrawReflectionRenderTexture(cam);
            foreach (Material mat in materials)
            {

                if (mat.HasProperty("_ReflectionTex"))
                {
                    mat.SetTexture("_ReflectionTex", _reflectionRenderTex);
                   // Debug.Log("Pass");
                }
            }
        }

        if (refType == FlageWaterRefType.Both || refType == FlageWaterRefType.Refraction)
        {
            this.gameObject.layer = 4;
            DrawRefractionRenderTexture(cam);
            foreach (Material mat in materials)
            {
                if (mat.HasProperty("_RefractionTex"))
                    mat.SetTexture("_RefractionTex", _refractionRenderTex);
            }
        }
        _refType = (float)refType;
        Matrix4x4 projmtx = WaterHelper.UV_Tex2DProj2Tex2D(transform, cam);
        foreach (Material mat in materials)
        {
            mat.SetMatrix("_ProjMatrix", projmtx);
            mat.SetFloat("_RefType", _refType);
        }
        if (DisablePixelLights)
            QualitySettings.pixelLightCount = oldPixelLightCount;

        _insideRendering = false;

    }

    void OnDisable()
    {
        if (_reflectionRenderTex)
        {
            DestroyImmediate(_reflectionRenderTex);
            _reflectionRenderTex = null;
        }
        if (_reflectionCamera)
        {
            DestroyImmediate(_reflectionCamera.gameObject);
            _reflectionCamera = null;
        }

        if (_refractionRenderTex)
        {
            DestroyImmediate(_refractionRenderTex);
            _refractionRenderTex = null;
        }
        if (_refractionCamera)
        {
            DestroyImmediate(_refractionCamera.gameObject);
            _refractionCamera = null;
        }
    }

    /// <summary>
    /// Refraction RenderTexture
    /// </summary>
    private void DrawRefractionRenderTexture(Camera cam)
    {
        CreateRenderTexAndCam(cam, ref _refractionRenderTex, ref _refractionCamera);
        WaterHelper.CloneCameraModes(cam, _refractionCamera);

        Vector3 pos = transform.position;
        Vector3 normal = transform.up;

        Matrix4x4 projection = cam.worldToCameraMatrix;
        projection *= Matrix4x4.Scale(new Vector3(1, Mathf.Clamp(1 - RefractionAngle, 0.001f, 1), 1));
        _refractionCamera.worldToCameraMatrix = projection;

        Vector4 clipPlane = WaterHelper.CameraSpacePlane(_refractionCamera, pos, normal, -1.0f, 0);
        projection = cam.projectionMatrix;

        /*projection[2] = clipPlane.x + projection[3];//x
        projection[6] = clipPlane.y + projection[7];//y
        projection[10] = clipPlane.z + projection[11];//z
        projection[14] = clipPlane.w + projection[15];//w*
        _refractionCamera.projectionMatrix = projection;*/

        _refractionCamera.projectionMatrix = WaterHelper.CalculateObliqueMatrix(projection, clipPlane);


        _refractionCamera.cullingMask = ~(1 << 4) & Layers.value; // never render water layer
        _refractionCamera.targetTexture = _refractionRenderTex;

        _refractionCamera.transform.position = cam.transform.position;
        _refractionCamera.transform.eulerAngles = cam.transform.eulerAngles;
        _refractionCamera.Render();
   
    }

    private void DrawReflectionRenderTexture(Camera cam)
    {
        Vector3 pos = transform.position;
        Vector3 normal = transform.up;

        CreateRenderTexAndCam(cam, ref _reflectionRenderTex, ref _reflectionCamera);

        WaterHelper.CloneCameraModes(cam, _reflectionCamera);

        float d = -Vector3.Dot(normal, pos) - reflectClipPlaneOffset;
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);


        Matrix4x4 reflection = WaterHelper.CalculateReflectionMatrix(Matrix4x4.zero, reflectionPlane);

        Vector3 oldpos = cam.transform.position;
        Vector3 newpos = reflection.MultiplyPoint(oldpos);
        _reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

        // Setup oblique projection matrix so that near plane is our reflection
        // plane. This way we clip everything below/above it for free.
        Vector4 clipPlane = WaterHelper.CameraSpacePlane(_reflectionCamera, pos, normal, 1.0f, reflectClipPlaneOffset);

        Matrix4x4 projection = cam.projectionMatrix;
        projection = WaterHelper.CalculateObliqueMatrix(projection, clipPlane);
        //Matrix4x4 projection = cam.CalculateObliqueMatrix(clipPlane);

        _reflectionCamera.projectionMatrix = projection;

        _reflectionCamera.cullingMask = ~(1 << 4) & Layers.value; // never render water layer
        _reflectionCamera.targetTexture = _reflectionRenderTex;

       GL.invertCulling = true;
        _reflectionCamera.transform.position = newpos;
        Vector3 euler = cam.transform.eulerAngles;
        _reflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
        _reflectionCamera.Render();
        _reflectionCamera.transform.position = oldpos;
        GL.invertCulling = false;
    }

    void CreateRenderTexAndCam(Camera srcCam, ref RenderTexture renderTex, ref Camera destCam)
    {
        //Reflection render Texture
        if (!renderTex || _OldTexSize != TexSize)
        {
            if (renderTex)
                DestroyImmediate(renderTex);
            renderTex = new RenderTexture(TexSize, TexSize, 0);
            renderTex.name = "__RefRenderTexture" + renderTex.GetInstanceID();
            renderTex.isPowerOfTwo = true;
            renderTex.hideFlags = HideFlags.DontSave;
            renderTex.antiAliasing = 4;
            renderTex.anisoLevel = 0;
            renderTex.depth = 16;
            _OldTexSize = TexSize;
        }

        if (!destCam)
        {
            GameObject go = new GameObject("__RefCamra_for_" + srcCam.GetInstanceID(), typeof(Camera), typeof(Skybox));
            destCam = go.GetComponent<Camera>();
            destCam.enabled = false;
            destCam.transform.position = transform.position;
            destCam.transform.rotation = transform.rotation;
            //FlareLayer component is a lensflare component to be used on cameras. It has no properties.
            destCam.gameObject.AddComponent<FlareLayer>();
            go.hideFlags = HideFlags.HideAndDontSave;
        }
    }
}

public static class WaterHelper
{
    public static void CloneCameraModes(Camera src, Camera dest)
    {
        if (dest == null)
            return;
        
        // set camera to clear the same way as current camera
        dest.clearFlags = src.clearFlags;
        dest.backgroundColor = src.backgroundColor;

        if (src.clearFlags == CameraClearFlags.Skybox)
        {
            Skybox sky = src.GetComponent(typeof(Skybox)) as Skybox;
            Skybox mysky = dest.GetComponent(typeof(Skybox)) as Skybox;
            if (!sky || !sky.material)
            {
                mysky.enabled = false;
            }
            else
            {
                mysky.enabled = true;
                mysky.material = sky.material;
            }
        }
        //dest.clearFlags = CameraClearFlags.Color;
        // update other values to match current camera.
        // even if we are supplying custom camera&projection matrices,
        // some of values are used elsewhere (e.g. skybox uses far plane)
        dest.depth = src.depth;
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
    }

    public static Matrix4x4 CalculateReflectionMatrix(Matrix4x4 sourceMatrix, Vector4 plane)
    {
        sourceMatrix.m00 = (1F - 2F * plane[0] * plane[0]);
        sourceMatrix.m01 = (-2F * plane[0] * plane[1]);
        sourceMatrix.m02 = (-2F * plane[0] * plane[2]);
        sourceMatrix.m03 = (-2F * plane[3] * plane[0]);

        sourceMatrix.m10 = (-2F * plane[1] * plane[0]);
        sourceMatrix.m11 = (1F - 2F * plane[1] * plane[1]);
        sourceMatrix.m12 = (-2F * plane[1] * plane[2]);
        sourceMatrix.m13 = (-2F * plane[3] * plane[1]);

        sourceMatrix.m20 = (-2F * plane[2] * plane[0]);
        sourceMatrix.m21 = (-2F * plane[2] * plane[1]);
        sourceMatrix.m22 = (1F - 2F * plane[2] * plane[2]);
        sourceMatrix.m23 = (-2F * plane[3] * plane[2]);

        sourceMatrix.m30 = 0F;
        sourceMatrix.m31 = 0F;
        sourceMatrix.m32 = 0F;
        sourceMatrix.m33 = 1F;
        return sourceMatrix;
    }

    /// <summary>
    /// Get the view Space Equation of a plane
    /// </summary>
    /// <param name="cam">Camera position</param>
    /// <param name="pos">a point on the Plane</param>
    /// <param name="normal">Normal of the Plane</param>
    /// <param name="sideSign">1: Normal Direction, -1 invert Normal Direction</param>
    /// <param name="dstToPlane">the distacen form the plane to a point(the direction from the point fo the plaen 
    ///                          are parellel to the normal of the plane)</param>
    /// <returns></returns>
    public static Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign, float dstToPlane)
    {
        Vector3 offsetPos = pos + normal * dstToPlane;
        Matrix4x4 viewMatrix = cam.worldToCameraMatrix;
        Vector3 viewPos = viewMatrix.MultiplyPoint(offsetPos);
        Vector3 viewNormal = viewMatrix.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(viewNormal.x, viewNormal.y, viewNormal.z, -Vector3.Dot(viewPos, viewNormal));
    }

    /// <summary>
    /// Get a new Projection Matrix based on the clipPlane
    /// </summary>
    /// <param name="projection">the Original Camera</param>
    /// <param name="clipPlane">Clipping Plane</param>
    /// <param name="sideSign">-1: cut blow, 1: cut above. In water case, the value always -1 no matter reflection or refraction</param>
    /// <returns></returns>
    public static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane, float sideSign = -1f)
    {
        Vector4 q = projection.inverse * new Vector4(
             sgn(clipPlane.x),
             sgn(clipPlane.y),
             1.0f,
             1.0f
        );
        Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
        // third row = clip plane - fourth row
        projection[2] = c.x + Mathf.Sign(sideSign) * projection[3];
        projection[6] = c.y + Mathf.Sign(sideSign) * projection[7];
        projection[10] = c.z + Mathf.Sign(sideSign) * projection[11];
        projection[14] = c.w + Mathf.Sign(sideSign) * projection[15];
        return projection;
    }

    private static float sgn(float a)
    {
        if (a > 0.0f) return 1.0f;
        if (a < 0.0f) return -1.0f;
        return 0.0f;
    }

    /// <summary>
    /// tex2DProj to tex2D'uv transformation Matrix
    /// in shader,
    /// vert=>o.posProj = mul(_ProjMatrix, v.vertex);
    /// frag=>tex2D(_RefractionTex,float2(i.posProj) / i.posProj.w)
    /// </summary>
    /// <param name="transform">Object to Mapping</param>
    /// <param name="cam">Current orignal Cam</param>
    /// <returns>return Projection Matrix</returns>
    public static Matrix4x4 UV_Tex2DProj2Tex2D(Transform transform, Camera cam)
    {
        Matrix4x4 scaleOffset = Matrix4x4.TRS(
            new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
        Vector3 scale = transform.lossyScale;
        Matrix4x4 _ProjMatrix = transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(1.0f / scale.x, 1.0f / scale.y, 1.0f / scale.z));
        _ProjMatrix = scaleOffset * cam.projectionMatrix * cam.worldToCameraMatrix * _ProjMatrix;
        return _ProjMatrix;
    }
}
