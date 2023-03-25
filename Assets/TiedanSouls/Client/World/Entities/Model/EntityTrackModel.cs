using System;
using System.Collections.Generic;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public struct EntityTrackModel {

        public float trackSpeed;
        public RelativeTargetGroupType trackTargetGroupType;
        public EntityTrackSelectorModel entityTrackSelectorModel;

        public IDArgs trackTarget;

    }

}




