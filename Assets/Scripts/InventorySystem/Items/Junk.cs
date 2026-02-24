using UnityEngine;

[NonTransferableItem]
[CreateAssetMenu(fileName = "New Junk", menuName = "Inventory System/Items/Junk")]
public class Junk : BaseItem
{
    public bool IsSoldAtBuyValue;
}