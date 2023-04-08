using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct SkillTM {

        [Header("类型ID")] public int typeID;
        [Header("技能名称")] public string skillName;
        [Header("技能类型")] public SkillType skillType;
        [Header("技能持续帧数")] public int maintainFrame;

        [Header("武器动画名")] public string weaponAnimName;
        [Header("仅用于编辑时: 武器动画文件GUID")] public string weaponAnimClip_GUID;

        [Header("原始技能")] public int originSkillTypeID;
        [Header("组合技名单")] public SkillCancelTM[] comboSkillCancelTMArray;
        [Header("连招技名单")] public SkillCancelTM[] cancelSkillCancelTMArray;

        [Header("技能效果器(组)")] public SkillEffectorTM[] skillEffectorTMArray;
        [Header("技能位移曲线(组)")] public SkillMoveCurveTM[] skillMoveCurveTMArray;
        [Header("碰撞器(组)")] public EntityColliderTriggerTM[] collisionTriggerTMArray;

    }

}