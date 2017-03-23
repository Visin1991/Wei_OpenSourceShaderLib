using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatebleData : ScriptableObject {

    public event System.Action OnvaluesUpdate;
    public bool autoUpdate;

    protected virtual void OnValidate()
    {
        if (autoUpdate) {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += NotifyUpdateValues;
#endif
        }
    }

    public void NotifyUpdateValues()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.update -= NotifyUpdateValues;
        if (OnvaluesUpdate != null)
        {
            OnvaluesUpdate();
        }
#endif
    }

}
