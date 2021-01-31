using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // This loads scenes using the buttons in the main menu (canvas)

    public void Load_tutorial(string level)
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void Load_randomGame(string level)
    {
        SceneManager.LoadScene("RandomMapSelector");
    }
    public void Load_mapsLoader(string level)
    {
        SceneManager.LoadScene("TwitterMapSelector");
    }
    public void Load_credits(string level)
    {
        SceneManager.LoadScene("Credits");
    }
    public void Load_editor(string level)
    {
        SceneManager.LoadScene("EditorMode");
    }
    public void Load_mainMenu(string level)
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
