using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
    }

    public static void SceneChangeAdditive(string name)
    {
        SceneManager.LoadScene(name, LoadSceneMode.Additive);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
