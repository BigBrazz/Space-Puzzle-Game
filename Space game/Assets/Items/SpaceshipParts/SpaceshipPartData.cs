using UnityEngine;

[CreateAssetMenu(fileName = "NewSpaceshipPart", menuName = "Items/Spaceship Part")]
public class SpaceshipPartData : UniqueItemData
{
    [SerializeField] private GameObject model;
    public GameObject Model => model;
    public override ItemCategory Category => ItemCategory.SpaceshipPart;
}
