using System;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 移动曲线模型
    /// </summary>
    [Serializable]
    public struct MoveCurveModel {

        public float[] moveSpeedArray;          // 移动速度数组
        public Vector3[] moveDirArray;          // 移动方向数组

    }

}