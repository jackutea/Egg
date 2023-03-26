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

        bool pressJump;
        public bool PressJump => pressJump;
        public void SetPressJump(bool value) => this.pressJump = value;

        bool pressSkillMelee;
        public bool PressSkillMelee => pressSkillMelee;
        public void SetPressSkillMelee(bool value) => this.pressSkillMelee = value;

        bool pressSkillSpecMelee;
        public bool PressSkillSpecMelee => pressSkillSpecMelee;
        public void SetPressSkillSpecMelee(bool value) => this.pressSkillSpecMelee = value;

        bool pressSkillBoomMelee;
        public bool PressSkillBoomMelee => pressSkillBoomMelee;
        public void SetPressSkillBoomMelee(bool value) => this.pressSkillBoomMelee = value;

        bool pressSkillInfinity;
        public bool PressSkillInfinity => pressSkillInfinity;
        public void SetPressSkillInfinity(bool value) => this.pressSkillInfinity = value;

        bool pressSkillDash;
        public bool PressSkillDash => pressSkillDash;
        public void SetPressSkillDash(bool value) => this.pressSkillDash = value;

        bool pressPick;
        public bool PressPick => pressPick;
        public void SetPressPick(bool value) => this.pressPick = value;

        Vector2 chosenPoint;
        public Vector2 ChosenPoint => chosenPoint;
        public void SetChosenPoint(Vector2 value) => this.chosenPoint = value;

        public InputComponent() { }

        public void Reset() {
            moveAxis = Vector2.zero;
            hasMoveOpt = false;
            pressJump = false;
            pressSkillMelee = false;
            pressSkillSpecMelee = false;
            pressSkillBoomMelee = false;
            pressSkillInfinity = false;
            pressSkillDash = false;
            pressPick = false;
            chosenPoint = Vector2.zero;
        }

        public SkillType GetSkillType() {
            if (pressSkillMelee) {
                return SkillType.Melee;
            }
            if (pressSkillSpecMelee) {
                return SkillType.SpecMelee;
            }
            if (pressSkillBoomMelee) {
                return SkillType.BoomMelee;
            }
            if (pressSkillInfinity) {
                return SkillType.Infinity;
            }
            if (pressSkillDash) {
                return SkillType.Dash;
            }
            return SkillType.None;
        }

    }

}