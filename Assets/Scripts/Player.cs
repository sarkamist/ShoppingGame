using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IConsume
{
    public float Damage = 10.0f;
    public float MaxHealth = 100.0f;
    public float CurrentHealth = 100.0f;

    private Image healthBarForeground;
    private TextMeshProUGUI textHealth;
    private float maxWidth;

    private void Start()
    {
        if (healthBarForeground == null)
        {
            healthBarForeground = transform.Find("Foreground").GetComponent<Image>();
            maxWidth = healthBarForeground.rectTransform.rect.width;
            textHealth = transform.Find("TextHealth").GetComponent<TextMeshProUGUI>();
        }
        

        UpdateHealthBar();
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        float ratio = (float) CurrentHealth / MaxHealth;
        RectTransform rt = healthBarForeground.rectTransform;

        rt.sizeDelta = new Vector2(maxWidth * ratio, rt.sizeDelta.y);
        textHealth.text = $"{CurrentHealth} / {MaxHealth}";
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
