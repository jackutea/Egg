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
        public Vector3 localPos;
        public Vector3 localEulerAngles;

        public int bulletEntityID;

    }

}