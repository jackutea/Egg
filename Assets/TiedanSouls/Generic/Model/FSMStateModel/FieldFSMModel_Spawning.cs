namespace TiedanSouls.Generic {

    public class FieldFSMModel_Spawning {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public bool isRespawning;

        public int curFrame;

        public FieldFSMModel_Spawning() { }

        public void Reset() {
            isEntering = false;
            curFrame = 0;
        }

    }

}