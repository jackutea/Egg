using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 弹道实体
    /// </summary>
    public class ProjectileEntity : IEntity {

        #region [组件]

        IDComponent idCom;
        public IDComponent IDCom => idCom;

        ProjectileFSMComponent fsmCom;
        public ProjectileFSMComponent FSMCom => fsmCom;

        #endregion

        #region [弹道子弹模型组]

        ProjectileBulletModel[] projectileBulletModelArray;
        public ProjectileBulletModel[] ProjectileBulletModelArray => projectileBulletModelArray;

        #endregion

        #region [生命周期]

        int curFrame;
        public int CurFrame => curFrame;
        public void AddFrame() => curFrame++;

        #endregion

        public void Ctor() {
            idCom = new IDComponent();
            idCom.SetEntityType(EntityType.Projectile);
            fsmCom = new ProjectileFSMComponent();
            curFrame = -1;
        }

        public void TearDown() { }

        public void Reset() { }

        #region [激活 & 取消激活]

        public void Activate() {
            var len = projectileBulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = projectileBulletModelArray[i];
                var bullet = bulletModel.bulletEntity;
                bullet.Activate();
            }
        }

        public void Deactivate() {
            var len = projectileBulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = projectileBulletModelArray[i];
                var bullet = bulletModel.bulletEntity;
                bullet.Deactivate();
            }
        }

        #endregion

        #region [设置Transform]

        public void Renderer_Sync() {
            var len = projectileBulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = projectileBulletModelArray[i];
                var bullet = bulletModel.bulletEntity;
                bullet.Renderer_Sync();
            }
        }

        public void Renderer_Easing(float dt) {
            var len = projectileBulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = projectileBulletModelArray[i];
                var bullet = bulletModel.bulletEntity;
                bullet.Renderer_Easing(dt);
            }
        }

        #endregion

        public void SetProjectileBulletModelArray(ProjectileBulletModel[] projectileBulletModelArray) {
            var len = projectileBulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = projectileBulletModelArray[i];
                var bullet = bulletModel.bulletEntity;
            }
            this.projectileBulletModelArray = projectileBulletModelArray;
        }

    }

}