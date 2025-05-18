using System;
using System.Collections.Generic;
using MapChanger;

public static class Localization
{
    private static readonly List<Func<string, string>> _localizers = [];

    /// <summary>
    /// For other mods to register their own translation method
    /// </summary>
    /// <param name="localizer"></param>
    public static void AddLocalizer(Func<string, string> localizer)
    {
        _localizers.Add(localizer);
    }

    /// <summary>
    /// Localize text
    /// </summary>
    /// <param name="text"></param>
    public static string L(this string text)
    {
        foreach (var localizer in _localizers)
        {
            if (localizer?.Invoke(text) is string externalResult)
            {
                return externalResult;
            }
        }

        return text;
    }

    /// <summary>
    /// Localize and clean text
    /// </summary>
    /// <param name="text"></param>
    public static string LC(this string text)
    {
        return text.L().ToCleanName();
    }
}
