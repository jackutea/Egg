using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class SkillorModel {

        int typeID;
        public int TypeID => typeID;

        SkillorFrameElement[] frames;
        int frameIndex;

        public SkillorModel() { }

        public void FromTM(Template.SkillorTM tm) {
            typeID = tm.typeID;
            if (tm.frames != null) {
                var frames = new SkillorFrameElement[tm.frames.Length];
                for (int i = 0; i < frames.Length; i += 1) {
                    frames[i] = new SkillorFrameElement();
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

        public void ActiveNextFrame(Vector2 parentPos, float parentZAngle) {
            // deactivate current frame
            var frames = this.frames;
            if (frames != null && frameIndex < frames.Length) {
                frames[frameIndex].Deactive();
            }

            // activate next frame
            frameIndex += 1;
            if (frames != null && frameIndex < frames.Length) {
                frames[frameIndex].Active(parentPos, parentZAngle);
            }
        }

    }

}