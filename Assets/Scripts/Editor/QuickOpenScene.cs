#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class QuickOpenScene
{
    [MenuItem("Open Scene/Game Scene")]
    public static void OpenGameScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
    }
    [MenuItem("Open Scene/Main Scene")]
    public static void OpenMainScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
    }
}
#endif