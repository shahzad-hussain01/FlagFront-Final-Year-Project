using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lovatto.FloatingTextAsset
{
    public class bl_DemoScene : MonoBehaviour
    {
        public Transform demoCamera;
        public float cameraSpeed = 50;
        public AnimationCurve hitEffectCurve = new AnimationCurve()
        {
            keys = new Keyframe[] { new Keyframe() { time = 0, value = 0 },
        new Keyframe() { time = 0.5f, value = 1 }, new Keyframe() { time = 1, value = 0 }}
        };

        private Vector3 camPos;
        private Vector3 camRot;

        private void Update()
        {
            MoveCamera();
        }

        void MoveCamera()
        {
            camPos = demoCamera.localPosition;
            if (Input.GetKey(KeyCode.A))
            {
                camPos.x -= Time.deltaTime * cameraSpeed;
                camRot.z = 5f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                camPos.x += Time.deltaTime * cameraSpeed;
                camRot.z = -5f;
            }
            else
            {
                camRot.z = 0;
            }

            if (Input.GetKey(KeyCode.W))
            {
                camPos.y += Time.deltaTime * cameraSpeed;
                camRot.x = 10f;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                camPos.y -= Time.deltaTime * cameraSpeed;
                camRot.x = -10f;
            }
            else
            {
                camRot.x = 0;
            }
            camPos.y = Mathf.Clamp(camPos.y, -205, -196);
            camPos.x = Mathf.Clamp(camPos.x, -528, -455);

            demoCamera.localPosition = Vector3.Lerp(demoCamera.localPosition, camPos, Time.deltaTime * 15);
            demoCamera.localRotation = Quaternion.Slerp(demoCamera.localRotation, Quaternion.Euler(camRot), Time.deltaTime * 2);
        }

        private static bl_DemoScene _instance;
        public static bl_DemoScene Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<bl_DemoScene>();
                return _instance;
            }
        }
    }
}