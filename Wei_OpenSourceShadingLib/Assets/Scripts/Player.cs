using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visin1_1
{
    public class Player : LivingEntity
    {
        [SerializeField] private PlayerInputs playerInputs;
        [SerializeField] private PlayerInfoProcesser playerInfos;
        [SerializeField] private Transform cameraT;
        [SerializeField] private Transform characterTF;

        public PlayerInfoProcesser PlayerInfos
        {
            get
            {
                return playerInfos;
            }
        }

        public Transform CameraT
        {
            get
            {
                return cameraT;
            }
        }

        public Transform CharacterTF
        {
            get
            {
                return characterTF;
            }
        }

        public override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {
            playerInputs.UpdateInputInfos();
            playerInfos.ProcessInputInfo(playerInputs);
        }

        private void FixedUpdate()
        {

        }

        protected sealed override void Die()
        {
            base.Die();
        }

        private void OnGUI()
        {
            // GUILayout.Label(new GUIContent("Player Health : " + health.ToString()));
        }
    }

    [System.Serializable]
    public class PlayerInfoProcesser
    {
        [SerializeField] private float speedSmoothTime = 0.1f;
        [SerializeField] private float moveSpeed = 5;
        [SerializeField] private float runSpeed = 10;
        [SerializeField] private bool isRuning = false;
        [SerializeField] private bool Puhching = false;
        [SerializeField] private Vector2 axisInput = Vector2.zero;
        [SerializeField] private float turnSmoothTime = 0.2f;

        public float SpeedSmoothTime
        {
            get
            {
                return speedSmoothTime;
            }
        }

        public float MoveSpeed
        {
            get
            {
                return moveSpeed;
            }
        }

        public float RunSpeed
        {
            get
            {
                return runSpeed;
            }
        }

        public bool IsRuning
        {
            get
            {
                return isRuning;
            }
        }

        public bool Puhching1
        {
            get
            {
                return Puhching;
            }
        }

        public Vector2 AxisInput
        {
            get
            {
                return axisInput;
            }
        }

        public float TurnSmoothTime
        {
            get
            {
                return turnSmoothTime;
            }
        }

        /// <summary>
        ///     We will check all key status every frame.
        ///We split the key input setting. and The Player Input status....
        ///So We can Reuse the PlayerInput........
        /// </summary>
        public void ProcessInputInfo(PlayerInputs inputsInfo)
        {
            isRuning = inputsInfo.RHold;
            Puhching = inputsInfo.XDown;
            axisInput = inputsInfo.AxisInput;
        }
    }
}