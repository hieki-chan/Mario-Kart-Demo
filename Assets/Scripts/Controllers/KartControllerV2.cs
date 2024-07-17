//github: https://github.com/hieki-chan or https://github.com/hieki-chan/Mario-Kart-Demo
using KartDemo.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KartDemo.Controllers
{
    public class KartControllerV2 : MonoBehaviour
    {
        [Header("Input Handler")]
        [SerializeField] private InputHandler inputHandler;
        static bool raceStarted = false;
        bool disableInput;
        [NonEditable, SerializeField]
        private float motorInput;
        [NonEditable, SerializeField]
        private float steeringInput;
        [SerializeField] private float motorPower;
        [SerializeField] private float steerPower;

        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float gravityForce = 3000;
        private float velocityYAccel;
        float currentSpeed;         //current speed
        float currentFinalSpeed;    //current speed but applied modifiers
        float currentAccel;
        float currentRotVelocity;
        public Transform kartGraphic;
        //[SerializeField] private Vector3 kartOffset;

        [Header("GROUND CHECK")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Vector3 checkOffset;
        [SerializeField] private float checkDistance;
        public bool IsGrouned => isGrounded;
        bool isGrounded;
        public float GroundDist => groundDist;
        float groundDist;

        [Header("Drift")]
        [SerializeField] private float driftSpeedMin;
        [SerializeField] private float driftRotSpeed = 9;
        [SerializeField] private float driftAngle = 20;
        bool driftLeft;
        bool driftRight;
        bool drift => driftLeft || driftRight;
        bool driftRot = true;

        [Header("Spin")]
        [SerializeField] private float spinTime = 1;
        WaitActivator spinActivator;

        [Header("Jump")]
        [SerializeField] private float jumpSpeedMin;    //jump only if current speed >= jump speed min
        [SerializeField] private float jumpHeightMin;   //jump only if ground distance >= jump height min
        [SerializeField] private float jumpAnimationTime = .4f;
        WaitActivator jumpActivator;

        [Header("Obstacle Hit")]
        [SerializeField] private float hitSpeedMin;     //min speed to play hit animation on hit

        Rigidbody m_KartBody;
        Animator m_KartAnimator;
        WheelV2 m_Wheels;
        [NonSerialized] public KartEffects m_KartEffects;
        [NonSerialized] public PlayerSounds m_PlayerSounds;

        //Modifiers
        List<Modifier> m_Modifiers = new List<Modifier>();


        private void Start()
        {
            m_KartBody = GetComponent<Rigidbody>();
            m_KartAnimator = GetComponentInChildren<Animator>();
            m_Wheels = GetComponent<WheelV2>();
            m_KartEffects = GetComponent<KartEffects>();
            m_PlayerSounds = GetComponent<PlayerSounds>();

            spinActivator = new WaitActivator(spinTime, OnSpinStart, OnSpinEnd);
            jumpActivator = new WaitActivator(jumpAnimationTime, null, JumpEnd);
        }

        private void Update()
        {
            HandleInput();
            GroundNormal();

            m_Wheels.ApplySteering(steeringInput);
            if (raceStarted)
                m_Wheels.ApplyMotorTorque(m_KartBody.velocity.magnitude);
            else
                return;

            ApplyModifiers();

            Drift();
            Move();
        }

        private void FixedUpdate()
        {
            //ground check
            if (Physics.Raycast(Position.Offset(transform, checkOffset), -transform.up, out RaycastHit hit, checkDistance, groundLayer))
            {
                isGrounded = true;
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation, Time.deltaTime * 7f);
#if UNITY_EDITOR
                Vector3 positionOffset = Position.Offset(transform, checkOffset);
                Debug.DrawLine(positionOffset, hit.point, Color.green, Time.fixedDeltaTime);
#endif
            }
            else
            {
                isGrounded = false;
#if UNITY_EDITOR
                Vector3 positionOffset = Position.Offset(transform, checkOffset);
                Debug.DrawLine(positionOffset, positionOffset - transform.up * checkDistance, Color.red, Time.fixedDeltaTime);
#endif
            }

            /*if (currentSpeed != 0*//* && isGrounded*//*)
            {
                m_KartBody.AddForce(transform.forward * (motorPower * currentSpeed * (isGrounded ? 1 : .5f)));
            }*/

            //fake gravity
            if (isGrounded)
            {
                m_KartBody.AddRelativeForce(Vector3.down * (Time.deltaTime * gravityForce), ForceMode.Acceleration);
                velocityYAccel = Mathf.Lerp(velocityYAccel, 0, Time.deltaTime);
            }
            else
            {
                velocityYAccel = Mathf.Lerp(velocityYAccel, 1, Time.deltaTime);
                m_KartBody.AddRelativeForce(Vector3.down * (Time.deltaTime * gravityForce * velocityYAccel), ForceMode.Acceleration);
            }

            if (isGrounded && !drift && !driftRot && !spinActivator.IsActivating())
            {
                //print("should be zero pos and rot");
                m_KartBody.transform.localPosition = Vector3.zero;
                m_KartBody.transform.localRotation = Quaternion.identity;
            }

            OnSpeedUpdate?.Invoke();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(PlayerConfig.PLAYER_TAG))
            {
                float targetPlayerSpeed = collision.rigidbody.velocity.magnitude;

                if (targetPlayerSpeed > currentSpeed && currentSpeed >= maxSpeed * .7f)
                {
                    SpinHit(collision.transform.position);
                }

                else
                    OnObstacleHit(collision.transform.position, 1);
            }
        }

        private void HandleInput()
        {
            //Move Input
            Vector2 move = !disableInput ? inputHandler.MoveValue() : Vector2.zero;

            if (!raceStarted)
            {
                if (move.y != 0 && isGrounded)
                {
                    m_KartAnimator.CrossFade(PlayerConfig.StartTurboHash, 0);
                    m_KartAnimator.SetBool(PlayerConfig.IdleMoveParamHash, false);

                    //accelleration and speed before start 
                    currentAccel = Mathf.Lerp(currentAccel, acceleration, Time.deltaTime / 3);
                    currentSpeed = Mathf.SmoothStep(currentSpeed, maxSpeed, Time.deltaTime * acceleration / 3);

                    m_Wheels.ApplyMotorTorque(currentSpeed * 2);
                }
                else
                {
                    m_KartAnimator.SetBool(PlayerConfig.IdleMoveParamHash, true);
                }
                //move = Vector2.zero;
                return;
            }
            motorInput = move.y;
            steeringInput = move.x;

            //calculate speed only if grounded
            if (motorInput > 0 && isGrounded)
            {
                currentAccel = Mathf.Lerp(currentAccel, acceleration, Time.deltaTime);
                currentSpeed = Mathf.SmoothStep(currentSpeed, maxSpeed, Time.deltaTime * acceleration);
            }
            else if (motorInput < 0 && isGrounded)
            {
                currentAccel = Mathf.Lerp(currentAccel, -acceleration, Time.deltaTime);
                currentSpeed = Mathf.Lerp(currentSpeed, -maxSpeed / 3, (currentSpeed == 0 ? acceleration * .75f : acceleration * .1f) * Time.deltaTime);
            }
            else    //motor input == 0
            {
                currentAccel = Mathf.Lerp(currentAccel, 0, Time.deltaTime * 4);

                if (isGrounded)
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * acceleration * .05f);
                    if (m_KartBody.velocity.magnitude < .5f)
                    {
                        currentSpeed = 0;
                    }
                }
            }

            //brake
            if (inputHandler.Brake())
            {
                currentAccel *= .995f;

                if (isGrounded)
                {
                    currentSpeed *= .995f;

                    if (currentSpeed <= maxSpeed * .3f)
                    {
                        currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 8);
                    }
                }
            }

            if (inputHandler.Drift() && !disableInput && isGrounded)
            {
                DoDrift();
            }
        }

        private void Move()
        {
            //transform.position = m_KartBody.transform.position + kartOffset;
            float angle = steeringInput * steerPower * (/*currentSpeed >= maxSpeed * .75f ? .8f : */currentSpeed != 0 ? 1 : 0) * (drift ? 1.85f : 1) * Time.deltaTime;
            angle *= currentFinalSpeed > 0 ? 1 : -1;
            angle *= isGrounded ? 1 : .5f;
            //float none = 0;
            angle = Mathf.SmoothDamp(0, angle, ref currentRotVelocity, Time.deltaTime * 3.5f);
            transform.Rotate(new Vector3(0, angle, 0), Space.Self);
            //transform.rotation = Quaternion.Euler(angle);

            Vector3 velocity = transform.forward * currentFinalSpeed - transform.up * velocityYAccel;
            m_KartBody.velocity = velocity;

            //m_KartBody.AddRelativeForce(Physics.gravity * 5, ForceMode.Acceleration);

            if (currentFinalSpeed > currentSpeed + 10 && currentFinalSpeed >= maxSpeed / 2)
            {
                m_KartEffects.PlaySpeedUp();
            }
            else
                m_KartEffects.StopSpeedUp();

            //kart's motor audio
            if (currentSpeed > 0 && motorInput != 0)
                m_PlayerSounds.KartLoop(m_PlayerSounds.accelNormal);
            else
                m_PlayerSounds.KartLoop(m_PlayerSounds.stopIdleSound);
        }

        private void GroundNormal()
        {
            if (Physics.Raycast(Position.Offset(transform, checkOffset), -transform.up, out RaycastHit hit, groundLayer))
            {
                //rotate body according to ground normal
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, Time.deltaTime * 7f);
#if UNITY_EDITOR
                //doesn't necessary in build
                Vector3 positionOffset = Position.Offset(transform, checkOffset);
                Debug.DrawLine(positionOffset, hit.point, Color.green, Time.fixedDeltaTime);
#endif
                groundDist = Vector3.Distance(transform.position, hit.point);
            }
            else
                groundDist += Time.deltaTime * 20;
        }

        private void DoDrift()
        {
            m_KartAnimator.CrossFade(PlayerConfig.DriftHopHash, 0);

            if (steeringInput == 0)
            {
                m_PlayerSounds.KartOneShot(m_PlayerSounds.DriftHopSound);
            }
        }

        private void Drift()
        {
            if (drift && isGrounded)
            {
                //m_KartEffects.PlaySkidMarks();
                m_PlayerSounds.EffectLoop(m_PlayerSounds.DriftSteerSound);
            }
            else
            {
                m_PlayerSounds.EffectStopIf(m_PlayerSounds.DriftSteerSound);
                m_KartEffects.StopSkidMarks();
            }

            //shouldn't drift
            if (!inputHandler.DriftHold() || currentSpeed < driftSpeedMin)
            {
                if (driftRot && isGrounded)
                {
                    Vector3 kartLocalEuler = kartGraphic.localEulerAngles;
                    kartLocalEuler.y = 0;
                    kartGraphic.localEulerAngles = Rotation.LerpAngle(kartGraphic.localEulerAngles, kartLocalEuler, Time.deltaTime * driftRotSpeed);
                    driftLeft = driftRight = false;
                }

                m_KartEffects.Stop(m_KartEffects.driftLeftEffectPrefab);
                m_KartEffects.Stop(m_KartEffects.driftRightEffectPrefab);

                return;
            }

            if (!isGrounded)
            {

            }
            else if (steeringInput > 0)      //drift right
            {
                Vector3 kartLocalEuler = kartGraphic.localEulerAngles;
                kartLocalEuler.y = driftAngle;
                kartGraphic.localEulerAngles = Rotation.LerpAngle(kartGraphic.localEulerAngles, kartLocalEuler, Time.deltaTime * driftRotSpeed);
                driftRight = true;
            }
            else if (steeringInput < 0)     //drift left
            {
                Vector3 kartLocalEuler = kartGraphic.localEulerAngles;
                kartLocalEuler.y = -driftAngle;
                kartGraphic.localEulerAngles = Rotation.LerpAngle(kartGraphic.localEulerAngles, kartLocalEuler, Time.deltaTime * driftRotSpeed);
                driftLeft = true;
            }

            if (isGrounded)
            {
                if (driftLeft)
                {
                    m_KartEffects.Play(m_KartEffects.driftLeftEffectPrefab);
                    m_KartEffects.Stop(m_KartEffects.driftRightEffectPrefab);
                }
                else if (driftRight)
                {
                    m_KartEffects.Play(m_KartEffects.driftRightEffectPrefab);
                    m_KartEffects.Stop(m_KartEffects.driftLeftEffectPrefab);
                }
            }
            else
            {
                m_KartEffects.Stop(m_KartEffects.driftLeftEffectPrefab);
                m_KartEffects.Stop(m_KartEffects.driftRightEffectPrefab);
            }
        }

        private void StopDrift()
        {
            Vector3 kartLocalEuler = kartGraphic.localEulerAngles;
            kartLocalEuler.y = 0;
            kartGraphic.localEulerAngles = kartLocalEuler;

            m_KartEffects.Stop(m_KartEffects.driftLeftEffectPrefab);
            m_KartEffects.Stop(m_KartEffects.driftRightEffectPrefab);

            driftLeft = driftRight = false;
        }

        public void SpinHit(Vector3 hitPoint)
        {
            Vector3 toPlayer = (hitPoint - transform.position).normalized;
            bool leftSide = Vector3.Dot(toPlayer, transform.right) > 0;

            SpinHit(leftSide ? -1 : 1);
        }

        public void SpinHit(int side)
        {
            if (driftLeft)
            {
                m_KartAnimator.CrossFade(PlayerConfig.SpinLeftHash, 0);
                StopDrift();
            }
            else if (driftRight)
            {
                m_KartAnimator.CrossFade(PlayerConfig.SpinRightHash, 0);
                StopDrift();
            }
            else if (side == 1)
            {
                m_KartAnimator.CrossFade(PlayerConfig.SpinRightHash, 0);
            }
            else if (side == -1)
            {
                m_KartAnimator.CrossFade(PlayerConfig.SpinLeftHash, 0);
            }
            else
            {
                Debug.LogWarning("Spin Error: Unknown spin direction", this);
            }

            Activator.Active(spinActivator);
        }

        void OnSpinStart()
        {
            disableInput = true;
            driftRot = false;
            currentSpeed *= .5f;
        }

        void OnSpinEnd()
        {
            disableInput = false;
            driftRot = true;
        }

        public void JumpSpin()
        {
            //only jump if speed and ground distance are large enough
            if (currentSpeed < jumpSpeedMin || groundDist < jumpHeightMin)
            {
                return;
            }
            if (driftLeft)
            {
                m_KartAnimator.CrossFade(PlayerConfig.SpinLeftHash, 0);
                StopDrift();
            }
            else if (driftRight)
            {
                m_KartAnimator.CrossFade(PlayerConfig.SpinRightHash, 0);
                StopDrift();
            }
            else
            {
                m_KartAnimator.CrossFade(PlayerConfig.JumpHash, 0);
            }

            m_PlayerSounds.PlayBoostSound();
            driftRot = false;
            Activator.Active(jumpActivator);
        }

        void JumpEnd()
        {
            driftRot = true;
        }

        public void OnObstacleHit(Vector3 hitPoint, float speedReducer = .8f)
        {
            if (currentSpeed < hitSpeedMin)
            {
                currentSpeed *= speedReducer;
                return;
            }

            Vector3 hitDirection = (hitPoint - transform.position).normalized;
            float dotLeftRightSide = Vector3.Dot(transform.right, hitDirection);
            float dotForBackwardSide = Vector3.Dot(transform.forward, hitDirection);

            bool rightSide = dotLeftRightSide > .5f;
            bool leftSide = dotLeftRightSide < -.5f;
            bool forwardSide = dotLeftRightSide >= -.5f && dotLeftRightSide <= .5f && dotForBackwardSide >= 0;

            Debug.Log(rightSide ? "right" : leftSide ? "left" : forwardSide ? "forward" : "backward");

            if (rightSide)
            {
                m_KartAnimator.CrossFade(PlayerConfig.LightHitRightHash, 0);
                currentSpeed *= speedReducer;
            }
            else if (leftSide)
            {
                m_KartAnimator.CrossFade(PlayerConfig.LightHitLeftHash, 0);
                currentSpeed *= speedReducer;
            }
            else if (forwardSide)
            {
                m_KartAnimator.CrossFade(PlayerConfig.ObstacleHitHash, 0);
                m_PlayerSounds.KartOneShot(m_PlayerSounds.kartBumpSound);
                currentSpeed = 0;
            }
            else
            {
                //backward
            }

            if (drift)
                StopDrift();
        }

        #region Modifiers
        public void AddModifier(Modifier modifier)
        {
            if (!modifier.CanStackUp())
            {
                Modifier m = m_Modifiers.Find(m => m.GetType() == modifier.GetType());
                if (m != null)
                {
                    m.OnReset();
                    return;
                }
            }

            m_Modifiers.Add(modifier);
            modifier.OnApplied();
        }

        void ApplyModifiers()
        {
            if (m_Modifiers.Count == 0)
            {
                currentFinalSpeed = Mathf.Lerp(currentFinalSpeed, currentSpeed, Time.deltaTime * 4);
                return;
            }

            currentFinalSpeed = currentSpeed;

            for (int i = 0; i < m_Modifiers.Count; i++)
            {
                Modifier modifier = m_Modifiers[i];
                modifier.Update(Time.deltaTime);
                currentFinalSpeed = modifier.ModifySpeed(currentFinalSpeed);

                if (modifier.IsExpired())
                {
                    m_Modifiers.RemoveAtSwapBack(i);
                    i--;
                    modifier.OnRemoved();
                }
            }
        }
        #endregion

        public void MultiplySpeed(float speedMultiplier)
        {
            currentSpeed = ClampSpeed(currentSpeed * speedMultiplier);
        }

        public void AddSpeed(float speed)
        {
            currentSpeed = ClampSpeed(currentSpeed + speed);
        }

        float ClampSpeed(float speed) => Mathf.Clamp(speed, -maxSpeed - 50, maxSpeed + 50);

        public void OnRaceStarted()
        {
            raceStarted = true;
            //m_KartAnimator.CrossFade(PlayerConfig.IdleHash, .02f);
            //kartGraphic.transform.localPosition = Vector3.zero;
            //kartGraphic.localRotation = Quaternion.identity;
            m_KartAnimator.SetBool(PlayerConfig.IdleMoveParamHash, true);
        }

        #region Kart Model Funtions

        public UnityAction OnSpeedUpdate;
        public float GetCurrentSpeed() => m_KartBody.velocity.magnitude;

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Vector3 positionOffset = Position.Offset(transform, checkOffset);
                Gizmos.DrawLine(positionOffset, positionOffset - transform.up * checkDistance);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * jumpHeightMin);
        }
#endif
    }
}