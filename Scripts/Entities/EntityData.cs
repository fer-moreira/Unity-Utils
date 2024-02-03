using UnityEngine;

namespace FFM.Entity {
    [CreateAssetMenu(fileName = "Entity Data", menuName = "Scriptables/New Entity")]
    public class EntityData : ScriptableObject {

        [Header("Entity")]
        public Sprite EntitySprite;
        public EntityHitLayer HitLayer;
        public int HitPoints = 100;

        [Space(10)]
        [Header("Rigidbody")]
        public int Gravity = 0;
        public int Mass = 10;
        public int Drag = 10;

        [Space(10)]
        [Header("GameObject")]
        public SingleUnityLayer EntityLayer;

        [TagSelector]
        public string EntityTag;
    }
}
