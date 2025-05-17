using System;
using TMPro;
using UnityEngine;

namespace MapChanger.MonoBehaviours;

public class QuickMapTitle : ColoredMapObject
{
    private TextMeshPro _tmp;

    public static QuickMapTitle Instance { get; private set; }
    public override Vector4 Color
    {
        get => _tmp.color;
        set => _tmp.color = value;
    }

    public override void Initialize()
    {
        Instance = this;

        _tmp = GetComponent<TextMeshPro>();
        OrigColor = _tmp.color;

        gameObject.SetActive(true);

        MapObjectUpdater.Add(this);
    }

    public override void UpdateColor()
    {
        if (MapChangerMod.IsEnabled())
        {
            try
            {
                Color = ModeManager.CurrentMode().QuickMapTitleColorOverride(this) ?? OrigColor;
            }
            catch (Exception e)
            {
                MapChangerMod.Instance.LogError(e);
            }
        }
        else
        {
            ResetColor();
        }
    }
}
