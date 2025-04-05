using MapChanger.Defs;
using UnityEngine;

namespace MapChanger.MonoBehaviours;

public class Pin : MapObject
{
    private bool _snapPosition = true;

    private IMapPosition _mapPosition;

    private float _size = 1f;

    public bool SnapPosition
    {
        get => _snapPosition;
        set
        {
            if (_snapPosition != value)
            {
                _snapPosition = value;
                UpdatePosition();
            }
        }
    }

    public IMapPosition MapPosition
    {
        get => _mapPosition;
        set
        {
            if (_mapPosition != value)
            {
                _mapPosition = value;
                UpdatePosition();
            }
        }
    }

    protected SpriteRenderer Sr { get; private set; }

    public Sprite Sprite
    {
        get => Sr.sprite;
        set => Sr.sprite = value;
    }

    public Vector4 Color
    {
        get => Sr.color;
        set => Sr.color = value;
    }

    public float Size
    {
        get => _size;
        set
        {
            _size = value;
            transform.localScale = new(_size, _size, transform.localScale.z);
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        GameObject goPinSprite = new($"{transform.name} Pin Sprite") { layer = UI_LAYER };
        goPinSprite.transform.SetParent(transform, false);

        Sr = goPinSprite.AddComponent<SpriteRenderer>();
        Sr.sortingLayerName = HUD;
    }

    private void UpdatePosition()
    {
        if (_mapPosition is null)
        {
            return;
        }

        if (_snapPosition)
        {
            transform.localPosition = new Vector3(
                _mapPosition.X.Snap(),
                _mapPosition.Y.Snap(),
                transform.localPosition.z
            );
        }
        else
        {
            transform.localPosition = new Vector3(_mapPosition.X, _mapPosition.Y, transform.localPosition.z);
        }
    }
}
