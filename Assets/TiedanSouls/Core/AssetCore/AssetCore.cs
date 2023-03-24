using System.Threading.Tasks;

namespace TiedanSouls.Asset {

    public class AssetCore {

        public ContainerModAsset ContainerModAsset { get; private set; }
        public FieldModAsset FieldModAsset { get; private set; }
        public SpriteAsset SpriteAsset { get; private set; }
        public WeaponModAsset WeaponModAsset { get; private set; }
        public RoleModAsset RoleModAsset { get; private set; }
        public ItemModAsset ItemModAsset { get; private set; }
        public HUDAsset HUDAsset { get; private set; }
        public VFXAsset VFXAsset { get; private set; }

        public AssetCore() {
            ContainerModAsset = new ContainerModAsset();
            FieldModAsset = new FieldModAsset();
            SpriteAsset = new SpriteAsset();
            WeaponModAsset = new WeaponModAsset();
            RoleModAsset = new RoleModAsset();
            ItemModAsset = new ItemModAsset();
            HUDAsset = new HUDAsset();
            VFXAsset = new VFXAsset();
        }

        public async Task Init() {
            await HUDAsset.LoadAll();
            await ContainerModAsset.LoadAll();
            await FieldModAsset.LoadAll();
            await SpriteAsset.LoadAll();
            await WeaponModAsset.LoadAll();
            await RoleModAsset.LoadAll();
            await ItemModAsset.LoadAll();
            await VFXAsset.LoadAll();
        }

    }
}