
using UnityEngine;
using UnityEngine.UI;

public class EscapeEnding : MonoBehaviour
{
    public Text objectiveText; 
    public OxygenSystem oxygenSystem;
    private bool escaped = false;

    private void OnTriggerEnter(Collider other)
    {
        if (escaped || !other.CompareTag("Player"))
            return;

        if (SpaceShipParts.partsCollected >= SpaceShipParts.totalParts)
        {
            escaped = true; 

            if (oxygenSystem != null) 
            { 
                oxygenSystem.WinGame(); 
            } 
            else 
            {
                PartsCounter counter = FindObjectOfType<PartsCounter>();

                if (counter != null)
                    counter.ShowVictory();
                else if (objectiveText != null)
                {
                    //counter.ResetPartsCounter();
                    objectiveText.text =
                        "MISSION COMPLETE!\n\n" +
                        "CONGRATULATIONS!\n" +
                        "You escaped the planet!\n\nYOU WIN!";
                }
                Debug.Log("MISSION COMPLETE! YOU WIN!");
            }
        }
    }
}
