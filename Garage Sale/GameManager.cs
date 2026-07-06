using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    MainMenu,
    Paused,
    Organizing,
    Selling,
    LevelComplete,
    LevelFailed
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    // ** SETTINGS FIELDS **
    [SerializeField]
    public bool hasTutorial;

    // *** LEVEL EVENT ***
    public static event Action OnLevelLoaded;

    // *** UI PANELS ***
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject organizingPanel;
    [SerializeField] private GameObject sellingPanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject levelFailedPanel;
    [SerializeField] private GameObject winPanel;


    public GameObject SellingPanel { get { return sellingPanel; } }

    // *** LEVEL DATA ***
    [SerializeField] private List<LevelData> levels;
    public int CurrentLvlIndex { get; set; }
    public LevelData CurrentLevel { get; private set; }

    // *** CAMERA ***
    [SerializeField] private Camera mainCamera;

    // *** MANAGERS ***
    [SerializeField] private MouseManager mouseManager;
    public MouseManager MouseManager { get { return mouseManager; } }

    [SerializeField] private TutorialManager tutorialManager;
    public TutorialManager TutorialManager { get { return tutorialManager; } }

    // *** GAME STATES ***
    // other scripts can see what state the game is in
    // use UpdateGameState to change it
    public GameState CurrentState { get; private set; }
    public GameState PreviousState { get; private set; }

    private CustomerManager customerManager;

    // *** GAME FIELDS ***
    private float currentMoney;
    private int customersLeft;
    private int[] currentItemNums;
    private List<Item> itemsInScene;
    private List<Item> startingItems;

    public float Money { get { return currentMoney; } set { currentMoney = value; } }
    public int CustomersLeft { get { return customersLeft; } set { customersLeft = value; } }
    public int[] CurrentItemNums { get { return currentItemNums; } set { currentItemNums = value; } }

    public List<Item> ItemsInScene { get { return itemsInScene; } }

    /// <summary>
    /// Initializes GameManager as persistent Singleton
    /// Marked as DontDestroyOnLoad to retain global player data (money, item backups) across scene transitions
    /// </summary>
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        itemsInScene = new List<Item>();
        startingItems = new List<Item>();

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        CurrentLvlIndex = 0;
        mainCamera.transform.rotation = Quaternion.Euler(30, 45, 0);
    }

    /// <summary>
    /// Evaluates current game state (win/lose)
    /// </summary>
    private void Update()
    {
        if (CurrentState == GameState.Selling)
        {
            if (customerManager.customers.Count <= 0 && customersLeft <= 0)
            {
                if (currentMoney >= CurrentLevel.MoneyGoal)
                {
                    UpdateGameState(GameState.LevelComplete);
                }
                else
                {
                    UpdateGameState(GameState.LevelFailed);
                }
            }
            else if (itemsInScene.Count <= 0 && currentMoney < CurrentLevel.MoneyGoal)
            {
                UpdateGameState(GameState.LevelFailed);
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoad;
    }

    private void HandleSceneLoad(Scene scene, LoadSceneMode mode)
    {
        customerManager = FindObjectOfType<CustomerManager>();
        mainCamera = Camera.main;
        mouseManager.SetCamera(mainCamera);
        mainMenuPanel = FindSceneComponent(scene, "Main Menu Panel");
        pausePanel = FindSceneComponent(scene, "Pause Menu Panel");
        organizingPanel = FindSceneComponent(scene, "Organizing Panel");
        sellingPanel = FindSceneComponent(scene, "Selling Panel");
        levelFailedPanel = FindSceneComponent(scene, "Level Failed Panel");
        levelCompletePanel = FindSceneComponent(scene, "Level Complete Panel");
        winPanel = FindSceneComponent(scene, "Win Panel");
        tutorialManager = FindSceneComponent(scene, "UI").GetComponent<TutorialManager>();
        if (mainMenuPanel != null)
        {
            UpdateGameState(GameState.MainMenu);
        }
        else
        {
            UpdateGameState(GameState.Organizing);
            if(tutorialManager!= null)
            {
                UpdateGameState(GameState.Paused);
                pausePanel.SetActive(false);
                tutorialManager.NextPanel(0);
            }
        }

        // Hook Up Buttons
        GameObject levelFailPress = null, levelSucceedPress = null, quitPanel = null;
        Button levelFailPressButton = null, levelSucceedPressButton = null, quitButton = null;
        levelFailPress = FindSceneComponent(scene, "Level Select Button Fail");
        levelSucceedPress = FindSceneComponent(scene, "Level Select Button");
        quitPanel = FindSceneComponent(scene, "Quit");

        if (levelFailPress != null) levelFailPressButton = levelFailPress.GetComponent<Button>();
        if (levelSucceedPress != null) levelSucceedPressButton = levelSucceedPress.GetComponent<Button>();
        if (quitPanel != null) quitButton = quitPanel.GetComponent<Button>();

        if(levelFailPressButton != null) levelFailPressButton.onClick.AddListener(OnLvlSelectPress);
        if (levelSucceedPressButton != null) levelSucceedPressButton.onClick.AddListener(OnLvlSelectPress);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);     
    }

    /// <summary>
    /// Searches for a game object within a scene for its reference by name
    /// </summary>
    /// <param name="scene">The scene being searched</param>
    /// <param name="name">The name of the game object</param>
    /// <returns></returns>
    public GameObject FindSceneComponent(Scene scene, String name)
    {
        List<GameObject> rootObjects = new List<GameObject>();
        scene.GetRootGameObjects(rootObjects);

        foreach (GameObject rootObj in rootObjects)
        {
            if (rootObj.name == name)
            {
                return rootObj;
            }

            Transform foundTransform = FindInChildren(rootObj.transform, name);
            if (foundTransform != null)
            {
                return foundTransform.gameObject;
            }
        }

        return null;
    }

    /// <summary>
    /// Searched a game objects children for a gameobjet by name
    /// </summary>
    /// <param name="parent">The object</param>
    /// <param name="name">The name of the object being searched for</param>
    /// <returns></returns>
    private Transform FindInChildren(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
            Transform found = FindInChildren(child, name);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    /// <summary>
    /// Initializes level data
    /// Instantiates hidden backup pool of starting items
    /// Allows level to be reset without reloading scene
    /// </summary>
    private void LoadLvl(int lvlIndex)
    {
        if (lvlIndex < 0 || lvlIndex >= levels.Count)
        {
            Debug.LogError("Level index out of range!");
            return;
        }

        // If change levels, clean up the backup
        if (PreviousState == GameState.LevelComplete)
        {
            foreach (Item item in startingItems)
            {
                if (item != null) Destroy(item.gameObject);
            }
            startingItems.Clear();
        }

        // create a backup if we don't have one already
        if (startingItems.Count == 0 && itemsInScene != null)
        {
            foreach (Item item in itemsInScene)
            {
                if(item != null)
                {
                    Item newItem = Instantiate(item);
                    newItem.transform.position = item.transform.position;
                    newItem.transform.rotation = item.transform.rotation;
                    newItem.gameObject.SetActive(false);
                    startingItems.Add(newItem);
                }
                
            }
        }

        CurrentLvlIndex = lvlIndex;
        CurrentLevel = levels[CurrentLvlIndex];

        currentMoney = 0;
        customersLeft = CurrentLevel.CustomerNum;
        currentItemNums = new int[CurrentLevel.MaxItemNums.Length];
        for (int i = 0; i < CurrentLevel.MaxItemNums.Length; i++)
        {
            currentItemNums[i] = CurrentLevel.MaxItemNums[i];
        }
        OnLevelLoaded?.Invoke();
    }

    public void AddToItemList(Item itemToAdd)
    {
        itemsInScene.Add(itemToAdd);
    }

    /// <summary>
    /// Removes existing items from the scene
    /// Instantiates persisting items at original positions and rotations
    /// </summary>
    public void LoadStartingItems()
    {
        if (itemsInScene != null)
        {
            foreach (Item item in itemsInScene)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            itemsInScene.Clear();
        }

        foreach (Item backupItem in startingItems)
        {
            if (backupItem != null)
            {
                Item newItem = Instantiate(backupItem);
                newItem.gameObject.SetActive(true);
                newItem.transform.position = backupItem.transform.position;
                newItem.transform.rotation = backupItem.transform.rotation;
                itemsInScene.Add(newItem);
            }
        }
    }

    /// <summary>
    /// Centralized state machine router. Ensures that previous states are accurately cached 
    /// (unless paused) and triggers the appropriate UI and camera setup for the incoming state.
    /// </summary>
    public void UpdateGameState(GameState newState)
    {
        if (CurrentState != GameState.Paused)
        {
            PreviousState = CurrentState;
        }

        if (CurrentState != newState)
        {
            CurrentState = newState;
            switch (newState)
            {
                case GameState.MainMenu:
                    HandleMainMenu();
                    break;
                case GameState.Paused:
                    HandlePaused();
                    break;
                case GameState.Organizing:
                    HandleOrganizing();
                    break;
                case GameState.Selling:
                    HandleSelling();
                    break;
                case GameState.LevelComplete:
                    HandleLevelComplete();
                    break;
                case GameState.LevelFailed:
                    HandleLevelFailed();
                    break;
            }
        }
    }

    /// <summary>
    /// Pauses game in appropriate game state
    /// </summary>
    /// <param name="context"></param>
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (CurrentState == GameState.Organizing || CurrentState == GameState.Selling)
            {
                Time.timeScale = 0f;
                HandlePaused();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    /// <summary>
    /// When game is resumed, restore appropriate UI panel
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        
        switch (CurrentState)
        {
            case GameState.Organizing:
                organizingPanel.SetActive(true);
                break;
            case GameState.Selling:
                sellingPanel.SetActive(true);
                break;
            case GameState.MainMenu:
                mainMenuPanel.SetActive(true);
                break;
            default:
                break;
        }
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        if (levelFailedPanel != null) levelFailedPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game.");
        Application.Quit();
    }

    /// <summary>
    /// Displays main menu
    /// Sets current level back to 0
    /// </summary>
    private void HandleMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        pausePanel.SetActive(false);
        organizingPanel.SetActive(false);
        sellingPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        if (levelFailedPanel != null) levelFailedPanel.SetActive(false);

        CurrentLvlIndex = 0;
        SceneManager.LoadScene(sceneName: "MainMenu");

        Debug.Log("Game State = Main Menu");
    }

    /// <summary>
    /// Display pause panel
    /// </summary>
    private void HandlePaused()
    {
        pausePanel.SetActive(true);

        organizingPanel.SetActive(false);
        sellingPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        if (levelFailedPanel != null) levelFailedPanel.SetActive(false);

        Debug.Log("Game State = Paused");
    }

    /// <summary>
    /// Display organizing UI panel
    /// Zoom out and move camera to fit scene
    /// </summary>
    private void HandleOrganizing()
    {
        LoadLvl(CurrentLvlIndex);

        organizingPanel.SetActive(true);
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        pausePanel.SetActive(false);
        sellingPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        if (levelFailedPanel != null) levelFailedPanel.SetActive(false);

        mainCamera.transform.position = new Vector3(-1.85f, 8, -9.41f);
        mainCamera.orthographicSize = 9;

        Debug.Log("Game State = Organizing");
    }

    /// <summary>
    /// Display selling UI panel
    /// Zoom in and center camera to fit scene
    /// </summary>
    private void HandleSelling()
    {
        sellingPanel.SetActive(true);

        organizingPanel.SetActive(false);
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        pausePanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        if (levelFailedPanel != null) levelFailedPanel.SetActive(false);

        mainCamera.transform.position = new Vector3(-5.62f, 8, -5.64f);
        mainCamera.orthographicSize = 7.5f;

        Debug.Log("Game State = Selling");
    }

    /// <summary>
    /// Display win panel when player reaches the end
    /// Display level complete panel when player completes level
    /// </summary>
    private void HandleLevelComplete()
    {
        if (CurrentLvlIndex >= levels.Count - 1)
        {
            winPanel.SetActive(true);
        }
        else
        {
            levelCompletePanel.SetActive(true);
        }

        sellingPanel.SetActive(false);
        organizingPanel.SetActive(false);
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
        pausePanel.SetActive(false);
        levelFailedPanel.SetActive(false);

        Debug.Log("Game State = Level Complete");
    }

    /// <summary>
    /// Display level failed UI panel
    /// </summary>
    private void HandleLevelFailed()
    {
        levelFailedPanel.SetActive(true);

        sellingPanel.SetActive(false);
        organizingPanel.SetActive(false);
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
        pausePanel.SetActive(false);
        levelCompletePanel.SetActive(false);

        Debug.Log("Game State = Level Failed");
    }

    /// <summary>
    /// Scene Loading persistent methods
    /// </summary>
    public void OnLvlSelectPress()
    {
        SceneManager.activeSceneChanged += OnLvlSelectLoad;
        instance.CurrentLvlIndex = 0;
        SceneManager.LoadScene(sceneName: "MainMenu");
        Debug.Log(SceneManager.GetActiveScene().name);
    }

    public void OnLvlSelectLoad(Scene scene, Scene next)
    {
        GameObject mainMenuPanel = FindSceneComponent(SceneManager.GetActiveScene(), "Main Menu Panel");
        GameObject levelSelectPanel = FindSceneComponent(SceneManager.GetActiveScene(), "Levels");
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        SceneManager.activeSceneChanged -= OnLvlSelectLoad;
    }
}
