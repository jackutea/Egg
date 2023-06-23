using TiedanSouls.Generic;
using System;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 弹幕实体
    /// </summary>
    public class ProjectileEntity : IEntity {

        #region [组件]

        EntityIDComponent idCom;
        public EntityIDComponent IDCom => idCom;

        ProjectileFSMComponent fsmCom;
        public ProjectileFSMComponent FSMCom => fsmCom;

        #endregion

        #region [弹幕子弹模型组]

        ProjectileBulletModel[] projectileBulletModelArray;
        public ProjectileBulletModel[] ProjectileBulletModelArray => projectileBulletModelArray;
        public void SetProjectileBulletModelArray(ProjectileBulletModel[] projectileBulletModelArray) => this.projectileBulletModelArray = projectileBulletModelArray;

        #endregion

        #region [生命周期]

        int curFrame;
        public int CurFrame => curFrame;
        public void AddFrame() => curFrame++;

        #endregion

        int[] bulletIDArray;
        public int[] BulletIDArray => bulletIDArray;
        public void SetBulletIDArray(int[] bulletIDArray) => this.bulletIDArray = bulletIDArray;

        public ProjectileEntity() {
            idCom = new EntityIDComponent();
            idCom.SetEntityType(EntityType.Projectile);
            idCom.SetHolderPtr(this);
            fsmCom = new ProjectileFSMComponent();
            curFrame = -1;
        }

        public void TearDown() { }

        public void Reset() { }

    }

}