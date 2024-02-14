using Sirenix.OdinInspector;
using UnityEngine;

namespace FFM.Inventory {
    public class PlayerActionSlot : SlotBase {
        [BoxGroup("Action")]
        [SerializeField] private string m_ActionName;

        public int ActionNameHash => m_ActionName.GetHashCode();


        public override void SetSlotData(ItemData data) {
            base.SetSlotData(data);

            if (data.HasItem) {
                Debug.Log(m_ActionName);
            }
        }
    }
}
