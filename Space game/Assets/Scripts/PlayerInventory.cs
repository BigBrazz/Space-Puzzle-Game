using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool hasEnergyCell = false;

    public void CollectEnergyCell()
    {
        hasEnergyCell = true;

        Debug.Log("Energy cell collected!");
    }

    public void UseEnergyCell()
    {
        hasEnergyCell = false;
    }
}