using System.Threading.Tasks;

namespace TiedanSouls.Asset {

    public class AssetCore {

        public ContainerModAssets ContainerModAssets { get; private set; }
        public FieldModAssets FieldModAssets { get; private set; }
        public SpriteAssets SpriteAssets { get; private set; }
        public WeaponModAssets WeaponModAssets { get; private set; }
        public RoleModAssets RoleModAssets { get; private set; }
        public ItemModAsset ItemModAsset { get; private set; }
        public HUDAssets HUDAssets { get; private set; }
        public VFXAssets VFXAssets { get; private set; }

        public AssetCore() {
            ContainerModAssets = new ContainerModAssets();
            FieldModAssets = new FieldModAssets();
            SpriteAssets = new SpriteAssets();
            WeaponModAssets = new WeaponModAssets();
            RoleModAssets = new RoleModAssets();
            ItemModAsset = new ItemModAsset();
            HUDAssets = new HUDAssets();
            VFXAssets = new VFXAssets();
        }

        public async Task Init() {
            await HUDAssets.LoadAll();
            await ContainerModAssets.LoadAll();
            await FieldModAssets.LoadAll();
            await SpriteAssets.LoadAll();
            await WeaponModAssets.LoadAll();
            await RoleModAssets.LoadAll();
            await ItemModAsset.LoadAll();
            await VFXAssets.LoadAll();
        }

    }
}