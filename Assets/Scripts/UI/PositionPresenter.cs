using KartDemo.Controllers;
using KartDemo.MVP;
using System;
using UnityEngine;

[Serializable]
public class PositionPresenter : Presenter<PositionView, KartControllerV2>
{
    public int position;

    public PositionPresenter(PositionView positionView, KartControllerV2 player, int position)
    {
        this.model = player;
        this.view = positionView;
        this.position = position;
    }
    public override void Init()
    {
        model.OnSpeedUpdate += UpdateSpeed;
    }

    public void UpdateSpeed()
    {
        int speed = Mathf.Abs((int)model.GetCurrentSpeed());
        view.UpdateSpeed(speed);
    }

    public void UpdatePosition()
    {
        view.UpdatePosition(position);
    }
}