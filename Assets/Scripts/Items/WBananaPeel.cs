using KartDemo;
using KartDemo.Controllers;
using KartDemo.Item;
using UnityEngine;

public sealed class WBananaPeel : WorldItem
{
    protected override void OnPlayerTrigger(KartControllerV2 player, PlayerTrack track)
    {
        Vector3 toPlayer = (transform.position - player.transform.position).normalized;
        bool leftSide = Vector3.Dot(toPlayer, player.transform.right) > 0;
        player.SpinHit(leftSide ? -1 : 1);
    }
}

