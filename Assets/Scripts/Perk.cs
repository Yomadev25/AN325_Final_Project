using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perk : ScriptableObject
{
    public string name;
    public Sprite icon;

    [Header("Passive Setting")]
    public float duration;

    public virtual void Execute(Player player)
    {

    }
}
