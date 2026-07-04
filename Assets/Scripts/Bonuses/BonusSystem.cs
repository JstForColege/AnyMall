using UnityEngine;

//                              ГАЙД ПО БОНУСАМ (Код ниже)
// 
// 1. КАК ЗАПУСКАТЬ БОНУСЫ
//     Нужно вызвать один из двух методов:
//          ActivateMultiplier(время_длительности_в_секундах) //Множитель прибыли
//          ActivateSpeedBoost(время_длительности_в_секундах) //Ускоритель работы техники и животных  
//
// 2. НАСТРОЙКА БОНУСА МНОЖИТЕЛЯ ПРИБЫЛИ
//     Пример для того, кто будет заниматься скриптингом прибыли с продажи
//     В своём методе получения прибыли нужно написать дополнительное условие, чтобы всё работало
//
//     if(BonusSystem.Instance.IsMultiplierActive() == true)
//     {
//         //Сбор денег увеличивается
//     }
//
// 3. НАСТРОЙКА БОНУСА УСКОРИТЕЛЯ РАБОТЫ ТЕХНИКИ И ЖИВОТНЫХ
//     Пример для того, кто будет заниматься скриптингом животных и станков
//     В своём методе нужно написать дополнительное условие, чтобы всё работало
//
//     if(BonusSystem.Instance.IsSpeedBoostActive() == true)
//     {
//         //Время работы ускоряется
//     }
//
// 4. ДЛЯ UI ИНТЕРФЕЙСА И СЧЁТА
//     Для какого-нибудь визуального счётчика Используйте методы:
//     GetMultiplierRemainingTime() //Счётчик у множителя
//     GetSpeedBoostRemainingTime() //Счётчик у ускорителя работы
//     


public class BonusSystem : MonoBehaviour
{
    private static BonusSystem _instance;
    public static BonusSystem Instance => _instance;

    private float _multiplierTimer = 0f;
    private float _speedBoostTimer = 0f;
    private bool _isMultiplierActive = false;
    private bool _isSpeedActive = false;

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

    private void Update()
    {
        if (_isMultiplierActive)
        {
            _multiplierTimer -= Time.deltaTime;
            if (_multiplierTimer <= 0f)
            {
                _isMultiplierActive = false;
                _multiplierTimer = 0f;
                Debug.Log("Bonus: Multiplier ×2 expired");
            }
        }

        if (_isSpeedActive)
        {
            _speedBoostTimer -= Time.deltaTime;
            if (_speedBoostTimer <= 0f)
            {
                _isSpeedActive = false;
                _speedBoostTimer = 0f;
                Debug.Log("Bonus: Speed boost expired");
            }
        }
    }

    public void ActivateMultiplier(float duration)
    {
        _isMultiplierActive = true;
        _multiplierTimer = duration;
        Debug.Log($"Bonus: Multiplier ×2 activated for {duration} seconds");
    }

    public void ActivateSpeedBoost(float duration)
    {
        _isSpeedActive = true;
        _speedBoostTimer = duration;
        Debug.Log($"Bonus: Speed boost activated for {duration} seconds");
    }


    public bool IsMultiplierActive()
    {
        return _isMultiplierActive;
    }

    public float GetMultiplierRemainingTime()
    {
        return _multiplierTimer;
    }

    public bool IsSpeedBoostActive()
    {
        return _isSpeedActive;
    }

    public float GetSpeedBoostRemainingTime()
    {
        return _speedBoostTimer;
    }
}