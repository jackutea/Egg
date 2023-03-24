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

        [Header("打击效果器")] public EffectorEM hitEffectorEM;
        [Header("死亡效果器")] public EffectorEM deathEffectorEM;

        [Header("子弹特效")] public GameObject vfxPrefab;

        [Header("位移(cm)")] public int moveDistance_cm;
        [Header("位移时间(帧)")] public int moveTotalFrame;
        [Header("位移曲线")] public AnimationCurve disCurve;

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
            this.hitEffectorEM = TM2EMUtil.GetEM_Effector(tm.hitEffectorTM);
            this.deathEffectorEM = TM2EMUtil.GetEM_Effector(tm.deathEffectorTM);

            var vfxGUI = tm.vfxPrefab_GUID;
            this.vfxPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(vfxGUI), typeof(GameObject)) as GameObject;

            this.moveDistance_cm = tm.moveDistance_cm;
            this.moveTotalFrame = tm.moveTotalFrame;
            this.disCurve = TM2EMUtil.GetAnimationCurve(tm.disCurve_KeyframeTMArray);
        }

    }

}