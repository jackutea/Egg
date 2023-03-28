namespace TiedanSouls.Client.Service {

    public class IDService {

        int roleIDRecord;
        int skillIDRecord;
        int itemIDRecord;
        int fieldIDRecord;
        int projectileIDRecord;
        int bulletIDRecord;
        int buffIDRecord;

        public IDService() { }

        public void Reset() {
            roleIDRecord = 0;
            skillIDRecord = 0;
            itemIDRecord = 0;
            fieldIDRecord = 0;
            projectileIDRecord = 0;
            bulletIDRecord = 0;
            buffIDRecord = 0;
        }

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

        public int PickProjectileID() {
            projectileIDRecord += 1;
            return projectileIDRecord;
        }

        public int PickBulletID() {
            bulletIDRecord += 1;
            return bulletIDRecord;
        }

        public int PickBuffID() {
            buffIDRecord += 1;
            return buffIDRecord;
        }

    }
}