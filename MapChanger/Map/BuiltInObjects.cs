using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace MapChanger.Map
{
    public class BuiltInObjects : HookModule
    {
        private static Dictionary<string, RoomSpriteGroupDef> roomSpriteGroupDefs;
        private static Dictionary<string, MiscObjectDef> miscObjectDefs;
        public static ReadOnlyDictionary<string, RoomSprite> MappedRooms { get; private set; }
        public static ReadOnlyDictionary<string, SelectableGroup<RoomSprite>> SelectableRooms { get; private set; }

        internal static void Load()
        {
            roomSpriteGroupDefs = JsonUtil.Deserialize<Dictionary<string, RoomSpriteGroupDef>>("MapChanger.Resources.roomSprites.json");
            miscObjectDefs = JsonUtil.Deserialize<Dictionary<string, MiscObjectDef>>("MapChanger.Resources.miscObjects.json");
        }

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
                if (t0.name.Contains("WHITE_PALACE") || t0.name.Contains("GODS_GLORY")) continue;

                foreach (Transform t1 in t0.transform)
                {
                    foreach (Transform t2 in t1.transform)
                    {
                        if (t2.name == "pin_blue_health")
                        {
                            LifebloodPin lifebloodPin = t2.gameObject.AddComponent<LifebloodPin>();
                            lifebloodPin.Initialize();
                        }
                        else if (t2.name == "pin_dream_tree")
                        {
                            WhisperingRootPin whisperingRootPin = t2.gameObject.AddComponent<WhisperingRootPin>();
                            whisperingRootPin.Initialize();
                        }
                    }
                }
            }
            
            Dictionary<string, RoomSprite> mappedRooms = [];
            Dictionary<string, SelectableGroup<RoomSprite>> selectableRooms = [];

            foreach ((string sceneName, RoomSpriteGroupDef rsgd) in roomSpriteGroupDefs.Select(kvp => (kvp.Key, kvp.Value)))
            {
                RoomSpriteDef rsd = new()
                {
                    ColorSetting = rsgd.ColorSetting,
                    SceneName = sceneName
                };

                List<RoomSprite> roomSprites = [];
                foreach (string objName in rsgd.RoomSprites)
                {
                    Transform child = goMap.transform.FindChildInHierarchy(objName);
                    if (child == null) continue;

                    child.gameObject.SetActive(false);
                    RoomSprite roomSprite = child.gameObject.AddComponent<RoomSprite>();
                    roomSprite.Initialize(rsd);

                    mappedRooms[roomSprite.name] = roomSprite;
                    roomSprites.Add(roomSprite);
                }

                if (!roomSprites.Any()) continue;

                SelectableGroup<RoomSprite> roomSpriteGroup = new(roomSprites);
                selectableRooms[sceneName] = roomSpriteGroup;
            }

            MappedRooms = new(mappedRooms);
            SelectableRooms = new(selectableRooms);

            // Disable extra map arrow
            goMap.transform.FindChildInHierarchy("Deepnest/Fungus2_25/Next Area (3)")?.gameObject.SetActive(false);

            foreach ((string pathName, MiscObjectDef mod) in miscObjectDefs.Select(kvp => (kvp.Key, kvp.Value)))
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
                    Transform areaName = child.FindChildInHierarchy("Area Name");
                    if (areaName != null)
                    {
                        NextAreaName nextAreaName = areaName.gameObject.AddComponent<NextAreaName>();
                        nextAreaName.Initialize(mod);
                    }

                    Transform mapArrow = child.FindChildInHierarchy("Map_Arrow");
                    if (mapArrow != null)
                    {
                        NextAreaArrow nextAreaArrow = mapArrow.gameObject.AddComponent<NextAreaArrow>();
                        nextAreaArrow.Initialize(mod);
                    }
                }
                else if (mod.Type == MiscObjectType.AreaName)
                {
                    AreaName areaName = child.gameObject.AddComponent<AreaName>();
                    areaName.Initialize(mod);
                }
            }

            GameObject goQmt = GameCameras.instance.hudCamera.transform.FindChildInHierarchy("Quick Map/Area Name").gameObject;
            goQmt.SetActive(false);
            QuickMapTitle qmt = goQmt.AddComponent<QuickMapTitle>();
            qmt.Initialize();

            MapChangerMod.Instance.LogDebug("~AttachMapModifiers");
        }

        internal static bool TryGetMapRoomPosition(string scene, out float x, out float y)
        {
            (x, y) = (0f, 0f);

            if (scene is null) return false;

            if (MappedRooms.TryGetValue(scene, out RoomSprite roomSprite))
            {
                Vector2 position = roomSprite.transform.parent.localPosition + roomSprite.transform.localPosition;
                (x, y) = (position.x, position.y);
                return true;
            }
            return false;
        }
    }
}
