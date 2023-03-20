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

        #region [坐标 & 出生点 & 来源类型]

        Vector2 pos;
        public Vector2 Pos => pos;
        public void SetPos(Vector2 value) => this.pos = value;

        Vector2 bornPos;
        public Vector2 BornPos => bornPos;
        public void SetBornPos(Vector2 value) => this.bornPos = value;

        int fromFieldTypeID;
        public int FromFieldTypeID => fromFieldTypeID;
        public void SetFromFieldTypeID(int value) => this.fromFieldTypeID = value;

        #endregion

        #region [弹道元素]

        ProjectileElement rootElement;
        public ProjectileElement RootElement => rootElement;
        public void SetRootElement(ProjectileElement value) => this.rootElement = value;

        ProjectileElement[] leafElements;
        public ProjectileElement[] LeafElements => leafElements;
        public void SetLeafElements(ProjectileElement[] value) => this.leafElements = value;

        #endregion

        public void Ctor() {
            idCom = new IDComponent();
            idCom.SetEntityType(EntityType.Projectile);
            fsmCom = new ProjectileFSMComponent();
        }

        public void TearDown() {
            rootElement.TearDown();
            var len = leafElements.Length;
            for (int i = 0; i < len; i++) {
                leafElements[i].TearDown();
            }
        }

        public void Reset() {
            rootElement.Reset();
            var len = leafElements.Length;
            for (int i = 0; i < len; i++) {
                leafElements[i].Reset();
            }
        }

        public void Hide() {
            rootElement.Hide();
            var len = leafElements.Length;
            for (int i = 0; i < len; i++) {
                leafElements[i].Hide();
            }
        }

        public void Show() {
            rootElement.Show();
            var len = leafElements.Length;
            for (int i = 0; i < len; i++) {
                leafElements[i].Show();
            }
        }

        #region [Locomotion]

        public void Move() {
            // todo： 移动所有Element，在move的过程中，
            // ~ 会根据生命周期，触发各种事件，比如生命周期结束导致元素死亡。
            // ~ 会根据生命周期，设置不同的速度。
        }

        #endregion

        #region [Renderer]

        public void Renderer_Sync() {
            // todo: 同步所有Element
        }

        public void Renderer_Easing(float dt) {
            // todo: 平滑过渡所有Element
        }

        #endregion

    }

}