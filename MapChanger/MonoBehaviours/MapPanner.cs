using System.Collections;
using MapChanger.Map;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class MapPanner : MapObject, IPeriodicUpdater
    {
        private const float PAN_TIME = 0.5f;
        private const float DELTA_TIME = 0.01f;
        private const float NUMBER_OF_CYCLES = PAN_TIME / DELTA_TIME;

        public static MapPanner Instance { get; private set; }

        public static bool IsPanning => periodicUpdate is not null;
        private static Coroutine periodicUpdate;

        private static GameObject goMap => GameManager.instance?.gameMap?.gameObject;

        public float UpdateWaitSeconds => DELTA_TIME;

        private static Vector2 startPosition;
        private static Vector2 targetPosition;

        private static int cycle;

        public override void Initialize()
        {
            base.Initialize();

            Instance = this;
        }

        public override void OnMainUpdate(bool active)
        {
            StopPeriodicUpdate();
        }

        public static void PanTo(Vector2 position)
        {
            Instance.StopPeriodicUpdate();

            // We want to pan such that the room's position becomes (0, 0)
            // In other words, subtract the position from the map's current position
            startPosition = (Vector2)goMap.transform.position;
            targetPosition = startPosition - position;

            Instance.StartPeriodicUpdate();
        }

        public static void PanTo(string scene)
        {
            if (!(Finder.GetMappedScene(scene) is var mappedScene)
                || !BuiltInObjects.MappedRooms.TryGetValue(mappedScene, out RoomSprite room)
                || !room.CanSelect()) return;

            PanTo(room.transform.position);
        }

        public IEnumerator PeriodicUpdate()
        {
            cycle = 1;

            while (true)
            {
                if (cycle > NUMBER_OF_CYCLES)
                {
                    goMap.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
                    periodicUpdate = null;
                    yield break;
                }

                goMap.transform.position = startPosition + (targetPosition - startPosition) * (cycle / NUMBER_OF_CYCLES);

                yield return new WaitForSecondsRealtime(UpdateWaitSeconds);

                cycle++;
            }
        }

        private void StartPeriodicUpdate()
        {
            if (periodicUpdate is null)
            {
                periodicUpdate = StartCoroutine(PeriodicUpdate());
            }
        }

        private void StopPeriodicUpdate()
        {
            if (periodicUpdate is not null)
            {
                StopCoroutine(periodicUpdate);
                periodicUpdate = null;
            }
        }

    }
}