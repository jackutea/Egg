namespace TiedanSouls.World.Entities {

    public class RoleCastingStateModel {

        public SkillorModel castingSkillor;
        public int maintainFrame;

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        public RoleCastingStateModel() { }

    }
}