using System.Collections;
using MapChanger.Map;
using UnityEngine;

namespace MapChanger.MonoBehaviours;

public class MapPanner : MapObject, IPeriodicUpdater
{
    private const float PAN_TIME = 0.5f;
    private const float DELTA_TIME = 0.01f;
    private const float NUMBER_OF_CYCLES = PAN_TIME / DELTA_TIME;

    private static Coroutine _periodicUpdate;
    private static Vector2 _startPosition;
    private static Vector2 _targetPosition;
    private static int _cycle;
    private static GameObject _goMap;

    public static MapPanner Instance { get; private set; }
    public static bool IsPanning => _periodicUpdate is not null;
    public float UpdateWaitSeconds => DELTA_TIME;

    public void Initialize(GameObject goMap)
    {
        base.Initialize();

        _goMap = goMap;
        Instance = this;
    }

    public override void OnMainUpdate(bool active)
    {
        StopPeriodicUpdate();
    }

    public static void PanToMappedScene(string scene)
    {
        if (
            !(Finder.GetMappedScene(scene) is var mappedScene)
            || !BuiltInObjects.MappedRooms.TryGetValue(mappedScene, out var room)
            || !room.CanSelect()
        )
        {
            return;
        }

        PanTo(room.transform.position);
    }

    public static void PanTo(Vector2 position)
    {
        Instance.StopPeriodicUpdate();

        // We want to pan such that the room's position becomes (0, 0)
        // In other words, subtract the position from the map's current position
        _startPosition = (Vector2)_goMap.transform.position;
        _targetPosition = _startPosition - position;

        Instance.StartPeriodicUpdate();
    }

    public IEnumerator PeriodicUpdate()
    {
        _cycle = 1;

        while (true)
        {
            if (_cycle > NUMBER_OF_CYCLES)
            {
                _goMap.transform.position = new Vector3(_targetPosition.x, _targetPosition.y, 0);
                _periodicUpdate = null;
                yield break;
            }

            _goMap.transform.position =
                _startPosition + ((_targetPosition - _startPosition) * (_cycle / NUMBER_OF_CYCLES));

            yield return new WaitForSecondsRealtime(UpdateWaitSeconds);

            _cycle++;
        }
    }

    private void StartPeriodicUpdate()
    {
        _periodicUpdate ??= StartCoroutine(PeriodicUpdate());
    }

    private void StopPeriodicUpdate()
    {
        if (_periodicUpdate is not null)
        {
            StopCoroutine(_periodicUpdate);
            _periodicUpdate = null;
        }
    }
}
