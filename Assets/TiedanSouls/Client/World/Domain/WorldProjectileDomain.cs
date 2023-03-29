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
            if (!factory.TryCreateProjectile(typeID, out projectile)) {
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
                var bulletFather = projectileIDCom.ToArgs();
                if (!bulletDomain.TryGetOrCreate(bulletTypeID, bulletFather, out var bullet)) {
                    TDLog.Error($"创建实体弹道的 '子弹' 失败! - {bulletTypeID}");
                    return false;
                }

                // Pos & Rot
                var worldRot = baseRot * Quaternion.Euler(projectileBulletModel.localEulerAngles);
                var worldPos = worldRot * projectileBulletModel.localPos + summonPos;
                bullet.SetLogicPos(worldPos);
                bullet.SetLogicRotation(worldRot);
                bullet.SetBaseRotation(worldRot);
                bullet.SyncRenderer();

                // ID
                var bulletIDCom = bullet.IDCom;
                projectileBulletModel.bulletEntityID = bulletIDCom.EntityID;
                projectileBulletModelArray[i] = projectileBulletModel;

                bullet.SetExtraPenetrateCount(projectileBulletModel.extraPenetrateCount);   // 子弹的额外穿透次数

                bullet.RootGO.name = $"子弹_{bulletIDCom}"; // 为了方便调试，这里给子弹加上名字
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

        public void RecycleProjectiles(int fieldTypeID) {
            var repo = worldContext.BulletRepo;
            repo.RecycleToPool(fieldTypeID);
        }

        #endregion

    }

}