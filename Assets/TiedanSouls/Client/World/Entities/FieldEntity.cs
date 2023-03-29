using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class FieldEntity : MonoBehaviour {


        #region [组件]

        EntityIDComponent idCom;
        public EntityIDComponent IDCom => idCom;

        FieldFSMComponent fsmComponent;
        public FieldFSMComponent FSMComponent => fsmComponent;

        #endregion

        #region  [章节信息]

        ushort chapter;
        public ushort Chapter => chapter;

        ushort level;
        public ushort Level => level;

        #endregion

        #region [实体生成控制模型]

        EntitySpawnCtrlModel[] entitySpawnCtrlModelArray;
        public EntitySpawnCtrlModel[] EntitySpawnCtrlModelArray => entitySpawnCtrlModelArray;
        public void SetEntitySpawnCtrlModelArray(EntitySpawnCtrlModel[] v) => entitySpawnCtrlModelArray = v;

        Vector2[] itemSpawnPosArray;
        public Vector2[] ItemSpawnPosArray => itemSpawnPosArray;
        public void SetItemSpawnPosArray(Vector2[] v) => itemSpawnPosArray = v;

        #endregion

        FieldType fieldType;
        public FieldType FieldType => fieldType;
        public void SetFieldType(FieldType v) => fieldType = v;

        FieldDoorModel[] fieldDoorArray;
        public FieldDoorModel[] FieldDoorArray => fieldDoorArray;
        public void SetFieldDoorArray(FieldDoorModel[] v) => fieldDoorArray = v;

        BoxCollider2D confiner;
        public Vector2 ConfinerSize => confiner.size;

        public void Ctor() {
            confiner = transform.Find("confiner").GetComponent<BoxCollider2D>();
            TDLog.Assert(confiner != null);

            idCom = new EntityIDComponent();
            idCom.SetEntityType(EntityType.Field);
            fsmComponent = new FieldFSMComponent();
        }

        public void Hide() {
            TDLog.Log($"隐藏关卡: {name}");
            gameObject.SetActive(false);
        }

        public void Show() {
            TDLog.Log($"显示关卡: {name}");
            gameObject.SetActive(true);
        }

        public bool TryFindDoorByIndex(int doorIndex, out FieldDoorModel door) {
            door = default;
            if (doorIndex < 0 || doorIndex >= fieldDoorArray.Length) {
                return false;
            }

            door = fieldDoorArray[doorIndex];
            return true;
        }

    }
}
