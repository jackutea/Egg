namespace TiedanSouls.World.Service {

    public class IDService {

        int roleIDRecord;
        int skillorIDRecord;

        public IDService() { }

        public int PickRoleID() {
            roleIDRecord += 1;
            return roleIDRecord;
        }

        public int PickSkillorID() {
            skillorIDRecord += 1;
            return skillorIDRecord;
        }
        
    }
}