using KartDemo.Controllers;
using KartDemo.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Cinemachine;
using KartDemo;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance { get; private set; }

    public const float OUT_OF_RACE_DISTANCE = 200;
    public const float OUT_OF_GROUND_DISTANCE = 60;

    public int PlayerCount => playerCount;
    private int playerCount;
    public bool Started => started;
    private bool started;

    private float elapsedTime;
    public UnityAction<float> OnElapse;     //Race  Model Event

    public int LapCount => lapCount;
    [SerializeField] private int lapCount;

    [Header("Check Points")]
    public Transform checkPointContainer;
    public List<CheckPoint> CheckPoints => m_checkPoints;
    List<CheckPoint> m_checkPoints;

    public CinemachineSmoothPath path;

    #region RACE EVENTS
    public delegate void RaceFinishedEvent(PlayerRaceResult[] result);
    public UnityAction OnFinalLap { get => m_OnFinalLap; set => m_OnFinalLap = value; }
    public UnityAction m_OnFinalLap;
    public RaceFinishedEvent OnRaceFinished { get => m_OnRaceFinished; set => m_OnRaceFinished = value; }
    [SerializeField]
    private RaceFinishedEvent m_OnRaceFinished;

    bool mineFinised;

    //[System.Serializable]
    //public class RaceFinishedEvent : UnityEvent<PlayerRaceResult[]> { }
    #endregion

    public PositionView positionViewPrefab;

    public KartControllerV2[] Players => players;
    KartControllerV2[] players;
    PlayerTrack[] tracks;
    PositionPresenter[] positionPresenter;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning($"There's more than 1 {GetType()}", this);
        }

        m_checkPoints = new List<CheckPoint>(checkPointContainer.childCount);
        for (int i = 0; i < checkPointContainer.childCount; i++)
        {
            if (checkPointContainer.GetChild(i).TryGetComponent<CheckPoint>(out var c))
            {
                m_checkPoints.Add(c);
                c.OnPlayerCheck += OnPlayerCheck;
            }
        }

        players = FindObjectsByType<KartControllerV2>(FindObjectsSortMode.None);
        playerCount = players.Length;
        tracks = new PlayerTrack[playerCount];
        positionPresenter = new PositionPresenter[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            positionPresenter[i] = new PositionPresenter(Instantiate(positionViewPrefab, players[i].transform), players[i], i + 1);
            positionPresenter[i].Init();

            tracks[i] = players[i].GetComponent<PlayerTrack>();
        }

        StartCoroutine(UpdatePositionsDelayed());
    }

    private void Update()
    {
        if (started)
        {
            elapsedTime += Time.deltaTime;
            OnElapse?.Invoke(elapsedTime);
        }
    }

    public void StartRace()
    {
        started = true;

        FindObjectsByType<KartControllerV2>(FindObjectsSortMode.None).Foreach(kart => kart.OnRaceStarted());
    }

    WaitForSeconds waitForUpdate = new WaitForSeconds(.1f);
    IEnumerator UpdatePositionsDelayed()
    {
        while (true)
        {
            yield return waitForUpdate;
            UpdatePositons();
        }
    }

    void UpdatePositons()
    {
        for (int i = 0; i < playerCount; i++)
        {
            int position = 1;
            PlayerTrack currentTrack_i = tracks[i];

            //assuming that current track is the top 1 player, if real lap is lower, current pos ++
            for (int j = 0; j < playerCount; j++)
            {
                if (i == j)
                    continue;

                PlayerTrack track_j = tracks[j];
                //current top player is in next lap
                if (currentTrack_i.finishedLapCount > track_j.finishedLapCount)
                    continue;

                if (currentTrack_i.finishedLapCount == track_j.finishedLapCount)
                {
                    if (currentTrack_i.lastCheckPoint > track_j.lastCheckPoint)
                    {
                        continue;
                    }

                    //check distance to next check point if players are in the same lap and at the same check point
                    if (currentTrack_i.lastCheckPoint == track_j.lastCheckPoint)
                    {
                        float dist_j = (players[j].transform.position - CP(track_j.lastCheckPoint + 1).transform.position).sqrMagnitude;
                        float dist_i = (players[i].transform.position - CP(currentTrack_i.lastCheckPoint + 1).transform.position).sqrMagnitude;

                        if (dist_i <= dist_j)
                            continue;
                    }
                }

                position++;
            }


            positionPresenter[i].position = position;
            positionPresenter[i].UpdatePosition();
        }
    }

    void OnPlayerCheck(CheckPoint checkPoint, PlayerTrack track)
    {
        //You haven't reached any next checkpoint in the lap, but you reached last checkpoint
        bool isBack = (track.lastCheckPoint == 0 && checkPoint.order >= m_checkPoints.Count / 2);

        if (isBack)
        {
            Debug.Log($"{track.name} is moving against the direction of the track!");
            return;
        }

        bool shouldFinish = checkPoint.order == 0 && track.highestReachedCheckPoint > m_checkPoints.Count / 2;

        if (checkPoint.order < track.lastCheckPoint && !shouldFinish)
        {
            Debug.Log($"{track.name} is moving in the opposite direction!");
            track.lastCheckPoint = checkPoint.order;
            return;
        }

        if (checkPoint.IsNext(track.highestReachedCheckPoint))
        {
            track.highestReachedCheckPoint = checkPoint.order;
        }

        //check point is start-finish but you have to reach the last check point to count it once
        if (checkPoint.isStartFinish && track.lastCheckPoint != 0 && track.highestReachedCheckPoint != 0 && track.highestReachedCheckPoint > m_checkPoints.Count - 2)
        {
            track.OnLapFinished(lapCount);
            track.highestReachedCheckPoint = 0;

            //race finished
            if (track.raceFinished)
            {
                track.OnRaceFinised(elapsedTime);

                if (mineFinised)
                    RaceFinished();
                if (track.isMine)
                {
                    RaceFinished();
                    mineFinised = true;
                }
            }
            //final lap
            else if (track.finishedLapCount == lapCount - 1 && track.isMine)
                OnFinalLap?.Invoke();
        }

        track.lastCheckPoint = checkPoint.order;
    }

    void RaceFinished()
    {
        PlayerRaceResult[] result = new PlayerRaceResult[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            result[i] = new PlayerRaceResult()
            {
                name = players[i].name,
                position = positionPresenter[i].position,
                time = tracks[i].raceFinishedTime,
                isMine = tracks[i].isMine,
                lapCout = tracks[i].finishedLapCount,
                finished = tracks[i].raceFinished
            };
        }

        result = result.OrderBy(t => t.position).ToArray();

        OnRaceFinished?.Invoke(result);
    }

    CheckPoint CP(int index)
    {
        int i = index < 0 ? CheckPoints.Count + index : index >= CheckPoints.Count ? index - CheckPoints.Count : index;
        return CheckPoints[i];
    }

    public void RestartRace()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}