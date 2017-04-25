using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace Visin1_1
{
    public class AMB : StateMachineBehaviour
    {
        static List<MethodInfo> allCallbackableMethodInfos = new List<MethodInfo>();
        static List<MethodInfo> allWPCallbackbleMethodInfos = new List<MethodInfo>();

        private AnimationController controller;
       
        private delegate void EnterDel(AnimationController controller);
        private EnterDel enterDels;
        [SerializeField]
        public List<int> enterCallbackIndices = new List<int>();

        private delegate void ExitDel(AnimationController controller);
        private ExitDel exitDels;
        [SerializeField]
        public List<int> exitCallbackIndices = new List<int>();

        [SerializeField]
        public List<BoolValues> boolValues = new List<BoolValues>();

        [SerializeField]
        public List<IntValueExit> intExit = new List<IntValueExit>();

        [SerializeField]
        public List<IntValueEnter> intValuesEnter = new List<IntValueEnter>();
        
        private delegate void EnterDelWP(AnimationController controller,params object[] objs);
        private EnterDelWP enterDelsWP;
        [SerializeField]
        public List<WPCallback> enterWPCallbackIndices = new List<WPCallback>();
        
        private void OnEnable()
        {
            if(allCallbackableMethodInfos.Count <= 0) //if not initialized
                allCallbackableMethodInfos = GetAllMethods();
         
            foreach (int i in enterCallbackIndices)
            {    
                enterDels += (EnterDel)Delegate.CreateDelegate(typeof(EnterDel), null, allCallbackableMethodInfos[i]);
            }

            foreach (int i in exitCallbackIndices)
            {
                exitDels += (ExitDel)Delegate.CreateDelegate(typeof(ExitDel), null, allCallbackableMethodInfos[i]);
            }
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (BoolValues b in boolValues)
            {
                animator.SetBool(b.boolName, b.enterStatu);
            }

           foreach (IntValueEnter i in intValuesEnter)
           {
               animator.SetInteger(i.intName, i.value);
           }

            if (controller != null)
            {
                if (enterDels != null) { enterDels(controller); }
                else { Debug.Log("enterDels = null"); }
            }
            else
            {
                controller = animator.transform.parent.GetComponent<AnimationController>();
                if (!controller)
                {
                    Debug.LogError("No AnimationController attach to this animator's parent GameObject");
                }
                else {
                    if (enterDels != null) { enterDels(controller); }
                    else { Debug.Log("enterDels = null"); }
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            foreach (BoolValues b in boolValues)
            {
                if (b.resetOnExit) { animator.SetBool(b.boolName, !b.enterStatu); }
            }
            foreach (IntValueExit i in intExit)
            {
                animator.SetInteger(i.intName, i.value);
            }

            if (controller != null)
            {
                if (exitDels != null) { exitDels(controller); }
                else { Debug.Log("enterDels = null"); }
            }
            else
            {
                controller = animator.transform.parent.GetComponent<AnimationController>();
                if (!controller)
                {
                    Debug.LogError("No AnimationController attach to this animator's parent GameObject");
                }
                else
                {
                    if (exitDels != null) { exitDels(controller); }
                    else { Debug.Log("enterDels = null"); }
                }
            }
        }

        static List<MethodInfo> GetAllMethods()
        {
            List<MethodInfo> allCallbackableInfos = new List<MethodInfo>();
            MethodInfo[] ms = typeof(AnimationController).GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (MethodInfo m in ms)
            {
                AMBCallback attr = System.Attribute.GetCustomAttribute(m, typeof(AMBCallback)) as AMBCallback;
                if (attr != null)
                {
                    allCallbackableInfos.Add(m);
                }
            }
            return allCallbackableInfos;
        }

        [System.Serializable]
        public class BoolValues
        {
            public string boolName;
            public bool enterStatu;
            public bool resetOnExit;

            public BoolValues(string name="Non",bool enter=true,bool reset=false)
            {
                boolName = name;
                enterStatu = enter;
                resetOnExit = reset;
            }
        }

        /// <summary>
        ///     For some Serialize problem we need to save 2 type of array/list of IntValue. Otherwise we will losing
        /// some data
        /// </summary>
        [System.Serializable]
        public class IntValueExit
        {
            public string intName;
            public int value;
            public IntValueExit(string name = "Non",int v=0)
            {
                intName = name;
                value = v;
            }
        }

        [System.Serializable]
        public class IntValueEnter
        {
            public string intName;
            public int value;
            public IntValueEnter(string name = "Non", int v = 0)
            {
                intName = name;
                value = v;
            }
        }

        /// <summary>
        ///     For some reason, when we add list of Vector3 or list of int in side the class
        /// and work with custorm editor. we will just get non-reference error everytime we reopen
        /// the inspector.
        /// 
        ///     when we create a delegate. we should keep the index for each type of parameter.
        /// make sure the parameters pass by correct order
        /// </summary>
        [System.Serializable]
        public class WPCallback
        {
            public int index;
            public int privewIndex;
            [SerializeField] public List<Vector3> v3s = new List<Vector3>();
            [SerializeField] public List<int> ints = new List<int>();
            [SerializeField] public List<float> floats = new List<float>();
            [SerializeField] public List<bool> bools = new List<bool>();
            public WPCallback(int _index = 0)
            {
                index = _index;
                privewIndex = -999;
            }
        }

    }
}
