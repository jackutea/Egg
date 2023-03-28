using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    [Serializable]
    public struct BuffTM {

        [Header("类型ID")] public int typeID;
        [Header("名称")] public string buffName;
        [Header("描述")] public string description;
        [Header("图标")] public string iconName;
        [Header("编辑时: 图标GUID")] public string iconGUID;

        [Header("延时(帧)")] public int delayFrame;
        [Header("间隔(帧)")] public int intervalFrame;
        [Header("持续时间(帧)")] public int durationFrame;
        [Header("次数")] public int triggerTimes;

        [Header("属性影响(组)")] public AttributeEffectTM[] attributeEffectTMArray;

    }

}