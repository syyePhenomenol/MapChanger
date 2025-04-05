using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace MapChanger.Map;

public class BuiltInObjects : HookModule
{
    public static ReadOnlyDictionary<string, RoomSprite> MappedRooms { get; private set; }
    public static ReadOnlyDictionary<string, SelectableGroup<RoomSprite>> SelectableRooms { get; private set; }

    public override void OnEnterGame()
    {
        Events.OnSetGameMapInternal += Make;
    }

    public override void OnQuitToMenu()
    {
        MappedRooms = null;
        SelectableRooms = null;

        Events.OnSetGameMapInternal -= Make;
    }

    private static void Make(GameObject goMap)
    {
        MapChangerMod.Instance.LogDebug("AttachMapModifiers");

        // Destroy empty Ruins1_31b object and rename the actual one to it
        if (goMap.transform.FindChildInHierarchy("City of Tears/Ruins1_31b") is Transform emptyRoom)
        {
            Object.Destroy(emptyRoom.gameObject);
        }

        if (goMap.transform.FindChildInHierarchy("City of Tears/Ruins1_31_top_2") is Transform elegantKeyRoom)
        {
            elegantKeyRoom.name = "Ruins1_31b";
        }

        foreach (Transform t0 in goMap.transform)
        {
            if (t0.name.Contains("WHITE_PALACE") || t0.name.Contains("GODS_GLORY"))
            {
                continue;
            }

            foreach (Transform t1 in t0.transform)
            {
                foreach (Transform t2 in t1.transform)
                {
                    if (t2.name == "pin_blue_health")
                    {
                        var lifebloodPin = t2.gameObject.AddComponent<LifebloodPin>();
                        lifebloodPin.Initialize();
                    }
                    else if (t2.name == "pin_dream_tree")
                    {
                        var whisperingRootPin = t2.gameObject.AddComponent<WhisperingRootPin>();
                        whisperingRootPin.Initialize();
                    }
                }
            }
        }

        Dictionary<string, RoomSprite> mappedRooms = [];
        Dictionary<string, SelectableGroup<RoomSprite>> selectableRooms = [];

        var roomSpriteGroupDefs = JsonUtil.Deserialize<Dictionary<string, RoomSpriteGroupDef>>(
            "MapChanger.Resources.roomSprites.json"
        );

        foreach ((var sceneName, var rsgd) in roomSpriteGroupDefs.Select(kvp => (kvp.Key, kvp.Value)))
        {
            RoomSpriteDef rsd = new() { ColorSetting = rsgd.ColorSetting, SceneName = sceneName };

            List<RoomSprite> roomSprites = [];
            foreach (var objName in rsgd.RoomSprites)
            {
                var child = goMap.transform.FindChildInHierarchy(objName);
                if (child == null)
                {
                    continue;
                }

                child.gameObject.SetActive(false);
                var roomSprite = child.gameObject.AddComponent<RoomSprite>();
                roomSprite.Initialize(rsd);

                mappedRooms[roomSprite.name] = roomSprite;
                roomSprites.Add(roomSprite);
            }

            if (roomSprites.Count == 0)
            {
                continue;
            }

            selectableRooms[sceneName] = new(roomSprites);
        }

        MappedRooms = new(mappedRooms);
        SelectableRooms = new(selectableRooms);

        // Disable extra map arrow
        goMap.transform.FindChildInHierarchy("Deepnest/Fungus2_25/Next Area (3)")?.gameObject.SetActive(false);

        var miscObjectDefs = JsonUtil.Deserialize<Dictionary<string, MiscObjectDef>>(
            "MapChanger.Resources.miscObjects.json"
        );

        foreach ((var pathName, var mod) in miscObjectDefs.Select(kvp => (kvp.Key, kvp.Value)))
        {
            Transform child;
            if (pathName.Contains("*"))
            {
                child = goMap.transform.FindChildInHierarchy(pathName.Split('*')[0]).GetChild(1);
            }
            else
            {
                child = goMap.transform.FindChildInHierarchy(pathName);
            }

            if (child == null)
            {
                MapChangerMod.Instance.LogDebug($"Transform not found: {pathName}");
                continue;
            }

            child.gameObject.SetActive(true);

            if (mod.Type == MiscObjectType.NextArea)
            {
                var areaName = child.FindChildInHierarchy("Area Name");
                if (areaName != null)
                {
                    var nextAreaName = areaName.gameObject.AddComponent<NextAreaName>();
                    nextAreaName.Initialize(mod);
                }

                var mapArrow = child.FindChildInHierarchy("Map_Arrow");
                if (mapArrow != null)
                {
                    var nextAreaArrow = mapArrow.gameObject.AddComponent<NextAreaArrow>();
                    nextAreaArrow.Initialize(mod);
                }
            }
            else if (mod.Type == MiscObjectType.AreaName)
            {
                var areaName = child.gameObject.AddComponent<AreaName>();
                areaName.Initialize(mod);
            }
        }

        var goQmt = GameCameras.instance.hudCamera.transform.FindChildInHierarchy("Quick Map/Area Name").gameObject;
        goQmt.SetActive(false);
        var qmt = goQmt.AddComponent<QuickMapTitle>();
        qmt.Initialize();

        MapChangerMod.Instance.LogDebug("~AttachMapModifiers");
    }

    internal static bool TryGetMapRoomPosition(string scene, out float x, out float y)
    {
        (x, y) = (0f, 0f);

        if (scene is null)
        {
            return false;
        }

        if (MappedRooms.TryGetValue(scene, out var roomSprite))
        {
            Vector2 position = roomSprite.transform.parent.localPosition + roomSprite.transform.localPosition;
            (x, y) = (position.x, position.y);
            return true;
        }

        return false;
    }
}
