using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BannerDefinition : MonoBehaviour
{
    public TextMeshProUGUI CurrencyValue;
    public TextMeshProUGUI ReputationValue;
    public TextMeshProUGUI TimeValue;
    public TextMeshProUGUI ReputationName;

    private const long timeStep = 60;
    public int ReputationLevel = 0;
    public int MaxLevel => ReputationLevels.Length - 1;
    public KeyValuePair<int, string>[] ReputationLevels =
    {
        new KeyValuePair<int, string>(1000, "Trash Gatherer"),
        new KeyValuePair<int, string>(2000, "Loot Gatherer"),
        new KeyValuePair<int, string>(3000, "Collector"),
        new KeyValuePair<int, string>(4000, "Professional Collector"),
        new KeyValuePair<int, string>(5000, "Hoarder"),
        new KeyValuePair<int, string>(7500, "Treasure Hoarder"),
        new KeyValuePair<int, string>(10000, "Curator"),
        new KeyValuePair<int, string>(15000, "Master Curator"),
        new KeyValuePair<int, string>(20000, "Loot Pinata"),
        new KeyValuePair<int, string>(25000, "Overflowing Loot Pinata"),
        new KeyValuePair<int, string>(int.MaxValue, "Loot, Treasure, Shinies"),
    };

    public void SetTime(int totalSeconds)
    {
        TimeValue.text = 
            $"{(totalSeconds / timeStep).ToString("00")}" +
            $":" +
            $"{(totalSeconds % timeStep).ToString("00")}";
    }
    
    public void SetCurrency(int currencyLeft)
    {
        CurrencyValue.text = $"{currencyLeft}";
    }
    
    public void SetReputation(int currentReputation)
    {
        var level = GetReputationDataForLevel(currentReputation);
        SetReputation(currentReputation, level.Key, level.Value);
    }

    public void SetReputation(int currentReputation, int maxReputation, string titleReputation)
    {
        ReputationName.text = $"{titleReputation}";
        if (maxReputation == int.MaxValue)
        {
            ReputationValue.text =
                $"{currentReputation}";
        }
        else
            ReputationValue.text =
                $"{currentReputation}" +
                $"/" +
                $"{maxReputation}";
    }

    public KeyValuePair<int, string> GetReputationDataForLevel(int currentReputation)
    {
        int i = 0;
        while (currentReputation >= ReputationLevels[i].Key
            && i < ReputationLevels.Length - 1)
        {
            ++i;
        }
        ReputationLevel = i;
        return ReputationLevels[i];
    }
}