using Cinemachine;
using KartDemo;
using KartDemo.Controllers;
using KartDemo.Utils;
using UnityEngine;

namespace KartDemo.AI
{
    public class ComputerDriver : InputHandler
    {
        [SerializeField, NonEditable] int currentPoint;

        [SerializeField, NonEditable] private Vector2 input;
        [SerializeField, NonEditable] private bool shouldDrift;
        CinemachineSmoothPath path;

        ItemManager itemManager;
        float throwDelayTime;
        float throwTimer;

        Vector3 lastPosition;
        float mayStuckTimer;
        float tryToMoveBackTimer;

        PlayerTrack track;

        [Header("Fence Detection")]
        [SerializeField] float rayDist;

        Vector3 this[int i]
        {
            get
            {
                var points = path.m_Waypoints;
                Vector3 point = i >= 0 && i < points.Length ? points[i].position :
                     i < 0 ? points[points.Length + i].position :
                     points[i - points.Length].position;

                return point + path.transform.position.ToFlat();
            }
        }

        private void Start()
        {
            throwDelayTime = Random.Range(1, 4);
            itemManager = GetComponent<ItemManager>();
            track = GetComponent<PlayerTrack>();
            path = RaceManager.instance.path;
        }


        public override bool Brake()
        {
            return false;
        }

        public override Vector2 MoveValue()     //this function update per frame so we dont need the update funtion
        {
            Steer();
            ThrowItem();
            DetectFence();
            CheckStuck();

            return input;
        }

        void Steer()
        {
            int wayPointCount = path.m_Waypoints.Length;

            Vector3 toWayPoint = transform.position - this[currentPoint];
            Vector3 nextDir = this[currentPoint + 1] - this[currentPoint];
            Vector3 currentDir = this[currentPoint] - this[currentPoint - 1];
            Debug.DrawLine(transform.position, this[currentPoint], Color.yellow);

            if (toWayPoint.sqrMagnitude <= 10 * 10)
            {
                currentPoint = currentPoint + 1 >= wayPointCount ? 0 : currentPoint + 1;
            }

            int minPointIndexer = FindCloset(currentPoint);

            if (minPointIndexer > currentPoint
                || minPointIndexer <= currentPoint - 2)
            {
                currentPoint = minPointIndexer;
            }

            currentPoint = currentPoint < 0 ? wayPointCount + currentPoint :
                 currentPoint >= wayPointCount ? currentPoint - wayPointCount : currentPoint;


            float forward = Vector3.Dot(transform.forward, nextDir.normalized);
            float dotSide = Vector3.Dot(transform.right, nextDir.normalized);
            float dotSide2 = Vector3.Dot(transform.right, (nextDir + currentDir).normalized);

            float currentNode = Vector3.Dot(toWayPoint.normalized, transform.forward);

            shouldDrift = currentNode < 0;
            input.y = 1;

            float motor = (forward > .6f || shouldDrift ? 1 : 0) * (dotSide > 0 ? 1 : -1) * (currentNode > .6f ? .5f : 1);
            input.x = motor * input.x < 0 ? motor : Mathf.Lerp(input.x, motor, Time.deltaTime * 2f);
        }

        void DetectFence()
        {
            Ray forwardRay = new Ray(transform.position + transform.up * .5f, transform.forward);
            Ray rightRay = new Ray(transform.position + transform.up, transform.right);
            Ray leftRay = new Ray(transform.position + transform.up * .5f, -transform.right);


            if (RayCast(forwardRay, out RaycastHit hit))
            {
                input.y *= .75f;
                input.x *= 1.35f;
                shouldDrift = true;
            }
            else
            {
                if (Mathf.Abs(input.x) <= .5f)
                    input.x *= 1.35F;
            }

            if (RayCast(rightRay, out hit))
            {
                input.x -= .5f;
            }

            if (RayCast(leftRay, out hit))
            {
                input.x += .5f;
            }
        }

        bool RayCast(Ray ray, out RaycastHit hit)
        {
            bool result = Physics.Raycast(ray, out hit, rayDist, 1 << 7);

            if (result)
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red);
            }
            else
            {
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * rayDist, Color.green);
            }

            return result;
        }

        void CheckStuck()
        {
            if (!RaceManager.instance.Started)
                return;

            if ((lastPosition - transform.position).sqrMagnitude <= .25f * .25f)
            {
                mayStuckTimer += Time.deltaTime;

                if (mayStuckTimer >= 2F && tryToMoveBackTimer <= 0)
                {
                    tryToMoveBackTimer = Random.Range(1.5F, 2.25F);
                }
            }
            else
                mayStuckTimer = 0;

            if (tryToMoveBackTimer > 0)
            {
                input.y *= -.75F;
                input.x *= -2;
                tryToMoveBackTimer -= Time.deltaTime;

                if (tryToMoveBackTimer <= 0 && mayStuckTimer >= 1)
                {
                    track.Respawn();
                }
            }

            lastPosition = transform.position;
        }

        public override bool Drift()
        {
            return false;
        }

        public override bool DriftHold()
        {
            return shouldDrift;
        }

        private void ThrowItem()
        {
            if (itemManager.ItemCount < 0)
            {
                throwTimer = 0;
                return;
            }

            throwTimer += Time.deltaTime;
            if (throwTimer >= throwDelayTime)
            {
                itemManager.ThrowItem();
                throwTimer = 0;
                throwDelayTime = Random.Range(1, 6);
            }
        }

        public int FindCloset(int current)
        {
            float minDist = (transform.position - this[current]).magnitude;
            int minPoint = current;
            for (int i = current; i < current + path.m_Waypoints.Length - 2; i++)
            {
                var point = this[i];

                float dist = (transform.position - point).magnitude;

                if (dist < minDist)
                {
                    minDist = dist;
                    minPoint = i;
                }
            }

            return minPoint;
        }

#if UNITY_EDITOR
        [Header("Display"), Space]
        public Color textColor = Color.blue;

        private void OnDrawGizmos()
        {
            SceneUtils.DrawText(GUI.skin, name, transform.position + transform.up * 4, textColor);
        }
#endif
    }
}