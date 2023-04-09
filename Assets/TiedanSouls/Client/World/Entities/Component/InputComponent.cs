using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class InputComponent {

        Vector2 moveAxis;
        public Vector2 MoveAxis => moveAxis;
        public void SetMoveAxis(Vector2 value) => this.moveAxis = value;

        bool hasMoveOpt;
        public bool HasMoveOpt => hasMoveOpt;
        public void SetHasMoveOpt(bool value) => this.hasMoveOpt = value;

        bool inputJump;
        public bool InputJump => inputJump;
        public void SetInputJump(bool value) => this.inputJump = value;

        bool inputNormalSkill;
        public bool InputSkillMelee => inputNormalSkill;
        public void SetInputSkillMelee(bool value) => this.inputNormalSkill = value;

        bool inputSpecialSkill;
        public bool InputSpecialSkill => inputSpecialSkill;
        public void SetInputSpecialSkill(bool value) => this.inputSpecialSkill = value;

        bool inputUltimateSkill;
        public bool InputUltimateSkill => inputUltimateSkill;
        public void SetInputUltimateSkill(bool value) => this.inputUltimateSkill = value;

        bool inputDashSkill;
        public bool InputDashSkill => inputDashSkill;
        public void SetInputDashSkill(bool value) => this.inputDashSkill = value;

        bool inputPick;
        public bool InputPick => inputPick;
        public void SetInputPick(bool value) => this.inputPick = value;

        Vector2 chosenPoint;
        public Vector2 ChosenPoint => chosenPoint;
        public void SetChosenPoint(Vector2 value) => this.chosenPoint = value;

        public InputComponent() { }

        public void Reset() {
            moveAxis = Vector2.zero;
            hasMoveOpt = false;
            inputJump = false;
            inputNormalSkill = false;
            inputSpecialSkill = false;
            inputUltimateSkill = false;
            inputDashSkill = false;
            inputPick = false;
            chosenPoint = Vector2.zero;
        }

        public SkillType GetSkillType() {
            if (inputNormalSkill) {
                return SkillType.Normal;
            }
            if (inputSpecialSkill) {
                return SkillType.Special;
            }
            if (inputUltimateSkill) {
                return SkillType.Ultimate;
            }
            if (inputDashSkill) {
                return SkillType.Dash;
            }
            return SkillType.None;
        }

    }

}