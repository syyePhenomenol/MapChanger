using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using HutongGames.PlayMaker;
using MapChanger.Map;
using MapChanger.UI;
using UnityEngine;
using Vasi;

namespace MapChanger;

public static class Events
{
    internal static readonly List<HookModule> HookModules =
    [
        new Settings(),
        new Tracker(),
        new VariableOverrides(),
        new BehaviourChanges(),
        new BuiltInObjects(),
        new PauseMenu(),
        new MapUILayerUpdater(),
        new MapObjectUpdater(),
    ];

    private static readonly MapZone[] _customMapZones = [MapZone.WHITE_PALACE, MapZone.GODS_GLORY];

    public static event Action OnEnterGame;
    public static event Action OnQuitToMenu;
    internal static event Action<GameObject> OnSetGameMapInternal;
    public static event Action<GameObject> OnSetGameMap;
    internal static event Action<GameMap> OnWorldMapInternal;
    public static event Action<GameMap> OnWorldMap;
    public static event Action<GameMap, MapZone> OnQuickMap;
    public static event Action<GameMap> OnCloseMap;

    internal static void Initialize()
    {
        On.GameManager.StartNewGame += StartNewGame;
        On.GameManager.ContinueGame += ContinueGame;
        On.QuitToMenu.Start += QuitToMenu;
    }

    private static void StartNewGame(
        On.GameManager.orig_StartNewGame orig,
        GameManager self,
        bool permadeathMode,
        bool bossRushMode
    )
    {
        orig(self, permadeathMode, bossRushMode);
        EnterGame();
    }

    private static void ContinueGame(On.GameManager.orig_ContinueGame orig, GameManager self)
    {
        orig(self);
        EnterGame();
    }

    private static void EnterGame()
    {
        // Allow map mods to inject map modes before MapChangerMod does anything
        try
        {
            OnEnterGame?.Invoke();
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }

        // Disable all changes if no modes were added
        if (!Settings.HasModes())
        {
            return;
        }

        On.GameManager.SetGameMap += SetGameMap;
        On.GameMap.WorldMap += WorldMap;
        On.GameMap.QuickMapAncientBasin += QuickMapAncientBasin;
        On.GameMap.QuickMapCity += QuickMapCity;
        On.GameMap.QuickMapCliffs += QuickMapCliffs;
        On.GameMap.QuickMapCrossroads += QuickMapCrossroads;
        On.GameMap.QuickMapCrystalPeak += QuickMapCrystalPeak;
        On.GameMap.QuickMapDeepnest += QuickMapDeepnest;
        On.GameMap.QuickMapDirtmouth += QuickMapDirtmouth;
        On.GameMap.QuickMapFogCanyon += QuickMapFogCanyon;
        On.GameMap.QuickMapFungalWastes += QuickMapFungalWastes;
        On.GameMap.QuickMapGreenpath += QuickMapGreenpath;
        On.GameMap.QuickMapKingdomsEdge += QuickMapKingdomsEdge;
        On.GameMap.QuickMapQueensGardens += QuickMapQueensGardens;
        On.GameMap.QuickMapRestingGrounds += QuickMapRestingGrounds;
        On.GameMap.QuickMapWaterways += QuickMapWaterways;
        On.GameMap.CloseQuickMap += CloseMap;

        foreach (var hookModule in HookModules)
        {
            hookModule.OnEnterGame();
        }
    }

    private static IEnumerator QuitToMenu(On.QuitToMenu.orig_Start orig, QuitToMenu self)
    {
        try
        {
            OnQuitToMenu?.Invoke();
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }

        if (!Settings.HasModes())
        {
            return orig(self);
        }

        On.GameManager.SetGameMap -= SetGameMap;
        On.GameMap.WorldMap -= WorldMap;
        On.GameMap.QuickMapAncientBasin -= QuickMapAncientBasin;
        On.GameMap.QuickMapCity -= QuickMapCity;
        On.GameMap.QuickMapCliffs -= QuickMapCliffs;
        On.GameMap.QuickMapCrossroads -= QuickMapCrossroads;
        On.GameMap.QuickMapCrystalPeak -= QuickMapCrystalPeak;
        On.GameMap.QuickMapDeepnest -= QuickMapDeepnest;
        On.GameMap.QuickMapDirtmouth -= QuickMapDirtmouth;
        On.GameMap.QuickMapFogCanyon -= QuickMapFogCanyon;
        On.GameMap.QuickMapFungalWastes -= QuickMapFungalWastes;
        On.GameMap.QuickMapGreenpath -= QuickMapGreenpath;
        On.GameMap.QuickMapKingdomsEdge -= QuickMapKingdomsEdge;
        On.GameMap.QuickMapQueensGardens -= QuickMapQueensGardens;
        On.GameMap.QuickMapRestingGrounds -= QuickMapRestingGrounds;
        On.GameMap.QuickMapWaterways -= QuickMapWaterways;
        On.GameMap.CloseQuickMap -= CloseMap;

        foreach (var hookModule in HookModules)
        {
            hookModule.OnQuitToMenu();
        }

        return orig(self);
    }

