using UnityEngine;

namespace MapChanger;

/// <summary>
/// Built-in color settings that are MapMod agnostic.
/// </summary>
public enum ColorSetting
{
    None,

    UI_On,
    UI_Neutral,
    UI_Custom,
    UI_Disabled,
    UI_Special,
    UI_Borders,

    Map_Ancient_Basin,
    Map_City_of_Tears,
    Map_Crystal_Peak,
    Map_Deepnest,
    Map_Dirtmouth,
    Map_Fog_Canyon,
    Map_Forgotten_Crossroads,
    Map_Fungal_Wastes,
    Map_Godhome,
    Map_Greenpath,
    Map_Howling_Cliffs,
    Map_Kingdoms_Edge,
    Map_Queens_Gardens,
    Map_Resting_Grounds,
    Map_Royal_Waterways,
    Map_White_Palace,

    Map_Abyss,
    Map_Hive,
    Map_Ismas_Grove,
    Map_Mantis_Village,
    Map_Queens_Station,
    Map_Soul_Sanctum,
    Map_Watchers_Spire,
}

public static class Colors
{
    /// <summary>
    /// For the default colors of the map rooms, use RoomSprite.OrigColor.
    /// </summary>
    /// <param name="colorSetting"></param>
    public static Vector4 GetColor(ColorSetting colorSetting)
    {
        return colorSetting switch
        {
            ColorSetting.None or ColorSetting.UI_Neutral or ColorSetting.UI_Borders => Color.white,
            ColorSetting.UI_On => Color.green,
            ColorSetting.UI_Custom => Color.yellow,
            ColorSetting.UI_Disabled => Color.red,
            ColorSetting.UI_Special => Color.cyan,
            _ => Vector4.negativeInfinity,
        };
    }
}
