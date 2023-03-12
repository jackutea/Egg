using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class SkillModel {

        // - Identity
        // TODO: REFACTOR OWNER
        object owner;
        public RoleEntity Owner => owner as RoleEntity;
        public void SetOwner(object v) => owner = v;

        public EntityType EntityType => EntityType.Skill;

        int id;
        public int ID => id;
        public void SetID(int v) => id = v;

        int typeID;
        public int TypeID => typeID;

        SkillType skillType;
        public SkillType SkillType => skillType;

        // - Combo
        int originalSkillTypeID;
        public int OriginalSkillTypeID => originalSkillTypeID;

        // - Frames
        SkillFrameElement[] allFrames;
        int curFrameIndex;
        public int CurFrameIndex => curFrameIndex;

        // - Renderer
        public string weaponAnimName;

        // - Event
        public event Action<SkillModel, Collider2D> OnTriggerEnterHandle;

        public SkillModel() {
        }

        public void FromTM(Template.SkillTM tm) {

            // - Identity
            typeID = tm.typeID;
            skillType = tm.skillType;

            // - Combo
            originalSkillTypeID = tm.originalSkillTypeID;

            // - Frames
            if (tm.frames != null) {
                var frames = new SkillFrameElement[tm.frames.Length];
                for (int i = 0; i < frames.Length; i += 1) {
                    frames[i] = new SkillFrameElement(this);
                    frames[i].FromTM(tm.frames[i]);
                }
                this.allFrames = frames;
            }

            // - Renderer
            weaponAnimName = tm.weaponAnimName;

        }

        public bool TryGetCurrentFrame(out SkillFrameElement frame) {
            if (allFrames != null && curFrameIndex < allFrames.Length) {
                frame = allFrames[curFrameIndex];
                return true;
            }
            frame = null;
            return false;
        }

        public void ActiveNextFrame(Vector2 parentPos, float parentZAngle, sbyte faceXDir) {
            ResetFrame(curFrameIndex);
            curFrameIndex++;
            if (allFrames != null && curFrameIndex < allFrames.Length) {
                allFrames[curFrameIndex].Active(parentPos, parentZAngle, faceXDir);
            }
        }

        void ResetAllFrames() {
            var len = allFrames.Length;
            for (int i = 0; i < len; i += 1) {
                allFrames[i].DeactiveFrameBoxes();
            }
        }

        void ResetFrame(int frameIndex) {
            if (allFrames != null && frameIndex < allFrames.Length) {
                allFrames[frameIndex].DeactiveFrameBoxes();
            }
        }

        public void Reset() {
            ResetAllFrames();
            curFrameIndex = 0;
        }

        public void OnEnterOther(Collider2D other) {
            OnTriggerEnterHandle.Invoke(this, other);
        }

    }

}