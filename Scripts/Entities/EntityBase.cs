using FFM.Utils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace FFM.Entity {
    [Flags]
    public enum EntityHitLayer {
        None = 0,
        Any = Axe | Pickaxe | Sword | Arrow,
        Axe = 1 << 0,
        Pickaxe = 1 << 1,
        Sword = 1 << 2,
        Arrow = 1 << 3
    }

    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(PolygonCollider2D))]
    public class EntityBase : MonoBehaviour, IDamageable {

        [field: SerializeField]
        public EntityData Data { get; private set; }

        [ShowInInspector, ReadOnly]
        public int HitPoints { get; private set; }

        private SpriteRenderer entityRenderer;
        private PolygonCollider2D entityCollider;
        private Rigidbody2D entityRigidbody;

        private void Awake() {
            entityRenderer = GetComponent<SpriteRenderer>();
            entityCollider = GetComponent<PolygonCollider2D>();
            entityRigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start() {
            Refresh();
        }

        public void SetData(EntityData newData) {
            Data = newData;
            Refresh();
        }

        public bool IncludeLayer(EntityHitLayer layer) {
            return (layer & Data.HitLayer) != EntityHitLayer.None;
        }

        public virtual void DoDamage(int damage) {
            HitPoints -= damage;

            if (HitPoints <= 0)
                Destroy(this.gameObject);

        }

        private void Refresh() {
            HitPoints = Data.HitPoints;
            entityRenderer.sprite = Data.EntitySprite;
            entityCollider.UpdateShapeToSprite(Data.EntitySprite);
        }

#if UNITY_EDITOR
        public void GetComponents () {
            entityRenderer = GetComponent<SpriteRenderer>();
            entityCollider = GetComponent<PolygonCollider2D>();
            entityRigidbody = GetComponent<Rigidbody2D>();
        }
#endif
    }

    public interface IDamageable {
        public abstract void DoDamage(int damage);
    }
}
