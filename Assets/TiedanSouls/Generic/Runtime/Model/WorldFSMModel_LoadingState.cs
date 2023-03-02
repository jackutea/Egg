namespace TiedanSouls {

    public class WorldFSMModel_LoadingState {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        bool isLoadingComplete;
        public bool IsLoadingComplete => isLoadingComplete;
        public void SetIsLoadingComplete(bool value) => isLoadingComplete = value;

        int fromFieldTypeID;
        public int FromFieldTypeID => fromFieldTypeID;
        public void SetFromFieldTypeID(int value) => fromFieldTypeID = value;

        int nextFieldTypeID;
        public int NextFieldTypeID => nextFieldTypeID;
        public void SetNextFieldTypeID(int value) => nextFieldTypeID = value;

        int doorIndex;
        public int DoorIndex => doorIndex;
        public void SetDoorIndex(int value) => doorIndex = value;

    }

}