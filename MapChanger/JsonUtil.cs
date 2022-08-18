using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Reflection;

// Code taken from homothety: https://github.com/homothetyhk/RandomizerMod/

namespace MapChanger
{
    public static class JsonUtil
    {
        private static readonly JsonSerializer _js;

        public static T Deserialize<T>(string embeddedResourcePath)
        {
            using StreamReader sr = new(typeof(JsonUtil).Assembly.GetManifestResourceStream(embeddedResourcePath));
            using JsonTextReader jtr = new(sr);
            return _js.Deserialize<T>(jtr);
        }

        public static T DeserializeFromAssembly<T>(Assembly assembly, string embeddedResourcePath)
        {
            using StreamReader sr = new(assembly.GetManifestResourceStream(embeddedResourcePath));
            using JsonTextReader jtr = new(sr);
            return _js.Deserialize<T>(jtr);
        }

        public static T DeserializeFromExternalFile<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                using StreamReader sr = new(filePath);
                using JsonTextReader jtr = new(sr);
                return _js.Deserialize<T>(jtr);
            }

            return default;
        }

        public static T DeserializeString<T>(string json)
        {
            using StringReader sr = new(json);
            using JsonTextReader jtr = new(sr);
            return _js.Deserialize<T>(jtr);
        }

        public static void Serialize(object o, string fileName)
        {
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(typeof(JsonUtil).Assembly.Location), fileName), Serialize(o));
        }

        public static string Serialize(object o)
        {
            using StringWriter sw = new();
            _js.Serialize(sw, o);
            sw.Flush();
            return sw.ToString();
        }

        static JsonUtil()
        {
            _js = new JsonSerializer
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
            };

            _js.Converters.Add(new StringEnumConverter());
        }
    }
}