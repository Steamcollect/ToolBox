using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUtils : MonoBehaviour
{
    
    /// <summary>
    /// Load Scene Asyncronely and call action at the end
    /// </summary>
    /// <param name="sceneIndex"></param>
    /// <param name="loadMode"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IEnumerator LoadSceneAsync(int sceneIndex, LoadSceneMode loadMode, Action action)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, loadMode);

        yield return new WaitUntil(() => asyncLoad.isDone);

        action.Invoke();
    }

    /// <summary>
    /// Unload Scene Asyncronely and call action at the end
    /// </summary>
    /// <param name="sceneIndex"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IEnumerator UnloadSceneAsync(int sceneIndex, Action action)
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneIndex);

        yield return new WaitUntil(() => asyncLoad.isDone);

        action.Invoke();
    }
}