using FFM.Player;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace FFM.Inventory {
    public class QuickSlot : SlotBase {

        [FoldoutGroup("Quickslot Settings"), SerializeField]
        private string m_SlotInputName = "Slot_1_Key";
        [FoldoutGroup("Quickslot Settings"), SerializeField, DrawWithUnity]
        private Sprite m_SelectedSprite;

        private Sprite normalSprite;

        private bool isSelected;
        public bool IsSelected {
            get => isSelected;
            set {
                base.SlotImage.sprite = (value ?
                    m_SelectedSprite :
                    normalSprite
                );

                isSelected = value;
            }
        }

        private void Start() {
            normalSprite = base.SlotSprite;
        }


        private void Update() {
            if (InputReader.GetButtonDown(m_SlotInputName))
                this.UseSlot();

        }

        public override void SetSlotData(ItemData newData) {
            CheckForEquipable();

            base.SetSlotData(newData);


            switch (newData.Item) {
                case IEquipable equipable:
                    if (IsSelected) this.UseSlot(true);
                    break;
                default:
                    break;
            }
        }

        public override void ClearSlot() {
            CheckForEquipable();
            base.ClearSlot();
        }

        public void UseSlot(bool isPlayerInput = false) {
            if (HasItem) {
                switch (Data.Item) {
                    case IEquipable equipable:
                        equipable.Equip();
                        QuickslotContainer.SetAsSelected(this);
                        break;
                    case IConsumible consumible:
                        if (isPlayerInput) {
                            SetSlotData(new ItemData(Data.Item, Data.Count - 1, Data.Durability));

                            if (Data.Count <= 0)
                                ClearSlot();

                            consumible.Consume();
                        } else {
                            PlayerItemBehaviour.current.UnequipItem();
                        }
                        break;
                    default:
                        break;
                }
            } else {
                QuickslotContainer.SetAsSelected(this);
                PlayerItemBehaviour.current.UnequipItem();
            }
        }


        public void CheckForEquipable() {
            if (Data != null && Data.HasItem && IsSelected) {
                switch (Data.Item) {
                    case IEquipable equipable:
                        equipable.UnEquip();
                        break;
                }
            }
        }
    }
}
