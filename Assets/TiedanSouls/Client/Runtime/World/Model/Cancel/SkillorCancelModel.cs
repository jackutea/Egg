namespace TiedanSouls.World.Entities {

    public struct SkillorCancelModel {

        public int nextSkillorTypeID;
        public int nextStartFrame;
        public bool isCombo;

        public void FromTM(Template.SkillorCancelTM tm) {
            this.nextSkillorTypeID = tm.nextSkillorTypeID;
            this.nextStartFrame = tm.nextStartFrame;
            this.isCombo = tm.isCombo;
        }

    }

}