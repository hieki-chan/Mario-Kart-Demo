using KartDemo;
using KartDemo.Controllers;
using KartDemo.Item;

public sealed class WMushroom : WorldItem
{
    public float speed;
    public float duration;

    SpeedUpModifier speedUpModifier;

    private void Start()
    {
        speedUpModifier = new SpeedUpModifier(duration, speed);
    }

    protected override void OnPlayerTrigger(KartControllerV2 player, PlayerTrack track)
    {
        player.m_PlayerSounds.PlayBoostSound();
        player.AddModifier(speedUpModifier);
    }
}

