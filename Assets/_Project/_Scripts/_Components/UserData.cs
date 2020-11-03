using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class UserDataPair
{
    public string key;
    public string value;

    public UserDataPair() { }
    public UserDataPair(string _key, string _value) { key = _key; value = _value; }
}

[System.Serializable]
public class UserDataContent
{
    public List<UserDataPair> data = new List<UserDataPair>();
}

public class UserData : MonoBehaviour
{
    public static UserData instance;

    string path;
    public string filename = "save";

    UserDataContent content;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

#if UNITY_EDITOR
        path = Application.dataPath + "/" + filename + ".json";
#else
            path = Application.persistentDataPath + "/" + filename + ".json";
#endif
        Load();
    }

    void Load()
    {
        if (!File.Exists(path))
        {
            content = new UserDataContent();
            var json = JsonUtility.ToJson(content);
            File.WriteAllText(path, json);
        }
        else
        {
            content = JsonUtility.FromJson<UserDataContent>(File.ReadAllText(path));
        }
    }

    public bool HasKey(string key)
    {
        return GetPair(key) != null;
    }

    public void AddPair(string key, string value)
    {
        var pair = GetPair(key);

        if (pair == null)
            content.data.Add(new UserDataPair(key, value));
        else
            pair.value = value;

        Save();
    }

    public void SetString(string key, string value)
    {
        AddPair(key, value);
    }

    public void SetInt(string key, int value)
    {
        AddPair(key, value.ToString());
    }

    public void SetFloat(string key, float value)
    {
        AddPair(key, value.ToString());
    }

    public void SetBool(string key, bool value)
    {
        AddPair(key, value.ToString());
    }

    public UserDataPair GetPair(string key)
    {
        return content.data.FirstOrDefault(it => it.key == key);
    }

    public string GetString(string key, string defaultValue = "")
    {
        var pair = GetPair(key);
        return pair != null ? pair.value : defaultValue;
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        var pair = GetPair(key);

        return pair != null ? System.Convert.ToInt32(pair.value) : defaultValue;
    }

    public float GetFloat(string key, float defaultValue = 0.0f)
    {
        var pair = GetPair(key);
        return pair != null ? System.Convert.ToSingle(pair.value) : defaultValue;
    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        var pair = GetPair(key);
        return pair != null ? System.Convert.ToBoolean(pair.value) : defaultValue;
    }

    public void Save()
    {
        var json = JsonUtility.ToJson(content);
        File.WriteAllText(path, json);
    }
}
