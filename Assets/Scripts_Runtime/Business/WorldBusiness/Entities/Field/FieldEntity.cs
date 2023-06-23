using TiedanSouls.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TiedanSouls.Client.Entities {

    public class FieldEntity : IEntity {

        // - 组件
        EntityIDComponent idCom;
        public EntityIDComponent IDCom => idCom;

        FieldFSMComponent fsmComponent;
        public FieldFSMComponent FSMComponent => fsmComponent;

        // - 章节信息
        ushort chapter;
        public ushort Chapter => chapter;

        ushort level;
        public ushort Level => level;

        FieldType fieldType;
        public FieldType FieldType => fieldType;
        public void SetFieldType(FieldType v) => fieldType = v;

        // - 实体生成控制
        FieldSpawnEntityCtrlModel[] entitySpawnCtrlModelArray;
        public FieldSpawnEntityCtrlModel[] EntitySpawnCtrlModelArray => entitySpawnCtrlModelArray;
        public void SetEntitySpawnCtrlModelArray(FieldSpawnEntityCtrlModel[] v) => entitySpawnCtrlModelArray = v;

        Vector2[] itemSpawnPosArray;
        public Vector2[] ItemSpawnPosArray => itemSpawnPosArray;
        public void SetItemSpawnPosArray(Vector2[] v) => itemSpawnPosArray = v;

        // - 通道
        FieldDoorModel[] fieldDoorArray;
        public FieldDoorModel[] FieldDoorArray => fieldDoorArray;
        public void SetFieldDoorArray(FieldDoorModel[] v) => fieldDoorArray = v;

        // GO
        public GameObject ModGO { get; private set; }
        public GameObject ConfinerGO { get; private set; }
        public BoxCollider2D Confiner { get; private set; }
        public Vector2 ConfinerSize { get; private set; }

        // - 实体碰撞器
        public EntityCollider[] EntityColliderModelArray { get; private set; }

        public FieldEntity() {
            idCom = new EntityIDComponent();
            idCom.SetEntityType(EntityType.Field);
            idCom.SetHolderPtr(this);
            fsmComponent = new FieldFSMComponent();
        }

        public void SetFieldMod(GameObject v) {
            this.ModGO = v;
            this.ConfinerGO = ModGO.transform.Find("confiner").gameObject;
            this.Confiner = ConfinerGO.GetComponent<BoxCollider2D>();
            this.ConfinerSize = Confiner.size;

            // - 实体碰撞器
            var colliderArray = ModGO.GetComponentsInChildren<TilemapCollider2D>();
            var colliderCount = colliderArray.Length;
            EntityColliderModelArray = new EntityCollider[colliderCount];
            for (int i = 0; i < colliderCount; i++) {
                var collider = colliderArray[i];
                var entityCollider = collider.gameObject.AddComponent<EntityCollider>();
                entityCollider.Ctor();
                entityCollider.SetHolder(idCom);
                EntityColliderModelArray[i] = entityCollider;
            }
        }

        public void Hide() {
            TDLog.Log($"隐藏关卡: {ModGO.name}");
            ModGO.SetActive(false);
        }

        public void Show() {
            TDLog.Log($"显示关卡: {ModGO.name}");
            ModGO.SetActive(true);
        }

        public bool TryFindDoorByIndex(int doorIndex, out FieldDoorModel door) {
            if(fieldDoorArray == null || fieldDoorArray.Length == 0){
                door = default;
                return false;
            }

            if (doorIndex < 0 || doorIndex >= fieldDoorArray.Length) {
                door = default;
                return false;
            }

            door = fieldDoorArray[doorIndex];
            return true;
        }

    }
}
