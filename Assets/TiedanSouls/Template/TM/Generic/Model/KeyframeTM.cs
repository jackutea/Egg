using System;
using UnityEngine;

namespace TiedanSouls.Template {

    [Serializable]
    public struct KeyframeTM {

        public float time;
        public float value;
        public float inTangent;
        public float outTangent;
        public float inWeight;
        public float outWeight;

        public KeyframeTM(in Keyframe keyframe) {
            time = keyframe.time;
            value = keyframe.value;
            inTangent = keyframe.inTangent;
            outTangent = keyframe.outTangent;
            inWeight = keyframe.inWeight;
            outWeight = keyframe.outWeight;
        }

    }

}