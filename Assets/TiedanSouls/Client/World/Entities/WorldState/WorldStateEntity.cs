namespace TiedanSouls.World.Entities {

    public class WorldStateEntity {

        WorldFSMState status;
        public WorldFSMState Status => status;

        // ====== FSM Model ======
        WorldFSMModel_LobbyState lobbyStateModel;
        public WorldFSMModel_LobbyState LobbyStateModel => lobbyStateModel;

        WorldFSMModel_BattleState battleStateModel;
        public WorldFSMModel_BattleState BattleStateModel => battleStateModel;

        WorldFSMModel_LoadingState loadingStateModel;
        public WorldFSMModel_LoadingState LoadingStateModel => loadingStateModel;

        int curFieldTypeID;
        public int CurFieldTypeID => curFieldTypeID;

        public WorldStateEntity() {
            this.lobbyStateModel = new WorldFSMModel_LobbyState();
            this.battleStateModel = new WorldFSMModel_BattleState();
            this.loadingStateModel = new WorldFSMModel_LoadingState();
        }

        public void EnterState_Loading(int fromFieldTypeID, int nextFieldTypeID, int doorIndex) {
            this.status = WorldFSMState.Loading;

            loadingStateModel.SetIsEntering(true);
            loadingStateModel.SetIsLoadingComplete(false);
            loadingStateModel.SetFromFieldTypeID(fromFieldTypeID);
            loadingStateModel.SetNextFieldTypeID(nextFieldTypeID);
            loadingStateModel.SetDoorIndex(doorIndex);

            TDLog.Log($"------> 世界状态: '{status}' FromFieldTypeID: {fromFieldTypeID} / NextFieldTypeID: {nextFieldTypeID} / DoorIndex: {doorIndex}");
        }

        public void EnterState_Lobby(int fieldTypeID) {
            this.status = WorldFSMState.Lobby;
            this.curFieldTypeID = fieldTypeID;

            lobbyStateModel.SetIsEntering(true);

            TDLog.Log($"------> 世界状态: '{status}' FieldTypeID: {fieldTypeID}");
        }

        public void EnterState_Battle(int fieldTypeID) {
            this.status = WorldFSMState.Battle;
            this.curFieldTypeID = fieldTypeID;

            battleStateModel.SetIsEntering(true);

            TDLog.Log($"------> 世界状态: '{status}' FieldTypeID: {fieldTypeID}");
        }

        public void EnterState_Store(int fieldTypeID) {
            this.status = WorldFSMState.Store;

            TDLog.Log($"------> 世界状态: '{status}' FieldTypeID: {fieldTypeID}");
        }

    }

}