using TiedanSouls.Generic;
using System;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 弹道实体
    /// </summary>
    public class ProjectileEntity : IEntity {

        #region [组件]

        EntityIDComponent idCom;
        public EntityIDComponent IDCom => idCom;

        ProjectileFSMComponent fsmCom;
        public ProjectileFSMComponent FSMCom => fsmCom;

        #endregion

        #region [弹道子弹模型组]

        ProjectileBulletModel[] projectileBulletModelArray;
        public ProjectileBulletModel[] ProjectileBulletModelArray => projectileBulletModelArray;
        public void SetProjectileBulletModelArray(ProjectileBulletModel[] projectileBulletModelArray) => this.projectileBulletModelArray = projectileBulletModelArray;

        #endregion

        #region [生命周期]

        int curFrame;
        public int CurFrame => curFrame;
        public void AddFrame() => curFrame++;

        #endregion

        public void Ctor() {
            idCom = new EntityIDComponent();
            idCom.SetEntityType(EntityType.Projectile);
            fsmCom = new ProjectileFSMComponent();
            curFrame = -1;
        }

        public void TearDown() { }

        public void Reset() { }

        /// <summary>
        /// 遍历当前帧需要激活的子弹ID
        /// </summary>
        public void Foreach_NeedActivatedBulletID(Action<int> action) {
            var len = projectileBulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var model = projectileBulletModelArray[i];
                var startFrame = model.startFrame;
                var endFrame = model.endFrame;
                if (curFrame == startFrame) {
                    action(model.bulletEntityID);
                }
            }
        }

        /// <summary>
        /// 遍历当前帧需要取消激活的子弹ID
        /// </summary>
        public void Foreach_BulletID(Action<int> action) {
            var len = projectileBulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var model = projectileBulletModelArray[i];
                var startFrame = model.startFrame;
                var endFrame = model.endFrame;
                if (curFrame == endFrame) {
                    action(model.bulletEntityID);
                }
            }
        }

    }

}