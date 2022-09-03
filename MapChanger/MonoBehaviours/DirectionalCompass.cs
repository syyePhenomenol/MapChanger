using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RandoMapMod.UI
{
    /// <summary>
    /// Points from some entity to the nearest of a group of objects, during gameplay.
    /// </summary>
    public class DirectionalCompass : MonoBehaviour
    {
        private GameObject entity;

        private GameObject compassInternal;
        private SpriteRenderer sr;
        private Color color;

        private Func<bool> Condition;

        private float scale;
        private float radius;

        private bool lerp;
        private float lerpDuration;

        private float lerpStartTime;
        private GameObject currentTarget;
        private Vector3 currentDir;
        private float currentAngle;

        public List<GameObject> TrackedObjects;

        public static GameObject Create(string name, GameObject entity, Sprite sprite, Color color, float radius, float scale, Func<bool> condition, bool lerp, float lerpDuration)
        {
            // This object is a container for the script. Can be set active/inactive externally to control script
            GameObject compass = new(name);
            DontDestroyOnLoad(compass);

            DirectionalCompass dc = compass.AddComponent<DirectionalCompass>();

            // This object is the actual compass sprite. Set active/inactive by the script itself
            dc.compassInternal = new(name + " Internal", typeof(SpriteRenderer));

            dc.sr = dc.compassInternal.GetComponent<SpriteRenderer>();
            dc.sr.sprite = sprite;

            dc.color = color;

            dc.compassInternal.transform.parent = compass.transform;

            dc.entity = entity;
            dc.scale = scale;
            dc.radius = radius;
            dc.Condition = condition;
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
            if (entity != null && Condition() && TryGetClosestObject(out GameObject o))
            {
                Vector2 dir = o.transform.position - entity.transform.position;

                dir.Scale(Vector3.one * 0.5f);

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

                // Clamp to radius
                dir = Vector2.ClampMagnitude(dir, radius);

                // Do lerp stuff
                if (lerp)
                {
                    if (currentTarget == null || currentTarget != o)
                    {
                        currentTarget = o;
                        lerpStartTime = Time.time;
                    }
                    
                    if (Time.time - lerpStartTime < lerpDuration)
                    {
                        dir = Vector2.Lerp(currentDir, dir, (Time.time - lerpStartTime) / lerpDuration);
                        angle = Mathf.LerpAngle(currentAngle, angle, (Time.time - lerpStartTime) / lerpDuration);
                    }
                }

                currentDir = dir;
                currentAngle = angle;

                compassInternal.transform.position = new Vector3(entity.transform.position.x + dir.x, entity.transform.position.y + dir.y, 0f);
                compassInternal.transform.eulerAngles = new(0, 0, angle);
                compassInternal.transform.localScale = new Vector3(dir.magnitude / radius * scale, dir.magnitude / radius * scale, 1f);
                sr.color = dir.magnitude / radius * color;

                compassInternal.SetActive(true);
            }
            else
            {
                compassInternal.SetActive(false);
            }
        }

        private bool TryGetClosestObject(out GameObject o)
        {
            if (TrackedObjects is null || !TrackedObjects.Any() || entity == null)
            {
                o = null;

                return false;
            }

            o = TrackedObjects.Aggregate((i1, i2) => SqrDistanceFromEntity(i1) < SqrDistanceFromEntity(i2) ? i1 : i2);
            
            return o != null;
        }

        private float SqrDistanceFromEntity(GameObject o)
        {
            if (o == null || entity == null) return float.PositiveInfinity;

            return ((Vector2)(o.transform.position - entity.transform.position)).sqrMagnitude;
        }
    }
}
