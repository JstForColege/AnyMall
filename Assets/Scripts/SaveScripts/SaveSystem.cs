using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

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

    public bool HasSave()
    {
        return File.Exists(_saveFilePath);
    }
}