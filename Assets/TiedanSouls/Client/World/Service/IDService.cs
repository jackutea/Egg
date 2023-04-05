namespace TiedanSouls.Client.Service {

    public class IDService {

        short roleIDRecord;
        short skillIDRecord;
        short itemIDRecord;
        short fieldIDRecord;
        short projectileIDRecord;
        short bulletIDRecord;
        short buffIDRecord;

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

        public short PickRoleID() {
            roleIDRecord += 1;
            return roleIDRecord;
        }

        public short PickSkillID() {
            skillIDRecord += 1;
            return skillIDRecord;
        }

        public short PickItemID() {
            itemIDRecord += 1;
            return itemIDRecord;
        }

        public short PickFieldID() {
            fieldIDRecord += 1;
            return fieldIDRecord;
        }

        public short PickProjectileID() {
            projectileIDRecord += 1;
            return projectileIDRecord;
        }

        public short PickBulletID() {
            bulletIDRecord += 1;
            return bulletIDRecord;
        }

        public short PickBuffID() {
            buffIDRecord += 1;
            return buffIDRecord;
        }

    }
}