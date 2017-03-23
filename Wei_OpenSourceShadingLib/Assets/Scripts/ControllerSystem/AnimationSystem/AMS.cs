using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace Visin1_1
{

    public class AMS : StateMachineBehaviour
    {
        static List<MethodInfo> allCallbackableMethodInfos = new List<MethodInfo>();

        private AnimationController controller;
       
        private delegate void EnterDel(AnimationController controller);
        private EnterDel enterDels;
        public List<int> enterCallbackIndices = new List<int>();

        private delegate void ExitDel(AnimationController controller);
        private ExitDel exitDels;
        public List<int> exitCallbackIndices = new List<int>();
           
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
                ASMCallback attr = System.Attribute.GetCustomAttribute(m, typeof(ASMCallback)) as ASMCallback;
                if (attr != null)
                {
                    allCallbackableInfos.Add(m);
                }
            }
            return allCallbackableInfos;
        }
    }
}
