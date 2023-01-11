using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class SkillorModel {

        object owner;
        public RoleEntity Owner => owner as RoleEntity;

        int typeID;
        public int TypeID => typeID;

        SkillorType skillorType;
        public SkillorType SkillorType => skillorType;

        SkillorFrameElement[] frames;
        int frameIndex;

        public event Action<SkillorModel, Collider2D> OnTriggerEnterHandle;

        public SkillorModel(RoleEntity owner) {
            this.owner = owner;
        }

        public void FromTM(Template.SkillorTM tm) {
            typeID = tm.typeID;
            skillorType = tm.skillorType;
            if (tm.frames != null) {
                var frames = new SkillorFrameElement[tm.frames.Length];
                for (int i = 0; i < frames.Length; i += 1) {
                    frames[i] = new SkillorFrameElement(this);
                    frames[i].FromTM(tm.frames[i]);
                }
                this.frames = frames;
            }
        }

        public bool TryGetCurrentFrame(out SkillorFrameElement frame) {
            if (frames != null && frameIndex < frames.Length) {
                frame = frames[frameIndex];
                return true;
            }
            frame = null;
            return false;
        }

        public void ActiveNextFrame(Vector2 parentPos, float parentZAngle, sbyte faceXDir) {
            DeactiveCurrent();
            // activate next frame
            frameIndex += 1;
            if (frames != null && frameIndex < frames.Length) {
                frames[frameIndex].Active(parentPos, parentZAngle, faceXDir);
            }
        }

        void DeactiveCurrent() {
            // deactivate current frame
            var frames = this.frames;
            if (frames != null && frameIndex < frames.Length) {
                frames[frameIndex].Deactive();
            }
        }

        public void Reset() {
            DeactiveCurrent();
            frameIndex = 0;
        }

        public void OnEnterOther(Collider2D other) {
            OnTriggerEnterHandle.Invoke(this, other);
        }

    }

}