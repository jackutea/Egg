using UnityEngine;

namespace TiedanSouls.Generic {

    public class FieldFSMModel_Ready {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        FieldDoorModel enterDoorModel;
        public FieldDoorModel EnterDoorModel => enterDoorModel;
        public void SetEnterDoorModel(FieldDoorModel value) => enterDoorModel = value;

        public FieldFSMModel_Ready() { }

        public void Reset() {
            isEntering = false;
            enterDoorModel = default;
        }

    }

}