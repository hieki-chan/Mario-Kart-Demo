using System;
using UnityEngine;

namespace KartDemo.Controllers
{
    [Obsolete]
    public class KartController : MonoBehaviour
    {
        [Header("WHEELS")]
        /// <summary> forward left wheel.</summary>
        [SerializeField] private Wheel FLWheel;
        /// <summary> forward right wheel.</summary>
        [SerializeField] private Wheel FRWheel;
        /// <summary> back left wheel.</summary>
        [SerializeField] private Wheel BLWheel;
        /// <summary> back right wheel.</summary>
        [SerializeField] private Wheel BRWheel;

        [Header("INPUT")]
        [SerializeField] private float motorInput;
        [SerializeField] private float steeringInput;
        [SerializeField] private float motorPower = 100;
        [SerializeField] private float brakePower = 100;
        [SerializeField] private AnimationCurve steeringCurve;
        [SerializeField, Range(0, 60)] private float steerAngle = 45;
        private float speed;

        Rigidbody rigidBody;

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            HandleInput();
            ApplyMotor();
            ApplySteering();
            ApplyWheels();
        }

        private void HandleInput()
        {
            motorInput = Input.GetAxis("Vertical");
            steeringInput = Input.GetAxis("Horizontal");
        }

        private void ApplyWheels()
        {
            ApplyWheelTransform(FLWheel);
            ApplyWheelTransform(FRWheel);
            ApplyWheelTransform(BLWheel);
            ApplyWheelTransform(BRWheel);
        }

        private void ApplyWheelTransform(Wheel wheel)
        {
            WheelCollider wCollider = wheel.wCollider;
            MeshRenderer wMesh = wheel.wMesh;

            wCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);

            wMesh.transform.transform.position = pos;
            wMesh.transform.rotation = rot;
        }

        private void ApplyMotor()
        {
            BLWheel.wCollider.motorTorque = motorInput * motorPower;
            BRWheel.wCollider.motorTorque = motorInput * motorPower;
        }

        private void ApplyBrake()
        {
            FLWheel.wCollider.brakeTorque = 0 * brakePower;
            FRWheel.wCollider.brakeTorque = 0 * brakePower;
            BLWheel.wCollider.brakeTorque = 0 * brakePower;
            BRWheel.wCollider.brakeTorque = 0 * brakePower;
        }

        private void ApplySteering()
        {
            speed = rigidBody.velocity.magnitude;
            float steerAngle = steeringInput * this.steerAngle * steeringCurve.Evaluate(speed);
            FLWheel.wCollider.steerAngle = FRWheel.wCollider.steerAngle = steerAngle;
        }
    }

    [Serializable]
    public class Wheel
    {
        public WheelCollider wCollider;
        public MeshRenderer wMesh;
    }
}