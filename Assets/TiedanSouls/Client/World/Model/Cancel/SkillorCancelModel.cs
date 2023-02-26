namespace TiedanSouls.World.Entities {

    public struct SkillorCancelModel {

        public int skillorTypeID;
        public int startFrame;
        public bool isCombo;

        public void FromTM(Template.SkillorCancelTM tm) {
            this.skillorTypeID = tm.skillorTypeID;
            this.startFrame = tm.startFrame;
            this.isCombo = tm.isCombo;
        }

    }

}