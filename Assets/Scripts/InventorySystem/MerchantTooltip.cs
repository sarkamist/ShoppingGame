using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class MerchantTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string MerchantNameKey;
    public string MerchantDescriptionKey;

    private int tooltipRequestId;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TooltipManager.Instance == null) return;

        string name = LocalizationManager.Instance.Localize(MerchantNameKey);
        string description = LocalizationManager.Instance.LocalizeWithFormat(
            MerchantDescriptionKey,
            new object[] {
                ShopManager.Instance.BuyCostModifier.ToString("P0"),
                ShopManager.Instance.SellCostModifier.ToString("P0")
            }
        );
        tooltipRequestId = TooltipManager.Instance.Show(name, description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.Instance == null) return;

        TooltipManager.Instance.Hide(tooltipRequestId);
    }
}
