using FFM.UI;
using UnityEngine;

namespace FFM.Inventory {
    public class BackpackContainer : SlotContainer {
        public static BackpackContainer current;
        private void Awake() => current = this;
    }
}
