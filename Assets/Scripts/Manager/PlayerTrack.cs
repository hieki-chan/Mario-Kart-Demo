using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KartDemo.Utils;
using KartDemo.Controllers;

namespace KartDemo
{
    public class PlayerTrack : MonoBehaviour
    {
        public bool isMine;

        public int lastCheckPoint { get => m_lastCheckPoint; set { m_lastCheckPoint = value; } }
        private int m_lastCheckPoint;

        public int highestReachedCheckPoint { get => m_highestReachedCheckPoint; set => m_highestReachedCheckPoint = value; }
        private int m_highestReachedCheckPoint;

        public UnityAction<int, int> OnLapFinishedEvent;
        public UnityAction OnRaceFinished;

        public int finishedLapCount { get; private set; }

        public bool raceFinished { get; private set; }
        public float raceFinishedTime { get; private set; }

        private int collectedCoin;
        public UnityAction OnCoinCollect;

        KartControllerV2 controller;
#if UNITY_EDITOR
        [NonEditable, SerializeField] int Lap;
        [NonEditable, SerializeField] string Time;
#endif

        public void OnLapFinished(int maxLapCount)
        {
            //finishedLapCount = Mathf.Clamp(finishedLapCount + 1, 0, maxLapCount);
            finishedLapCount++;
            OnLapFinishedEvent?.Invoke(Mathf.Clamp(finishedLapCount + 1, 0, maxLapCount), maxLapCount);
#if UNITY_EDITOR
            Lap = finishedLapCount;
#endif
            if (finishedLapCount == maxLapCount)
            {
                OnRaceFinished?.Invoke();
                raceFinished = true;
            }
        }

        public void OnRaceFinised(float elapsed)
        {
            if(raceFinishedTime == 0)
            {
                raceFinishedTime = elapsed;
#if UNITY_EDITOR
                Time = string.Format("0:00, 1:00", raceFinishedTime);
#endif
            }
        }

        public void CollectCoin()
        {
            collectedCoin++;
            OnCoinCollect?.Invoke();
        }

        public int CoinAmount()
        {
            return collectedCoin;
        }

        private void Start()
        {
            controller = GetComponent<KartControllerV2>();
            StartCoroutine(OutOfRaceCheck());
        }

        WaitForSeconds checkBoundDelay = new WaitForSeconds(.2f);
        IEnumerator OutOfRaceCheck()
        {
            List<CheckPoint> checkPoints = RaceManager.instance.CheckPoints;

            while (true)
            {
                checkPoints.FindClosest(transform, out float distance);

                if (distance > RaceManager.OUT_OF_RACE_DISTANCE && controller.GroundDist > RaceManager.OUT_OF_GROUND_DISTANCE)
                {
                    //out of bounds

                    Respawn();
                }

                yield return checkBoundDelay;
            }
        }

        /// <summary>
        /// Respawn at last check point.
        /// </summary>
        public void Respawn()
        {
            CheckPoint checkPoint = RaceManager.instance.CheckPoints[lastCheckPoint];

            transform.position = checkPoint.transform.position;
            transform.rotation = checkPoint.transform.rotation;

            controller.MultiplySpeed(.222f);
        }
    }
}