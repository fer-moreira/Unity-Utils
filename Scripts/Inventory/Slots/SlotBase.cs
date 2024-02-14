using FFM.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FFM.Inventory {
    public interface ISlotEvents : IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler { }

    [System.Serializable]
    public class ItemData {
        public ItemBase Item;
        public int Count;
        public int Durability;

        public bool HasItem => (Item != null || Count > 0);

        public float DurabilityAmmount => ((float)Durability / (float)Item?.MaxDurability);

        public ItemData(ItemBase item, int count, int durability) {
            Item = item;
            Count = count;
            Durability = durability;
        }
    }

    public class SlotBase : MonoBehaviour, ISlotEvents {

        #region Variables
        [FoldoutGroup("UI"), SerializeField]
        private Image m_IconImage;

        [FoldoutGroup("UI"), SerializeField]
        private Image m_SlotImage;

        [FoldoutGroup("UI"), SerializeField]
        private TextMeshProUGUI m_CountLabel;

        [FoldoutGroup("UI"), SerializeField]
        private GameObject m_DurabilityUI;

        [FoldoutGroup("UI"), SerializeField]
        private Image m_DurabilityFillBar;

        [LabelText("Data"), FoldoutGroup("Slot Data")]
        public ItemData Data;
        #endregion

        #region APIs
        public bool HasItem => (Data != null) ? Data.HasItem : false;
        public bool IsSlotFull => HasItem && Data.Count >= Data.Item.MaxStackCount;

        public Vector2 Position => transform.position;

        public Sprite SlotSprite {
            set => m_SlotImage.sprite = value;
            get => m_SlotImage.sprite;
        }

        public Image SlotImage => m_SlotImage;

        public virtual void SetSlotData(ItemData newData) {
            Data = new ItemData(
                newData.Item,
                newData.Item.Stackable ? (Mathf.Clamp(newData.Count, 0, newData.Item.MaxStackCount)) : 1,
                newData.Durability
            );
            this.RefreshUI();
        }

        [FoldoutGroup("Slot Data"), Button("Clear Data")]
        public virtual void ClearSlot() {
            Data = null;
            this.RefreshUI();
        }

        [FoldoutGroup("UI"), Button("Refresh")]
        public virtual void RefreshUI() {
            m_IconImage.gameObject.SetActive(HasItem);
            m_CountLabel.gameObject.SetActive(HasItem ? Data.Count > 1 : false);

            m_DurabilityUI.SetActive(HasItem && Data.Item.HasDurability);

            if (HasItem) {
                m_IconImage.sprite = Data.Item.ItemSprite;
                m_CountLabel.text = Data.Count.ToString();
                m_DurabilityFillBar.fillAmount = Data.DurabilityAmmount;
            }

            if (UICraftingDetails.current) {
                UICraftingDetails.current.UpdateIngredients();
            }
        }

        public bool DraggingState {
            set {
                Color color = (value ?
                    SlotDataController.DraggingColor :
                    SlotDataController.NormalColor
                );

                m_IconImage.color = color;
                m_CountLabel.color = color;
            }
        }
        #endregion


        #region Events
        public virtual void OnPointerEnter(PointerEventData eventData) {
            SlotDataController.OnHoverSlot(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData) {
            SlotDataController.OnExitSlot(this);
        }

        public virtual void OnBeginDrag(PointerEventData eventData) {
            SlotDataController.OnBeginDrag(this);
        }

        public virtual void OnEndDrag(PointerEventData eventData) {
            SlotDataController.EndDrag(this);
        }

        #endregion
    }
}
