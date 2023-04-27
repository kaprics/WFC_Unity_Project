using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace WFC.CameraController
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private bool cameraMovementInverted;
        [SerializeField] private bool cameraRotationInverted;
        [SerializeField] private float cameraMovementSpeed;
        [SerializeField] private float cameraRotationSpeed;
        
        private Vector2 lastMousePos;
        private Vector2 currentMousePos;
        private Vector2 deltaMousePos;

        private Camera mainCam;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        private void Update()
        {
            currentMousePos = Input.mousePosition;
            deltaMousePos = currentMousePos - lastMousePos;
            if (cameraMovementInverted) deltaMousePos = -deltaMousePos;
            
            if (Input.GetMouseButton(0))
            {
                transform.position += transform.TransformVector(new Vector3(deltaMousePos.x / Screen.width, 0, deltaMousePos.y / Screen.height) * (Time.deltaTime * cameraMovementSpeed));
                Debug.Log(Screen.width);
            }

            if (Input.GetMouseButton(1))
            {
                if (cameraRotationInverted)
                {
                    transform.eulerAngles += new Vector3(0, -deltaMousePos.x, 0) * (Time.deltaTime * cameraRotationSpeed);
                }
                else
                {
                    transform.eulerAngles += new Vector3(0, deltaMousePos.x, 0) * (Time.deltaTime * cameraRotationSpeed);
                }
            }
            
            lastMousePos = Input.mousePosition;
        }
    }
}
