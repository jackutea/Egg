using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public class SkillTM {

        [Header("基础信息 =================================== ")]
        public int typeID;
        public string skillName;
        public SkillType skillType;

        [Header("生命周期 =================================== ")]
        [Header("开始帧")] public int startFrame;
        [Header("结束帧")] public int endFrame;

        [Header("原始技能")] public int originSkillTypeID;
        [Header("组合技名单")] public SkillCancelTM[] comboSkillCancelTMArray;
        [Header("连招技名单")] public SkillCancelTM[] cancelSkillCancelTMArray;
        [Header("武器动画名")] public string weaponAnimName;
        [Header("仅用于编辑时: 武器动画文件GUID")] public string weaponAnimClip_GUID;

        [Header("打击力度(组)")] public HitPowerTM[] hitPowerTMArray;
        [Header("碰撞器(组)")] public CollisionTriggerTM[] collisionTriggerTMArray;

        public SkillTM() { }

    }

}