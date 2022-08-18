using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MapChanger
{
    /// <summary>
    /// Code copied from ItemChanger, originally written by Homothety.
    /// The main difference is that mipmapping is enabled, making smaller scaled sprites look a bit better.
    /// I have also added a method for getting the Texture2D.
    /// https://github.com/homothetyhk/HollowKnight.ItemChanger/blob/master/ItemChanger/Internal/SpriteManager.cs
    /// </summary>
    public class SpriteManager
    {
        /// <summary>
        /// Effective length/height in pixels of the built in pins, not including shadow.
        /// </summary>
        public static readonly int DEFAULT_PIN_SPRITE_SIZE = 59;

        private readonly Assembly _assembly;
        private readonly Dictionary<string, string> _resourcePaths;
        private readonly Dictionary<string, Sprite> _cachedSprites = new();
        private readonly Dictionary<string, Texture2D> _cachedTextures = new();
        private readonly Info _info;

        public class Info
        {
            public Dictionary<string, float> overridePPUs;
            public Dictionary<string, FilterMode> overrideFilterModes;
            public FilterMode defaultFilterMode = FilterMode.Bilinear;
            public float defaultPixelsPerUnit = 100f;

            public virtual float GetPixelsPerUnit(string name)
            {
                if (overridePPUs != null && overridePPUs.TryGetValue(name, out float ppu)) return ppu;
                return defaultPixelsPerUnit;
            }

            public virtual FilterMode GetFilterMode(string name)
            {
                if (overrideFilterModes != null && overrideFilterModes.TryGetValue(name, out FilterMode mode)) return mode;
                return defaultFilterMode;
            }
        }

        /// <summary>
        /// The SpriteManager with access to embedded MapChanger pngs.
        /// </summary>
        public static SpriteManager Instance { get; } = new(
            typeof(SpriteManager).Assembly,
            "MapChanger.Resources.Sprites.",
            new Info()
            {
                defaultFilterMode = FilterMode.Bilinear,
                defaultPixelsPerUnit = 100f,
            });

        /// <summary>
        /// Creates a SpriteManager to lazily load and cache Sprites from the embedded png files in the specified assembly.
        /// <br/>Only filepaths with the matching prefix are considered, and the prefix is removed to determine sprite names (e.g. "ItemChangerMod.Resources." is the prefix for Instance).
        /// <br/>Images will be loaded with default Bilinear filter mode and 100 pixels per unit.
        /// </summary>
        public SpriteManager(Assembly a, string resourcePrefix) : this(a, resourcePrefix, new()) { }

        /// <summary>
        /// Creates a SpriteManager to lazily load and cache Sprites from the embedded png files in the specified assembly.
        /// <br/>Only filepaths with the matching prefix are considered, and the prefix is removed to determine sprite names (e.g. "ItemChangerMod.Resources." is the prefix for Instance).
        /// </summary>
        public SpriteManager(Assembly a, string resourcePrefix, Info info)
        {
            _assembly = a;
            _resourcePaths = a.GetManifestResourceNames()
                .Where(n => n.EndsWith(".png") && n.StartsWith(resourcePrefix))
                .ToDictionary(n => n.Substring(resourcePrefix.Length, n.Length - resourcePrefix.Length - ".png".Length));
            _info = info;
        }

        /// <summary>
        /// Fetches the Sprite with the specified name. If it has not yet been loaded, loads it from embedded resources and caches the result.
        /// <br/>The name is the path of the image as an embedded resource, with the SpriteManager prefix and file extension removed.
        /// <br/>For example, the image at "ItemChanger.Resources.ShopIcons.Geo.png" has key "ShopIcons.Geo" in SpriteManager.Instance.
        /// </summary>
        public Sprite GetSprite(string name)
        {
            if (_cachedSprites.TryGetValue(name, out Sprite sprite)) return sprite;
            else if (_resourcePaths.TryGetValue(name, out string path))
            {
                using Stream s = _assembly.GetManifestResourceStream(path);
                return _cachedSprites[name] = Load(ToArray(s), _info.GetFilterMode(name), _info.GetPixelsPerUnit(name));
            }
            else
            {
                MapChangerMod.Instance.LogError($"{name} did not correspond to an embedded image file.");
                return Modding.CanvasUtil.NullSprite();
            }
        }

        public Texture2D GetTexture(string name)
        {
            if (_cachedTextures.TryGetValue(name, out Texture2D tex)) return tex;
            else if (_resourcePaths.TryGetValue(name, out string path))
            {
                byte[] data = ToArray(_assembly.GetManifestResourceStream(path));
                tex = new(1, 1, TextureFormat.RGBA32, true);
                tex.LoadImage(data, markNonReadable: true);
                tex.filterMode = _info.GetFilterMode(name);

                return _cachedTextures[name] = tex;
            }
            else
            {
                MapChangerMod.Instance.LogError($"{name} did not correspond to an embedded image file.");
                return null;
            }
        }

        /// <summary>
        /// Loads a sprite from the png file passed as a stream.
        /// </summary>
        public static Sprite Load(Stream data, FilterMode filterMode = FilterMode.Bilinear)
        {
            return Load(ToArray(data), filterMode);
        }

        /// <summary>
        /// Loads a sprite from the png file passed as a byte array.
        /// </summary>
        public static Sprite Load(byte[] data, FilterMode filterMode)
        {
            return Load(data, filterMode, 100f);
        }

        public static Sprite Load(byte[] data, FilterMode filterMode, float pixelsPerUnit)
        {
            Texture2D tex = new(1, 1, TextureFormat.RGBA32, true);
            tex.LoadImage(data, markNonReadable: true);
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
}
