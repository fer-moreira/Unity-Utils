using FFM.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FFM.Inventory {

    [System.Flags]
    public enum DamagerType {
        None        = 0,
        Sword       = 1 << 1,
        Axe         = 1 << 2,
        Pickaxe     = 1 << 3,
        Shovel      = 1 << 4,
        Projectile  = 1 << 5
    }

    public class FloatingItemBase : ItemBase, IEquipable {

        [LabelWidth(100), LabelText("Damager Type"), VerticalGroup("Base Settings/Split/Right")]
        public DamagerType ItemDamagerType;

        [BoxGroup("Floating Settings")]
        public int MinDamage = 1;
        [BoxGroup("Floating Settings")]
        public int MaxDamage = 5;

        [Range(0, 100)]
        [BoxGroup("Floating Settings")]
        public int CriticalChance = 10;

        [BoxGroup("Floating Settings")]
        public float CriticalMultiplier = 2;

        [BoxGroup("Floating Settings")]
        public float CollisionRadius = 0.3f;

        public int GetDamage(out bool critical) {
            critical = (Mathf.RoundToInt(Random.value * 100) >= CriticalChance);
            float d = Random.Range((float)MinDamage, (float)MaxDamage);
            d = (d * (critical ? CriticalMultiplier : 1f));
            return Mathf.RoundToInt(d);
        }

        public void Equip() {
            PlayerItemBehaviour.current.Equipitem(this);
        }

        public void UnEquip() {
            PlayerItemBehaviour.current.UnequipItem();
        }
    }
}
