using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;


namespace Visin1_1
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidBodyController : MotionController
    {
        private Rigidbody rg;

        private delegate void StartDel();
        private StartDel startDel;

        private delegate void UpdateDel();
        private UpdateDel updateDel;

        sealed protected override void Start()
        {
            //Base
            base.Start();
            //Binding
            BindStartDel();
            if (startDel != null) { startDel(); }
            BindUpdateDel();
            //Extra
            rg = GetComponent<Rigidbody>();
        }

        sealed protected override void Update()
        {
            base.Update();
            if (updateDel != null)
            {
                updateDel();
            }
        }

        private void FixedUpdate()
        {
            rg.MovePosition(rg.position + velocity3D * Time.fixedDeltaTime);
        }

        void BindStartDel()
        {
            MethodInfo[] ms = typeof(RigidBodyController).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (MethodInfo m in ms)
            {
                BindToStart attr = System.Attribute.GetCustomAttribute(m, typeof(BindToStart)) as BindToStart;
                if (attr != null)
                {
                    System.Delegate test = System.Delegate.CreateDelegate(typeof(BindToStart), this, m, false);
                    startDel -= (StartDel)test;
                    startDel += (StartDel)test;
                }
            }
        }

        void BindUpdateDel()
        {
            MethodInfo[] ms = typeof(RigidBodyController).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (MethodInfo m in ms)
            {
                BindToUpdate attr = System.Attribute.GetCustomAttribute(m, typeof(BindToUpdate)) as BindToUpdate;
                if (attr != null)
                {
                    /* System.Type t = typeof(RigidBodyController);
                     MethodInfo mi = t.GetMethod(attr._methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                     if (mi == null) { Debug.LogError("WTF are you doing?"); }*/
                    System.Delegate test = System.Delegate.CreateDelegate(typeof(UpdateDel), this, m, false);
                    updateDel -= (UpdateDel)test;
                    updateDel += (UpdateDel)test;
                }
            }
        }
     
    }
}

