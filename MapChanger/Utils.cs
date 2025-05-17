using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GlobalEnums;
using UnityEngine;
using UnityEngine.SceneManagement;
using SM = UnityEngine.SceneManagement.SceneManager;

namespace MapChanger;

public static class Utils
{
    /// <summary>
    /// Generic method for creating a new GameObject with the MonoBehaviour, and returning the MonoBehaviour.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns>The new MonoBehaviour instance</returns>
    public static T MakeMonoBehaviour<T>(GameObject parent, string name)
        where T : MonoBehaviour
    {
        GameObject go = new(name);
        if (parent != null)
        {
            go.transform.SetParent(parent.transform, false);
        }

        go.SetActive(false);
        return go.AddComponent<T>();
    }

    public static string ToCleanName(this string name)
    {
        return name.Replace("-", " ").Replace("_", " ");
    }

    public static string ToWhitespaced(this string text)
    {
        return Regex.Replace(text, "(?<char>[A-Z])", match => " " + match.Groups["char"].Value);
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
        if (scene == "")
            return "";
        var sceneSplit = scene.Split('_');
        var truncatedScene = sceneSplit[0];
        for (var i = 1; i < sceneSplit.Length - 1; i++)
        {
            truncatedScene += "_" + sceneSplit[i];
        }

        return truncatedScene;
    }

    private static readonly List<GameObject> _rootObjects = new(500);

    /// <summary>
    /// Finds a GameObject in the given scene by its full path.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="path">The full path to the GameObject, with forward slash ('/') separators.</param>
    /// <returns></returns>
    public static GameObject FindGameObject(this Scene s, string path)
    {
        s.GetRootGameObjects(_rootObjects); // clears list

        var index = path.IndexOf('/');
        GameObject result = null;
        if (index >= 0)
        {
            var rootName = path.Substring(0, index);
            var root = _rootObjects.FirstOrDefault(g => g.name == rootName);
            if (root != null)
                result = root.transform.Find(path.Substring(index + 1)).gameObject;
        }
        else
        {
            result = _rootObjects.FirstOrDefault(g => g.name == path);
        }

        _rootObjects.Clear();
        return result;
    }

    public static GameObject FindGameObjectInCurrentScene(string path)
    {
        SM.GetActiveScene().GetRootGameObjects(_rootObjects); // clears list

        var index = path.IndexOf('/');
        GameObject result = null;
        if (index >= 0)
        {
            var rootName = path.Substring(0, index);
            var root = _rootObjects.FirstOrDefault(g => g.name == rootName);
            if (root != null)
                result = root.transform.Find(path.Substring(index + 1)).gameObject;
        }
        else
        {
            result = _rootObjects.FirstOrDefault(g => g.name == path);
        }

        _rootObjects.Clear();
        return result;
    }

    public static Transform FindChildInHierarchy(this Transform parent, string pathName)
    {
        var splitNames = pathName.Split('/');
        var transform = parent;

        foreach (var splitName in splitNames)
        {
            if (splitName == "")
            {
                return transform;
            }

            var child = transform.Find(splitName);
            if (child == null)
            {
                return null;
            }

            transform = child;
        }

        return transform;
    }

    /// <summary>
    /// Calculates the average of both components in a Vector2Int.
    /// </summary>
    /// <param name="vec"></param>
    public static float Average(this Vector2Int vec)
    {
        return (vec.x + vec.y) / 2;
    }

    /// <summary>
    /// Snaps the value to the nearest 0.1.
    /// </summary>
    /// <param name="offset"></param>
    public static float Snap(this float offset)
    {
        // Snap to nearest 0.1
        return (float)Math.Round(offset * 10f, MidpointRounding.AwayFromZero) / 10f;
    }

    /// <summary>
    /// Snaps the vector2 to a (0.1, 0.1) grid.
    /// </summary>
    /// <param name="vec"></param>
    public static Vector2 Snap(this Vector2 vec)
    {
        return new(vec.x.Snap(), vec.y.Snap());
    }

    /// <summary>
    /// Sets the w component of a Vector4 to one.
    /// If the Vector4 is interpreted as a color, it will be opaque.
    /// </summary>
    /// <param name="color"></param>
    public static Vector4 ToOpaque(this Vector4 color)
    {
        return new(color.x, color.y, color.z, 1f);
    }

    public static string CurrentScene()
    {
        return GameManager.GetBaseSceneName(GameManager.instance.sceneName);
    }

