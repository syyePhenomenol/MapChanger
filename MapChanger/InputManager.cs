using System.Collections.Generic;
using System.Linq;
using MapChanger.Input;
using MapChanger.MonoBehaviours;
using UnityEngine.SceneManagement;

namespace MapChanger;

public class InputManager : HookModule
{
    private static readonly HashSet<string> _nonGameplayScenes =
    [
        "Cinematic_Ending_A",
        "Cinematic_Ending_B",
        "Cinematic_Ending_C",
        "Cinematic_Ending_E",
        "Cinematic_MrMushroom",
        "Cinematic_Stag_travel",
        "Cutscene_Boss_Door",
        "End_Credits",
        "End_Game_Completion",
        "Menu_Credits",
        "Menu_Title",
        "Opening_Sequence",
        "PermaDeath",
        "PermaDeath_Unlock",
    ];

    private static readonly HashSet<string> _bindingNames = [];
    private static readonly List<BindableInput> _bindableInputs = [];
    private static InputListener _il;

    public static void AddRange(IEnumerable<BindableInput> inputs)
    {
        foreach (var input in inputs)
        {
            Add(input);
        }
    }

    public static void Add(BindableInput input)
    {
        _bindableInputs.Add(input);

        if (!Dependencies.HasDebugMod)
        {
            return;
        }

        if (!_bindingNames.Contains(input.Name))
        {
            DebugModInterop.AddBinding(input.Name, input.Category);
            _ = _bindingNames.Add(input.Name);
        }
    }

    public override void OnEnterGame()
    {
        var useInputs = _bindableInputs.Where(b => b.UseCondition());

        if (!useInputs.Any())
        {
            return;
        }

        _il = Utils.MakeMonoBehaviour<InputListener>(null, "MapChanger Input Listener");
        _il.Initialize(useInputs);

        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ActiveSceneChanged;
    }

    public override void OnQuitToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ActiveSceneChanged;

        _il?.Destroy();
        _il = null;
    }

    private static void ActiveSceneChanged(Scene from, Scene to)
    {
        _il?.gameObject?.SetActive(!_nonGameplayScenes.Contains(to.name));
    }
}
