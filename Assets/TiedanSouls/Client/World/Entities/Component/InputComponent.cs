using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class InputComponent {

        Vector2 moveAxis;
        public Vector2 MoveAxis => moveAxis;
        public void SetInput_Locomotion_Move(Vector2 value) => this.moveAxis = value;

        bool hasInput_Locomotion_Jump;
        public bool HasInput_Locomotion_JumpDown => hasInput_Locomotion_Jump;
        public void SetInput_Locomotion_Jump(bool value) => this.hasInput_Locomotion_Jump = value;

        bool hasInput_Skill_Melee;
        public bool HasInput_Skill_Melee => hasInput_Skill_Melee;
        public void SetInput_Skill__Melee(bool value) => this.hasInput_Skill_Melee = value;

        bool hasInput_Skill_SpecMelee;
        public bool HasInput_Skill_SpecMelee => hasInput_Skill_SpecMelee;
        public void SetInput_Skill_SpecMelee(bool value) => this.hasInput_Skill_SpecMelee = value;

        bool hasInpout_Skill_BoomMelee;
        public bool HasInpout_BoomMelee => hasInpout_Skill_BoomMelee;
        public void SetInput_Skill_BoomMelee(bool value) => this.hasInpout_Skill_BoomMelee = value;

        bool hasInput_Skill_Infinity;
        public bool HasInput_Skill_Infinity => hasInput_Skill_Infinity;
        public void SetInput_Skill_Infinity(bool value) => this.hasInput_Skill_Infinity = value;

        bool hasInput_Skill_Dash;
        public bool HasInput_Skill_Dash => hasInput_Skill_Dash;
        public void SetInput_Skill_Dash(bool value) => this.hasInput_Skill_Dash = value;

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
            hasInput_Skill_Melee = false;
            hasInput_Skill_SpecMelee = false;
            hasInpout_Skill_BoomMelee = false;
            hasInput_Skill_Infinity = false;
            hasInput_Skill_Dash = false;
            hasInput_Basic_Pick = false;
            chosePoint = Vector2.zero;
        }

        public SkillType GetSkillType() {
            if (hasInput_Skill_Melee) {
                return SkillType.Melee;
            }
            if (hasInput_Skill_SpecMelee) {
                return SkillType.SpecMelee;
            }
            if (hasInpout_Skill_BoomMelee) {
                return SkillType.BoomMelee;
            }
            if (hasInput_Skill_Infinity) {
                return SkillType.Infinity;
            }
            if (hasInput_Skill_Dash) {
                return SkillType.Dash;
            }
            return SkillType.None;
        }

    }

}