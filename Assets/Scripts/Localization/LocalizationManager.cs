using System;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    [Serializable]
    public class LocaleJson
    {
        public Data data;
        public List<Entry> entries;
    }

    [Serializable]
    public class Data
    {
        public string code;
        public string name;
    }

    [Serializable]
    public class Entry
    {
        public string key;
        public string value;
    }

    [Serializable]
    public class LocaleAsset
    {
        public TextAsset json;
    }

    [Serializable]
    public struct Token
    {
        public string key;
        public string value;
    }

    public static LocalizationManager Instance { get; private set; }

    [Header("Locales Configuration")]
    public string FallbackLanguageCode = "en-GB";
    public string CurrentLanguageCode = "en-GB";
    public List<LocaleAsset> Locales;
    public List<Token> Tokens;

    private readonly List<(string code, string name)> languages = new List<(string code, string name)>();
    private readonly Dictionary<string, Dictionary<string, string>> tablesByCode = new Dictionary<string, Dictionary<string, string>>();
    private Dictionary<string, string> tokenMap = new Dictionary<string, string>();
    private Dictionary<string, string> currentTable = new Dictionary<string, string>();
    

    public IReadOnlyList<(string code, string name)> Languages => languages;

    public event Action OnLanguageChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllLocales();
        BuildTokenMap();
        var saved = PlayerPrefs.GetString("lang", CurrentLanguageCode);
        if (!SetLanguage(saved, notify: false))
        {
            SetLanguage(FallbackLanguageCode, notify: false);
        }
    }

    void Start()
    {
        OnLanguageChanged?.Invoke();
    }

    public bool SetLanguage(string code, bool notify = true)
    {
        if (!tablesByCode.TryGetValue(code, out var table))
        {
            Debug.LogWarning($"Localization: Unknown language code '{code}'.");
            return false;
        }

        CurrentLanguageCode = code;
        currentTable = table;

        PlayerPrefs.SetString("lang", code);
        PlayerPrefs.Save();

        if (notify) OnLanguageChanged?.Invoke();
        return true;
    }

    public string Localize(string key)
    {
        if (string.IsNullOrEmpty(key)) return "";

        string raw;

        if (currentTable != null && currentTable.TryGetValue(key, out var v))
            raw = v;
        else if (!string.IsNullOrEmpty(FallbackLanguageCode) &&
                 tablesByCode.TryGetValue(FallbackLanguageCode, out var fb) &&
                 fb.TryGetValue(key, out var fv))
            raw = fv;
        else
            raw = $"#{key}";

        return ApplyTokens(raw);
    }

    public string LocalizeWithFormat(string key, params object[] args)
    {
        var raw = Localize(key);
        try { return string.Format(raw, args); }
        catch { return raw; }
    }

    private void LoadAllLocales()
    {
        languages.Clear();
        tablesByCode.Clear();

        foreach (var loc in Locales)
        {
            if (loc?.json == null) continue;

            LocaleJson parsed;
            try
            {
                parsed = JsonUtility.FromJson<LocaleJson>(loc.json.text);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Localization: Failed to parse {loc.json.name} (exception): {e.Message}");
                continue;
            }

            if (parsed == null || parsed.data == null ||
                string.IsNullOrEmpty(parsed.data.code) ||
                string.IsNullOrEmpty(parsed.data.name))
            {
                Debug.LogWarning($"Localization: Invalid locale file '{loc.json.name}' (missing meta).");
                continue;
            }

            if (parsed.entries == null)
            {
                Debug.LogWarning($"Localization: Locale '{parsed.data.code}' has no entries list.");
                continue;
            }

            var table = new Dictionary<string, string>(parsed.entries.Count);
            foreach (var e in parsed.entries)
            {
                if (e == null || string.IsNullOrEmpty(e.key)) continue;
                table[e.key] = e.value ?? "";
            }

            tablesByCode[parsed.data.code] = table;
            languages.Add((parsed.data.code, parsed.data.name));
        }

        if (languages.Count == 0)
        {
            Debug.LogError("Localization: No locale JSONs assigned/loaded. Assign them in LocalizationManager inspector.");
        }
    }

    private void BuildTokenMap()
    {
        tokenMap = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var t in Tokens)
        {
            if (string.IsNullOrWhiteSpace(t.key)) continue;
            tokenMap[t.key.Trim()] = t.value ?? "";
        }
    }

    private string ApplyTokens(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        if (tokenMap == null) BuildTokenMap();
        if (tokenMap.Count == 0) return input;

        foreach (var kv in tokenMap)
        {
            string token = "{t:" + kv.Key + "}";
            input = input.Replace(token, kv.Value);
        }

        return input;
    }
}