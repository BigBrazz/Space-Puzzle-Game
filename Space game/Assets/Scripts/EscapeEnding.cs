
using UnityEngine;
using UnityEngine.UI;

public class EscapeEnding : MonoBehaviour
{
    public Text objectiveText;
    private bool escaped = false;

    private void OnTriggerEnter(Collider other)
    {
        if (escaped || !other.CompareTag("Player"))
            return;

        if (SpaceShipParts.partsCollected >= SpaceShipParts.totalParts)
        {
            escaped = true;

            if (objectiveText != null)
            {
                objectiveText.text =
                    "MISSION COMPLETE!\n\n" +
                    "CONGRATULATIONS!\n" +
                    "You repaired your spaceship and escaped the planet!\n\n" +
                    "YOU WIN!";
            }

            Debug.Log("MISSION COMPLETE! YOU WIN!");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
