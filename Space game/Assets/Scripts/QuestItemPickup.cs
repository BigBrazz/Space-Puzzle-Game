using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItemPickup : MonoBehaviour
{
    public void Pickup(GameObject player)
    {
        PlayerInventory inventory =
            player.GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.Log("Player inventory not found!");
            return;
        }

        if (inventory.hasEnergyCell)
        {
            Debug.Log("You already have an energy cell!");
            return;
        }

        inventory.CollectEnergyCell();

        Destroy(gameObject);
    }
}