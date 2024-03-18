using System;
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
        private GameObject entity;
        private Func<GameObject> GetEntity;

        private GameObject compassInternal;
        private SpriteRenderer sr;

        private Func<bool> Condition;

        public CompassLocation CurrentTarget { get; private set; }

        private float scale;
        private float radius;
        private bool rotateSprite;
        private bool lerp;
        private float lerpDuration;

        private float lerpStartTime;
        private Vector2 currentDir;
        private float currentAngle;

        public Dictionary<string, CompassLocation> Locations = new();

        public static GameObject Create(string name, Func<GameObject> getEntity, float radius, float scale, Func<bool> condition, bool rotateSprite, bool lerp, float lerpDuration)
        {
            // This object is a container for the script. Can be set active/inactive externally to control script
            GameObject compass = new(name);
            DontDestroyOnLoad(compass);

            DirectionalCompass dc = compass.AddComponent<DirectionalCompass>();

            dc.compassInternal = new($"{name} Internal", typeof(SpriteRenderer));
            dc.sr = dc.compassInternal.GetComponent<SpriteRenderer>();
            dc.compassInternal.transform.parent = compass.transform;

            dc.GetEntity = getEntity;
            dc.radius = radius;
            dc.scale = scale;
            dc.Condition = condition;
            dc.rotateSprite = rotateSprite;
            dc.lerp = lerp;
            dc.lerpDuration = lerpDuration;

            return compass;
        }

        public void Destroy()
        {
            Destroy(compassInternal);
            Destroy(gameObject);
        }

        public void Update()
        {
            if (Condition() && UpdateClosestObject())
            {
                Vector2 dir = CurrentTarget.Position - (Vector2)entity.transform.position;

                dir.Scale(Vector2.one * 0.5f);

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

                // Clamp to radius
                dir = Vector2.ClampMagnitude(dir, radius);

                // Do lerp stuff
                if (lerp && Time.time - lerpStartTime < lerpDuration)
                {
                    dir = Vector2.Lerp(currentDir, dir, (Time.time - lerpStartTime) / lerpDuration);
                    angle = Mathf.LerpAngle(currentAngle, angle, (Time.time - lerpStartTime) / lerpDuration);
                }

                currentDir = dir;
                currentAngle = angle;

                compassInternal.transform.position = new Vector3(entity.transform.position.x + dir.x, entity.transform.position.y + dir.y, 0f);
                
                if (rotateSprite)
                {
                    compassInternal.transform.eulerAngles = new(0, 0, angle);
                }
                
                compassInternal.transform.localScale = new Vector3(dir.magnitude / radius * scale, dir.magnitude / radius * scale, 1f);
                sr.color = dir.magnitude / radius * CurrentTarget.Color;

                compassInternal.SetActive(true);
            }
            else
            {
                compassInternal.SetActive(false);
            }
        }

        private bool UpdateClosestObject()
        {
            entity = GetEntity.Invoke();

            CompassLocation newTarget;

            if (entity == null || Locations is null || !Locations.Any())
            {
                newTarget = null;
            }
            else
            {
                var activeLocations = Locations.Values.Where(l => l.IsActive);

                newTarget = activeLocations.Any() ? activeLocations.Aggregate((i1, i2) => SqrDistanceFromEntity(i1) < SqrDistanceFromEntity(i2) ? i1 : i2) : null;
            }

            if (newTarget != CurrentTarget)
            {
                CurrentTarget = newTarget;
                UpdateSprite();
                lerpStartTime = Time.time;
            }

            return CurrentTarget != null;
        }

        private float SqrDistanceFromEntity(CompassLocation location)
        {
            if (location == null || entity == null) return float.PositiveInfinity;

            return (location.Position - (Vector2)entity.transform.position).sqrMagnitude;
        }

        public void UpdateSprite()
        {
            if (CurrentTarget is null || CurrentTarget.Sprite is null) return;

            sr.sprite = CurrentTarget.Sprite;
        }
    }
}
