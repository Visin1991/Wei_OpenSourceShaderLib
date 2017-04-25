using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Visin1_1
{
    public partial class AnimationController
    {
       
        [AMBCallback]
        public void AddForce()
        {
            Debug.Log("AddForce");
        }

        [AMBCallback]
        public void RotateYAxies()
        {
            Debug.Log("RotateYAxies");
        }

        [AMBCallback]
        public void PunchEffect()
        {
            Debug.Log("PunchEffect");
        }

        [AMBCallback]
        public void KickEffect()
        {
            Debug.Log("KickEffect");
        }

        [AMBCallback]
        public void Hid()
        {
            Debug.Log("Hid");
        }

        [AMBCallback]
        public void DoSomeThing()
        {
            Debug.Log("DoSomeThing");
        }

        [AMBCallback]
        public void DoWhatever()
        {
            Debug.Log("DoWhatever");
        }

        [AMBCallback]
        public void DoShit()
        {
            Debug.Log("DoShit");
        }

        [AMBWPCallback("Vector3 dir : the direction you wanna go")]
        public void GoDirection(Vector3 dir,bool isRun,int ii)
        {

        }

        [AMBWPCallback]
        public void HitMe(float strength)
        {

        }

        [AMBCallBackF]
        public void DoDamageHaha(float damage)
        {

        }
    
    }
}
