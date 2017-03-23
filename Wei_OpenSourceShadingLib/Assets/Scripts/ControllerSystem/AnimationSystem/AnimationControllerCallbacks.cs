using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Visin1_1
{
    public partial class AnimationController
    {
       
        [ASMCallback]
        public void AddForce()
        {
            Debug.Log("AddForce");
        }

        [ASMCallback]
        public void RotateYAxies()
        {
            Debug.Log("RotateYAxies");
        }

        [ASMCallback]
        public void PunchEffect()
        {
            Debug.Log("PunchEffect");
        }

        [ASMCallback]
        public void KickEffect()
        {
            Debug.Log("KickEffect");
        }

        [ASMCallback]
        public void Hid()
        {
            Debug.Log("Hid");
        }
    
    }
}
