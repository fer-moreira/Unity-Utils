using FFM.Managers;
using FFM.Player;
using FFM.Pooling;
using UnityEngine;

namespace FFM.Inventory {
    public class SlotDataController : Singleton<SlotDataController> {
        public static Color DraggingColor { get; private set; }
        public static Color NormalColor { get; private set; }

        protected override void SetDefaults() {
            base.SetDefaults();

            NormalColor = new Color(1, 1, 1, 1);
            DraggingColor = new Color(1, 1, 1, 0.1f);
        }

        private static SlotBase hoveringSlot;
        private static SlotBase draggingSlot;

        public static Vector2 DragSlotPosition => draggingSlot.Position;
        public static Vector2 TargetSlotPosition => hoveringSlot.Position;

        public static bool IsDragging => (draggingSlot != null);
        public static bool HasTarget => (hoveringSlot != null);

        public static bool IsPotentialDrop => (IsDragging && HasTarget);

        public static ItemData DraggingData => draggingSlot?.Data;

        public static void OnHoverSlot(SlotBase slot) {
            if (hoveringSlot != slot && draggingSlot != slot)
                hoveringSlot = slot;
        }

        public static void OnExitSlot(SlotBase slot) {
            hoveringSlot = null;
        }

        public static void OnBeginDrag(SlotBase slot) {
            if (slot.HasItem) {
                draggingSlot = slot;
                draggingSlot.DraggingState = true;
            }
        }

        public static void EndDrag(SlotBase slot) {
            if (IsDragging) {
                DragNDropUtils.OnEndDrag(draggingSlot, hoveringSlot);

                draggingSlot.DraggingState = false;
                draggingSlot = null;

                hoveringSlot = null;
            }
        }
    }

    public static class DragNDropUtils {
        private static SlotBase InitialSlot;
        private static SlotBase TargetSlot;

        public static void OnEndDrag(SlotBase initialSlot, SlotBase targetSlot) {
            InitialSlot = initialSlot;
            TargetSlot = targetSlot;

            if (InitialSlot != null && TargetSlot != null) {
                if (TargetSlot.HasItem) {
                    if (TargetSlot.Data.Item.ItemId == InitialSlot.Data.Item.ItemId) {
                        MoveToSameItem();
                    } else {
                        SwapSlotData();
                    }
                } else {
                    MoveToEmpty();
                }
            } else {
                if (!GameManager.CursorOverUI) {
                    DropAsLoot();
                }
            }
        }

        private static void MoveToEmpty() {
            TargetSlot.SetSlotData(InitialSlot.Data);
            InitialSlot.ClearSlot();
        }

        private static void SwapSlotData() {
            ItemData targetData = TargetSlot.Data;

            TargetSlot.SetSlotData(InitialSlot.Data);
            InitialSlot.SetSlotData(targetData);
        }

        private static void MoveToSameItem() {
            ItemData initialItem = InitialSlot.Data;
            ItemData targetItem = TargetSlot.Data;
            ItemBase item = initialItem.Item;
            int itemMaxStack = item.MaxStackCount;

            int totalItemCount = initialItem.Count + targetItem.Count;

            if (totalItemCount > itemMaxStack) {
                int remaining = totalItemCount - itemMaxStack;

                // Fill the target slot to its maximum stack count
                targetItem.Count = itemMaxStack;
                TargetSlot.RefreshUI();

                // Place the remaining items back into the initial slot
                initialItem.Count = remaining;
                InitialSlot.RefreshUI();
            } else {
                // Move all items to the target slot and clear the initial slot
                targetItem.Count = totalItemCount;
                TargetSlot.RefreshUI();
                InitialSlot.ClearSlot();
            }
        }

        private static void DropAsLoot() {
            var loot = LootPooler.current.Get();

            loot.MarkAsBlocked();
            loot.Position = PlayerController.current.Position;
            loot.SetLootData(InitialSlot.Data);

            InitialSlot.ClearSlot();
        }
    }
}
