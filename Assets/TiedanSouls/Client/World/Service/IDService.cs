namespace TiedanSouls.World.Service {

    public class IDService {

        int roleIDRecord;
        int skillorIDRecord;
        int itemIDRecord;

        public IDService() { }

        public int PickRoleID() {
            roleIDRecord += 1;
            return roleIDRecord;
        }

        public int PickSkillorID() {
            skillorIDRecord += 1;
            return skillorIDRecord;
        }

        public int PickItemID() {
            itemIDRecord += 1;
            return itemIDRecord;
        }

    }
}