using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFM.Entity {
    public class InteractableEntity : EntityBase {

        private CircleCollider2D interactionCollider;

        protected override void SetupComponents() {
            base.SetupComponents();

            bool hasCollider = TryGetComponent<CircleCollider2D>(out interactionCollider);

            if (!hasCollider)
                interactionCollider = gameObject.AddComponent<CircleCollider2D>();

            interactionCollider.isTrigger = true;
            interactionCollider.radius = 1;
        }

        public virtual void OnInteract() {
            throw new NotImplementedException();
        }
    }
}
