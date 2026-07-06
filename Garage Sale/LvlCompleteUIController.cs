using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LvlCompleteUIController : MonoBehaviour
{

    public void OnNextLvlPress()
    {
        GameManager.instance.CurrentLvlIndex++;
        CustomerManager manager = FindObjectOfType<CustomerManager>();
        manager.nextSpawnTime = 0f;
        GameManager.instance.UpdateGameState(GameState.Organizing);
    }

    public void OnMainMenuPress()
    {
        GameManager.instance.UpdateGameState(GameState.MainMenu);
    }

    public void OnRetryPress()
    {
        GameManager.instance.LoadStartingItems();
        GameManager.instance.UpdateGameState(GameState.Organizing);
    }

    public void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
    }
}
