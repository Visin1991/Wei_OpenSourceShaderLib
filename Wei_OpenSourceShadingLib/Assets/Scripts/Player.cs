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
        public PlayerInputs PlayerInputs
        {
            get
            {
                return playerInputs;
            }
        }
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
        public float moveSpeed = 5;
        public float runSpeed = 10;
        public bool isRuning = false;
        public bool Puhching = false;

        /// <summary>
        ///     We will check all key status every frame.
        ///We split the key input setting. and The Player Input status....
        ///So We can Reuse the PlayerInput........
        /// </summary>
        public void ProcessInputInfo(PlayerInputs inputsInfo)
        {
            isRuning = inputsInfo.RHold;
            Puhching = inputsInfo.XDown;
        }
    }
}