using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        Button ButtonPlay = root.Q<Button>("Play-Btn");
        ButtonPlay.clicked += Play;

        //Button AboutPlay = root.Q<Button>("About-Btn");
        ////AboutPlay.clicked += About;

        Button ExitPlay = root.Q<Button>("Exit-Btn");
        ExitPlay.clicked += Exit;

    }

    private void Play()
    {
        SceneManager.LoadScene("Level1");
    }

    //private void About()
    //{
    //    SceneManager.LoadScene("AboutScene");
    //}

    private void Exit()
    {
        Application.Quit();
    }

}
