using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive Perk", menuName = "Perk/Passive")]
public class PassiveRate : Perk
{
    public float critical;
    public float bomb;
    public float ice;
    public float flame;

    public override void Execute(Player player)
    {
        player.criticalRate += critical;
        player.boomRate += bomb;
        player.flameRate += flame;
        player.iceRate += ice;

        GameManager.instance.UpdatePassivePerk(player.criticalRate, player.flameRate, player.iceRate, player.boomRate);
    }
}
