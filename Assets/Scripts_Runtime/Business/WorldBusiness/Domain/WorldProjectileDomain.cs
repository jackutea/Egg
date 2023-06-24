using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldProjectileDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldProjectileDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        #region [生成]

        public bool TrySpawnProjectile(Vector3 basePos, Quaternion baseRot, in EntityIDComponent father, in ProjectileCtorModel projectileCtorModel, out ProjectileEntity projectile) {
            // 1. 创建弹幕
            var typeID = projectileCtorModel.typeID;
            var factory = worldContext.Factory;
            if (!factory.TryCreateProjectile(typeID, out projectile)) {
                TDLog.Error($"创建实体 '弹幕' 失败! - {typeID}");
                return false;
            }
            var projectileIDCom = projectile.IDCom;
            projectileIDCom.SetEntityID(worldContext.IDService.PickProjectileID());
            projectileIDCom.SetFather(father);

            // 2. 子弹ID数组
            var rootDomain = worldContext.RootDomain;
            var bulletDomain = rootDomain.BulletDomain;
            var projectileBulletModelArray = projectile.ProjectileBulletModelArray;
            var len = projectileBulletModelArray != null ? projectileBulletModelArray.Length : 0;
            int[] bulletEntityIDArray = new int[len];
            for (int i = 0; i < len; i++) {
                var projectileBulletModel = projectileBulletModelArray[i];
                var bulletTypeID = projectileBulletModel.bulletTypeID;
                if (!bulletDomain.TryGetOrCreate(bulletTypeID, projectileIDCom, out var bullet)) {
                    TDLog.Error($"创建实体弹幕的 '子弹' 失败! - {bulletTypeID}");
                    return false;
                }

                // Pos & Rot
                var worldRot = baseRot * Quaternion.Euler(projectileBulletModel.localEulerAngles);
                var worldPos = worldRot * projectileBulletModel.localPos + basePos;
                bullet.SetLogicPos(worldPos);
                bullet.SetLogicRotation(worldRot);
                bullet.SetBaseRotation(worldRot);
                bullet.SyncRenderer();

                // ID
                var bulletIDCom = bullet.IDCom;
                bulletEntityIDArray[i] = bulletIDCom.EntityID;
                bullet.RootGO.name = $"子弹_{bulletIDCom}"; // 为了方便调试，这里给子弹加上名字
            }
            projectile.SetBulletIDArray(bulletEntityIDArray);

            // 3. 激活弹幕 TODO: 走配置，不一定立刻激活，可能需要等待一段时间，或者等待某个条件满足
            var fsmCom = projectile.FSMCom;
            fsmCom.Enter_Activated();

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