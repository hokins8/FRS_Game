using UnityEngine;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [SerializeField] Slider loadingBar;

    public static LoadingController Instance;

    void Awake()
    {
        Instance = this;
    }

    public void SetBar(float value)
    {
        loadingBar.value = value;
    }
}
