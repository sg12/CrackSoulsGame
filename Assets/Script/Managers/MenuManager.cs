using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public void NewGame()
    {
        PlayerPrefs.SetInt("MenuInTransitionDataRef", 0);
        PlayerPrefs.SetString("GlobalSaveSceneNameDataRef", "");

        SceneManager.LoadScene("Level1");
    }

    public void LoadGame()
    {
        if (PlayerPrefs.GetString("GlobalSaveSceneNameDataRef") == "")
        {
            Debug.LogWarning("Save file not found");
            return;
        }

        PlayerPrefs.SetInt("MenuInTransitionDataRef", 1);

        SceneManager.LoadScene(PlayerPrefs.GetString("GlobalSaveSceneNameDataRef"));
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
