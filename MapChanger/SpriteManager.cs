using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MapChanger;

/// <summary>
/// Code copied from ItemChanger, originally written by Homothety.
/// The main difference is that mipmapping is enabled, making smaller scaled sprites look a bit better.
/// I have also added a method for getting the Texture2D.
/// https://github.com/homothetyhk/HollowKnight.ItemChanger/blob/master/ItemChanger/Internal/SpriteManager.cs
/// </summary>
/// <param name="assembly"></param>
/// <param name="resourcePrefix"></param>
/// <param name="info"></param>
/// <remarks>
/// Creates a SpriteManager to lazily load and cache Sprites from the embedded png files in the specified assembly.
/// <br/>Only filepaths with the matching prefix are considered, and the prefix is removed to determine sprite names (e.g. "ItemChangerMod.Resources." is the prefix for Instance).
/// </remarks>
public class SpriteManager(Assembly assembly, string resourcePrefix, SpriteManager.Info info)
{
    /// <summary>
    /// Effective length/height in pixels of the built in pins, not including shadow.
    /// </summary>
    public static readonly int DEFAULT_PIN_SPRITE_SIZE = 59;
    private readonly Dictionary<string, string> _resourcePaths = assembly
        .GetManifestResourceNames()
        .Where(n => n.EndsWith(".png") && n.StartsWith(resourcePrefix))
        .ToDictionary(n => n.Substring(resourcePrefix.Length, n.Length - resourcePrefix.Length - ".png".Length));
    private readonly Dictionary<string, Sprite> _cachedSprites = [];
    private readonly Dictionary<string, Texture2D> _cachedTextures = [];

    public class Info
    {
        public Dictionary<string, float> OverridePPUs { get; init; }
        public Dictionary<string, FilterMode> OverrideFilterModes { get; init; }

        public FilterMode DefaultFilterMode { get; init; } = FilterMode.Bilinear;
        public float DefaultPixelsPerUnit { get; init; } = 100f;

        public virtual float GetPixelsPerUnit(string name)
        {
            if (OverridePPUs != null && OverridePPUs.TryGetValue(name, out var ppu))
                return ppu;
            return DefaultPixelsPerUnit;
        }

        public virtual FilterMode GetFilterMode(string name)
        {
            if (OverrideFilterModes != null && OverrideFilterModes.TryGetValue(name, out var mode))
                return mode;
            return DefaultFilterMode;
        }
    }

    /// <summary>
    /// The SpriteManager with access to embedded MapChanger pngs.
    /// </summary>
    public static SpriteManager Instance { get; } =
        new(
            typeof(SpriteManager).Assembly,
            "MapChanger.Resources.Sprites.",
            new Info() { DefaultFilterMode = FilterMode.Bilinear, DefaultPixelsPerUnit = 100f }
        );

    /// <summary>
    /// Creates a SpriteManager to lazily load and cache Sprites from the embedded png files in the specified assembly.
    /// <br/>Only filepaths with the matching prefix are considered, and the prefix is removed to determine sprite names (e.g. "ItemChangerMod.Resources." is the prefix for Instance).
    /// <br/>Images will be loaded with default Bilinear filter mode and 100 pixels per unit.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="resourcePrefix"></param>
    public SpriteManager(Assembly a, string resourcePrefix)
        : this(a, resourcePrefix, new()) { }

    /// <summary>
    /// Fetches the Sprite with the specified name. If it has not yet been loaded, loads it from embedded resources and caches the result.
    /// <br/>The name is the path of the image as an embedded resource, with the SpriteManager prefix and file extension removed.
    /// <br/>For example, the image at "ItemChanger.Resources.ShopIcons.Geo.png" has key "ShopIcons.Geo" in SpriteManager.Instance.
    /// </summary>
    /// <param name="name"></param>
    public Sprite GetSprite(string name)
    {
        if (_cachedSprites.TryGetValue(name, out var sprite))
        {
            return sprite;
        }
        else if (_resourcePaths.TryGetValue(name, out var path))
        {
            using var s = assembly.GetManifestResourceStream(path);
            return _cachedSprites[name] = Load(ToArray(s), info.GetFilterMode(name), info.GetPixelsPerUnit(name));
        }
        else
        {
            MapChangerMod.Instance.LogWarn($"{name} did not correspond to an embedded image file.");
            return null;
        }
    }

    public Texture2D GetTexture(string name)
    {
        if (_cachedTextures.TryGetValue(name, out var tex))
        {
            return tex;
        }
        else if (_resourcePaths.TryGetValue(name, out var path))
        {
            var data = ToArray(assembly.GetManifestResourceStream(path));
            tex = new(1, 1, TextureFormat.RGBA32, true);
            _ = tex.LoadImage(data, markNonReadable: true);
            tex.filterMode = info.GetFilterMode(name);

            return _cachedTextures[name] = tex;
        }
        else
        {
            MapChangerMod.Instance.LogWarn($"{name} did not correspond to an embedded image file.");
            return null;
        }
    }

    /// <summary>
    /// Loads a sprite from the png file passed as a stream.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="filterMode"></param>
    public static Sprite Load(Stream data, FilterMode filterMode = FilterMode.Bilinear)
    {
        return Load(ToArray(data), filterMode);
    }

    /// <summary>
    /// Loads a sprite from the png file passed as a byte array.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="filterMode"></param>
    public static Sprite Load(byte[] data, FilterMode filterMode)
    {
        return Load(data, filterMode, 100f);
    }

    public static Sprite Load(byte[] data, FilterMode filterMode, float pixelsPerUnit)
    {
        Texture2D tex = new(1, 1, TextureFormat.RGBA32, true);
        _ = tex.LoadImage(data, markNonReadable: true);
        tex.filterMode = filterMode;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }

    private static byte[] ToArray(Stream s)
    {
        using MemoryStream ms = new();
        s.CopyTo(ms);
        return ms.ToArray();
    }
}
