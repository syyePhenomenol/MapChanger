using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public interface ISelectable
    {
        bool Selected { get; set; }
        string Key { get; }
        Vector2 Position { get; }
        bool CanSelect();
    }
}
