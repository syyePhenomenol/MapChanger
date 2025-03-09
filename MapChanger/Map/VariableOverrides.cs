using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Newtonsoft.Json;

namespace MapChanger.Map
{
    /// <summary>
    /// Replaces the name of variables, so we can override their values
    /// to e.g. enable full map or disable vanilla pins with the mod enabled.
    /// </summary>
    internal class VariableOverrides : HookModule
    {
        private enum OverrideType
        {
            Map,
            Pins,
            Markers
        }

        private record FsmBoolOverrideDef
        {
            [JsonProperty]
            internal Dictionary<string, FsmActionBoolOverride[]> BoolsIndex { get; init; }
            [JsonProperty]
            internal Dictionary<string, FsmActionBoolRangeOverride> BoolsRange { get; init; }
        }

        private record FsmActionBoolOverride
        {
            [JsonProperty]
            internal int Index { get; init; }
            [JsonProperty]
            internal OverrideType Type { get; init; }
        }

        private record FsmActionBoolRangeOverride
        {
            [JsonProperty]
            internal int Range { get; init; }
            [JsonProperty]
            internal OverrideType Type { get; init; }
        }

        internal const string MAP_PREFIX = "MCO0";
        internal const string PINS_PREFIX = "MCO1";
        internal const string MARKERS_PREFIX = "MC02";
        internal const string HAS_MAP = "hasMap";
        internal const string GOT_WHITE_PALACE_MAP = "AdditionalMapsGotWpMap";
        internal const string GOT_GODHOME_MAP = "AdditionalMapsGotGhMap";

        private static readonly Dictionary<string, string> ilVariables = new()
        {
            //{ "hasQuill", MAP_PREFIX },
            { "mapAllRooms", MAP_PREFIX },
            { "mapAbyss", MAP_PREFIX },
            { "mapCity", MAP_PREFIX },
            { "mapCliffs", MAP_PREFIX },
            { "mapCrossroads", MAP_PREFIX },
            { "mapMines", MAP_PREFIX },
            { "mapDeepnest", MAP_PREFIX },
            { "mapFogCanyon", MAP_PREFIX },
            { "mapFungalWastes", MAP_PREFIX },
            { "mapGreenpath", MAP_PREFIX },
            { "mapOutskirts", MAP_PREFIX },
            { "mapRoyalGardens", MAP_PREFIX },
            { "mapRestingGrounds", MAP_PREFIX },
            { "mapWaterways", MAP_PREFIX },
            { "AdditionalMapsGotWpMap", MAP_PREFIX },
            { "AdditionalMapsGotGhMap", MAP_PREFIX }
        };

        private static Dictionary<string, FsmBoolOverrideDef> fsmOverrideDefs;

        internal static void Load()
        {
            fsmOverrideDefs = JsonUtil.Deserialize<Dictionary<string, FsmBoolOverrideDef>>("MapChanger.Resources.fsmOverrides.json");
        }

        public override void OnEnterGame()
        {
            On.PlayMakerFSM.Start += ReplaceVariablesFSM;

            IL.GameMap.WorldMap += ReplaceVariablesIL;
            IL.RoughMapRoom.OnEnable += ReplaceVariablesIL;

            ModHooks.GetPlayerBoolHook += GetBoolOverride;
            ModHooks.GetPlayerVariableHook += GetVariableOverride;
        }

        public override void OnQuitToMenu()
        {
            On.PlayMakerFSM.Start -= ReplaceVariablesFSM;

            IL.GameMap.WorldMap -= ReplaceVariablesIL;
            IL.RoughMapRoom.OnEnable -= ReplaceVariablesIL;

            ModHooks.GetPlayerBoolHook -= GetBoolOverride;
            ModHooks.GetPlayerVariableHook -= GetVariableOverride;
        }

