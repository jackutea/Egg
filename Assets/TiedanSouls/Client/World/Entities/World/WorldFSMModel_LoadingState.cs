namespace TiedanSouls.Generic {

    public class WorldStateModel_LoadingState {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        bool isLoadingCompleted;
        public bool IsLoadingCompleted => isLoadingCompleted;
        public void SetIsLoadingCompleted(bool value) => isLoadingCompleted = value;

        int fromFieldTypeID;
        public int FromFieldTypeID => fromFieldTypeID;
        public void SetFromFieldTypeID(int value) => fromFieldTypeID = value;

        int nextFieldTypeID;
        public int NextFieldTypeID => nextFieldTypeID;
        public void SetNextFieldTypeID(int value) => nextFieldTypeID = value;

        int doorIndex;
        public int DoorIndex => doorIndex;
        public void SetDoorIndex(int value) => doorIndex = value;

        public int completeLoadingDelayFrame;

    }

}