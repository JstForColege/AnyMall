using UnityEngine;

//                              ГАЙД ПО РАБОТЕ С ОКНАМИ (Код ниже)
// 
// 1. ВАЖНОЕ ПРЕДУПРЕЖДЕНИЕ
//     Из-за малого кол-ва времени, скрипт получился одноразовый 
//     То есть один код работает только с одни окном
//     Чтобы добавить ещё, вы должны добавить этот код на объект UIManager на сцене и
//     добавить ссылку на своё окно(предварительно сделав его SetActive(false))
//
// 2. ОТКРЫТИЕ ОКНА
//     Чтобы заставить ваше окно открыться, вы должны нацепить объект UIManager в поле OnClick() у нужной кнопки
//     После чего выбрать в качестве функции UISystem и выбрать метод OpenUI()
//     
// 3. ЗАКРЫТИЕ ОКНА
//     Чтобы заставить ваше окно закрыться, вы должны нацепить объект UIManager в поле OnClick() у нужной кнопки
//     После чего выбрать в качестве функции UISystem и выбрать метод CloseUI()
//     



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
    }
    public void CloseUI()
    {
        UI.SetActive(false);
    }
}
