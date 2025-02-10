using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuNavUI : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void StartGame()
    {
        SceneManager.LoadScene("SweetTooth");
    }

    // Update is called once per frame
    public void SettingsMenu()
    {
        SceneManager.LoadScene("SettingsMenu");
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
