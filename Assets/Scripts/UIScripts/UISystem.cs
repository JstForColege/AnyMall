using UnityEngine;

public class UISystem : MonoBehaviour
{

    private static UISystem _instance;
    public GameObject UI;
    public static UISystem Instance => _instance;
    private void Awake()
    {

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OpenUI()
    {
        UI.SetActive(true);

        UpgradeWindow window = UI.GetComponent<UpgradeWindow>();
        if (window != null)
        {
            window.OnWindowOpen();
        }
    }
    public void CloseUI()
    {
        UI.SetActive(false);
    }
    
}
