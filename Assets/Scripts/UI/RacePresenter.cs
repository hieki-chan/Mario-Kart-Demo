using KartDemo.MVP;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class RacePresenter : Presenter<RaceView, RaceManager>
{
    public override void Init()
    {
        model.StartCoroutine(Countdown());
        //model.OnRaceFinished += OnRaceFinished;
        model.OnRaceFinished += (OnRaceFinished);
        model.OnFinalLap += OnFinalLap;
        waitForSec = new WaitForSeconds(1);

        model.OnElapse += view.UpdateTime;
    }

    WaitForSeconds waitForSec;
    IEnumerator Countdown()
    {
        view.PlayStartSound();
        view.SetText("");
        yield return waitForSec;
        view.SetText("3");
        view.LineAnimationOut();
        yield return waitForSec;
        view.SetText("2");
        yield return waitForSec;
        view.SetText("1");
        yield return waitForSec;
        view.SetText("Go!");
        view.SetActiveHeader(false);
        view.LineAnimationIn();
        yield return waitForSec;
        view.gameObject.SetActive(false);
        model.StartRace();
    }

    void OnFinalLap()
    {
        view.OnFinalLap();
    }

    void OnRaceFinished(PlayerRaceResult[] players)
    {
        view.OnRaceFinished();
    }
}