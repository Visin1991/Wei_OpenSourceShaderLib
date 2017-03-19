using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/// <summary>
///     With the Attributes to Bind to a member function. We can use partial function very eazly...
///  Add new test or do whatever will not effec the old Script. We can do any test on another partial without change the old....
/// </summary>
namespace Visin1_1
{
    [RequireComponent(typeof(Player))]
    public partial class MotionController : MonoBehaviour
    {
        private float turnSmoothVelocity;

        protected Player player;
        protected Transform cameraT;

        protected Vector2 axisInput;
        private float defaultYRotation = 0.0f;

        protected Vector3 velocityNor = Vector3.zero;
        protected Vector3 velocity3D = Vector3.zero;
        protected float postTargetMoveSpeed = 0.0f;
        protected Vector3 force = Vector3.zero;
        protected float currentSpeed = 0.0f;
        private float speedSmoothVelocity;

        public Vector3 VelocityNor
        {
            get
            {
                return velocityNor;
            }
        }
        public Vector3 Velocity3D
        {
            get
            {
                return velocity3D;
            }
        }
        public float PostProcessedMoveSpeed
        {
            get
            {
                return postTargetMoveSpeed;
            }
        }
        public float DefaultYRotation
        {
            get
            {
                return defaultYRotation;
            }

            set
            {
                defaultYRotation = value;
            }
        }

        private delegate void StartDel();
        private StartDel startDel;
        private delegate void UpdateDel();
        private UpdateDel updateDel;

        virtual protected void Start()
        {
            BindStartDel();
            if (startDel != null) { startDel(); }
            player = GetComponent<Player>();
            cameraT = player.CameraT;
            BindUpdateDel();
        }

        virtual protected void Update()
        {
            axisInput = player.PlayerInfos.AxisInput;

            if (updateDel != null)
            {
                updateDel();
            }
        }

        #region Facing Direction
 
        void FacingInputDir()
        {
            if (axisInput != Vector2.zero)
            {
                player.CharacterTF.eulerAngles = Vector3.up * Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg;
            }
            else
            {
                player.CharacterTF.eulerAngles = Vector3.up * defaultYRotation;
            }

        }

        void FacingInputDir_Stay()
        {
            if (axisInput != Vector2.zero)
            {
                player.CharacterTF.eulerAngles = Vector3.up * Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg;
            }
        }

        void FacingInputDir_Stay_AlignCameraDir()
        {
            if (axisInput != Vector2.zero)
            {
                player.CharacterTF.eulerAngles = Vector3.up * (Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y);
            }
        }

        [BindToUpdate("W key will always make the player facing the forward direction of the Camera...")]
        void FacingInputDir_Stay_AlignCamaraDir_Smooth()
        {
            if (axisInput != Vector2.zero)
            {
                float targetRotation = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
                player.CharacterTF.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(player.CharacterTF.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(player.PlayerInfos.TurnSmoothTime));
                //Debug.LogFormat(" target : {0}, current : {1}",targetRotation, player.HipTransform.eulerAngles.y);
                
            }
        }

        #endregion

        #region Velocity Direction

        [BindToUpdate("Player's Velocity will always match the Player's Facing Direction")]
        void UpdateVelocityDir_WithFacingDir()
        {
            velocityNor = player.CharacterTF.forward;
        }
        
        void UpdateVelocityDir_WithInputDir()
        {
            velocityNor = new Vector3(axisInput.x, 0, axisInput.y);
        }

        #endregion

        #region Post Process Velocity and Direction

        void PostProcessed_MoveSpeed()
        {
            postTargetMoveSpeed = player.PlayerInfos.IsRuning ? axisInput.magnitude * player.PlayerInfos.RunSpeed : axisInput.magnitude * player.PlayerInfos.MoveSpeed;
        }

        [BindToUpdate("Post Process the Move speed and Velocity ......")]
        void PostProcessed_MoveSpeed_Velocity()
        {
            postTargetMoveSpeed = (player.PlayerInfos.IsRuning ? player.PlayerInfos.RunSpeed : player.PlayerInfos.MoveSpeed) * axisInput.magnitude ;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, postTargetMoveSpeed, ref speedSmoothVelocity, player.PlayerInfos.SpeedSmoothTime);
            velocity3D = currentSpeed * velocityNor;
        }

        #endregion

        float GetModifiedSmoothTime(float smoothTime)
        {
            return smoothTime;
        }

        #region Function Delegate

        void BindStartDel()
        {
            MethodInfo[] ms = typeof(MotionController).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
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
            MethodInfo[] ms = typeof(MotionController).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
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
    }
}
