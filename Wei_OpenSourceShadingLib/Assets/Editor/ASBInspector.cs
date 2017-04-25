using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace Visin1_1
{
    [CustomEditor(typeof(AMB))]
    [CanEditMultipleObjects]
    public class ASBInspector : Editor
    {

        AMB amb;

        bool showVoidCallBack = false;
        bool showBoolValues = false;
        bool showIntValues = false;
        bool showWPCallbacks = false;

        void OnEnable()
        {
            amb = target as AMB;
        }

        void InitTarget()
        {
            
        }

        public override void OnInspectorGUI()
        {
            //base.DrawDefaultInspector();
            DrawCallBackFunctions();
            DrawBoolValues();
            DrawIntValues();
            DrawIntValuesExit();
            //DrawWPCallBacks();
        }


        #region Main Draw

        void DrawCallBackFunctions()
        {
            GUILayout.BeginVertical("Box");
            {
                showVoidCallBack = EditorGUILayout.Foldout(showVoidCallBack, "Void Callback");
                if (showVoidCallBack)
                {

                    string[] functionNames = GetCallBackNames();

                    GUILayout.BeginVertical("Box");
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("AC CallBack Enter");
                            GUI.color = Color.green;
                            if (GUILayout.Button("Add")) { if (amb.enterCallbackIndices.Count < functionNames.Length) { amb.enterCallbackIndices.Add(0); } }
                            GUI.color = Color.white;
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
                            GUI.color = Color.green;
                            if (GUILayout.Button("Add")) { if (amb.exitCallbackIndices.Count < functionNames.Length) { amb.exitCallbackIndices.Add(0); } }
                            GUI.color = Color.white;
                        }
                        GUILayout.EndHorizontal();
                        DrawEixtFunctionsEnum(functionNames);
                    }
                    GUILayout.EndVertical();

                    serializedObject.ApplyModifiedProperties();
                    SceneView.RepaintAll();
                }
            }
            GUILayout.EndVertical();
        }

        void DrawBoolValues()
        {
            GUILayout.BeginVertical("Box");
            {
                showBoolValues = EditorGUILayout.Foldout(showBoolValues, "BoolValues");
                if (showBoolValues)
                {
                    GUI.color = Color.green;
                    if (GUILayout.Button("Add"))
                    {
                        AMB.BoolValues boolValue = new AMB.BoolValues();
                        amb.boolValues.Add(boolValue);
                    }
                    GUI.color = Color.white;

                    GUILayout.BeginVertical("Box");
                    {
                        for (int i = 0; i < amb.boolValues.Count; i++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("BoolName : ");
                            amb.boolValues[i].boolName = GUILayout.TextField(amb.boolValues[i].boolName, 25);
                            GUILayout.EndHorizontal();
                            amb.boolValues[i].enterStatu = GUILayout.Toggle(amb.boolValues[i].enterStatu, "EnterStatu");
                            amb.boolValues[i].resetOnExit = GUILayout.Toggle(amb.boolValues[i].resetOnExit, "ResetOnExit");

                            GUI.color = Color.red;
                            if (GUILayout.Button("Delete"))
                            {
                                amb.boolValues.RemoveAt(i);
                            }
                            GUI.color = Color.white;
                        }
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();
        }

        void DrawIntValues()
        {
            GUILayout.BeginVertical("Box");
             {
                 showIntValues = EditorGUILayout.Foldout(showIntValues, "IntValues");
                 if (showIntValues)
                 {
                     //-------------------------------------------------------.
                     GUILayout.Space(10);
                     //-------------------------------------------------------

                     //Enter
                     GUILayout.BeginVertical("Box");
                     {
                         GUILayout.BeginHorizontal();
                         {
                             GUILayout.Label("Enter");

                             GUI.color = Color.green;
                             if (GUILayout.Button("Add"))
                             {
                                 AMB.IntValueEnter intValue = new AMB.IntValueEnter();
                                 amb.intValuesEnter.Add(intValue);
                             }
                             GUI.color = Color.white;
                         }
                         GUILayout.EndHorizontal();

                         GUILayout.BeginVertical("Box");
                         for (int i = 0; i < amb.intValuesEnter.Count; i++)
                         {
                             GUILayout.BeginHorizontal();
                             GUILayout.Label("IntName : ");
                             amb.intValuesEnter[i].intName = GUILayout.TextField(amb.intValuesEnter[i].intName, 25);
                             GUILayout.EndHorizontal();
                             amb.intValuesEnter[i].value = EditorGUILayout.IntField("Value", amb.intValuesEnter[i].value);

                             GUI.color = Color.red;
                             if (GUILayout.Button("Delete"))
                             {
                                 amb.intValuesEnter.RemoveAt(i);
                             }
                             GUI.color = Color.white;
                         }
                         GUILayout.EndVertical();
                     }
                     GUILayout.EndVertical();

                 }
                 GUILayout.EndVertical();
             }//End showIntValues   
             
        }

        void DrawIntValuesExit()
        {
            //Exit
            GUILayout.BeginVertical("Box");
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Exit");

                    GUI.color = Color.green;
                    if (GUILayout.Button("Add"))
                    {
                        AMB.IntValueExit intValue = new AMB.IntValueExit();
                        amb.intExit.Add(intValue);
                    }
                    GUI.color = Color.white;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical("Box");

                for (int i = 0; i < amb.intExit.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("IntName : ");
                    amb.intExit[i].intName = GUILayout.TextField(amb.intExit[i].intName, 25);
                    GUILayout.EndHorizontal();
                    amb.intExit[i].value = EditorGUILayout.IntField("Value", amb.intExit[i].value);

                    GUI.color = Color.red;
                    if (GUILayout.Button("Delete"))
                    {
                        amb.intExit.RemoveAt(i);
                    }
                    GUI.color = Color.white;
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        void DrawWPCallBacks()
        {
            GUILayout.BeginVertical("Box");
            {
                showWPCallbacks = EditorGUILayout.Foldout(showWPCallbacks, "WPCallbacks");
                if (showWPCallbacks)
                {
                    List<string> names = new List<string>();
                    List<ParameterInfo[]> parameterInfos = new List<ParameterInfo[]>();
                    GetWPCallBackInfos(ref names, ref parameterInfos);
                    string[] functionNames = names.ToArray();

                    GUILayout.BeginVertical("Box");
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("AC WP Callbacks Enter");
                            GUI.color = Color.green;
                            if (GUILayout.Button("Add")) { if (amb.enterWPCallbackIndices.Count < functionNames.Length) { AMB.WPCallback wpCallback = new AMB.WPCallback(); amb.enterWPCallbackIndices.Add(wpCallback); } }
                            GUI.color = Color.white;
                        }
                        GUILayout.EndHorizontal();
                        DrawEnterWPCallbacksEnum(functionNames, parameterInfos);
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();
        }

        #endregion

        #region Sub Draw

        void DrawEnterFunctionsEnum(string[] functionNames)
        {
            for (int i = 0; i < amb.enterCallbackIndices.Count; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    amb.enterCallbackIndices[i] = EditorGUILayout.Popup("", amb.enterCallbackIndices[i], functionNames, EditorStyles.popup);
                    GUI.color = Color.red;
                    if (GUILayout.Button("Delete")) { if (amb.enterCallbackIndices.Count > 0) { amb.enterCallbackIndices.RemoveAt(i); } }
                    GUI.color = Color.white;
                }
                GUILayout.EndHorizontal();
            }
        }

        void DrawEixtFunctionsEnum(string[] functionNames)
        {
            for (int i = 0; i < amb.exitCallbackIndices.Count; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    amb.exitCallbackIndices[i] = EditorGUILayout.Popup("", amb.exitCallbackIndices[i], functionNames, EditorStyles.popup);
                    GUI.color = Color.red;
                    if (GUILayout.Button("Delete")) { if (amb.exitCallbackIndices.Count > 0) { amb.exitCallbackIndices.RemoveAt(i); } }
                    GUI.color = Color.white;
                }
                GUILayout.EndHorizontal();
            }
        }

        void DrawEnterWPCallbacksEnum(string[] functionNames, List<ParameterInfo[]> parametInfos)
        {
            
            for (int i = 0; i < amb.enterWPCallbackIndices.Count; i++)
            {
                

                GUILayout.BeginHorizontal();
                
                    ParameterInfo[] previewParametInfo = parametInfos[amb.enterWPCallbackIndices[i].index];
                    amb.enterWPCallbackIndices[i].index = EditorGUILayout.Popup("", amb.enterWPCallbackIndices[i].index, functionNames, EditorStyles.popup);
                    ParameterInfo[] currentparametInfo = parametInfos[amb.enterWPCallbackIndices[i].index];
                    if (amb.enterWPCallbackIndices[i].index != amb.enterWPCallbackIndices[i].privewIndex){
                       
                    }
                    GUI.color = Color.red;
                    if (GUILayout.Button("Delete")) {
                        if (amb.enterWPCallbackIndices.Count > 0) {
                           
                            amb.enterWPCallbackIndices.RemoveAt(i);
                            continue;
                        }
                    }
                    GUI.color = Color.white;
                
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical("Box");
                {   
                    for(int j=0; j< currentparametInfo.Length;j++)
                    {
                        
                    }
                }
                GUILayout.EndVertical();
           }
        }

        #endregion

        string[] GetCallBackNames()
        {
            List<string> callbackInfoNames = new List<string>();
            MethodInfo[] ms = typeof(AnimationController).GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (MethodInfo m in ms)
            {
                AMBCallback attr = System.Attribute.GetCustomAttribute(m, typeof(AMBCallback)) as AMBCallback;
                if (attr != null)
                {
                    callbackInfoNames.Add(m.Name);
                }
            }

            string[] names = callbackInfoNames.ToArray();
            return names;
        }

        void GetWPCallBackInfos(ref List<string> names,ref List<ParameterInfo[]> parameterInfos)
        {
            MethodInfo[] ms = typeof(AnimationController).GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (MethodInfo m in ms)
            {
                AMBWPCallback attr = System.Attribute.GetCustomAttribute(m, typeof(AMBWPCallback)) as AMBWPCallback;
                if (attr != null)
                {
                    names.Add(m.Name);
                    parameterInfos.Add(m.GetParameters());
                }
            }
        }
        //------------------------------
    }//ENd class
}//end namespace
