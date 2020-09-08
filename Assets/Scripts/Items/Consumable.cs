﻿using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
public class Consumable : Item
{
    public int heal;

    public override void Use()
    {
        if(heal >= 0)
        {
            Player.instance.Heal(heal);
        }
        else
        {
            Player.instance.TakeDamage(Mathf.Abs(heal));
        }
    }
}
