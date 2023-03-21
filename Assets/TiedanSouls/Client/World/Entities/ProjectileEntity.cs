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

        #region [弹道元素]

        ProjectileElement rootElement;
        public ProjectileElement RootElement => rootElement;

        ProjectileElement[] leafElements;
        public ProjectileElement[] LeafElements => leafElements;

        Transform rootElementTrans;
        Transform leafElementGroupTrans;

        #endregion

        public void Ctor() {
            idCom = new IDComponent();
            idCom.SetEntityType(EntityType.Projectile);
            fsmCom = new ProjectileFSMComponent();

            rootElementTrans = transform.Find("root_element");
            leafElementGroupTrans = transform.Find("leaf_element_group");
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
            rootElement.Deactivated();
            var len = leafElements.Length;
            for (int i = 0; i < len; i++) {
                leafElements[i].Deactivated();
            }
        }

        public void Show() {
            rootElement.Activated();
            var len = leafElements.Length;
            for (int i = 0; i < len; i++) {
                leafElements[i].Activated();
            }
        }

        public void SetRootElement(ProjectileElement element) {
            element.RootGO.transform.SetParent(rootElementTrans, false);
            this.rootElement = element;
        }

        public void SetLeafElements(ProjectileElement[] elementArray) {
            var len = elementArray.Length;
            for (int i = 0; i < len; i++) {
                elementArray[i].RootGO.transform.SetParent(leafElementGroupTrans, false);
            }
            this.leafElements = elementArray;
        }

        public void SetPos(Vector2 value) {
            transform.position = value;
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