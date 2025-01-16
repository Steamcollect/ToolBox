using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public static class Utils
{
    #region IENUMERABLE
    public static T GetRandom<T>(this IEnumerable<T> elems)
    {
        if (elems.Count() == 0)
        {
            Debug.LogError("Try to get random elem from empty IEnumerable");
        }
        return elems.ElementAt(new System.Random().Next(0, elems.Count()));
    }
    #endregion

    #region COROUTINE
    public static IEnumerator Delay(float delay, Action ev)
    {
        yield return new WaitForSeconds(delay);
        ev?.Invoke();
    }
    #endregion

    #region SCENE
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
    #endregion
}