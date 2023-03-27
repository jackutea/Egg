using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct RoleStateEffectTM {

        [Header("添加状态标记")] public RoleStateFlag addStateFlag;
        [Header("模型: 击飞")] public KnockUpTM knockUpTM;
        [Header("模型: 击退")] public KnockBackTM knockBackTM;

    }

}