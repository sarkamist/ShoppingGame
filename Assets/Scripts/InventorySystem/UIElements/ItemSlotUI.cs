using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // NOTE: Inventory UI slots support drag&drop,
    // implementing the Unity provided interfaces by events system

    public Image Image;
    public TextMeshProUGUI AmountText;

    private Canvas canvas;
    private Transform parent;
    private ItemBase item;
    private InventoryUI inventory;

    public void Initialize(ItemSlot slot, InventoryUI inventory)
    {
        Image.sprite = slot.Item.ImageUI;

        AmountText.text = slot.Amount.ToString();
        AmountText.enabled = (slot.Amount > 1);

        item = slot.Item;
        this.inventory = inventory;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // We need canvas as new UI reference (lazy initialization)
        if (!canvas) canvas = GetComponentInParent<Canvas>();

        // Store previous reference position
        parent = transform.parent;

        // Change parent of our item to the canvas
        transform.SetParent(canvas.transform, true);
        
        // And set it as last child to be rendered on top of UI
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Moving object around screen using mouse delta
        transform.localPosition += new Vector3(eventData.delta.x, eventData.delta.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Find scene objects colliding with mouse point on end dragging
        RaycastHit2D hitData = Physics2D.GetRayIntersection(
            Camera.main.ScreenPointToRay(Input.mousePosition));

        if (hitData)
        {
            Debug.Log("Drop over object: " + hitData.collider.gameObject.name);

            var consumer = hitData.collider.gameObject.GetComponent<IConsume>();

            if ((consumer != null) && (item is ConsumableItem))
            {
                (item as ConsumableItem).Use(consumer);
                inventory.UseItem(item);
            }
        }

        // Changing parent back to slot
        transform.SetParent(parent.transform);

        // And centering item position
        transform.localPosition = Vector3.zero;
    }
}
