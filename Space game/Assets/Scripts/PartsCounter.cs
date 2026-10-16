
using UnityEngine;
using UnityEngine.UI;

public class PartsCounter : MonoBehaviour
{
    public Text counterText;

    void Update()
    {
        counterText.text = "Spaceship Parts: "
            + SpaceShipParts.partsCollected + "/"
            + SpaceShipParts.totalParts;
    }
}
