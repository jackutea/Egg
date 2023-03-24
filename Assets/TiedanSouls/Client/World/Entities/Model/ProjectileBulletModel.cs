using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 弹道子弹模型
    /// </summary>
    public struct ProjectileBulletModel {

        public int startFrame;
        public int endFrame;
        public int bulletTypeID;
        public int extraHitTimes;
        public Vector3Int localPos;
        public Vector3Int localEulerAngles;

        public int bulletEntityID;

    }

}