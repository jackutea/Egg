using TiedanSouls.World.Domain;

namespace TiedanSouls.World.Facades {

    public class WorldDomain {

        WorldGameDomain gameDomain;
        public WorldGameDomain GameDomain => gameDomain;

        WorldFieldDomain fieldDomain;
        public WorldFieldDomain FieldDomain => fieldDomain;

        WorldRoleDomain roleDomain;
        public WorldRoleDomain RoleDomain => roleDomain;

        public WorldDomain() {
            gameDomain = new WorldGameDomain();
            fieldDomain = new WorldFieldDomain();
            roleDomain = new WorldRoleDomain();
        }

    }
}