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

        [Header("轨迹类型")] public TrajectoryType trajectoryType;
        [Header("模型: 位移曲线")] public MoveCurveEM moveCurveEM;
        [Header("模型: 实体追踪")] public EntityTrackEM entityTrackingEM;
        [Header("碰撞器")] public CollisionTriggerEM collisionTriggerEM;

        [Header("死亡效果器(类型ID)")] public int deathEffectorTypeID;

        [Header("子弹特效")] public GameObject vfxPrefab;

        public void Save() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }

            so.tm = EM2TMUtil.GetBulletTM(this);

            EditorUtility.SetDirty(so);
            EditorUtility.SetDirty(gameObject);

            var labelName = AssetLabelCollection.SO_BULLET;
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

            this.collisionTriggerEM = TM2EMUtil.GetCollisionTriggerEM(tm.collisionTriggerTM);
            this.deathEffectorTypeID = tm.deathEffectorTypeID;

            var vfxGUID = tm.vfxPrefab_GUID;
            this.vfxPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(vfxGUID), typeof(GameObject)) as GameObject;

            this.trajectoryType = tm.trajectoryType;

            this.moveCurveEM = TM2EMUtil.GetMoveCurveEM(tm.moveCurveTM);
            this.entityTrackingEM = TM2EMUtil.GetEntityTrackEM(tm.entityTrackTM);

        }

    }

}