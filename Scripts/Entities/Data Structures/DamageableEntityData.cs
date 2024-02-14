using FFM.Inventory;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFM.Entity {
    public class DamageableEntityData : LootableEntityData {

        [FoldoutGroup("Damage Settings")]
        public int HitPoints = 100;

        [FoldoutGroup("Damage Settings")]
        public DamagerType DamagerType;
    }
}
