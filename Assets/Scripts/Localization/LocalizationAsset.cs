using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Localization Asset", menuName = "Localization/Localization Asset")]
public class LocalizationAsset : ScriptableObject
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

    [Header("Locales Configuration")]
    public string FallbackLanguageCode = "en-GB";
    public string CurrentLanguageCode = "en-GB";
    public List<LocaleAsset> Locales;
    public List<Token> Tokens;

    [Header("Global Localization Keys")]
    public string ValueLineKey;
    public string JunkInfoTrueKey;
    public string JunkInfoFalseKey;
}
