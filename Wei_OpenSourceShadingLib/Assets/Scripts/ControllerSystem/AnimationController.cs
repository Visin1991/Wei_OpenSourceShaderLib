using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Visin1_1
{
    [RequireComponent(typeof(Player))]
    public partial class AnimationController : MonoBehaviour
    {
        private Animator animator;
        private Player player;
        private Transform cameraT;

        private delegate void StartDel();
        private StartDel startDel;
        private delegate void UpdateDel();
        private UpdateDel updateDel;

        public Animator Animator
        {
            get
            {
                return animator;
            }
        }

        public Transform CameraT
        {
            get
            {
                return cameraT;
            }
        }

        void Start()
        {
            player = GetComponent<Player>();
            cameraT = player.CameraT;
            animator = GetComponentInChildren<Animator>();

            BindStartDel();
            if (startDel != null) { startDel(); }
            BindUpdateDel();
        }

        void Update()
        {
            if (updateDel != null)
            {
                updateDel();
            }
        }

        #region Function Delegate

        void BindStartDel()
        {
            MethodInfo[] ms = typeof(AnimationController).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
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
            MethodInfo[] ms = typeof(AnimationController).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (MethodInfo m in ms)
            {
                BindToUpdate attr = System.Attribute.GetCustomAttribute(m, typeof(BindToUpdate)) as BindToUpdate;
                if (attr != null)
                {
                    System.Delegate test = System.Delegate.CreateDelegate(typeof(UpdateDel), this, m, false);
                    updateDel -= (UpdateDel)test;
                    updateDel += (UpdateDel)test;
                }
            }
        }

        void AddToUpdateDel(UpdateDel method)
        {
            updateDel -= method;
            updateDel += method;
        }

        void AddToStartDel(StartDel method)
        {
            startDel -= method;
            startDel += method;
        }

        void DeleteFromUpdateDel(UpdateDel method)
        {
            updateDel -= method;
        }

        void DeleteFromStartDel(StartDel method)
        {
            startDel -= method;
        }

        #endregion

        void Test1()
        {
            AddToStartDel(Test2);
        }

        void Test2()
        {
            Debug.Log("WTF Are you doing? ");
        }
    }
}
