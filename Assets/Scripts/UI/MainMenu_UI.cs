using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Tools.UnityUtilities;
using System;

public class MainMenu_UI : MonoBehaviour
{
    [Header("Main Menu Panels")]
    [SerializeField] GameObject mainPanel; 
    [SerializeField] GameObject playPanel;
    [SerializeField] GameObject loadPanel;

    [Space]
    [Header("Camera")]
    [SerializeField] Camera mainMenuCamera;

    [Space]
    [Header("Background")]
    [SerializeField] GameObject backgroundObjects;

    [Space]
    [Header("Play Panel")]
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button playButton;

    [Space]
    [Space]
    [Header("Load Panel")]
    [SerializeField] LoadElementUI loadElement_prefab;
    [SerializeField] Transform loadRoot;

    private readonly Pool<LoadElementUI> loadElementPool = new()
    {
        AutoExpand = true,
        ExpansionStep = 10,
    };
    
    private LoadElementUI[] pooled;

    public string SavesLocation => "Saves";
    [HideInInspector] public string CurrentFolder;

    public static MainMenu_UI Instance;

    private AsyncOperationHandle<SceneInstance> loadingScene;

    Vector3 wantedPos = Vector3.zero;
    Quaternion wantedRot = Quaternion.identity;

    void Awake()
    {
        Instance = this;
        CreatePool();
        mainPanel.SetActive(true);
        Directory.CreateDirectory(SavesLocation);
        playButton.interactable = false;
    }

    void Start()
    {
        SpawnLoadButtons();
    }

    private void CreatePool()
    {
        loadElementPool.SetFactory(() => Instantiate(loadElement_prefab, loadRoot));
        loadElementPool.OnFree = i => { i.gameObject.SetActive(false); };
        loadElementPool.OnUse = i => { i.gameObject.SetActive(true); };
        loadElementPool.CreateElements(10);
    }
    
    public void SpawnLoadButtons()
    {
        string[] saveFolders = Directory.GetDirectories(SavesLocation);
    
        loadElementPool.ReturnAll();
        pooled = loadElementPool.RecyclePool(saveFolders.Length);
    
        for (int i = 0; i < saveFolders.Length; i++)
        {
            string saveFolder = $"{saveFolders[i]}";
            string gameName = saveFolder.Substring(SavesLocation.Length + 1);
            Action loadGameAction = () => 
            {
                CurrentFolder = gameName;
                StartPlay();
            };
            LoadElementUI tmp = pooled[i];
            tmp.SetupButton(gameName, loadGameAction);
            tmp.transform.SetAsLastSibling();
        }
    }

    public void StartPlay()
    {
        loadingScene = Addressables.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
        Addressables.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        ShowAllPanels(false);
        mainMenuCamera.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Directory.CreateDirectory($"{SavesLocation}\\{CurrentFolder}");
    }

    public void OnGameNameChange(string name)
    {
        CurrentFolder = name;
        playButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void UnloadLoadingScene()
    {
        if (loadingScene.IsDone)
            Addressables.UnloadSceneAsync(loadingScene);
    }

    public void SetCameraWantedPos(Transform pos)
    {
        wantedPos = pos.position;
        wantedRot = pos.rotation;
    }

    void LateUpdate()
    {
        if (mainMenuCamera.gameObject.activeSelf && wantedPos != Vector3.zero && wantedRot != Quaternion.identity)
        {
            mainMenuCamera.transform.position = Vector3.Lerp(mainMenuCamera.transform.position, wantedPos, 2f * Time.deltaTime);
            mainMenuCamera.transform.rotation = Quaternion.Lerp(mainMenuCamera.transform.rotation, wantedRot, 2f * Time.deltaTime);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void ShowAllPanels(bool show)
    {
        backgroundObjects.SetActive(show);

        mainPanel.SetActive(show);
        loadPanel.SetActive(show);
        playPanel.SetActive(show);
    }
}
