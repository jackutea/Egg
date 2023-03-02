namespace TiedanSouls {

    public class WorldFSMModel_BattleState {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        int fieldTypeID;
        public int FieldTypeID => fieldTypeID;
        public void SetFieldTypeID(int value) => fieldTypeID = value;

    }

}