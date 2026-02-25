using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Player : MonoBehaviour,
    IConsume, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Inventory Inventory;

    [Header("Health Parameters")]
    public float BaseDamage = 10.0f;
    public float MaxHealth = 100.0f;
    public float CurrentHealth = 75.0f;

    [Header("Animation Parameters")]
    public float FillSpeed = 50f;
    public float FlickerDuration = 0.125f;
    public int FlickerCount = 2;
    public Color HealthBarFlickerColor = new Color(0.25f, 0.625f, 0.2f, 0.25f);
    public Color DamageCounterFlickerColor = new Color(0.85f, 0.075f, 0.25f, 1f);

    [Header("References")]
    public Image HealthBarBackground;
    public TextMeshProUGUI TextHealth;
    public TextMeshProUGUI TextDamage;

    private float maxWidth;
    private float totalDamage;
    private float displayedHealth;
    private float lastHealth;
    private Coroutine healthBarFlickerRoutine;
    private Coroutine textDamageFlickerRoutine;

    private void Start()
    {
        maxWidth = HealthBarBackground.rectTransform.rect.width;
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
        RectTransform rt = HealthBarBackground.rectTransform;

        rt.sizeDelta = new Vector2(maxWidth * ratio, rt.sizeDelta.y);
        TextHealth.text = $"{CurrentHealth} / {MaxHealth}";
    }

    public bool Use(BaseConsumableItem consumable)
    {
        Inventory playerInventory = ShopManager.Instance.PlayerInventoryUI.InventoryModel;
        if (consumable is Potion potion)
        {
            if (potion.BaubleOnConsume)
            {
                if (playerInventory.CanHold(potion.BaubleItem))
                {
                    playerInventory.AddItem(potion.BaubleItem);
                }
                else
                {
                    ShopManager.Instance.PlayerInventoryUI.StartInventoryFlicker(
                        ShopManager.Instance.ErrorFlickerColor
                    );
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

        CurrentHealth = Mathf.Clamp(
            CurrentHealth + consumable.CurrentHealthIncrease,
            0f, MaxHealth
        );

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
            StartDamageCounterFlicker();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.DamageReceived);

            CurrentHealth = Mathf.Clamp(CurrentHealth - totalDamage, 0f, MaxHealth);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.Instance.AddState(CursorManager.CursorState.Attack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.Instance.RemoveState(CursorManager.CursorState.Attack);
    }

    public void StartHealthBarFlicker()
    {
        if (healthBarFlickerRoutine != null) StopCoroutine(healthBarFlickerRoutine);
        healthBarFlickerRoutine = StartCoroutine(HealthBarFlickerRoutine());
    }

    private IEnumerator HealthBarFlickerRoutine()
    {
        Color c = HealthBarBackground.color;

        for (int i = 0; i < FlickerCount; i++)
        {
            HealthBarBackground.color = HealthBarFlickerColor;
            yield return new WaitForSeconds(FlickerDuration);

            HealthBarBackground.color = c;
            yield return new WaitForSeconds(FlickerDuration);
        }

        HealthBarBackground.color = c;
    }

    public void StartDamageCounterFlicker()
    {
        if (textDamageFlickerRoutine != null) StopCoroutine(textDamageFlickerRoutine);
        textDamageFlickerRoutine = StartCoroutine(DamageCounterFlickerRoutine());
    }

    private IEnumerator DamageCounterFlickerRoutine()
    {
        Image damageCounter = TextDamage.GetComponentInParent<Image>();
        Color c = damageCounter.color;

        for (int i = 0; i < FlickerCount; i++)
        {
            damageCounter.color = DamageCounterFlickerColor;
            yield return new WaitForSeconds(FlickerDuration);

            damageCounter.color = c;
            yield return new WaitForSeconds(FlickerDuration);
        }

        damageCounter.color = c;
    }
}
