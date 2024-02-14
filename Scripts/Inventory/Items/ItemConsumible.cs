using UnityEngine;

namespace FFM.Inventory {
    [CreateAssetMenu(fileName = "Item Consumble", menuName = "Scriptables/New Consumible")]
    public class ItemConsumible : ItemBase, IConsumible {

        public void Consume() {
            //Debug.Log("Consumible");
        }
    }
}
