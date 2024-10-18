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
    public string jsonFilePath = "Assets/Data/weaponData.json"; // ���� � JSON-�����
    public WeaponList weaponList;

    void Start()
    {
        LoadWeaponsFromJSON();
        DisplayAllWeapons();
    }

    // �������� ������ �� JSON-�����
    void LoadWeaponsFromJSON()
    {
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            weaponList = JsonUtility.FromJson<WeaponList>(json);
            Debug.Log("������ ������� ��������� �� JSON.");
        }
        else
        {
            Debug.LogError("JSON-���� � ���������������� ������ �� ������!");
        }
    }

    // ������ ������� ��� ����������� ���� ����������� ������
    void DisplayAllWeapons()
    {
        foreach (WeaponData weapon in weaponList.weapons)
        {
            Debug.Log($"������: {weapon.name}, ����: {weapon.minDamage}-{weapon.maxDamage}, �������� �����: {weapon.attackSpeed}");
        }
    }

    // ��������� ������ �� ID
    public WeaponData GetWeaponByID(int id)
    {
        return weaponList.weapons.Find(weapon => weapon.weaponID == id);
    }
}
