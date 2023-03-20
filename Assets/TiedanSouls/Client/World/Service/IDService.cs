namespace TiedanSouls.Client.Service {

    public class IDService {

        int roleIDRecord;
        int skillIDRecord;
        int itemIDRecord;
        int fieldIDRecord;

        public IDService() { }

        public int PickRoleID() {
            roleIDRecord += 1;
            return roleIDRecord;
        }

        public int PickSkillID() {
            skillIDRecord += 1;
            return skillIDRecord;
        }

        public int PickItemID() {
            itemIDRecord += 1;
            return itemIDRecord;
        }

        public int PickFieldID() {
            fieldIDRecord += 1;
            return fieldIDRecord;
        }

    }
}