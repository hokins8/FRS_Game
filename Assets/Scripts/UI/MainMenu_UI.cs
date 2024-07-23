using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class MainMenu_UI : MonoBehaviour
{
    [Header("Main Menu Panels")]
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject loadPanel;
    [SerializeField] GameObject savePanel;
    [Space]
    [Header("Camera")]
    [SerializeField] Camera mainMenuCamera;

    public void StartPlay()
    {
        Addressables.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        ShowAllPanels(false);
        mainMenuCamera.gameObject.SetActive(false);
    }

    // WIP -> Load & Save

    public void ExitGame()
    {
        Application.Quit();
    }

    private void ShowAllPanels(bool show)
    {
        mainPanel.SetActive(show);
        loadPanel.SetActive(show);
        savePanel.SetActive(show);
    }
}
