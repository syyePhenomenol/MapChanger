using GlobalEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using SM = UnityEngine.SceneManagement.SceneManager;

namespace MapChanger
{
    public static class Utils
    {
        /// <summary>
        /// Generic method for creating a new GameObject with the MonoBehaviour, and returning the MonoBehaviour.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns>The new MonoBehaviour instance</returns>
        public static T MakeMonoBehaviour<T>(GameObject parent, string name) where T : MonoBehaviour
        {
            GameObject go = new(name);
            if (parent is not null)
            {
                go.transform.SetParent(parent.transform, false);
            }
            go.SetActive(false);
            return go.AddComponent<T>();
        }

        //public static string ToCleanPreviewText(string text)
        //{
        //    return text.Replace("Pay ", "")
        //        .Replace("Once you own ", "")
        //        .Replace(", I'll gladly sell it to you.", "")
        //        .Replace("Requires ", "");
        //}

        public static string ToCleanName(this string name)
        {
            return name.Replace("-", " ").Replace("_", " ");
        }

        public static bool HasMapSetting(MapZone mapZone)
        {
            return mapZone switch
            {
                MapZone.ABYSS => PlayerData.instance.GetBool("mapAbyss"),
                MapZone.CITY => PlayerData.instance.GetBool("mapCity"),
                MapZone.CLIFFS => PlayerData.instance.GetBool("mapCliffs"),
                MapZone.CROSSROADS => PlayerData.instance.GetBool("mapCrossroads"),
                MapZone.MINES => PlayerData.instance.GetBool("mapMines"),
                MapZone.DEEPNEST => PlayerData.instance.GetBool("mapDeepnest"),
                MapZone.TOWN => PlayerData.instance.GetBool("mapDirtmouth"),
                MapZone.FOG_CANYON => PlayerData.instance.GetBool("mapFogCanyon"),
                MapZone.WASTES => PlayerData.instance.GetBool("mapFungalWastes"),
                MapZone.GREEN_PATH => PlayerData.instance.GetBool("mapGreenpath"),
                MapZone.OUTSKIRTS => PlayerData.instance.GetBool("mapOutskirts"),
                MapZone.ROYAL_GARDENS => PlayerData.instance.GetBool("mapRoyalGardens"),
                MapZone.RESTING_GROUNDS => PlayerData.instance.GetBool("mapRestingGrounds"),
                MapZone.WATERWAYS => PlayerData.instance.GetBool("mapWaterways"),
                MapZone.WHITE_PALACE => PlayerData.instance.GetBool("AdditionalMapsGotWpMap"),
                MapZone.GODS_GLORY => PlayerData.instance.GetBool("AdditionalMapsGotGhMap"),
                _ => false,
            };
        }

        public static string DropSuffix(this string scene)
        {
            if (scene == "") return "";
            string[] sceneSplit = scene.Split('_');
            string truncatedScene = sceneSplit[0];
            for (int i = 1; i < sceneSplit.Length - 1; i++)
            {
                truncatedScene += "_" + sceneSplit[i];
            }
            return truncatedScene;
        }

        private static readonly List<GameObject> rootObjects = new(500);

        /// <summary>
        /// Finds a GameObject in the given scene by its full path.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="path">The full path to the GameObject, with forward slash ('/') separators.</param>
        /// <returns></returns>
        public static GameObject FindGameObject(this Scene s, string path)
        {
            s.GetRootGameObjects(rootObjects); // clears list

            int index = path.IndexOf('/');
            GameObject result = null;
            if (index >= 0)
            {
                string rootName = path.Substring(0, index);
                GameObject root = rootObjects.FirstOrDefault(g => g.name == rootName);
                if (root != null) result = root.transform.Find(path.Substring(index + 1)).gameObject;
            }
            else
            {
                result = rootObjects.FirstOrDefault(g => g.name == path);
            }

            rootObjects.Clear();
            return result;
        }

        public static GameObject FindGameObjectInCurrentScene(string path)
        {
            SM.GetActiveScene().GetRootGameObjects(rootObjects); // clears list

            int index = path.IndexOf('/');
            GameObject result = null;
            if (index >= 0)
            {
                string rootName = path.Substring(0, index);
                GameObject root = rootObjects.FirstOrDefault(g => g.name == rootName);
                if (root != null) result = root.transform.Find(path.Substring(index + 1)).gameObject;
            }
            else
            {
                result = rootObjects.FirstOrDefault(g => g.name == path);
            }

            rootObjects.Clear();
            return result;
        }

        public static Transform FindChildInHierarchy(this Transform parent, string pathName)
        {
            string[] splitNames = pathName.Split('/');
            Transform transform = parent;

            foreach (string splitName in splitNames)
            {
                if (splitName == "") return transform;
                Transform child = transform.Find(splitName);
                if (child is null) return null;
                transform = child;
            }
            return transform;
        }

        /// <summary>
        /// Calculates the average of both components in a Vector2Int.
        /// </summary>
        public static float Average(this Vector2Int vec)
        {
            return (vec.x + vec.y) / 2;
        }

        /// <summary>
        /// Snaps the value to the nearest 0.1.
        /// </summary>
        public static float Snap(this float offset)
        {
            // Snap to nearest 0.1
            return (float)Math.Round(offset * 10f, MidpointRounding.AwayFromZero) / 10f;
        }

        /// <summary>
        /// Snaps the vector2 to a (0.1, 0.1) grid.
        /// </summary>
        public static Vector2 Snap(this Vector2 vec)
        {
            return new(vec.x.Snap(), vec.y.Snap());
        }

        /// <summary>
        /// Sets the w component of a Vector4 to one.
        /// If the Vector4 is interpreted as a color, it will be opaque.
        /// </summary>
        public static Vector4 ToOpaque(this Vector4 color)
        {
            return new(color.x, color.y, color.z, 1f);
        }

        public static string CurrentScene()
        {
            return GameManager.GetBaseSceneName(GameManager.instance.sceneName);
        }

        public static string GetBindingsText(List<InControl.BindingSource> bindings)
        {
            string text = "";

            text += $"[{bindings.First().Name}]";

            if (bindings.Count > 1 && bindings[1].BindingSourceType == InControl.BindingSourceType.DeviceBindingSource)
            {
                text += $" or ({bindings[1].Name})";
            }

            return text;
        }
    }
}
