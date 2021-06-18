using System;
using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    // Game Reference
    public Game GameData;
    private float LastTime = 0;
    internal UnityEvent OnGameTimerRunnedOut = new UnityEvent();
    internal UnityEvent OnGameCompleted = new UnityEvent();
    internal UnityEvent OnTimeMilestone = new UnityEvent();

    // Reputation
    public int CurrentReputationLevel { get; set; } = 0;
    public int CurrentReputation { get; set; } = 0;
    public int ReputationBase = 0;

    // Currency
    public int BasePassiveCurrencyGainPerReputationLevel = 1;
    public int CurrentCurrency { get; set; } = 0;
    public int CurrencyBase = 100;

    // Time
    public long BaseLevelTimeInSeconds = 600;
    public float CurrentTime { get; private set; } = 0;
    public float CurrentTimePassed { get; private set; } = 0;

    // Start is called before the first frame update
    public void Start()
    {
        LastTime = Time.time;
        CurrentReputation = 0;
        CurrentCurrency = CurrencyBase;
        CurrentTime = BaseLevelTimeInSeconds;
        CurrentTimePassed = 0;
    }

    // Update is called once per frame
    public void Update()
    {
        UpdateTime();
        UpdateCurrency();
        UpdateReputation();
        CheckEndConditions();

        var diff = Math.Abs(LastTime - Time.time);
        if (diff < 1)
        {
            return;
        }
        LastTime = Time.time - 1 + diff;
        PeriodicUpdateCurrency();
    }

    void UpdateTime()
    {
        CurrentTimePassed += Time.deltaTime;
        CurrentTime -= Time.deltaTime;
        if (CurrentTime < 0)
            CurrentTime = 0;
        if (CurrentTimePassed > 60)
        {
            CurrentTimePassed = 0;
            OnTimeMilestone?.Invoke();
        }
        GameData.Banner.SetTime((int)CurrentTime);
    }

    void UpdateCurrency()
    {
        GameData.Banner.SetCurrency(CurrentCurrency);
    }

    void UpdateReputation()
    {
        GameData.Banner.SetReputation(CurrentReputation);
        CurrentReputationLevel = GameData.Banner.ReputationLevel;
    }

    void PeriodicUpdateCurrency()
    {
        CurrentCurrency += 
            BasePassiveCurrencyGainPerReputationLevel
            *
            GameData.Banner.ReputationLevel;
    }

    public void CheckEndConditions()
    {
        CheckWin();
        AttemptToEndGameByTimer();
    }

    public void AttemptToEndGameByTimer()
    {
        if (CurrentTime == 0)
            OnGameTimerRunnedOut.Invoke();
    }

    public void CheckWin()
    {
        if (CurrentReputationLevel == GameData.Banner.MaxLevel)
            OnGameCompleted?.Invoke();
    }

    public void ReputationChange(int reputationChange)
    {
        CurrentReputation += reputationChange;
    }

    public void CurrencyChange(int currencyChange)
    {
        CurrentCurrency += currencyChange;
    }

    public bool TryReduceCurrency(float expectedSum)
    {
        if (CurrentCurrency >= expectedSum)
        {
            CurrentCurrency -= (int)expectedSum;
            return true;
        }

        return false;
    }
}
