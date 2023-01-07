using TiedanSouls.Asset;

namespace TiedanSouls.World.Facades {

    public class WorldContext {

        WorldFactory worldFactory;
        public WorldFactory WorldFactory => worldFactory;

        public WorldContext() {
            worldFactory = new WorldFactory();
        }

    }

}