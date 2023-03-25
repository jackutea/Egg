using UnityEngine;
using UnityEditor;
using GameArki.AddressableHelper;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    public class BulletEditorGO : MonoBehaviour {

        [Header("绑定配置文件")] public BulletSO so;

        [Header("类型ID")] public int typeID;
        [Header("子弹名称")] public string bulletName;

        [Header("碰撞器")] public CollisionTriggerEM collisionTriggerEM;
        [Header("死亡效果器(类型ID)")] public int deathEffectorTypeID;

        [Header("额外穿透次数")] public int extraPenetrateCount;

        [Header("子弹特效")] public GameObject vfxPrefab;

        #region [子弹轨迹 ----------------------------------------------------]

        [Header("轨迹类型")] public TrajectoryType trajectoryType;

        #region [直线轨迹] TODO: Model

        [Header("位移(m)")] public float moveDistance;
        [Header("位移时间(帧)")] public int moveTotalFrame;
        [Header("位移曲线")] public AnimationCurve disCurve;

        #endregion

        [Header("实体追踪模型")] public EntityTrackEM entityTrackingEM;

        #endregion

        public void Save() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }

            so.tm = EM2TMUtil.GetTM_Bullet(this);

            EditorUtility.SetDirty(so);
            EditorUtility.SetDirty(gameObject);

            var labelName = AssetsLabelCollection.SO_BULLET;
            AddressableHelper.SetAddressable(so, labelName, labelName);
        }

        public void Load() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }

            var tm = so.tm;
            this.typeID = tm.typeID;
            this.bulletName = tm.bulletName;

            this.collisionTriggerEM = TM2EMUtil.GetEM_CollisionTrigger(tm.collisionTriggerTM);
            this.deathEffectorTypeID = tm.deathEffectorTypeID;

            this.extraPenetrateCount = tm.extraPenetrateCount;

            var vfxGUI = tm.vfxPrefab_GUID;
            this.vfxPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(vfxGUI), typeof(GameObject)) as GameObject;

            this.trajectoryType = tm.trajectoryType;

            this.moveTotalFrame = tm.moveTotalFrame;
            this.moveDistance = TM2EMUtil.GetFloat_Shrink100(tm.moveDistance_cm);
            this.disCurve = TM2EMUtil.GetAnimationCurve(tm.disCurve_KeyframeTMArray);
            this.entityTrackingEM = TM2EMUtil.GetEM_EntityTrack(tm.entityTrackTM);

        }

    }

}