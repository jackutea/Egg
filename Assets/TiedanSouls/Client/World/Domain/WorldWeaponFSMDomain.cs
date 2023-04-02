using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldWeaponFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain rootDomain;

        public WorldWeaponFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.rootDomain = worldDomain;
        }

        void TickFSM(WeaponEntity weapon, float dt) {
            var fsm = weapon.FSMCom;
            if (fsm.State == WeaponFSMState.None) return;

            var state = fsm.State;
            if (state == WeaponFSMState.Activated) Tick_Buff(weapon, fsm, dt);
        }

        void Tick_Buff(WeaponEntity weapon, WeaponFSMComponent fsm, float dt) {
            var buffDomain = rootDomain.BuffDomain;

            var buffSlotCom = weapon.BuffSlotCom;
            var removeList = buffSlotCom.ForeachAndGetRemoveList((buff) => {
                buff.curFrame++;
                buffDomain.TryTriggerEffector(buff);
                buffDomain.TryEffectWeaponAttribute(weapon.AttributeCom, buff);
            });

            removeList.ForEach((buff) => {
                buffDomain.RevokeBuffFromWeaponttribute(buff, weapon.AttributeCom);
            });
        }

    }

}