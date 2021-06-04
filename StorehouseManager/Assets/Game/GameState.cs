using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public TextMeshProUGUI CurrencyText;
    public TextMeshProUGUI TimeText;
    public int Score = 0;
    public int Currency = 0;

    // Start is called before the first frame update
    void Start()
    {
        CurrencyText.text = "666666";
        TimeText.text = "60:00";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
