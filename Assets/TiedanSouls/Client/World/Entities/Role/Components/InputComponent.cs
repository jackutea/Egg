using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class InputComponent {

        Vector2 moveAxis;
        public Vector2 MoveAxis => moveAxis;
        public void SetInput_Locomotion_Move(Vector2 value) => this.moveAxis = value;

        bool hasInput_Locomotion_Jump;
        public bool HasInput_Locomotion_JumpDown => hasInput_Locomotion_Jump;
        public void SetInput_Locomotion_Jump(bool value) => this.hasInput_Locomotion_Jump = value;

        bool hasInput_Skillor_Melee;
        public bool HasInput_Skillor_Melee => hasInput_Skillor_Melee;
        public void SetInput_Skillor__Melee(bool value) => this.hasInput_Skillor_Melee = value;

        bool hasInput_Skillor_SpecMelee;
        public bool HasInput_Skillor_SpecMelee => hasInput_Skillor_SpecMelee;
        public void SetInput_Skillor_SpecMelee(bool value) => this.hasInput_Skillor_SpecMelee = value;

        bool hasInpout_Skillor_BoomMelee;
        public bool HasInpout_BoomMelee => hasInpout_Skillor_BoomMelee;
        public void SetInput_Skillor_BoomMelee(bool value) => this.hasInpout_Skillor_BoomMelee = value;

        bool hasInput_Skillor_Infinity;
        public bool HasInput_Skillor_Infinity => hasInput_Skillor_Infinity;
        public void SetInput_Skillor_Infinity(bool value) => this.hasInput_Skillor_Infinity = value;

        bool hasInput_Skillor_Dash;
        public bool HasInput_Skillor_Dash => hasInput_Skillor_Dash;
        public void SetInput_Skillor_Dash(bool value) => this.hasInput_Skillor_Dash = value;

        bool hasInput_Basic_Pick;
        public bool HasInput_Basic_Pick => hasInput_Basic_Pick;
        public void SetInput_Basic_Pick(bool value) => this.hasInput_Basic_Pick = value;

        Vector2 chosePoint;
        public Vector2 ChoosePoint => chosePoint;
        public void SetInput_Basic_ChoosePoint(Vector2 value) => this.chosePoint = value;

        public InputComponent() { }

        public void Reset() {
            moveAxis = Vector2.zero;
            hasInput_Locomotion_Jump = false;
            hasInput_Skillor_Melee = false;
            hasInput_Skillor_SpecMelee = false;
            hasInpout_Skillor_BoomMelee = false;
            hasInput_Skillor_Infinity = false;
            hasInput_Skillor_Dash = false;
            hasInput_Basic_Pick = false;
            chosePoint = Vector2.zero;
        }

        public SkillorType GetSkillorType() {
            if (hasInput_Skillor_Melee) {
                return SkillorType.Melee;
            }
            if (hasInput_Skillor_SpecMelee) {
                return SkillorType.SpecMelee;
            }
            if (hasInpout_Skillor_BoomMelee) {
                return SkillorType.BoomMelee;
            }
            if (hasInput_Skillor_Infinity) {
                return SkillorType.Infinity;
            }
            if (hasInput_Skillor_Dash) {
                return SkillorType.Dash;
            }
            return SkillorType.None;
        }

    }

}