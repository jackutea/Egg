using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    // 用例
    // 1. boss死亡时, 触发一个效果器, 销毁所有的敌人
    [Serializable]
    public struct EntityDestroyTM {

        [Header("销毁实体类型")] public EntityType entityType;
        [Header("目标类型")] public TargetGroupType targetGroupType;

        public bool isEnabled_attributeSelector;

        public AttributeSelectorTM attributeSelectorTM;

    }

}