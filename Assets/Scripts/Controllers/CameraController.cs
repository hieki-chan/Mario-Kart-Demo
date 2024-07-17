using KartDemo.Utils;
using System;
using UnityEngine;

namespace KartDemo.Controllers
{
    public class CameraController : MonoBehaviour
    {
        Transform tr;
        Camera cam;
        public Transform target;//The target vehicle
        Rigidbody targetBody;

        public float height;
        public float distance;

        public float smoothYRotSpeed = 0.02f;
        public float lookSpeed = .1f;
        public float upLookSpeed = 0.05f;

        Vector3 lookDir;
        float smoothYRot;
        Vector3 forwardLook;
        Vector3 upLook;
        Vector3 targetForward;
        Vector3 targetUp;

        [Tooltip("Mask for which objects will be checked in between the camera and target vehicle")]
        public LayerMask castMask;

        void Start()
        {
            tr = transform;
            cam = GetComponent<Camera>();
            Initialize();
        }

        public void Initialize()
        {
            if (target)
            {
                forwardLook = target.forward;
                upLook = target.up;
                targetBody = target.GetComponent<Rigidbody>();
            }

            //Set the audio listener update mode to fixed, because the camera moves in FixedUpdate
            //This is necessary for doppler effects to sound correct
            GetComponent<AudioListener>().velocityUpdateMode = AudioVelocityUpdateMode.Fixed;
        }

        void FixedUpdate()
        {
            if (target && targetBody && target.gameObject.activeSelf)
            {

                targetForward = target.forward;

                targetUp = transform.up;
                lookDir = Vector3.Slerp(lookDir, Vector3.forward, lookSpeed);
                smoothYRot = Mathf.Lerp(smoothYRot, targetBody.angularVelocity.y, smoothYRotSpeed);

                //Determine the upwards direction of the camera
                RaycastHit hit;
                if (Physics.Raycast(target.position, -targetUp, out hit, 1, castMask))
                {
                    upLook = Vector3.Lerp(upLook, (Vector3.Dot(hit.normal, targetUp) > 0.5 ? hit.normal : targetUp), upLookSpeed);
                }
                else
                {
                    upLook = Vector3.Lerp(upLook, targetUp, upLookSpeed);
                }

                //Calculate rotation and position variables
                forwardLook = Vector3.Lerp(forwardLook, targetForward, 0.05f);
                tr.rotation = Quaternion.LookRotation(forwardLook, upLook);
                tr.position = target.position;
                Vector3 lookDirActual = (lookDir - new Vector3(Mathf.Sin(smoothYRot), 0, Mathf.Cos(smoothYRot)) * Mathf.Abs(smoothYRot) * 0.2f).normalized;
                Vector3 forwardDir = tr.TransformDirection(lookDirActual);
                Vector3 localOffset = tr.TransformPoint(-lookDirActual * distance - lookDirActual * Mathf.Min(targetBody.velocity.magnitude * 0.05f, 2) + new Vector3(0, height, 0));

                //Check if there is an object between the camera and target vehicle and move the camera in front of it
                if (Physics.Linecast(target.position, localOffset, out hit, castMask))
                {
                    tr.position = hit.point + (target.position - localOffset).normalized * (cam.nearClipPlane + 0.1f);
                }
                else
                {
                    tr.position = localOffset;
                }

                tr.rotation = Quaternion.LookRotation(forwardDir, target.up);
            }
        }
    }
}