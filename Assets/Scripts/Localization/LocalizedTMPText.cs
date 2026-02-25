using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedTMPText : MonoBehaviour
{
    public string Key;

    private TMP_Text tmpText;

    private void Awake()
    {
        if (tmpText == null) tmpText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += RefreshLocalizedText;
            RefreshLocalizedText();
        }
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= RefreshLocalizedText;
        }
    }

    public void RefreshLocalizedText()
    {
        tmpText.text = LocalizationManager.Instance.Localize(Key);
    }
}