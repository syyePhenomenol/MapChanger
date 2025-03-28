using System.Collections.Generic;
using System.Linq;
using MapChanger.Defs;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    /// <summary>
    /// Points from some entity to the nearest of a group of objects, during gameplay.
    /// </summary>
    public class DirectionalCompass : MonoBehaviour
    {
        public CompassTarget CurrentTarget { get; private set; }

        private GameObject compassInternal;
        private SpriteRenderer sr;
        private CompassInfo info;
        private GameObject entity;
        private float lerpStartTime;
        private Vector2 currentDir;
        private float currentAngle;

        public static GameObject Make(CompassInfo ci)
        {
            // This object is a container for the script. Can be set active/inactive externally to control script
            GameObject compass = new(ci.Name);
            DontDestroyOnLoad(compass);

            DirectionalCompass dc = compass.AddComponent<DirectionalCompass>();

            dc.compassInternal = new($"{ci.Name} Internal", typeof(SpriteRenderer));
            dc.sr = dc.compassInternal.GetComponent<SpriteRenderer>();
            dc.compassInternal.transform.parent = compass.transform;

            dc.info = ci;

            return compass;
        }

        public void Destroy()
        {
            Destroy(compassInternal);
            Destroy(gameObject);
        }

        public void FixedUpdate()
        {
            foreach (CompassTarget compassTarget in info.CompassTargets.Where(ct => ct.IsActive()))
            {
                compassTarget.Position.UpdateEveryFrame();
            }
        }

        public void Update()
        {
            if (info.ActiveCondition() && info.CompassTargets.Any() && TryUpdateClosestLocation())
            {
                sr.sprite = CurrentTarget.GetSprite();
                
                Vector2 dir = (Vector2)CurrentTarget.Position.Value - (Vector2)entity.transform.position;

                dir.Scale(Vector2.one * 0.5f);

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

                // Clamp to radius
                dir = Vector2.ClampMagnitude(dir, info.Radius);

                // Do lerp stuff
                if (info.Lerp && Time.time - lerpStartTime < info.LerpDuration)
                {
                    dir = Vector2.Lerp(currentDir, dir, (Time.time - lerpStartTime) / info.LerpDuration);
                    angle = Mathf.LerpAngle(currentAngle, angle, (Time.time - lerpStartTime) / info.LerpDuration);
                }

                currentDir = dir;
                currentAngle = angle;

                compassInternal.transform.position = new Vector3(entity.transform.position.x + dir.x, entity.transform.position.y + dir.y, 0f);
                
                if (info.RotateSprite)
                {
                    compassInternal.transform.eulerAngles = new(0, 0, angle);
                }
                
                float inverseRadius = dir.magnitude / info.Radius;
                compassInternal.transform.localScale = Vector3.Scale(new Vector3(inverseRadius * info.Scale, inverseRadius * info.Scale, 1f), CurrentTarget.GetScale());
                sr.color = inverseRadius * CurrentTarget.GetColor();

                compassInternal.SetActive(true);
            }
            else
            {
                compassInternal.SetActive(false);
            }
        }

        private bool TryUpdateClosestLocation()
        {
            entity = info.GetEntity();

            CompassTarget newTarget = null;

            if (entity != null && info.CompassTargets.Where(ct => ct.IsActive() && ct.Position.Value is not null) is IEnumerable<CompassTarget> activeTargets && activeTargets.Any())
            {
                newTarget = activeTargets.Aggregate((i1, i2) => SqrDistance(entity, i1) < SqrDistance(entity, i2) ? i1 : i2);
            }

            if (newTarget != CurrentTarget)
            {
                CurrentTarget = newTarget;
                lerpStartTime = Time.time;
            }

            return CurrentTarget is not null;
        }

        private float SqrDistance(GameObject entity, CompassTarget location)
        {
            return ((Vector2)location.Position.Value - (Vector2)entity.transform.position).sqrMagnitude;
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
        public abstract GameObject GetEntity();
    }
}
