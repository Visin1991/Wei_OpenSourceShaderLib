using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visin1_1
{
    [System.Serializable]
    public class PlayerInputs
    {
        // Use this for initialization
        [SerializeField]
        private KeyCode X = KeyCode.X;
        [SerializeField]
        private KeyCode B = KeyCode.B;
        [SerializeField]
        private KeyCode Y = KeyCode.Y;
        [SerializeField]
        private KeyCode Z = KeyCode.Z;
        [SerializeField]
        private KeyCode R = KeyCode.R;
        [SerializeField]
        private string AxisUpDown = "Vertical";
        [SerializeField]
        private string AxisLeftRight = "Horizontal";
        private Vector2 axisInput = Vector2.zero;

        //Input Down Information
        bool xDown = false;
        bool bDown = false;
        bool yDown = false;
        bool zDown = false;
        bool rDown = false;

        //Input Up Information
        bool xUp = false;
        bool bUp = false;
        bool yUp = false;
        bool zUp = false;
        bool rUp = false;

        //Input Up Information
        bool xHold = false;
        bool bHold = false;
        bool yHold = false;
        bool zHold = false;
        bool rHold = false;

        public bool XDown
        {
            get
            {
                return xDown;
            }
        }

        public bool BDown
        {
            get
            {
                return bDown;
            }
        }

        public bool YDown
        {
            get
            {
                return yDown;
            }
        }

        public bool ZDown
        {
            get
            {
                return zDown;
            }
        }

        public bool RDown
        {
            get
            {
                return rDown;
            }
        }

        public bool XUp
        {
            get
            {
                return xUp;
            }
        }

        public bool BUp
        {
            get
            {
                return bUp;
            }
        }

        public bool YUp
        {
            get
            {
                return yUp;
            }
        }

        public bool ZUp
        {
            get
            {
                return zUp;
            }
        }

        public bool RUp
        {
            get
            {
                return rUp;
            }
        }

        public bool XHold
        {
            get
            {
                return xHold;
            }
        }

        public bool BHold
        {
            get
            {
                return bHold;
            }
        }

        public bool YHold
        {
            get
            {
                return yHold;
            }
        }

        public bool ZHold
        {
            get
            {
                return zHold;
            }
        }

        public bool RHold
        {
            get
            {
                return rHold;
            }
        }

        public Vector2 AxisInput
        {
            get
            {
                return axisInput;
            }
        }

        public void UpdateInputInfos()
        {
            axisInput.x = Input.GetAxisRaw(AxisLeftRight);
            axisInput.y = Input.GetAxisRaw(AxisUpDown);

            xDown = Input.GetKeyDown(X);
            bDown = Input.GetKeyDown(B);
            yDown = Input.GetKeyDown(Y);
            zDown = Input.GetKeyDown(Z);
            rDown = Input.GetKeyDown(R);

            xUp = Input.GetKeyUp(X);
            bUp = Input.GetKeyUp(B);
            yUp = Input.GetKeyUp(Y);
            zUp = Input.GetKeyUp(Z);
            rUp = Input.GetKeyUp(R);

            xHold = Input.GetKey(X);
            bHold = Input.GetKey(B);
            yHold = Input.GetKey(Y);
            zHold = Input.GetKey(Z);
            rHold = Input.GetKey(R);
        }
    }
}
