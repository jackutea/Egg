using UnityEngine;

namespace TiedanSouls.EditorTool {

    public struct TriggerFixedIntervalEM {

        [Header("延迟帧")] public int delayFrame;
        [Header("触发间隔帧")] public int intervalFrame;
        [Header("触发持续帧)")] public int maintainFrame;

    }

}