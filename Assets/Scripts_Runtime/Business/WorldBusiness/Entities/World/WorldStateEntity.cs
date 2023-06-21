using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class WorldStateEntity {

        WorldFSMState state;
        public WorldFSMState State => state;

        // ====== FSM Model ======
        WorldStateModel_LobbyState lobbyStateModel;
        public WorldStateModel_LobbyState LobbyStateModel => lobbyStateModel;

        WorldStateModel_BattleState battleStateModel;
        public WorldStateModel_BattleState BattleStateModel => battleStateModel;

        WorldStateModel_LoadingState loadingStateModel;
        public WorldStateModel_LoadingState LoadingStateModel => loadingStateModel;

        int curFieldTypeID;
        public int CurFieldTypeID => curFieldTypeID;

        public WorldStateEntity() {
            this.lobbyStateModel = new WorldStateModel_LobbyState();
            this.battleStateModel = new WorldStateModel_BattleState();
            this.loadingStateModel = new WorldStateModel_LoadingState();
        }

        public void EnterState_Loading(int fromFieldTypeID, int nextFieldTypeID, int doorIndex, int completeLoadingDelayFrame = 30) {
            this.state = WorldFSMState.Loading;

            loadingStateModel.SetIsEntering(true);
            loadingStateModel.SetIsLoadingCompleted(false);
            loadingStateModel.SetFromFieldTypeID(fromFieldTypeID);
            loadingStateModel.SetNextFieldTypeID(nextFieldTypeID);
            loadingStateModel.SetDoorIndex(doorIndex);
            loadingStateModel.completeLoadingDelayFrame = completeLoadingDelayFrame;

            TDLog.Log($"------> 世界状态: '{state}' FromFieldTypeID: {fromFieldTypeID} / NextFieldTypeID: {nextFieldTypeID} / DoorIndex: {doorIndex} / CompleteLoadingDelayFrame: {completeLoadingDelayFrame}");
        }

        public void EnterState_Lobby(int fieldTypeID) {
            this.state = WorldFSMState.Lobby;
            this.curFieldTypeID = fieldTypeID;

            lobbyStateModel.SetIsEntering(true);

            TDLog.Log($"------> 世界状态: '{state}' FieldTypeID: {fieldTypeID}");
        }

        public void EnterState_Battle(int fieldTypeID) {
            this.state = WorldFSMState.Battle;
            this.curFieldTypeID = fieldTypeID;

            battleStateModel.SetIsEntering(true);

            TDLog.Log($"------> 世界状态: '{state}' FieldTypeID: {fieldTypeID}");
        }

        public void EnterState_Store(int fieldTypeID) {
            this.state = WorldFSMState.Store;

            TDLog.Log($"------> 世界状态: '{state}' FieldTypeID: {fieldTypeID}");
        }

    }

}