using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    public static void SaveGlobalData(SystemManager gameData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + "Gamedata";
        if (File.Exists(path))
        {
            gameData.cc.infoMessage.info.text = path + " (Game data has been overwritten)";
        }
        else
        {
            gameData.cc.infoMessage.info.text = "(Game data has been saved)";
        }
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(gameData);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadGlobalData(SystemManager gameData)
    {
        string path = Application.persistentDataPath + "/" + "Gamedata";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            if (gameData.gameOverFUI.gameObject.activeInHierarchy)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
                gameData.cc.infoMessage.info.text = "(Save file not found in) " + path;

            return null;
        }
    }

    public static void SaveTransitionData(SystemManager transitionData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "TransitionData";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(transitionData);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadTransitionData(string sceneName)
    {
        string path = Application.persistentDataPath + "/" + sceneName + "TransitionData";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public static void SaveSceneData(SystemManager sceneData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "SceneData";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(sceneData);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadSceneData(string GetSavedScene)
    {
        string path = Application.persistentDataPath + "/" + GetSavedScene + "SceneData";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            if(GetSavedScene == SceneManager.GetActiveScene().name)
            {
                FindObjectOfType<SystemManager>().sceneData = data;
                return data;
            }
            return null;
        }
        else
        {
            return null;
        }
    }
}
