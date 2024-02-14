using FFM.Inventory;
using FFM.Player;
using FFM.Pooling;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FFM.Entity {
    [CreateAssetMenu(fileName = "Lootabl Entity Data", menuName = "Scriptables/Entity/New Lootable Data")]
    public class LootableEntityData : EntityData {
        [System.Serializable]
        public class LootItemData {
            public ItemBase Item;
            public bool RandomCount = false;

            [HideIf("RandomCount")]
            public int Count = 1;

            [ShowIf("RandomCount")]
            public Vector2Int CountRange = new Vector2Int(1, 2);

            [Range(0, 100)]
            public int Probability = 100;

            public int GetCount() => Random.Range(CountRange.x, CountRange.y);

            public ItemData Data => new ItemData(
                Item,
                RandomCount ? GetCount() : Count,
                Item.MaxDurability
            );

            public bool CanLoot => Random.value <= ((float)Probability / 100f);
        }

        [FoldoutGroup("Loot")]
        public LootItemData[] AvailableLoot;

        public void DropLoot(Vector2 position) {
            for (int i = 0; i < AvailableLoot.Length; i++) {
                LootItemData lootData = AvailableLoot[i];

                if (lootData.CanLoot) {
                    var loot = LootPooler.current.Get();
                    loot.Position = position + (Random.insideUnitCircle * 0.5f);
                    loot.SetLootData(lootData.Data);
                }
            }
        }
    }
}
