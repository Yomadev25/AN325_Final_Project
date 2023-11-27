using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Health Perk", menuName = "Perk/Health")]
public class HealItem : Perk
{
    public enum Type
    {
        Heal,
        AddHealth
    }

    public Type type;

    public override void Execute(Player player)
    {
        if (type == Type.Heal)
        {
            player.Heal();
        }
        else if(type == Type.AddHealth)
        {
            player.AddHealth();
        }
    }
}
