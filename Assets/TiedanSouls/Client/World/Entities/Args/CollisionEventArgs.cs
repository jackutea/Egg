namespace TiedanSouls.Client.Entities {

    public struct CollisionEventArgs {

        IDArgs a;
        public IDArgs A => a;

        IDArgs b;
        public IDArgs B => b;

        public CollisionEventArgs(in IDArgs a, in IDArgs b) {
            this.a = a;
            this.b = b;
        }

        public override string ToString() {
            return $"CollisionEventArgs: A方-{a}  B方{b}";
        }

    }

}