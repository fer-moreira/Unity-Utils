using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FFM.Inventory {
    public class SlotDraggingActor : MonoBehaviour {

        [SerializeField] private GameObject m_Graphics;
        [SerializeField] private Image m_IconRender;
        [SerializeField] private TextMeshProUGUI m_CountLabel;

        private bool isDragging;
        private Transform tf;
        private ItemData data;

        private void Awake() {
            tf = this.transform;
        }

        private void Start() {
            RefreshUI();
        }

        private void Update() {
            if (isDragging != SlotDataController.IsDragging) {
                isDragging = SlotDataController.IsDragging;

                if (isDragging) {
                    data = SlotDataController.DraggingData;
                } else {
                    data = null;
                }

                RefreshUI();
            }

            if (isDragging) {
                tf.position = ((SlotDataController.IsPotentialDrop) ?
                    SlotDataController.TargetSlotPosition :
                    InputReader.mousePosition
                );
            }
        }

        private void RefreshUI() {
            m_Graphics.SetActive(isDragging);

            if (isDragging) {
                m_CountLabel.text = data.Count > 1 ? data.Count.ToString() : "";
                m_IconRender.sprite = data.Item.ItemSprite;
            }
        }
    }
}
