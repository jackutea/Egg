using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Facades;
using TiedanSouls.World.Entities;

namespace TiedanSouls.World.Domain {

    public class WorldFieldDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldFieldDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public bool TryGetOrSpawnField(int typeID, out FieldEntity field) {
            var fieldRepo = worldContext.FieldRepo;
            if (!fieldRepo.TryGet(typeID, out field)) {
                var factory = worldContext.WorldFactory;
                if (!factory.TrySpawnFieldEntity(typeID, out field)) {
                    return false;
                }

                fieldRepo.Add(field);
            }

            field.Show();
            return true;
        }

        public void HideField(int typeID) {
            var fieldRepo = worldContext.FieldRepo;
            if (!fieldRepo.TryGet(typeID, out FieldEntity field)) {
                return;
            }

            field.Hide();
        }

        // ====== FSM Domain ======
        public void TickFSM(int typeID, float dt) {
            var fieldRepo = worldContext.FieldRepo;
            if (!fieldRepo.TryGet(typeID, out var field)) {
                TDLog.Log($"场景不存在! FieldTypeID: {typeID}");
            }

            var fsm = field.FSMComponent;
            var state = fsm.State;
            if (state == FieldFSMState.Ready) {
                ApplyFSMState_Ready(fsm, dt);
            } else if (state == FieldFSMState.Spawning) {
                ApplyFSMState_Spawning(fsm, dt);
            } else if (state == FieldFSMState.Finished) {
                ApplyFSMState_Finished(fsm, dt);
            }
        }

        void ApplyFSMState_Ready(FieldFSMComponent fsm, float dt) {
        }

        void ApplyFSMState_Spawning(FieldFSMComponent fsm, float dt) {

        }

        void ApplyFSMState_Finished(FieldFSMComponent fsm, float dt) {

        }

    }
}