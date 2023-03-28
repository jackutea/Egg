using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    [Serializable]
    public struct BuffTM {

        [Header("类型ID")] public int typeID;
        [Header("名称")] public string buffName;
        [Header("描述")] public string des;
        [Header("图标")] public Sprite icon;

        [Header("属性影响")] public AttributeEffectTM attributeEffectTM;

    }

}