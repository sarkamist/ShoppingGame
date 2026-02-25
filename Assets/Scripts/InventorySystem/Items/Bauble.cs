using UnityEngine;

[NonTransferableItem]
[CreateAssetMenu(fileName = "New Bauble", menuName = "Inventory System/Items/Bauble")]
public class Bauble : BaseItem
{
    public override string TypeKey => "item.types.bauble";

    public bool IsSoldAtBuyValue;
}