using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public interface ISelectable
    {
        bool Selected { get; set; }
        bool CanSelect();
        (string, Vector2) GetKeyAndPosition();
    }
}
