using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldProjectileDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain rootDomain;

        public WorldProjectileDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.rootDomain = worldDomain;
        }

        #region [生成]

        public bool TrySummonProjectile(Vector3 summonPos, Quaternion baseRot, in IDArgs summoner, in EntitySummonModel entitySummonModel, out ProjectileEntity projectile) {
            // 1. 创建弹道
            var typeID = entitySummonModel.typeID;
            var factory = worldContext.WorldFactory;
            if (!factory.TryCreateProjectile(typeID, summonPos, baseRot, out projectile)) {
                TDLog.Error($"创建实体 '弹道' 失败! - {typeID}");
                return false;
            }
            var projectileIDCom = projectile.IDCom;
            projectileIDCom.SetEntityID(worldContext.IDService.PickProjectileID());
            projectileIDCom.SetFather(summoner);

            // 2. 填充 弹道子弹模型数据 数组
            var bulletDomain = rootDomain.BulletDomain;
            var projectileBulletModelArray = projectile.ProjectileBulletModelArray;
            var len = projectileBulletModelArray?.Length;
            for (int i = 0; i < len; i++) {
                var projectileBulletModel = projectileBulletModelArray[i];
                var bulletTypeID = projectileBulletModel.bulletTypeID;
                if (!bulletDomain.TrySpawn(bulletTypeID, projectileIDCom.ToArgs(), out var bullet)) {
                    TDLog.Error($"创建实体弹道的 '子弹' 失败! - {bulletTypeID}");
                    return false;
                }

                // 子弹设置 位置 & 旋转
                var finalRot = baseRot * Quaternion.Euler(projectileBulletModel.localEulerAngles);
                bullet.SetLogicPos(finalRot * projectileBulletModel.localPos + summonPos);
                bullet.SyncRenderer();
                bullet.RotateMoveDir(finalRot);

                var bulletIDCom = bullet.IDCom;
                projectileBulletModel.bulletEntityID = bulletIDCom.EntityID;
                projectileBulletModelArray[i] = projectileBulletModel;

                // 为了方便调试，这里给子弹加上名字
                bullet.RootGO.name = $"子弹_{bulletIDCom}";
            }

            // 3. 激活弹道 TODO: 走配置，不一定立刻激活，可能需要等待一段时间，或者等待某个条件满足
            var fsm = projectile.FSMCom;
            fsm.Enter_Activated();

            // 4. 添加至仓库
            var repo = worldContext.ProjectileRepo;
            repo.Add(projectile);

            return true;
        }

        #endregion 

        #region [销毁]

        public void TearDownProjectile(ProjectileEntity projectile) {
            var repo = worldContext.ProjectileRepo;
            repo.TryRemove(projectile);
            repo.AddToPool(projectile);
        }

        public void TearDownFieldProjectiles(int fieldTypeID) {
            var repo = worldContext.BulletRepo;
            repo.TearDownToPool(fieldTypeID);
        }

        #endregion

    }

}