using UnityEngine;
using GameArki.FPEasing;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;

namespace TiedanSouls.World.Domain {

    public class WorldPhysicsDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldDomain worldDomain;

        public WorldPhysicsDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldDomain = worldDomain;
        }

        public void Tick(float dt) {

            var stateEntity = worldContext.StateEntity;
            short exeTimes = 0;
            float restTime = stateEntity.phxRestTime;

            restTime += dt;
            while (restTime >= Time.fixedDeltaTime) {
                restTime -= Time.fixedDeltaTime;
                Physics2D.Simulate(Time.fixedDeltaTime);
                exeTimes += 1;
            }

            if (exeTimes == 0) {
                Physics2D.Simulate(restTime);
                restTime = 0;
            }

            stateEntity.phxRestTime = restTime;

        }

    }

}