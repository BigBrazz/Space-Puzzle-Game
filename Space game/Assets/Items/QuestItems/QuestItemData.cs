using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestItem", menuName = "Items/Quest Item")]
public class QuestItemData : ItemData
{
    public const string EnergyCellName = "Energy Cell";
    public override ItemCategory Category => ItemCategory.QuestItem;
}
