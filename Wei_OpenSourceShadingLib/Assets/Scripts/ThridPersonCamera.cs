using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Visin1_1
{
    public class ThridPersonCamera : MonoBehaviour
    {
        [SerializeField] private Vector2 pitchMinMax = new Vector2(0, 85);
        [SerializeField] private float rotationSmoothTime = 0.5f;
        private Vector3 rotationSmoothVelocity;
        private Vector3 currentRotation;

        [SerializeField] private Transform target;
        [SerializeField] private Vector2 rangeToTarget = new Vector2(2, 20);
        [SerializeField] private float cameraMoveSensitivity = 10;
        private float dstToTarget = 20;

        private float yaw;  //Rotation around Y Axis
        private float pitch = 75;//Rotation around X Axis

        public bool Xbox;

        void LateUpdate()
        {
            if (Xbox)
            {
                yaw += Input.GetAxis("RXAxis") * cameraMoveSensitivity;
                pitch -= Input.GetAxis("RYAxis") * cameraMoveSensitivity;
                pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
                dstToTarget += Input.GetAxis("Trigger");
                dstToTarget = Mathf.Clamp(dstToTarget, rangeToTarget.x, rangeToTarget.y);
            }else{
                yaw += Input.GetAxis("Mouse X") * cameraMoveSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * cameraMoveSensitivity;
                pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
                dstToTarget += Input.GetAxis("Mouse ScrollWheel");
                dstToTarget = Mathf.Clamp(dstToTarget, rangeToTarget.x, rangeToTarget.y);
            }
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
            transform.eulerAngles = currentRotation;
            transform.position = target.position - transform.forward * dstToTarget;
        }

        public float Yaw
        {
            get { return yaw; }
        }

        public float Pitch
        {
            get { return pitch; }
        }
    }
}
