using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace FFM.Inventory {
    [System.Serializable]
    public class ItemIngredient {
        public ItemBase RequiredItem;
        public int RequiredCount;
    }

    public enum ResourceType {
        Resource,
        Key,
        Consumible,
        Tool,
        Weapon,
        Structure
    }

    [Flags]
    public enum ItemFlags {
        None = 0,
        Craftable = 1 << 0,
        Fuel = 1 << 1,
        Stackable = 1 << 2,
        Meltable = 1 << 3,
        Lootable = 1 << 4,
        HasDurability = 1 << 5
    }

    public enum ItemCategory {
        None = 0,
        Weapon = 1,
        Tool = 2,
        Armors = 3,
        Others = 4
    }


    [CreateAssetMenu(fileName = "New Item", menuName = "Scriptables/New Item")]
    public class ItemBase : ScriptableObject {

        [BoxGroup("Base Settings")]
        [HorizontalGroup("Base Settings/Split", width: 105)]

        [HideLabel, PreviewField(105, ObjectFieldAlignment.Left)]
        public Sprite ItemSprite;

        [LabelWidth(100), LabelText("Name"), VerticalGroup("Base Settings/Split/Right")]
        public string ItemName = "Item";

        [LabelWidth(100), LabelText("Id"), VerticalGroup("Base Settings/Split/Right")]
        public int ItemId;

        [LabelWidth(100), LabelText("Type"), VerticalGroup("Base Settings/Split/Right")]
        public ResourceType ItemType;

        [LabelWidth(100), LabelText("Flags"), VerticalGroup("Base Settings/Split/Right")]
        public ItemFlags ItemFlags;


        [BoxGroup("Base Settings/Settings"), ShowIf("HasDurability")]
        public int MaxDurability;

        [BoxGroup("Base Settings/Settings"), ShowIf("Stackable")]
        public int MaxStackCount;

        [BoxGroup("Base Settings/Settings"), ShowIf("IsFuel")]
        public int FuelTicks = 5;

        [BoxGroup("Base Settings/Crafting"), ShowIf("Craftable")]
        public int CraftResultCount = 1;

        [BoxGroup("Base Settings/Crafting"), ShowIf("Craftable")]
        public ItemCategory CraftItemCategory;

        [BoxGroup("Base Settings/Crafting"), ShowIf("Craftable"), TableList(ShowIndexLabels = false)]
        public ItemIngredient[] Recipe;


        [BoxGroup("Base Settings"), ShowIf("Meltable")]
        public ItemBase MeltResult;

        public bool Craftable => (ItemFlags & ItemFlags.Craftable) != 0;
        public bool Stackable => (ItemFlags & ItemFlags.Stackable) != 0;
        public bool IsFuel => (ItemFlags & ItemFlags.Fuel) != 0;
        public bool Meltable => (ItemFlags & ItemFlags.Meltable) != 0;
        public bool HasDurability => (ItemFlags & ItemFlags.HasDurability) != 0;


#if UNITY_EDITOR
        [Button("Refresh")]
        private void RefreshEditor() {
            ItemId = this.ItemName.GetHashCode();
        }
#endif
    }

    public interface IConsumible {
        public abstract void Consume();
    }

    public interface IEquipable {
        public abstract void Equip();

        public abstract void UnEquip();
    }
}
