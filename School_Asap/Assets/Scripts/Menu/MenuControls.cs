using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuControls : MonoBehaviour
{
    public Settings settings;

    private void Awake()
    {
        Cursor.visible = true;
    }

    public void PlayPressed()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitPressed()
    {
        Application.Quit();
    }

    public void Training()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void LeaderBoard()
    {
        SceneManager.LoadScene("HighscoreTable");
    }
}