using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOverUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        Button ButtonRestart = root.Q<Button>("Restart-Btn");
        ButtonRestart.clicked += Restart; 


        Button ButtonMainMenu = root.Q<Button>("MainMenu-Btn");
        ButtonMainMenu.clicked += MainMenu;
    }

    private void Restart()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
