using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace Visin1_1
{
    [CustomEditor(typeof(AMS))]
    [CanEditMultipleObjects]
    public class ASMInspector : Editor
    {

        AMS ams;

        private int functionIndex;

        private void OnEnable()
        {
            ams = target as AMS;
        }

        public override void OnInspectorGUI()
        {

            
            DrawCallBackFunctions();
            

            if (GUILayout.Button("WTF"))
            {
                Debug.Log("WTF");
            }
        }

        void DrawCallBackFunctions()
        {
            string[] functionNames = GetCallBackNames();

            GUILayout.BeginVertical("Box");
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("AC CallBack Enter");
                    if (GUILayout.Button("Add")) { if (ams.enterCallbackIndices.Count < functionNames.Length) { ams.enterCallbackIndices.Add(0); } }
                }
                GUILayout.EndHorizontal();
                DrawEnterFunctionsEnum(functionNames);
            }
            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical("Box");
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("AC CallBack Exit");
                    if (GUILayout.Button("Add")) { if (ams.exitCallbackIndices.Count < functionNames.Length) { ams.exitCallbackIndices.Add(0); } }
                }
                GUILayout.EndHorizontal();
                DrawEixtFunctionsEnum(functionNames);
            }
            GUILayout.EndVertical();



            serializedObject.ApplyModifiedProperties();
            SceneView.RepaintAll();
        }

        string[] GetCallBackNames()
        {
            List<string> callbackInfoNames = new List<string>();
            MethodInfo[] ms = typeof(AnimationController).GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (MethodInfo m in ms)
            {
                ASMCallback ams = System.Attribute.GetCustomAttribute(m, typeof(ASMCallback)) as ASMCallback;
                if (ams != null)
                {
                    callbackInfoNames.Add(m.Name);
                }
            }

            string[] names = callbackInfoNames.ToArray();
            return names;
        }

        void DrawEnterFunctionsEnum(string[] functionNames)
        {
            for (int i = 0; i < ams.enterCallbackIndices.Count; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    ams.enterCallbackIndices[i] = EditorGUILayout.Popup("", ams.enterCallbackIndices[i], functionNames, EditorStyles.popup);
                    if (GUILayout.Button("Delete")) { if (ams.enterCallbackIndices.Count > 0) { ams.enterCallbackIndices.RemoveAt(i); } }
                }
                GUILayout.EndHorizontal();
            }
        }

        void DrawEixtFunctionsEnum(string[] functionNames)
        {
            for (int i = 0; i < ams.exitCallbackIndices.Count; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    ams.exitCallbackIndices[i] = EditorGUILayout.Popup("", ams.exitCallbackIndices[i], functionNames, EditorStyles.popup);
                    if (GUILayout.Button("Delete")) { if (ams.exitCallbackIndices.Count > 0) { ams.exitCallbackIndices.RemoveAt(i); } }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
