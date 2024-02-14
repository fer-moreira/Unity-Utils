using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFM.Entity {
    [CreateAssetMenu(fileName = "Entity Data", menuName = "Scriptables/Entity/New Data")]
    public class EntityData : ScriptableObject {
        [FoldoutGroup("General")]
        public Sprite EntitySprite;

        [FoldoutGroup("General")]
        [Layer]
        public int EntityLayer;

        [FoldoutGroup("General")]
        [TagSelector]
        public string EntityTag;

        [FoldoutGroup("Physics")]
        public bool IsTrigger;
    }
}
