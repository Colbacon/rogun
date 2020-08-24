using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
public class Consumable : Item
{
    public override void Use()
    {
        Debug.Log("Using: "+ name);
    }
}
