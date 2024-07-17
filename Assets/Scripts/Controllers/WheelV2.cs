using KartDemo.Utils;
using UnityEngine;

namespace KartDemo.Controllers
{
    public class WheelV2 : MonoBehaviour
    {
        [Header("WHEELS")]
        /// <summary> forward left wheel.</summary>
        [SerializeField] private Transform FLWheel;
        /// <summary> forward right wheel.</summary>
        [SerializeField] private Transform FRWheel;
        /// <summary> back left wheel.</summary>
        [SerializeField] private Transform BLWheel;
        /// <summary> back right wheel.</summary>
        [SerializeField] private Transform BRWheel;


        /// <summary> right steering wheel.</summary>
        [SerializeField] private Transform SWL;
        /// <summary> left steering wheel.</summary>
        [SerializeField] private Transform SWR;

        [SerializeField] private float wheelTorqueSpeed;
        [SerializeField, Range(0, 60)] private float wheelSteeringAngle = 45;
        [SerializeField] private float wheelSteeringSpeed = 8;

        public void ApplyMotorTorque(float speed)
        {
            ApplyMotor(FLWheel, speed);
            ApplyMotor(FRWheel, speed);
            ApplyMotor(BLWheel, speed);
            ApplyMotor(BRWheel, speed);
        }

        private void ApplyMotor(Transform wheel, float speed)
        {
            //Vector3 localEuler = wheel.localEulerAngles;
            //localEuler.x += wheelTorqueSpeed * speed;
            //wheel.localEulerAngles = Rotation.LerpAngle(wheel.localEulerAngles, localEuler, Time.deltaTime * speed);

            wheel.Rotate(wheelTorqueSpeed * speed * Time.deltaTime, 0, 0);
        }

        public void ApplySteering(float input)
        {
            if (input > 0)
            {
                SWL.localEulerAngles = /*Rotation.LerpAngle(SWL.localEulerAngles, new Vector3(SWL.localEulerAngles.x, wheelSteeringAngle, 0), Time.deltaTime * wheelSteeringSpeed);*/
                SWR.localEulerAngles = Rotation.LerpAngle(SWR.localEulerAngles, new Vector3(SWR.localEulerAngles.x, wheelSteeringAngle, 0), Time.deltaTime * wheelSteeringSpeed);
            }
            else if (input < 0)
            {
                SWL.localEulerAngles = /*Rotation.LerpAngle(SWL.localEulerAngles, new Vector3(SWL.localEulerAngles.x, -wheelSteeringAngle, 0), Time.deltaTime * wheelSteeringSpeed);*/
                SWR.localEulerAngles = Rotation.LerpAngle(SWR.localEulerAngles, new Vector3(SWR.localEulerAngles.x, -wheelSteeringAngle, 0), Time.deltaTime * wheelSteeringSpeed);
            }
            else
            {
                SWL.localEulerAngles = /*Rotation.LerpAngle(SWL.localEulerAngles, new Vector3(SWL.localEulerAngles.x, 0, 0), Time.deltaTime * wheelSteeringSpeed);*/
                SWR.localEulerAngles = Rotation.LerpAngle(SWR.localEulerAngles, new Vector3(SWR.localEulerAngles.x, 0, 0), Time.deltaTime * wheelSteeringSpeed);
            }
        }
    }
}