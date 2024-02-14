using FFM.Managers;
using UnityEngine;

namespace FFM.Inventory {
    public class QuickslotContainer : SlotContainer {

        public static QuickslotContainer current;
        private void Awake() => current = this;

        private static QuickSlot selectedSlot;
        private int selectedIndex = 0;

        public static void SetAsSelected(QuickSlot slot) {
            if (selectedSlot != null) {
                selectedSlot.IsSelected = false;
            }

            selectedSlot = slot;
            selectedSlot.IsSelected = true;
        }

        private void Update() {
            if (!GameManager.CursorOverUI) {
                float scroll = InputReader.GetAxis("Scroll") * Time.deltaTime;

                if (scroll > 0)
                    this.ScrollIndex(-1);
                else if (scroll < 0)
                    this.ScrollIndex(1);
            }
        }

        private void ScrollIndex(int dir) {
            selectedIndex += dir;

            if (selectedIndex >= base.Slots.Length) {
                selectedIndex = 0;
            } else if (selectedIndex < 0) {
                selectedIndex = base.Slots.Length - 1;
            }

            QuickSlot slot = base.Slots[selectedIndex] as QuickSlot;

            if (slot != null) {
                SetAsSelected(slot);
                slot.UseSlot(false);
            }
        }
    }
}
