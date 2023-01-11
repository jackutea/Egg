namespace TiedanSouls.World.Entities {

    public class RoleCastingStateModel {

        public SkillorModel castingSkillor;
        public float restTime;
        public float targetRate = 1 / 30f;

        public bool isEntering;

        public RoleCastingStateModel() {}

    }
}