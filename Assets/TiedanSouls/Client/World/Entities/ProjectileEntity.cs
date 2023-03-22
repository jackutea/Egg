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

        ProjectileElement[] leafElementArray;
        public ProjectileElement[] LeafElementArray => leafElementArray;

        Transform rootElementTrans;
        Transform leafElementGroupTrans;

        #endregion

        int curFrame;
        public int CurFrame => curFrame;
        public void AddFrame() => curFrame++;

        public void Ctor() {
            idCom = new IDComponent();
            idCom.SetEntityType(EntityType.Projectile);
            fsmCom = new ProjectileFSMComponent();

            rootElementTrans = transform.Find("root_element");
            leafElementGroupTrans = transform.Find("leaf_element_group");

            curFrame = -1;
        }

        public void TearDown() {
            rootElement.TearDown();
            var len = leafElementArray.Length;
            for (int i = 0; i < len; i++) {
                leafElementArray[i].TearDown();
            }
        }

        public void Reset() {
            rootElement.Reset();
            var len = leafElementArray.Length;
            for (int i = 0; i < len; i++) {
                leafElementArray[i].Reset();
            }
        }

        public void Hide() {
            rootElement.Deactivated();
            var len = leafElementArray.Length;
            for (int i = 0; i < len; i++) {
                leafElementArray[i].Deactivated();
            }
        }

        public void Show() {
            rootElement.Activated();
            var len = leafElementArray.Length;
            for (int i = 0; i < len; i++) {
                leafElementArray[i].Activated();
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
            this.leafElementArray = elementArray;
        }

        public void SetPos(Vector2 value) {
            transform.position = value;
        }

        #region [根据帧 判断状态]

        public bool IsDead(int frame) {
            return frame > rootElement.EndFrame;
        }

        #endregion


        #region [表现层同步]

        public void Renderer_Sync() {
            rootElement.Renderer_Sync();
            var len = leafElementArray.Length;
            for (int i = 0; i < len; i++) {
                leafElementArray[i].Renderer_Sync();
            }
        }

        public void Renderer_Easing(float dt) {
            rootElement.Renderer_Easing(dt);
            var len = leafElementArray.Length;
            for (int i = 0; i < len; i++) {
                leafElementArray[i].Renderer_Easing(dt);
            }
        }

        #endregion

    }

}