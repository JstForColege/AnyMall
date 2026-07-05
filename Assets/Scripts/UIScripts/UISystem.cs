using UnityEngine;

public class UISystem : MonoBehaviour
{
    public GameObject UI;
    public void OpenUI()
    {
        UI.SetActive(true);
    }
    public void CloseUI()
    {
        UI.SetActive(false);
    }
}
