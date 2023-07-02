using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class InputComponent {

        Vector2 moveAxis;
        public Vector2 MoveAxis => moveAxis;
        public void SetMoveAxis(Vector2 value) => this.moveAxis = value;

        bool inputJump;
        public bool InputJump => inputJump;
        public void SetInputJump(bool value) => this.inputJump = value;

        bool inputMelee;
        public bool InputSkillMelee => inputMelee;
        public void SetInputSkillMelee(bool value) => this.inputMelee = value;

        bool inputSkill1;
        public bool InputSpecialSkill => inputSkill1;
        public void SetInputSpecialSkill(bool value) => this.inputSkill1 = value;

        bool inputSkill2;
        public bool InputUltimateSkill => inputSkill2;
        public void SetInputUltimateSkill(bool value) => this.inputSkill2 = value;

        bool inputDash;
        public bool InputDashSkill => inputDash;
        public void SetInputDashSkill(bool value) => this.inputDash = value;

        bool inputPick;
        public bool InputPick => inputPick;
        public void SetInputPick(bool value) => this.inputPick = value;

        Vector2 chosenPoint;
        public Vector2 ChosenPoint => chosenPoint;
        public void SetChosenPoint(Vector2 value) => this.chosenPoint = value;

        public InputComponent() { }

        public void Reset() {
            moveAxis = Vector2.zero;
            inputJump = false;
            inputMelee = false;
            inputSkill1 = false;
            inputSkill2 = false;
            inputDash = false;
            inputPick = false;
            chosenPoint = Vector2.zero;
        }

        public SkillCastKey GetSkillCastKey() {
            if (inputMelee) {
                return SkillCastKey.Melee;
            }
            if (inputSkill1) {
                return SkillCastKey.Skill1;
            }
            if (inputSkill2) {
                return SkillCastKey.Skill2;
            }
            if (inputDash) {
                return SkillCastKey.Dash;
            }
            return SkillCastKey.None;
        }

    }

}