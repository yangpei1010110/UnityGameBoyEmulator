#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils
{
    public static class Finder
    {
        /// <summary>
        ///     在 build settings 中的所有场景中查找指定类型的对象
        /// </summary>
        public static T FindObjectOfTypeOnAllScenes<T>() where T : Object
        {
            string currentSceneName = SceneManager.GetActiveScene().path;
            string[] scenesData = EditorBuildSettings.scenes
                                                     .Where(s => s.enabled)
                                                     .Select(s => s.path)
                                                     .ToArray();
            T result = Object.FindObjectOfType<T>();
            foreach (string sceneName in scenesData)
            {
                if (result)
                {
                    break;
                }

                EditorSceneManager.OpenScene(sceneName);
                result = Object.FindObjectOfType<T>();
            }

            if (!SceneManager.GetActiveScene().path.Equals(currentSceneName))
            {
                EditorSceneManager.OpenScene(currentSceneName);
            }

            return result;
        }

        /// <summary>
        ///     在 build settings 中的所有场景中查找指定类型的对象
        /// </summary>
        public static IReadOnlyList<T> FindObjectsOfTypeOnAllScenes<T>() where T : Object
        {
            string currentSceneName = SceneManager.GetActiveScene().path;
            string[] scenesData = EditorBuildSettings.scenes
                                                     .Where(s => s.enabled)
                                                     .Select(s => s.path)
                                                     .ToArray();
            List<T> result = new();
            foreach (string sceneName in scenesData)
            {
                EditorSceneManager.OpenScene(sceneName);
                result.AddRange(Object.FindObjectsOfType<T>());
            }

            if (!SceneManager.GetActiveScene().path.Equals(currentSceneName))
            {
                EditorSceneManager.OpenScene(currentSceneName);
            }

            return result;
        }
    }
}
#endif