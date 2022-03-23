using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class MainMenu : MonoBehaviour
{
    public void loadNextScene(string sceneName)
    {
        if (sceneName == "Menu")
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene(sceneName);
    }

    public void ToggleCredits()
    {
        TextMeshProUGUI creditsTextUI;
        creditsTextUI = GameObject.Find("Credits").GetComponent<TextMeshProUGUI>();
        if (creditsTextUI.enabled == false)
        {
            creditsTextUI.enabled = true;
        }
        else
        {
            creditsTextUI.enabled = false;
        }
    }
}
