using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace FFM.Inventory {
    public class SlotContainer : MonoBehaviour {
        [BoxGroup("Settings")]
        [SerializeField] SlotBase[] m_Slots;

#if UNITY_EDITOR
        [BoxGroup("Settings")]
        [Button("Get Slots")]
        private void INSPECTOR_getSlots() {
            m_Slots = GetComponentsInChildren<SlotBase>();
        }
#endif

        public bool HasEmptySlot => m_Slots.Any(s => !s.HasItem);

        public SlotBase[] Slots => m_Slots;

        public int SlotsCount => m_Slots.Length;

        public SlotBase FindSameItemSlot(ItemBase item) => m_Slots.FirstSameItem(item);

        public SlotBase[] FindSlotsWithSameItem(ItemBase item) =>
            m_Slots.Where(s => s.HasItem && s.Data.Item.ItemId == item.ItemId).ToArray();

        public virtual void SetItem(ItemData itemData, out int remaining) {
            int remainingCount = itemData.Count;
            int maxCount = itemData.Item.MaxStackCount;

            for (int i = 0; i < m_Slots.Length && remainingCount > 0; i++) {
                SlotBase currentSlot = m_Slots[i];

                if (!currentSlot.HasItem) {
                    currentSlot.SetSlotData(new ItemData(itemData.Item, Mathf.Min(maxCount, remainingCount), itemData.Durability));
                    remainingCount -= maxCount;
                } else if (currentSlot.Data.Item.ItemId == itemData.Item.ItemId && currentSlot.Data.Count < maxCount) {
                    int spaceAvailable = maxCount - currentSlot.Data.Count;
                    int amountToAdd = Mathf.Min(spaceAvailable, remainingCount);

                    currentSlot.Data.Count += amountToAdd;
                    currentSlot.RefreshUI();

                    remainingCount -= amountToAdd;
                }
            }

            remaining = remainingCount;
        }

        public virtual void ConsumeItem(ItemData itemData, out int remaining) {
            int remainingCount = itemData.Count;

            for (int i = 0; i < m_Slots.Length && remainingCount > 0; i++) {
                SlotBase currentSlot = m_Slots[i];

                if (currentSlot.HasItem && currentSlot.Data.Item.ItemId == itemData.Item.ItemId) {
                    if (currentSlot.Data.Count >= remainingCount) {
                        currentSlot.Data.Count -= remainingCount;

                        if (currentSlot.Data.Count == 0)
                            currentSlot.ClearSlot();
                        else
                            currentSlot.RefreshUI();

                        remainingCount = 0;
                    } else {
                        remainingCount -= currentSlot.Data.Count;
                        currentSlot.ClearSlot();
                    }
                }
            }

            remaining = remainingCount;
        }
    }


    public static class SlotExtensions {
        public static SlotBase FirstEmpty(this SlotBase[] slots) {
            return slots.FirstOrDefault(s => !s.HasItem);
        }

        public static SlotBase FirstSameItem(this SlotBase[] slots, ItemBase item) {
            return slots.FirstOrDefault(s => s.HasItem && (s.Data.Item.ItemId == item.ItemId));
        }

        public static SlotBase LastSameItem(this SlotBase[] slots, ItemBase item) {
            return slots.LastOrDefault(s => s.HasItem && (s.Data.Item.ItemId == item.ItemId));
        }

        public static SlotBase FirstSameThatHasSpace(this SlotBase[] slots, ItemBase item) {
            return slots.FirstOrDefault(s => s.HasItem
                && (s.Data.Item.ItemId == item.ItemId)
                && s.Data.Item.Stackable
                && s.Data.Count < s.Data.Item.MaxStackCount
            );
        }

        public static bool HasEmpty(this SlotBase[] slots) {
            return slots.Any(s => !s.HasItem);
        }
    }
}
