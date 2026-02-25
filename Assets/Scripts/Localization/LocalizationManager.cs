using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    public static LocalizationFormatProvider FormatProvider = new();

    public LocalizationAsset Data;

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
        if (!SetLanguage(Data.CurrentLanguageCode, notify: false))
        {
            SetLanguage(Data.FallbackLanguageCode, notify: false);
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

        Data.CurrentLanguageCode = code;
        currentTable = table;

        if (notify) OnLanguageChanged?.Invoke();
        return true;
    }

    public string Localize(string key)
    {
        if (string.IsNullOrEmpty(key)) return "";

        string raw;

        if (currentTable != null && currentTable.TryGetValue(key, out var v))
            raw = v;
        else if (!string.IsNullOrEmpty(Data.FallbackLanguageCode) &&
                 tablesByCode.TryGetValue(Data.FallbackLanguageCode, out var fb) &&
                 fb.TryGetValue(key, out var fv))
            raw = fv;
        else
            raw = $"#{key}";

        return ApplyTokens(raw);
    }

    public string LocalizeWithFormat(string key, params object[] args)
    {
        var raw = Localize(key);
        try { return string.Format(FormatProvider, raw, args); }
        catch { return raw; }
    }

    private void LoadAllLocales()
    {
        languages.Clear();
        tablesByCode.Clear();

        foreach (var loc in Data.Locales)
        {
            if (loc?.json == null) continue;

            LocalizationAsset.LocaleJson parsed;
            try
            {
                parsed = JsonUtility.FromJson<LocalizationAsset.LocaleJson>(loc.json.text);
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
        foreach (var t in Data.LocalizationTokens)
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

public sealed class LocalizationFormatProvider : IFormatProvider, ICustomFormatter
{
    private readonly IFormatProvider fallbackFormatProvider;

    public LocalizationFormatProvider(IFormatProvider inner = null)
    {
        fallbackFormatProvider = inner ?? CultureInfo.CurrentCulture;
    }

    public object GetFormat(Type formatType)
    {
        return formatType == typeof(ICustomFormatter) ? this : fallbackFormatProvider.GetFormat(formatType);
    }

    public string Format(string format, object arg, IFormatProvider formatProvider)
    {
        if (arg == null) return string.Empty;

        if (string.IsNullOrEmpty(format)) return FormatFallback(null, arg);

        var parts = format.Split(':');

        bool applyAbs = false;
        string nativeFormat = null;

        for (int i = 0; i < parts.Length; i++)
        {
            var part = parts[i].Trim();

            if (part.Equals("ABS", StringComparison.OrdinalIgnoreCase))
            {
                applyAbs = true;
                continue;
            }

            if (!string.IsNullOrEmpty(part) && nativeFormat == null)
                nativeFormat = part;
        }

        if (applyAbs)
            arg = ApplyAbs(arg);

        return FormatFallback(nativeFormat, arg);
    }

    private object ApplyAbs(object arg)
    {
        return arg switch
        {
            sbyte v => (sbyte)Math.Abs(v),
            short v => (short)Math.Abs(v),
            int v => Math.Abs(v),
            long v => Math.Abs(v),

            float v => Math.Abs(v),
            double v => Math.Abs(v),
            decimal v => Math.Abs(v),

            _ => arg
        };
    }

    private string FormatFallback(string format, object arg)
    {
        if (arg is IFormattable f)
            return f.ToString(format, fallbackFormatProvider);

        return arg.ToString();
    }
}
