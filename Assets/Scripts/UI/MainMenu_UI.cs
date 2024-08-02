using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
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

    public static MainMenu_UI Instance;

    private AsyncOperationHandle<SceneInstance> loadingScene;

    void Awake()
    {
        Instance = this;
    }

    public void StartPlay()
    {
        loadingScene = Addressables.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
        Addressables.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        ShowAllPanels(false);
        mainMenuCamera.gameObject.SetActive(false);
    }

    public void UnloadLoadingScene()
    {
        if (loadingScene.IsDone)
            Addressables.UnloadSceneAsync(loadingScene);
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
