using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WaterShaderEditor : ShaderGUI {

    MaterialEditor editor;
    MaterialProperty[] properties;
    static GUIContent staticLabel = new GUIContent();

    public override void OnGUI(MaterialEditor _editor, MaterialProperty[] _properties)
    {
        editor = _editor;
        properties = _properties;

        MaterialProperty tint = V_FindProperty("_Tint");
        editor.ColorProperty(tint, "Water Tint");
        GUILayout.Label("__________Distortion__________", EditorStyles.boldLabel);
        GUI_DUDV();
        GUILayout.Label("__________Waves__________", EditorStyles.boldLabel);
        GUI_WAVES();
        GUILayout.Label("__________Physics__________", EditorStyles.boldLabel);
        GUI_Metallic();
        GUI_Smoothness();
    }

    void GUI_DUDV()
    {
        MaterialProperty dudvTex = V_FindProperty("_DUDVMap");
        editor.TexturePropertySingleLine(MakeLabel(dudvTex, "DistortionMap"), dudvTex,V_FindProperty("_DistortionStrength"));
        editor.FloatProperty(V_FindProperty("_DistortionSpeedScaler"), "DS Speed Scaler");
    }

    void GUI_WAVES()
    {
        MaterialProperty normalMap = V_FindProperty("_NormalMap");
        editor.TexturePropertySingleLine(MakeLabel(normalMap), normalMap,
                                        normalMap.textureValue ? V_FindProperty("_BumpScale") : null);
        editor.TextureScaleOffsetProperty(normalMap);
        MaterialProperty speed1 = V_FindProperty("_Speed1");
        editor.VectorProperty(speed1,"X&Y-->Direction, Z speed");

        MaterialProperty detailNormal = V_FindProperty("_DetailNormalMap");
        editor.TexturePropertySingleLine(MakeLabel(detailNormal), detailNormal,
                                         detailNormal.textureValue ? V_FindProperty("_DetailBumpScale") : null);
        editor.TextureScaleOffsetProperty(detailNormal);
        MaterialProperty speed2 = V_FindProperty("_Speed2");
        editor.VectorProperty(speed2, "X&Y-->Direction, Z speed");
    }

    void GUI_Metallic()
    {
        MaterialProperty slider = V_FindProperty("_Metallic");
        editor.ShaderProperty(slider, MakeLabel(slider));
    }

    void GUI_Smoothness()
    {
        MaterialProperty slider = V_FindProperty("_Smoothness");
        editor.ShaderProperty(slider, MakeLabel(slider));
    }

    MaterialProperty V_FindProperty(string name)
    {
        return FindProperty(name, properties);
    }

    static GUIContent MakeLabel(string text, string tooltip = null)
    {
        staticLabel.text = text;
        staticLabel.tooltip = tooltip;
        return staticLabel;
    }

    static GUIContent MakeLabel(MaterialProperty property, string tooltip = null)
    {
        staticLabel.text = property.displayName;
        staticLabel.tooltip = tooltip;
        return staticLabel;
    }
}
