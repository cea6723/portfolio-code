using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SellingUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI customerText;

    public void OnOrganizePress()
    {
        GameManager.instance.UpdateGameState(GameState.Organizing);
    }

    private void Update()
    {
        if (GameManager.instance.CurrentLevel != null)
        {
           moneyText.text = "$ " + GameManager.instance.Money + " / " + GameManager.instance.CurrentLevel.MoneyGoal;
        }
        customerText.text = "Customers Left: " + GameManager.instance.CustomersLeft;
    }
}
