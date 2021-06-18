using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExpansionDefinition : MonoBehaviour
{
    public TextMeshProUGUI ExpansionName;
    public Button ExpansionButton;
    public TextMeshProUGUI ExpansionPrice;
    public IExpansion CurrentExpansion;
    public UnityEvent OnExpansionBuy;

    void Awake()
    {
        ExpansionButton.onClick.AddListener(OnClick);   
    }

    internal void SetExpansion(IExpansion expansion)
    {
        CurrentExpansion = expansion;

        ExpansionName.text = CurrentExpansion.Name;
        ExpansionPrice.text = $"{CurrentExpansion.Cost} coins";
    }

    internal void OnClick()
    {
        if (!CurrentExpansion.IsCompleted)
        {
            CurrentExpansion.OnBuy();
            OnExpansionBuy?.Invoke();
        }
    }
}