        private static void ReplaceVariablesFSM(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
        {
            orig(self);

            try
            {
                if (fsmOverrideDefs.TryGetValue(self.FsmName, out FsmBoolOverrideDef fod)
                    || fsmOverrideDefs.TryGetValue(self.name + "-" + self.FsmName, out fod))
                {
                    // MapChangerMod.Instance.LogFine($"Applying Fsm bool overrides to {self.name}-{self.FsmName}");
                    foreach (FsmState state in self.FsmStates)
                    {
                        if (fod.BoolsIndex.TryGetValue(state.Name, out FsmActionBoolOverride[] overrides))
                        {
                            foreach (FsmActionBoolOverride boolOverride in overrides)
                            {
                                // MapChangerMod.Instance.LogFine($"    Replacing bool in {state.Name} at index {boolOverride.Index} of type {boolOverride.Type}");
                                ReplaceBool(state, boolOverride.Index, boolOverride.Type);
                            }
                        }
                        if (fod.BoolsRange.TryGetValue(state.Name, out FsmActionBoolRangeOverride overrideRange))
                        {
                            // MapChangerMod.Instance.LogFine($"    Replacing bools in {state.Name} over range {overrideRange.Range} of type {overrideRange.Type}");
                            for (int i = 0; i < overrideRange.Range; i++)
                            {
                                ReplaceBool(state, i, overrideRange.Type);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MapChangerMod.Instance.LogError(e);
            }

            static void ReplaceBool(FsmState state, int index, OverrideType type)
            {
                if (index >= state.Actions.Length) return;

                FsmStateAction action = state.Actions[index];
                if (action.GetType() == typeof(PlayerDataBoolTest))
                {
                    PlayerDataBoolTest boolTest = (PlayerDataBoolTest)action;
                    boolTest.boolName = NewBoolName(boolTest.boolName, type);
                    return;
                }
                if (action.GetType() == typeof(GetPlayerDataBool))
                {
                    GetPlayerDataBool getBool = (GetPlayerDataBool)action;
                    getBool.boolName = NewBoolName(getBool.boolName, type);
                    return;
                }
                if (action.GetType() == typeof(PlayerDataBoolAllTrue))
                {
                    PlayerDataBoolAllTrue pdat = (PlayerDataBoolAllTrue)action;
                    pdat.stringVariables = pdat.stringVariables.Select(boolName => NewBoolName(boolName, type)).ToArray();
                    return;
                }
                MapChangerMod.Instance.LogWarn($"Unrecognized FsmAction: {state.Name}, {index}");
                return;

                static FsmString NewBoolName(FsmString name, OverrideType type)
                {
                    if (name.ToString().StartsWith(MAP_PREFIX)
                        || name.ToString().StartsWith(PINS_PREFIX)
                        || name.ToString().StartsWith(MARKERS_PREFIX))
                    {
                        return name;
                    }
                    return type switch
                    {
                        OverrideType.Map => MAP_PREFIX + name,
                        OverrideType.Pins => PINS_PREFIX + name,
                        OverrideType.Markers => MARKERS_PREFIX + name,
                        _ => name
                    };
                }
            }
        }
        
        private static void ReplaceVariablesIL(ILContext il)
        {
            ILCursor cursor = new(il);
            while (cursor.TryGotoNext(instr => instr.OpCode == OpCodes.Ldstr && ilVariables.ContainsKey((string)instr.Operand)))
            {
                string name = cursor.ToString().Split('\"')[1];
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, ilVariables[name] + name);
            }
        }

        private static bool GetBoolOverride(string name, bool orig)
        {
            if (name is GOT_WHITE_PALACE_MAP or GOT_GODHOME_MAP
                && Settings.MapModEnabled() && Settings.CurrentMode().FullMap)
            {
                return true;
            }

            if (Settings.MapModEnabled())
            {
                if (name.StartsWith(MAP_PREFIX))
                {
                    if (name.EndsWith(HAS_MAP))
                    {
                        if (Settings.CurrentMode().ForceHasMap)
                        {
                            return true;
                        }
                    }
                    if (Settings.CurrentMode().FullMap)
                    {
                        return true;
                    }
                    return GetOriginalBool(name);
                }
                if (name.StartsWith(PINS_PREFIX))
                {
                    return Settings.CurrentMode().VanillaPins ?? GetOriginalBool(name);
                }
                if (name.StartsWith(MARKERS_PREFIX))
                {
                    return Settings.CurrentMode().MapMarkers ?? GetOriginalBool(name);
                }
            }
            else if (name.StartsWith(MAP_PREFIX) || name.StartsWith(PINS_PREFIX) || name.StartsWith(MARKERS_PREFIX))
            {
                return GetOriginalBool(name);
            }

            return orig;

            static bool GetOriginalBool(string name)
            {
                return PlayerData.instance.GetBool(name.Remove(0, MAP_PREFIX.Length));
            }
        }

        private static object GetVariableOverride(Type type, string name, object value)
        {
            if (name is "scenesMapped"
                && Settings.MapModEnabled()
                && Settings.CurrentMode().ImmediateMapUpdate
                && (PlayerData.instance.GetBool("hasQuill") || Settings.CurrentMode().ForceHasQuill))
            {
                return Tracker.ScenesVisited.ToList();
            }
            if (!name.StartsWith(MAP_PREFIX) && !name.StartsWith(PINS_PREFIX))
            {
                return value;
            }
            if (!Settings.MapModEnabled())
            {
                return GetOriginalVariable<List<string>>(name);
            }
            if (name.StartsWith(PINS_PREFIX) && type == typeof(List<string>))
            {
                if ((!Settings.CurrentMode().VanillaPins) ?? false)
                {
                    return new List<string> { };
                }
                return GetOriginalVariable<List<string>>(name);
            }
            return value;

            static object GetOriginalVariable<T>(string name)
            {
                return PlayerData.instance.GetVariable<T>(name.Remove(0, MAP_PREFIX.Length));
            }
        }
    }
}
