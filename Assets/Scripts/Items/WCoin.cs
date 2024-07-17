using KartDemo;
using KartDemo.Controllers;
using KartDemo.Item;

public sealed class WCoin : WorldItem
{
    protected override void OnPlayerTrigger(KartControllerV2 player, PlayerTrack track)
    {
        track.CollectCoin();
    }
}

