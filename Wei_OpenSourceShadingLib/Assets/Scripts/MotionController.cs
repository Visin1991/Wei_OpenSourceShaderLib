using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/// <summary>
///     With the Attributes to Bind to a member function. We can use partial function very eazly...
///  Add new test or do whatever will not effec the old Script. We can do any test on another partial.
/// </summary>
namespace Visin1_1
{
    [RequireComponent(typeof(Player))]
    public partial class MotionController : MonoBehaviour
    {
        public float turnSmoothTime = 0.2f;     //the bigger the slower
        private float turnSmoothVelocity;

        protected Player player;
        protected Transform cameraT;

        protected Vector2 axisInput;
        private float defaultYRotation = 0.0f;

        protected Vector3 velocityNor = Vector3.zero;
        protected Vector3 velocity3D = Vector3.zero;
        protected float postProcessedMoveSpeed = 0.0f;
        protected Vector3 force = Vector3.zero;

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
                return postProcessedMoveSpeed;
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
            if (updateDel != null)
            {
                updateDel();
            }
        }

        #region Facing Direction
 
        void FacingInputDir()
        {
            axisInput = player.PlayerInputs.AxisInput;

            if (axisInput != Vector2.zero)
            {
                transform.eulerAngles = Vector3.up * Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg;
            }
            else
            {
                transform.eulerAngles = Vector3.up * defaultYRotation;
            }

        }

        void FacingInputDir_Stay()
        {
            axisInput = player.PlayerInputs.AxisInput;

            if (axisInput != Vector2.zero)
            {
                transform.eulerAngles = Vector3.up * Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg;
            }
        }

        void FacingInputDir_Stay_AlignCameraDir()
        {
            axisInput = player.PlayerInputs.AxisInput;

            if (axisInput != Vector2.zero)
            {
                transform.eulerAngles = Vector3.up * (Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y);
            }
        }

        [BindToUpdate("W key will always make the player facing the forward direction of the Camera...")]
        void FacingInputDir_Stay_AlignCamaraDir_Smooth()
        {
            axisInput = player.PlayerInputs.AxisInput;

            if (axisInput != Vector2.zero)
            {
                float targetRotation = Mathf.Atan2(axisInput.x, axisInput.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
            }
        }

        #endregion

        #region Velocity Direction

        [BindToUpdate("Player's Velocity will always match the Player's Facing Direction")]
        void UpdateVelocityDir_WithFacingDir()
        {
            velocityNor = transform.forward;
        }
        
        void UpdateVelocityDir_WithInputDir()
        {
            velocityNor = new Vector3(axisInput.x, 0, axisInput.y);
        }

        #endregion

        #region Post Process Velocity and Direction

        void PostProcessed_MoveSpeed()
        {
            postProcessedMoveSpeed = player.PlayerInfos.isRuning ? axisInput.magnitude * player.PlayerInfos.runSpeed : axisInput.magnitude * player.PlayerInfos.moveSpeed;
        }

        [BindToUpdate("Post Process the Move speed and Velocity ......")]
        void PostProcessed_MoveSpeed_Velocity()
        {
            postProcessedMoveSpeed = player.PlayerInfos.isRuning ? axisInput.magnitude * player.PlayerInfos.runSpeed : axisInput.magnitude * player.PlayerInfos.moveSpeed;
            velocity3D = postProcessedMoveSpeed * velocityNor;
        }

        #endregion

        float GetModifiedSmoothTime(float smoothTime)
        {
            return smoothTime;
        }

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
    }
}
