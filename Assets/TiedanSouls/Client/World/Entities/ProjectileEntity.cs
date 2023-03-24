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

        ProjectileBulletModel[] bulletModelArray;

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
            var len = bulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = bulletModelArray[i];
                var bullet = bulletModel.bullet;
                bullet.Deactivate();
            }
        }

        public void Activate() {
            var len = bulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = bulletModelArray[i];
                var bullet = bulletModel.bullet;
                bullet.Activate();
            }
        }

        public void SetBulletEntityArray(ProjectileBulletModel[] bulletModelArray) {
            var len = bulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = bulletModelArray[i];
                var bullet = bulletModel.bullet;
                bullet.RootGO.transform.SetParent(this.transform);
            }
            this.bulletModelArray = bulletModelArray;
        }

        public void SetPos(Vector2 value) {
            transform.position = value;
        }

        #region [表现层同步]

        public void Renderer_Sync() {
            var len = bulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = bulletModelArray[i];
                var bullet = bulletModel.bullet;
                bullet.Renderer_Sync();
            }
        }

        public void Renderer_Easing(float dt) {
            var len = bulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = bulletModelArray[i];
                var bullet = bulletModel.bullet;
                bullet.Renderer_Easing(dt);
            }
        }

        #endregion

    }

}