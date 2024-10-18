using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class WeaponData
{
    public int weaponID;
    public string name;
    public string type;
    public float minDamage;
    public float maxDamage;
    public float critMultiplier;
    public float attackSpeed;
    public float durability;
    public float weight;
}

[System.Serializable]
public class WeaponList
{
    public List<WeaponData> weapons;
}

public class WeaponsManager : MonoBehaviour
{
    public string jsonFilePath = "Assets/Data/weaponData.json"; // Путь к JSON-файлу
    public WeaponList weaponList;

    void Start()
    {
        LoadWeaponsFromJSON();
        DisplayAllWeapons();
    }

    // Загрузка данных из JSON-файла
    void LoadWeaponsFromJSON()
    {
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            weaponList = JsonUtility.FromJson<WeaponList>(json);
            Debug.Log("Оружие успешно загружено из JSON.");
        }
        else
        {
            Debug.LogError("JSON-файл с характеристиками оружия не найден!");
        }
    }

    // Пример функции для отображения всех загруженных оружий
    void DisplayAllWeapons()
    {
        foreach (WeaponData weapon in weaponList.weapons)
        {
            Debug.Log($"Оружие: {weapon.name}, Урон: {weapon.minDamage}-{weapon.maxDamage}, Скорость атаки: {weapon.attackSpeed}");
        }
    }

    // Получение оружия по ID
    public WeaponData GetWeaponByID(int id)
    {
        return weaponList.weapons.Find(weapon => weapon.weaponID == id);
    }
}
