using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

//                              ГАЙД ПО СЕЙВАМ (Код ниже)
//
// 1. КАК СОХРАНИТЬ СВОИ ДАННЫЕ
//    - Вызови: SaveSystem.Instance.SaveObject("уникальный_ключ", твоиДанные);
//    - Пример:
//        var data = new WalletData { money = 100, zones = new List<string>{"corn"} };
//        SaveSystem.Instance.SaveObject("wallet", data);
//    - SaveObject сам вызовет MarkDirty(), так что отдельно не нужно
//
// 2. КАК ЗАГРУЗИТЬ СВОИ ДАННЫЕ
//    - Вызови: object raw = SaveSystem.Instance.LoadObject("уникальный_ключ");
//    - Приведи к своему типу:
//        WalletData data = raw as WalletData;
//        if (data != null) { _money = data.money; _zones = data.zones; }
//    - Вызывай в Awake() или Start() – SaveSystem уже загрузил файл в Start()
//
// 3. УНИКАЛЬНЫЕ КЛЮЧИ
//    - Каждый модуль должен использовать свой ключ, чтобы не перезаписывать чужие данные
//    - Рекомендация: использовать имя модуля + суффикс, например "wallet", "shelf_1", "playerStats"
//    - Если ключ уже занят – данные перезапишутся
//
// 4. ЧТО ДЕЛАТЬ, ЕСЛИ Я МЕНЯЮ ДАННЫЕ НАПРЯМУЮ (БЕЗ SAVEOBJECT)?
//    - Если ты изменяешь данные, которые уже сохранил, и хочешь, чтобы они сохранились,
//      вызови SaveSystem.Instance.MarkDirty()
//    - Пример: _money += 10; SaveSystem.Instance.MarkDirty();
//    - Но проще использовать SaveObject – он сам вызовет MarkDirty
//
// 5. КАКИЕ ДАННЫЕ МОЖНО СОХРАНЯТЬ?
//    - Любые сериализуемые объекты: числа, строки, списки, словари, свои классы с [Serializable]
//    - Для своих классов добавляйте [System.Serializable] перед объявлением класса
//    - Избегай сохранения ссылок на Unity-объекты (GameObject, Transform) – они не сериализуются
//
// 6. АВТОСОХРАНЕНИЕ
//    - SaveSystem автоматически сохраняет все данные каждые 10 секунд, если есть изменения (isDirty)
//    - Также сохраняет при закрытии игры (OnApplicationQuit)
//    - Вы можете вызвать SaveSystem.Instance.SaveGame() для принудительного сохранения
//
// 7. ЗАГРУЗКА ПРИ СТАРТЕ
//    - SaveSystem автоматически загружает сохранение в методе Start()
//    - Вам не нужно вызывать LoadGame() самому – просто используй LoadObject()
//    - Например:
//      private void Start()
//      {
//          LoadWallet();
//      }
//      
//      private void LoadWallet()
//      {
//          object raw = SaveSystem.Instance.LoadObject("wallet");
//          if (raw != null)
//          {
//              WalletData data = raw as WalletData;
//              if (data != null)
//              {
//                  _money = data.money;
//                  _purchasedZones = data.zones;
//              }
//          }
//      }
//      
// 8. ПРИМЕР ПОЛНОГО ЦИКЛА
//    [System.Serializable]
//    public class MyData
//    {
//        public int score;
//        public string name;
//    }
// 
//    private void SaveMyData()
//    {
//        var data = new MyData { score = 42, name = "Player" };
//        SaveSystem.Instance.SaveObject("myData", data);
//    }
// 
//    private void LoadMyData()
//    {
//        var raw = SaveSystem.Instance.LoadObject("myData");
//        if (raw != null) {
//            MyData data = raw as MyData;
//            if (data != null) {
//                _score = data.score;
//                _name = data.name;
//            }
//        }
//    }
//
// 9. УДАЛЕНИЕ СОХРАНЕНИЯ (для тестов)
//    - При нажатии на кнопку I сохранение удаляется, используйте для тестов
//
// 10. ПРОВЕРКА НАЛИЧИЯ СОХРАНЕНИЯ
//     - if (SaveSystem.Instance.HasSave()) { ... }
//

public class SaveSystem : MonoBehaviour
{
    private static SaveSystem _instance;
    public static SaveSystem Instance => _instance;

    [SerializeField] private float _autosaveInterval = 10f;
    private string _saveFilePath;
    private Dictionary<string, object> _pendingData = new Dictionary<string, object>();
    private bool _isDirty = false;
    private float _timer = 0f;

    private Dictionary<string, object> _loadedData = new Dictionary<string, object>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
        Debug.Log($"Save file path: {_saveFilePath}");
    }

    private void Start()
    {
        LoadGame();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _autosaveInterval)
        {
            _timer = 0f;
            if (_isDirty)
            {
                SaveGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            DeleteSave();
        }
    }

    private void OnApplicationQuit()
    {
        if (_isDirty)
            SaveGame();
    }
    public void SaveObject(string key, object data)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("SaveSystem: key is null or empty!");
            return;
        }

        if (_pendingData.ContainsKey(key))
            _pendingData[key] = data;
        else
            _pendingData.Add(key, data);

        MarkDirty();
    }

    public object LoadObject(string key)
    {
        if (_loadedData.TryGetValue(key, out object value))
            return value;
        return null;
    }
    public void MarkDirty()
    {
        _isDirty = true;
    }

    public void SaveGame()
    {
        if (!_isDirty && _pendingData.Count == 0)
            return;

        SaveData saveData = new SaveData();
        foreach (var kvp in _pendingData)
        {
            saveData.data[kvp.Key] = kvp.Value;
        }

        string json = JsonConvert.SerializeObject(saveData, new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        });

        try
        {
            File.WriteAllText(_saveFilePath, json);
            _isDirty = false;
            Debug.Log("Game saved successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(_saveFilePath))
        {
            Debug.Log("No save file found, starting fresh.");
            _loadedData.Clear();
            return;
        }

        try
        {
            string json = File.ReadAllText(_saveFilePath);
            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            if (saveData != null)
            {
                _loadedData = saveData.data;
                _pendingData.Clear();
                foreach (var kvp in _loadedData)
                    _pendingData[kvp.Key] = kvp.Value;

                _isDirty = false;
                Debug.Log($"Game loaded. {_loadedData.Count} keys restored.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}. Starting fresh.");
            _loadedData.Clear();
            _pendingData.Clear();
            _isDirty = false;
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(_saveFilePath))
        {
            File.Delete(_saveFilePath);
            _loadedData.Clear();
            _pendingData.Clear();
            _isDirty = false;
            Debug.Log("Save deleted.");
        }
    }

    public bool HasSave()
    {
        return File.Exists(_saveFilePath);
    }
}