using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 弹道实体
    /// </summary>
    public class ProjectileEntity : MonoBehaviour, IEntity {

        #region [组件]

        IDComponent idCom;
        public IDComponent IDCom => idCom;

        ProjectileFSMComponent fsmCom;
        public ProjectileFSMComponent FSMCom => fsmCom;

        #endregion

        Vector2 bornPos;
        public Vector2 BornPos => bornPos;
        public void SetBornPos(Vector2 value) => this.bornPos = value;

        #region [弹道子弹模型组]

        ProjectileBulletModel[] projectileBulletModelArray;
        public ProjectileBulletModel[] ProjectileBulletModelArray => projectileBulletModelArray;

        #endregion

        int curFrame;
        public int CurFrame => curFrame;
        public void AddFrame() => curFrame++;

        public void Ctor() {
            idCom = new IDComponent();
            idCom.SetEntityType(EntityType.Projectile);
            fsmCom = new ProjectileFSMComponent();

            curFrame = -1;
        }

        public void TearDown() {
        }

        public void Reset() {
        }

        public void Deactivate() {
            var len = projectileBulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = projectileBulletModelArray[i];
                var bullet = bulletModel.bulletEntity;
                bullet.Deactivate();
            }
        }

        public void Activate() {
            var len = projectileBulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = projectileBulletModelArray[i];
                var bullet = bulletModel.bulletEntity;
                bullet.Activate();
            }
        }

        public void SeProjectileBulletModelArray(ProjectileBulletModel[] projectileBulletModelArray) {
            var len = projectileBulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = projectileBulletModelArray[i];
                var bullet = bulletModel.bulletEntity;
                bullet.RootGO.transform.SetParent(this.transform);
            }
            this.projectileBulletModelArray = projectileBulletModelArray;
        }

        public void SetPos(Vector2 value) {
            transform.position = value;
        }

        #region [表现层同步]

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

    }

}