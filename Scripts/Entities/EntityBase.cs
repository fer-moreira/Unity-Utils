using FFM.Utils;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFM.Entity {
    [RequireComponent(typeof(SpriteRenderer), typeof(PolygonCollider2D), typeof(Rigidbody2D))]
    public class EntityBase : MonoBehaviour {

        [SerializeField, InlineEditor]
        protected EntityData m_EntityData;

        protected SpriteRenderer entityRenderer;
        protected PolygonCollider2D entityCollider;
        protected Rigidbody2D entityRigidbody;
        protected Transform entityTransform;

        public SpriteRenderer Renderer => entityRenderer;

        private void Awake() {
            SetupComponents();
        }

        [Button("Setup")]
        protected virtual void SetupComponents() {
            gameObject.name = m_EntityData.name;

            entityRenderer = GetComponent<SpriteRenderer>();
            entityCollider = GetComponent<PolygonCollider2D>();
            entityRigidbody = GetComponent<Rigidbody2D>();
            entityTransform = transform;

            entityRenderer.sprite = m_EntityData.EntitySprite;
            entityRenderer.spriteSortPoint = SpriteSortPoint.Pivot;

            entityCollider.TryUpdateShapeToAttachedSprite();
            entityCollider.isTrigger = m_EntityData.IsTrigger;

            entityRigidbody.bodyType = RigidbodyType2D.Static;

            gameObject.tag = m_EntityData.EntityTag;
            gameObject.layer = m_EntityData.EntityLayer;
        }
    }
}
