using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private static PlayerData instance;

    public int level = 1;
    public int experience = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        if (experience >= 100 + (level - 1) * 10)
        {
            level++;
            experience = 0;
        }
    }

    public void SaveData()
    {
        Debug.Log("save data, level: " + level + ", exp: " + experience);
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("Experience", experience);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        level = PlayerPrefs.GetInt("Level", 1);
        experience = PlayerPrefs.GetInt("Experience", 0);
        Debug.Log("load data, level: " + level + ", exp: " + experience);
    }

    public Vector3 GetSavedPlayerPosition()
    {
        // default: Vector3(1061.54004,43,2931.80005)
        float x = PlayerPrefs.GetFloat("PlayerX", 1061.54004f);
        float y = PlayerPrefs.GetFloat("PlayerY", 43f);
        float z = PlayerPrefs.GetFloat("PlayerZ", 2931.80005f);

        return new Vector3(x, y, z);
    }

    public void SavePlayerPosition(Vector3 position)
    {
        // Save the player position and rotation to PlayerPrefs
        PlayerPrefs.SetFloat("PlayerX", position.x);
        PlayerPrefs.SetFloat("PlayerY", position.y);
        PlayerPrefs.SetFloat("PlayerZ", position.z);

        PlayerPrefs.Save();
    }
}
