namespace TiedanSouls.World.Service {

    public class IDService {

        int roleIDRecord;

        public IDService() { }

        public int PickRoleID() {
            roleIDRecord += 1;
            return roleIDRecord;
        }
    }
}