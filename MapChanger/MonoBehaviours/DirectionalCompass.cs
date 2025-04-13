using System.Collections.Generic;
using System.Linq;
using MapChanger.Defs;
using UnityEngine;

namespace MapChanger.MonoBehaviours;

/// <summary>
/// Points from some entity to the nearest of a group of objects, during gameplay.
/// </summary>
public class DirectionalCompass : MonoBehaviour
{
    private GameObject _compassInternal;
    private SpriteRenderer _sr;
    private CompassInfo _info;
    private Vector2 _compassCenter;
    private float _lerpStartTime;
    private Vector2 _currentDir;
    private float _currentAngle;

    public CompassTarget CurrentTarget { get; private set; }

    public static GameObject Make(CompassInfo ci)
    {
        // This object is a container for the script. Can be set active/inactive externally to control script
        GameObject compass = new(ci.Name);
        DontDestroyOnLoad(compass);

        var dc = compass.AddComponent<DirectionalCompass>();

        dc._compassInternal = new($"{ci.Name} Internal", typeof(SpriteRenderer));
        dc._sr = dc._compassInternal.GetComponent<SpriteRenderer>();
        dc._sr.sortingLayerName = "HUD";
        dc._compassInternal.transform.parent = compass.transform;

        dc._info = ci;

        return compass;
    }

    public void Destroy()
    {
        Destroy(_compassInternal);
        Destroy(gameObject);
    }

    public void FixedUpdate()
    {
        foreach (var compassTarget in _info.CompassTargets.Where(ct => ct.IsActive()))
        {
            compassTarget.Position.UpdateEveryFrame();
        }
    }

    public void Update()
    {
        if (_info.ActiveCondition() && _info.CompassTargets.Any() && TryUpdateClosestLocation())
        {
            _sr.sprite = CurrentTarget.GetSprite();

            var dir = (Vector2)CurrentTarget.Position.Value - _compassCenter;

            dir.Scale(Vector2.one * 0.5f);

            var angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90f;

            // Clamp to radius
            dir = Vector2.ClampMagnitude(dir, _info.Radius);

            // Do lerp stuff
            if (_info.Lerp && Time.time - _lerpStartTime < _info.LerpDuration)
            {
                dir = Vector2.Lerp(_currentDir, dir, (Time.time - _lerpStartTime) / _info.LerpDuration);
                angle = Mathf.LerpAngle(_currentAngle, angle, (Time.time - _lerpStartTime) / _info.LerpDuration);
            }

            _currentDir = dir;
            _currentAngle = angle;

            _compassInternal.transform.position = new Vector3(_compassCenter.x + dir.x, _compassCenter.y + dir.y, 0f);

            if (_info.RotateSprite)
            {
                _compassInternal.transform.eulerAngles = new(0, 0, angle);
            }

            var inverseRadius = _info.Radius is not 0f ? dir.magnitude / _info.Radius : 1f;

            _compassInternal.transform.localScale = Vector3.Scale(
                new Vector3(inverseRadius * _info.Scale, inverseRadius * _info.Scale, 1f),
                CurrentTarget.GetScale()
            );
            _sr.color = inverseRadius * CurrentTarget.GetColor();

            _compassInternal.SetActive(true);
        }
        else
        {
            _compassInternal.SetActive(false);
        }
    }

    private bool TryUpdateClosestLocation()
    {
        _compassCenter = _info.GetCompassCenter();

        CompassTarget newTarget = null;

        if (
            _info.CompassTargets.Where(ct => ct.IsActive() && ct.Position.Value is not null)
                is IEnumerable<CompassTarget> activeTargets
            && activeTargets.Any()
        )
        {
            newTarget = activeTargets.Aggregate((i1, i2) => SqrDistance(i1) < SqrDistance(i2) ? i1 : i2);
        }

        if (newTarget != CurrentTarget)
        {
            CurrentTarget = newTarget;
            _lerpStartTime = Time.time;
        }

        return CurrentTarget is not null;
    }

    private float SqrDistance(CompassTarget location)
    {
        return ((Vector2)location.Position.Value - _compassCenter).sqrMagnitude;
    }
}

public abstract class CompassInfo
{
    public abstract string Name { get; }
    public abstract float Radius { get; }
    public abstract float Scale { get; }
    public abstract bool RotateSprite { get; }
    public abstract bool Lerp { get; }
    public abstract float LerpDuration { get; }
    public List<CompassTarget> CompassTargets { get; } = [];

    public abstract bool ActiveCondition();
    public abstract Vector2 GetCompassCenter();
}
