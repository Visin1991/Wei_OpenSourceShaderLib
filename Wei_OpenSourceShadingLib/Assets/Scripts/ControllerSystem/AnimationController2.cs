using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visin1_1
{

    public partial class AnimationController
    {
        private float aniMoveSpeedPercent;
        public float speedSmoothTime = 0.1f;

        [BindToUpdate]
        void UpdateAnimations()
        {
            if (animator != null)
            {
                aniMoveSpeedPercent = ((player.PlayerInfos.IsRuning) ? 1 : 0.5f) * player.PlayerInfos.AxisInput.magnitude;
                animator.SetFloat("peedPercent", aniMoveSpeedPercent, player.PlayerInfos.SpeedSmoothTime, Time.deltaTime);
            }
        }


    }
}
