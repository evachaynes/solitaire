using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class MainMenu : MonoBehaviour
{
    public void loadNextScene(string sceneName)
    {
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

    public void CloseMenu()
    {
        GameObject exitMenuUI = GameObject.Find("ExitMenu");
        exitMenuUI.SetActive(false);
    }
}
