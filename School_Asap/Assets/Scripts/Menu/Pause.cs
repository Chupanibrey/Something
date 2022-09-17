using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject ingameMenu;
    public GameObject deathMenu;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnPause();
            deathMenu.SetActive(false);
        }
    }

    public void OnPause()
    {
        Time.timeScale = 0;

        ingameMenu.SetActive(true);
    }

    public void OnResume()
    {
        Time.timeScale = 1f;
        ingameMenu.SetActive(false);
        if(HealthSystem.HealphCount <= 0)
            deathMenu.SetActive(true);
    }

    public void OnRestart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        Time.timeScale = 1f;
    }

    public void ExitMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main_menu");
        Time.timeScale = 1f;
    }
}