    private static void SetGameMap(On.GameManager.orig_SetGameMap orig, GameManager self, GameObject goMap)
    {
        orig(self, goMap);

        // Get quick map for custom map zones to invoke properly
        var goQuickMap = GameObject.Find("Quick Map");
        var fsmQuickMap = goQuickMap.LocateMyFSM("Quick Map");

        foreach (var mapZone in _customMapZones)
        {
            if (fsmQuickMap.TryGetState(mapZone.ToString(), out var state))
            {
                state.AddAction(new QuickMapCustom(goQuickMap.GetComponent<GameMap>(), mapZone));
            }
        }

        // Used to set up BuiltInObjects before map mods do
        try
        {
            OnSetGameMapInternal?.Invoke(goMap);
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }

        try
        {
            OnSetGameMap?.Invoke(goMap);
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }
    }

    private static void WorldMap(On.GameMap.orig_WorldMap orig, GameMap self)
    {
        States.WorldMapOpen = true;
        States.CurrentMapZone = MapZone.NONE;

        try
        {
            OnWorldMap?.Invoke(self);
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }

        orig(self);

        // Used to override panning range properly
        try
        {
            OnWorldMapInternal?.Invoke(self);
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }
    }

    private static void QuickMap(GameMap gameMap, MapZone mapZone)
    {
        States.QuickMapOpen = true;
        States.CurrentMapZone = mapZone;

        try
        {
            OnQuickMap?.Invoke(gameMap, mapZone);
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }
    }

    private static void CloseMap(On.GameMap.orig_CloseQuickMap orig, GameMap self)
    {
        States.WorldMapOpen = false;
        States.QuickMapOpen = false;
        States.CurrentMapZone = MapZone.NONE;

        try
        {
            OnCloseMap?.Invoke(self);
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }

        orig(self);
    }

    private static void QuickMapAncientBasin(On.GameMap.orig_QuickMapAncientBasin orig, GameMap self)
    {
        QuickMap(self, MapZone.ABYSS);
        orig(self);
    }

    private static void QuickMapCity(On.GameMap.orig_QuickMapCity orig, GameMap self)
    {
        QuickMap(self, MapZone.CITY);
        orig(self);
    }

    private static void QuickMapCliffs(On.GameMap.orig_QuickMapCliffs orig, GameMap self)
    {
        QuickMap(self, MapZone.CLIFFS);
        orig(self);
    }

    private static void QuickMapCrossroads(On.GameMap.orig_QuickMapCrossroads orig, GameMap self)
    {
        QuickMap(self, MapZone.CROSSROADS);
        orig(self);
    }

    private static void QuickMapCrystalPeak(On.GameMap.orig_QuickMapCrystalPeak orig, GameMap self)
    {
        QuickMap(self, MapZone.MINES);
        orig(self);
    }

    private static void QuickMapDeepnest(On.GameMap.orig_QuickMapDeepnest orig, GameMap self)
    {
        QuickMap(self, MapZone.DEEPNEST);
        orig(self);
    }

    private static void QuickMapDirtmouth(On.GameMap.orig_QuickMapDirtmouth orig, GameMap self)
    {
        QuickMap(self, MapZone.TOWN);
        orig(self);
    }

    private static void QuickMapFogCanyon(On.GameMap.orig_QuickMapFogCanyon orig, GameMap self)
    {
        QuickMap(self, MapZone.FOG_CANYON);
        orig(self);
    }

    private static void QuickMapFungalWastes(On.GameMap.orig_QuickMapFungalWastes orig, GameMap self)
    {
        QuickMap(self, MapZone.WASTES);
        orig(self);
    }

    private static void QuickMapGreenpath(On.GameMap.orig_QuickMapGreenpath orig, GameMap self)
    {
        QuickMap(self, MapZone.GREEN_PATH);
        orig(self);
    }

    private static void QuickMapKingdomsEdge(On.GameMap.orig_QuickMapKingdomsEdge orig, GameMap self)
    {
        QuickMap(self, MapZone.OUTSKIRTS);
        orig(self);
    }

    private static void QuickMapQueensGardens(On.GameMap.orig_QuickMapQueensGardens orig, GameMap self)
    {
        QuickMap(self, MapZone.ROYAL_GARDENS);
        orig(self);
    }

    private static void QuickMapRestingGrounds(On.GameMap.orig_QuickMapRestingGrounds orig, GameMap self)
    {
        QuickMap(self, MapZone.RESTING_GROUNDS);
        orig(self);
    }

    private static void QuickMapWaterways(On.GameMap.orig_QuickMapWaterways orig, GameMap self)
    {
        QuickMap(self, MapZone.WATERWAYS);
        orig(self);
    }

    private class QuickMapCustom : FsmStateAction
    {
        private readonly MapZone _mapZone;
        private readonly GameMap _gameMap;

        internal QuickMapCustom(GameMap gameMap, MapZone mapZone)
        {
            _mapZone = mapZone;
            _gameMap = gameMap;
        }

        public override void OnEnter()
        {
            QuickMap(_gameMap, _mapZone);
            Finish();
        }
    }
}
