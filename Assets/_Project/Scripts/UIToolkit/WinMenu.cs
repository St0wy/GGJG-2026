using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class WinMenu : MonoBehaviour
{
    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        Button ButtonMainMenu = root.Q<Button>("MainMenuBtn");
        ButtonMainMenu.clicked += MainMenu;


        Button ButtonExit = root.Q<Button>("ExitBtn");
        ButtonExit.clicked += Exit;
    }

    private void Exit()
    {
        Application.Quit();
    }

    private void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
