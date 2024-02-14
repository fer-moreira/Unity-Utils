using FFM.Player;
using FFM.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFM.Entity {
    public class CollectableEntity : InteractableEntity {

        private LootableEntityData lootableData;

        protected override void SetupComponents() {
            base.SetupComponents();
            lootableData = m_EntityData as LootableEntityData;
        }

        public override void OnInteract() {
            lootableData.DropLoot(base.entityTransform.position);
            PlayerObjectInteraction.current.ResetObject();
            Destroy(this.gameObject);
        }
    }
}
