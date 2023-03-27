using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    [Serializable]
    public struct RoleStateEffectEM {

        [Header("添加状态标记")] public RoleStateFlag addStateFlag;
        [Header("模型: 击飞")] public KnockUpEM knockUpEM;
        [Header("模型: 击退")] public KnockBackEM knockBackEM;

    }

}