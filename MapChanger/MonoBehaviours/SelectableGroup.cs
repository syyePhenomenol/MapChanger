using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MapChanger.MonoBehaviours;
using UnityEngine;

public class SelectableGroup<T>(List<T> selectables) : ISelectable
    where T : ISelectable
{
    // The offset of the selection position from the first room sprite.
    private readonly Vector2 _averagePositionOffset =
        (new Vector2(selectables.Sum(s => s.Position.x), selectables.Sum(s => s.Position.y)) / selectables.Count())
        - selectables.First().Position;

    public ReadOnlyCollection<T> Selectables => new(selectables);

    public bool Selected
    {
        get => selectables.First().Selected;
        set
        {
            foreach (var rs in selectables)
            {
                rs.Selected = value;
            }
        }
    }

    public string Key => selectables.First().Key;
    public Vector2 Position => selectables.First().Position + _averagePositionOffset;

    public bool CanSelect()
    {
        return selectables.Any(s => s.CanSelect());
    }
}
