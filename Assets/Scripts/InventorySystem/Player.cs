using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour, IConsume, IPointerClickHandler
{
    public Inventory Inventory;

    [Header("Health Parameters")]
    public float BaseDamage = 10.0f;
    public float MaxHealth = 100.0f;
    public float CurrentHealth = 100.0f;

    [Header("Animation Parameters")]
    public float FillSpeed = 50f;
    public float FlickerDuration = 0.125f;
    public int FlickerCount = 2;

    [Header("References")]
    public Image HealthBarForeground;
    public TextMeshProUGUI TextHealth;
    public TextMeshProUGUI TextDamage;

    private float maxWidth;
    private float totalDamage;
    private float displayedHealth;
    private float lastHealth;
    private Coroutine healthBarFlickerRoutine;

    private void Start()
    {
        maxWidth = HealthBarForeground.rectTransform.rect.width;
        displayedHealth = CurrentHealth;
        lastHealth = CurrentHealth;

        UpdateStats();
        UpdateHealthBar();
    }

    private void OnEnable()
    {
        Inventory.OnInventoryChange += UpdateStats;
    }

    private void OnDisable()
    {
        Inventory.OnInventoryChange -= UpdateStats;
    }

    private void Update()
    {
        if (!Mathf.Approximately(displayedHealth, CurrentHealth))
        {
            displayedHealth = Mathf.MoveTowards(displayedHealth, CurrentHealth, FillSpeed * Time.deltaTime);

            if (Mathf.Abs(displayedHealth - CurrentHealth) < 0.05f) displayedHealth = CurrentHealth;

            UpdateHealthBar();
        }

        if (!Mathf.Approximately(lastHealth, CurrentHealth))
        {
            lastHealth = CurrentHealth;

            StartHealthBarFlicker();
        }

        if (CurrentHealth <= 0f && displayedHealth <= 0.1f)
        {
            SceneChanger.Instance.OnHealthBarDepleted();
        }
    }

    public void UpdateHealthBar()
    {
        float ratio = (float) displayedHealth / MaxHealth;
        RectTransform rt = HealthBarForeground.rectTransform;

        rt.sizeDelta = new Vector2(maxWidth * ratio, rt.sizeDelta.y);
        TextHealth.text = $"{CurrentHealth} / {MaxHealth}";
    }

    public bool Use(BaseConsumableItem consumable)
    {
        Inventory playerInventory = ShopManager.Instance.PlayerInventoryUI.InventoryModel;
        if (consumable is Potion potion)
        {
            if (potion.JunkOnConsume)
            {
                if (playerInventory.CanHold(potion.JunkItem))
                {
                    playerInventory.AddItem(potion.JunkItem);
                }
                else
                {
                    ShopManager.Instance.PlayerInventoryUI.StartInventoryFlicker(ShopManager.Instance.ErrorFlickerColor);
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.Error);
                    return false;
                }
            }

            MaxHealth += potion.MaxHealthIncrease;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.DrinkPotion);
        }
        else
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.FoodEaten);
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth + consumable.CurrentHealthIncrease, 0f, MaxHealth);

        return true;
    }

    private void UpdateStats()
    {
        totalDamage = BaseDamage + Inventory.Slots.Sum((slot) => {
            if (slot.Item is Weapon weapon)
            {
                return weapon.DamageIncrease * slot.Amount;
            }
            else return 0;
        });

        TextDamage.text = $"{totalDamage}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth - totalDamage, 0f, MaxHealth);
        }
    }

    public void StartHealthBarFlicker()
    {
        if (healthBarFlickerRoutine != null) StopCoroutine(healthBarFlickerRoutine);
        healthBarFlickerRoutine = StartCoroutine(HealthBarFlicker());
    }

    private System.Collections.IEnumerator HealthBarFlicker()
    {
        Color c = HealthBarForeground.color;

        for (int i = 0; i < FlickerCount; i++)
        {
            HealthBarForeground.color = new Color(c.r, c.g, c.b, 0.25f);
            yield return new WaitForSeconds(FlickerDuration);

            HealthBarForeground.color = new Color(c.r, c.g, c.b, 1f); ;
            yield return new WaitForSeconds(FlickerDuration);
        }

        HealthBarForeground.color = c;
    }
}
