using System.Collections.Generic;
using UnityEngine;
using KartDemo.Item;
using KartDemo.Controllers;
using KartDemo.Utils;
using KartDemo;

public sealed class WItemBox : WorldItem
{
    [SerializeField] private List<ThrowableItem> randomItems = new List<ThrowableItem>();

    protected override void OnPlayerTrigger(KartControllerV2 player, PlayerTrack track)
    {
        ThrowableItem random = randomItems.PickOne();

        ThrowableItem instance = ThrowableItem.Pool.GetOrCreate(random.GetType(), random, Vector3.zero, Quaternion.identity);
        ItemManager itemManage = player.GetComponent<ItemManager>();

        if (itemManage == null)
        {
            return;
        }

        if(!itemManage.AddItem(instance))
        {
            ThrowableItem.Pool.Return(instance.GetType(), instance);
        }
        else if (Random.Range(0, 2) == 0)
        {
            instance = ThrowableItem.Pool.GetOrCreate(random.GetType(), random, Vector3.zero, Quaternion.identity);
            if (!itemManage.AddItem(instance))
            {
                ThrowableItem.Pool.Return(instance.GetType(), instance);
            }
            else if (Random.Range(0, 3) == 0)
            {
                instance = ThrowableItem.Pool.GetOrCreate(random.GetType(), random, Vector3.zero, Quaternion.identity);
                if (!itemManage.AddItem(instance))
                {
                    ThrowableItem.Pool.Return(instance.GetType(), instance);
                }
            }
        }

        player.m_PlayerSounds.PlayBoostSound();
    }
}

