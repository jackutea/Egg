using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class SkillTM {

        [Header("类型ID")] public int typeID;
        [Header("技能名称")] public string skillName;
        [Header("技能类型")] public SkillType skillType;
        [Header("技能持续帧数")] public int maintainFrame;

        [Header("原始技能")] public int originSkillTypeID;
        [Header("组合技名单")] public SkillCancelTM[] comboSkillCancelTMArray;
        [Header("连招技名单")] public SkillCancelTM[] cancelSkillCancelTMArray;

        [Header("碰撞器(组)")] public CollisionTriggerTM[] collisionTriggerTMArray;
        
        [Header("武器动画名")] public string weaponAnimName;
        [Header("仅用于编辑时: 武器动画文件GUID")] public string weaponAnimClip_GUID;

        public SkillTM() { }

    }

}