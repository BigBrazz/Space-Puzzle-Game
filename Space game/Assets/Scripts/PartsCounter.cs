
using UnityEngine;
using UnityEngine.UI;

public class PartsCounter : MonoBehaviour
{
    public Text objectiveText;
    private bool gameWon = false;

    void Update()
    {
        if (gameWon || objectiveText == null)
            return;

        if (SpaceShipParts.partsCollected < SpaceShipParts.totalParts)
        {
            objectiveText.text = "OBJECTIVE:\nCollect Spaceship Parts (" +
                SpaceShipParts.partsCollected + "/" +
                SpaceShipParts.totalParts + ")";
        }
        else
        {
            objectiveText.text = "OBJECTIVE:\nReturn to the ship";
        }
    }

    public void ResetPartsCounter()
    {
        SpaceShipParts.partsCollected = 0;
    }

    public void ShowVictory()
    {
        gameWon = true;
        //ResetPartsCounter();
        if (objectiveText != null)
        {
            objectiveText.text =
                "MISSION COMPLETE!\n\n" +
                "CONGRATULATIONS!\n" +
                "You repaired your spaceship and escaped the planet!\n\n" +
                "YOU WIN!";
        }
    }
}
