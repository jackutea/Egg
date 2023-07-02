using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct SkillTM {

        [Header("类型ID")] public int typeID;
        [Header("技能名称")] public string skillName;
        public SkillCastKey castKey;
        [Header("技能类型")] public SkillType skillType;
        [Header("技能持续帧数")] public int maintainFrame;

        [Header("武器动画名")] public string weaponAnimName;
        [Header("仅用于编辑时: 武器动画文件GUID")] public string weaponAnimClip_GUID;

        [Header("原始技能")] public int originSkillTypeID;
        [Header("连招技名单     =================================== ")] public SkillCancelTM[] cancelSkillCancelTMArray;
        [Header("效果器组       =================================== ")] public EffectorTriggerTM[] effectorTriggerEMArray;
        [Header("角色召唤组     =================================== ")] public RoleSummonTM[] roleSummonTMArray;
        [Header("弹幕生成组     =================================== ")] public ProjectileCtorTM[] projectileCtorTMArray;
        [Header("Buff附加组     =================================== ")] public BuffAttachTM[] buffAttachTMArray;
        [Header("技能位移组     =================================== ")] public SkillMoveCurveTM[] skillMoveCurveTMArray;
        [Header("碰撞器组       =================================== ")] public HitToggleTM[] hitToggleTMArray;

    }

}