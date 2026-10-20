
using UnityEngine;
using UnityEngine.UI;

public class PartsCounter : MonoBehaviour
{
    public Text objectiveText;

    void Start()
    {
        if (objectiveText == null)
        {
            Debug.LogError("Objective Text is not assigned!");
            enabled = false;
        }
    }

    void Update()
    {
        if (SpaceShipParts.partsCollected < SpaceShipParts.totalParts)
        {
            objectiveText.text =
                "OBJECTIVE:\nCollect Spaceship Parts (" +
                SpaceShipParts.partsCollected + "/" +
                SpaceShipParts.totalParts + ")";
        }
        else
        {
            objectiveText.text = "OBJECTIVE:\nReturn to the ship";
        }
    }
}