    public static string ToChar(this KeyCode keyCode)
    {
        return keyCode switch
        {
            KeyCode.Backspace => "⌫",
            KeyCode.Delete => "⌦",
            KeyCode.Tab => "⇥",
            KeyCode.Clear => "⌧",
            KeyCode.Return => "⏎",
            KeyCode.Pause => "⏸",
            KeyCode.Escape => "⎋",
            KeyCode.Space => "␣",
            KeyCode.Keypad0 => "0",
            KeyCode.Keypad1 => "1",
            KeyCode.Keypad2 => "2",
            KeyCode.Keypad3 => "3",
            KeyCode.Keypad4 => "4",
            KeyCode.Keypad5 => "5",
            KeyCode.Keypad6 => "6",
            KeyCode.Keypad7 => "7",
            KeyCode.Keypad8 => "8",
            KeyCode.Keypad9 => "9",
            KeyCode.KeypadPeriod => ".",
            KeyCode.KeypadDivide => "/",
            KeyCode.KeypadMultiply => "✕",
            KeyCode.KeypadMinus => "-",
            KeyCode.KeypadPlus => "+",
            KeyCode.KeypadEnter => "⏎",
            KeyCode.KeypadEquals => "=",
            KeyCode.UpArrow => "↑",
            KeyCode.DownArrow => "↓",
            KeyCode.RightArrow => "→",
            KeyCode.LeftArrow => "←",
            KeyCode.Insert => "⎀",
            KeyCode.Home => "⇱",
            KeyCode.End => "⇲",
            KeyCode.PageUp => "⇞",
            KeyCode.PageDown => "⇟",
            KeyCode.F1 => "F1",
            KeyCode.F2 => "F2",
            KeyCode.F3 => "F3",
            KeyCode.F4 => "F4",
            KeyCode.F5 => "F5",
            KeyCode.F6 => "F6",
            KeyCode.F7 => "F7",
            KeyCode.F8 => "F8",
            KeyCode.F9 => "F9",
            KeyCode.F10 => "F10",
            KeyCode.F11 => "F11",
            KeyCode.F12 => "F12",
            KeyCode.F13 => "F13",
            KeyCode.F14 => "F14",
            KeyCode.F15 => "F15",
            KeyCode.Alpha0 => "0",
            KeyCode.Alpha1 => "1",
            KeyCode.Alpha2 => "2",
            KeyCode.Alpha3 => "3",
            KeyCode.Alpha4 => "4",
            KeyCode.Alpha5 => "5",
            KeyCode.Alpha6 => "6",
            KeyCode.Alpha7 => "7",
            KeyCode.Alpha8 => "8",
            KeyCode.Alpha9 => "9",
            KeyCode.Exclaim => "!",
            KeyCode.DoubleQuote => "\"",
            KeyCode.Hash => "#",
            KeyCode.Dollar => "$",
            KeyCode.Percent => "%",
            KeyCode.Ampersand => "&",
            KeyCode.Quote => "'",
            KeyCode.LeftParen => "(",
            KeyCode.RightParen => ")",
            KeyCode.Asterisk => "*",
            KeyCode.Plus => "+",
            KeyCode.Comma => ",",
            KeyCode.Minus => "-",
            KeyCode.Period => ".",
            KeyCode.Slash => "/",
            KeyCode.Colon => ":",
            KeyCode.Semicolon => ";",
            KeyCode.Less => "<",
            KeyCode.Equals => "=",
            KeyCode.Greater => ">",
            KeyCode.Question => "?",
            KeyCode.At => "@",
            KeyCode.LeftBracket => "[",
            KeyCode.Backslash => "\\",
            KeyCode.RightBracket => "]",
            KeyCode.Caret => "^",
            KeyCode.Underscore => "_",
            KeyCode.BackQuote => "`",
            KeyCode.A => "A",
            KeyCode.B => "B",
            KeyCode.C => "C",
            KeyCode.D => "D",
            KeyCode.E => "E",
            KeyCode.F => "F",
            KeyCode.G => "G",
            KeyCode.H => "H",
            KeyCode.I => "I",
            KeyCode.J => "J",
            KeyCode.K => "K",
            KeyCode.L => "L",
            KeyCode.M => "M",
            KeyCode.N => "N",
            KeyCode.O => "O",
            KeyCode.P => "P",
            KeyCode.Q => "Q",
            KeyCode.R => "R",
            KeyCode.S => "S",
            KeyCode.T => "T",
            KeyCode.U => "U",
            KeyCode.V => "V",
            KeyCode.W => "W",
            KeyCode.X => "X",
            KeyCode.Y => "Y",
            KeyCode.Z => "Z",
            KeyCode.LeftCurlyBracket => "{",
            KeyCode.Pipe => "|",
            KeyCode.RightCurlyBracket => "}",
            KeyCode.Tilde => "~",
            KeyCode.Numlock => "⇭",
            KeyCode.CapsLock => "⇪",
            KeyCode.ScrollLock => "⤓",
            KeyCode.RightShift => "⇧",
            KeyCode.LeftShift => "⇧",
            KeyCode.RightAlt => "Alt",
            KeyCode.LeftAlt => "Alt",
            KeyCode.LeftCommand => "⌘",
            KeyCode.LeftWindows => "⊞",
            KeyCode.RightCommand => "⌘",
            KeyCode.RightWindows => "⊞",
            _ => string.Empty,
        };
    }
}
