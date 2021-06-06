using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public TextMeshProUGUI CurrencyText;
    public TextMeshProUGUI TimeText;

    public long LevelTimeInSeconds = 600;
    public long CurrencyPassiveGainPerSecond = 1;
    public long CurrencyBase = 100;

    private float LastLevelTime;
    private float CurrentLevelTime;
    public long CurrentCurrency;
    public long CurrentScore;
    // Start is called before the first frame update
    void Start()
    {
        CurrentLevelTime = LevelTimeInSeconds;
        CurrentCurrency = CurrencyBase;
    }

    // Update is called once per frame
    void Update()
    {
        LastLevelTime = CurrentLevelTime;
        CurrentLevelTime -= Time.deltaTime;
        if (CurrentLevelTime < 0)
            CurrentLevelTime = 0;
        TimeText.text = $"{((long)(CurrentLevelTime / 60)).ToString("00")}" +
                        $":{((long)(CurrentLevelTime % 60)).ToString("00")}";
        if ((long) (LastLevelTime % 60) != (long) (CurrentLevelTime % 60))
            CurrentCurrency += CurrencyPassiveGainPerSecond;
        CurrencyText.text = $"{CurrentCurrency}";
    }

    public void OnFailCondition()
    {
        CurrentCurrency -= 999;
    }
    public void OnSuccessCondition()
    {
        CurrentCurrency += 999;
    }

    public void OnItemStore()
    {
        CurrentCurrency *= 2;
    }
}
