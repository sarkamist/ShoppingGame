using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, IConsume
{
    public Inventory PlayerInventory;
    public float Damage = 10.0f;
    public float MaxHealth = 100.0f;
    public float CurrentHealth = 100.0f;

    public Image HealthBarForeground;
    public TextMeshProUGUI TextHealthBar;

    private float maxWidth;

    private void Start()
    {
        maxWidth = HealthBarForeground.rectTransform.rect.width;

        UpdateHealthBar();
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        float ratio = (float) CurrentHealth / MaxHealth;
        RectTransform rt = HealthBarForeground.rectTransform;

        rt.sizeDelta = new Vector2(maxWidth * ratio, rt.sizeDelta.y);
        TextHealthBar.text = $"{CurrentHealth} / {MaxHealth}";
    }

    public void OnHealthBarClick()
    {
        CurrentHealth -= Damage;
    }

    public void Use(BaseConsumableItem consumable)
    {
    }

    public void ChangeHealth(float amount)
    {
        CurrentHealth = Mathf.Min(
            MaxHealth,
            Mathf.Max(0, CurrentHealth += amount)
        );
    }
}
