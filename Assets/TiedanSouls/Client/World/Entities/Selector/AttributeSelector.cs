using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {
    /// <summary>
    /// ComparisonType 为 None 时，不进行对比,即不在条件中
    /// </summary>
    public class AttributeSelectorModel {

        public int hp;
        public ComparisonType hp_ComparisonType;

        public int hpMax;
        public ComparisonType hpMax_ComparisonType;

        public int ep;
        public ComparisonType ep_ComparisonType;

        public int epMax;
        public ComparisonType epMax_ComparisonType;

        public int gp;
        public ComparisonType gp_ComparisonType;

        public int gpMax;
        public ComparisonType gpMax_ComparisonType;

        public float moveSpeed;
        public ComparisonType moveSpeed_ComparisonType;

        public float jumpSpeed;
        public ComparisonType jumpSpeed_ComparisonType;

        public float fallingAcceleration;
        public ComparisonType fallingAcceleration_ComparisonType;

        public float fallingSpeedMax;
        public ComparisonType fallingSpeedMax_ComparisonType;

        public bool IsMatch(AttributeComponent attributeCom) {
            return hp_ComparisonType.IsMatch(attributeCom.HP, hp)
            && hpMax_ComparisonType.IsMatch(attributeCom.HPMax, hpMax)
            && ep_ComparisonType.IsMatch(attributeCom.EP, ep)
            && epMax_ComparisonType.IsMatch(attributeCom.EPMax, epMax)
            && gp_ComparisonType.IsMatch(attributeCom.GP, gp)
            && gpMax_ComparisonType.IsMatch(attributeCom.GPMax, gpMax)
            && moveSpeed_ComparisonType.IsMatch(attributeCom.MoveSpeed, moveSpeed)
            && jumpSpeed_ComparisonType.IsMatch(attributeCom.JumpSpeed, jumpSpeed)
            && fallingAcceleration_ComparisonType.IsMatch(attributeCom.FallingAcceleration, fallingAcceleration)
            && fallingSpeedMax_ComparisonType.IsMatch(attributeCom.FallingSpeedMax, fallingSpeedMax);
        }

    }

}