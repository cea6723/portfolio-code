using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private GameObject currentPanel;
    private GameObject previousPanel;

    [SerializeField]
    private GameObject mainMenuPanel;

    [SerializeField]
    private GameObject levelMenuPanel;

    [SerializeField]
    private GameObject settingsMenuPanel;
    // Start is called before the first frame update
    void Start()
    {
        currentPanel = mainMenuPanel;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnStartPress()
    {
        if(GameManager.instance.hasTutorial)
        {
            SceneManager.LoadScene(sceneName: "Tutorial");
        }
        else
        {
            SceneManager.LoadScene(sceneName: "Isometric Test");
        }
    }

    public void OnLevelsPress()
    {
        mainMenuPanel.SetActive(false);
        levelMenuPanel.SetActive(true);
        currentPanel = levelMenuPanel;
        previousPanel = mainMenuPanel;
    }

    public void OnBackPressed()
    {
        currentPanel.SetActive(false);
        previousPanel.SetActive(true);
    }

}
