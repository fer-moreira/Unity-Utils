using FFM.Interfaces;
using FFM.Inventory;
using FFM.Managers;
using FFM.Player;
using FFM.Pooling;
using FFM.Utils;
using UnityEngine;

namespace FFM.Entity {
    public class DamageableEntity : EntityBase, IDamageable {

        private int hitpoints;
        private DamageableEntityData damageableData;

        protected override void SetupComponents() {
            base.SetupComponents();
            damageableData = base.m_EntityData as DamageableEntityData;
        }

        public void DoDamage(ItemBase item) {
            switch (item) {
                case FloatingItemBase floatingItem:
                    bool sameFlag = damageableData.DamagerType.IncludeFlag(floatingItem.ItemDamagerType);

                    if (sameFlag) {
                        int damage = floatingItem.GetDamage(out bool isCritical);
                        this.ReceiveDamage(damage);
                    } else {
                        Debug.Log("Not same flag");
                    }
                    break;
            }
        }

        protected virtual void ReceiveDamage(int damage) {
            hitpoints += damage;

            if (hitpoints >= damageableData.HitPoints)
                OnDestroyEntity();
            else
                GameManager.ApplyHitEffect(base.entityRenderer);
        }

        protected virtual void OnDestroyEntity() {
            damageableData.DropLoot(base.entityTransform.position);
            Destroy(this.gameObject);
        }
    }
}
