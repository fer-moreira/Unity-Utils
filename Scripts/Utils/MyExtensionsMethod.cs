using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace FFM.Utils {
    public static class Extensions {
        public static int CeilNegativeToInt(float value) {
            if (value > 0) return 1;
            else if (value < 0) return -1;
            else return 0;
        }

        public static void SetZAngle(this Transform t, float angle, bool local = false) {
            if (!local) {
                t.rotation = Quaternion.Euler(0, 0, angle);
            } else {
                t.localRotation = Quaternion.Euler(0, 0, angle);
            }
        }

        public static Vector2Int position2dInt(this Transform t) {
            return new Vector2Int(
                (int)(t.position.x),
                (int)(t.position.y)
            );
        }

        public static Vector2 position2d(this Transform t) =>
            new Vector2(t.position.x, t.position.y);

        public static Vector3Int positionInt(this Transform t) {
            return new Vector3Int(
                Mathf.RoundToInt(t.position.x),
                Mathf.RoundToInt(t.position.y),
                Mathf.RoundToInt(t.position.z)
            );
        }

        public static Color ByLevel(this Color c, float level) {
            c.r = level;
            c.g = level;
            c.b = level;

            return c;
        }

        public static void TryUpdateShapeToAttachedSprite(this PolygonCollider2D collider) {
            collider.UpdateShapeToSprite(collider.GetComponent<SpriteRenderer>().sprite);
        }

        public static void UpdateShapeToSprite(this PolygonCollider2D collider, Sprite sprite) {
            if (collider != null && sprite != null) {
                collider.pathCount = sprite.GetPhysicsShapeCount();
                List<Vector2> path = new List<Vector2>();
                for (int i = 0; i < collider.pathCount; i++) {
                    path.Clear();
                    sprite.GetPhysicsShape(i, path);
                    collider.SetPath(i, path.ToArray());
                }
            }
        }

        public static void DeleteAllChildren(this Transform t) {
            int childCount = t.childCount;

            for (int i = childCount - 1; i >= 0; i--) {
#if UNITY_EDITOR
                GameObject.DestroyImmediate(t.GetChild(i).gameObject);
#else
                GameObject.Destroy(t.GetChild(i).gameObject);
#endif
            }
        }

        public static bool IncludeFlag<T>(this T flags, T flag) {
            if (!typeof(T).IsEnum) {
                throw new ArgumentException("T must be an enumerated type");
            }
            var flagsValue = System.Convert.ToUInt64(flags);
            var flagValue = System.Convert.ToUInt64(flag);

            return (flagsValue & flagValue) != 0;
        }
    }
}