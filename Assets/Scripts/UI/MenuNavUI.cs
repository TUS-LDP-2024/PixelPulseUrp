using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavUI : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        // Unlock mouse for menus
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SweetTooth");
        // Lock mouse for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SettingsMenu()
    {
        SceneManager.LoadScene("SettingsMenu");
        // Unlock mouse for menus
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EndGame()
    {
        Application.Quit();
    }

    // Add this to handle cases where scene is loaded directly
    private void OnEnable()
    {
        // Check current scene name and set cursor accordingly
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "SweetTooth") // Gameplay scene
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else // Any other scene (menus)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}