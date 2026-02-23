using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageDropdown : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    void Start()
    {
        if (!dropdown) dropdown = GetComponent<TMP_Dropdown>();

        var lm = LocalizationManager.Instance;
        dropdown.ClearOptions();

        var options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < lm.Languages.Count; i++)
        {
            options.Add(lm.Languages[i].name);
            if (lm.Languages[i].code == lm.Data.CurrentLanguageCode)
                currentIndex = i;
        }

        dropdown.AddOptions(options);
        dropdown.SetValueWithoutNotify(currentIndex);

        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener(i =>
        {
            lm.SetLanguage(lm.Languages[i].code);
        });
    }
}
