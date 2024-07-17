using KartDemo;
using KartDemo.MVP;
using System;

[Serializable]
public class LapPresenter : Presenter<LapView, PlayerTrack>
{
    public RaceManager raceManager;
    public override void Init()
    {
        model.OnLapFinishedEvent += UpdateLap;
        UpdateLap(1, raceManager.LapCount);
    }

    public void UpdateLap(int lap, int lapCount)
    {
        view.UpdateLap(lap, lapCount);
    }
}