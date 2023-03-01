namespace TiedanSouls.World.Entities {

    public class WorldStateEntity {

        WorldFSMState status;
        public WorldFSMState Status => status;

        int ownerRoleID;
        public int OwnerRoleID => ownerRoleID;

        // ====== FSM Model ======
        WorldFSMModel_LobbyState lobbyStateModel;
        public WorldFSMModel_LobbyState LobbyStateModel => lobbyStateModel;

        WorldFSMModel_BattleFieldState battleFieldStateModel;
        public WorldFSMModel_BattleFieldState BattleFieldStateModel => battleFieldStateModel;

        public WorldStateEntity() {
            this.lobbyStateModel = new WorldFSMModel_LobbyState();
            this.battleFieldStateModel = new WorldFSMModel_BattleFieldState();
        }

        public void EnterState_Lobby(int ownerRoleID) {
            this.status = WorldFSMState.Lobby;
            this.ownerRoleID = ownerRoleID;
            lobbyStateModel.SetIsEntering(true);
            TDLog.Log($"进入大厅状态, OwnerRoleID: {ownerRoleID}");
        }

        public void EnterState_BattleField(int ownerRoleID) {
            this.status = WorldFSMState.BattleField;
            this.ownerRoleID = ownerRoleID;
            battleFieldStateModel.SetIsEntering(true);
            TDLog.Log($"进入战场状态, OwnerRoleID: {ownerRoleID}");
        }

    }

}