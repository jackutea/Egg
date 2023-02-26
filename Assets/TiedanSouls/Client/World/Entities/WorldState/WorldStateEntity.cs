namespace TiedanSouls.World.Entities {

    public class WorldStateEntity {

        WorldFSMStatus status;
        public WorldFSMStatus Status => status;

        int ownerRoleID;
        public int OwnerRoleID => ownerRoleID;

        public float phxRestTime;

        public WorldStateEntity() { }

        public void EnterState_Hall(int ownerRoleID) {
            this.status = WorldFSMStatus.Hall;
            this.ownerRoleID = ownerRoleID;
            TDLog.Log($"进入大厅状态, OwnerRoleID: {ownerRoleID}");
        }

    }

}