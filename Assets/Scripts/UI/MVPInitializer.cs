using UnityEngine;

public class MVPInitializer : MonoBehaviour
{
    [Header("Coin")]
    public CoinPresenter CoinPresenter;

    [Header("Race")]
    public RacePresenter racePresenter;

    [Header("Lap")]
    public LapPresenter lapPresenter;

    private void Start()
    {
        racePresenter.Init();
        lapPresenter.Init();
    }
}