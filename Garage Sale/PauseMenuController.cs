using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public void OnResumePress()
    {
        GameManager.instance.ResumeGame();
    }

    public void OnQuitPress()
    {
        GameManager.instance.QuitGame();
    }
}
