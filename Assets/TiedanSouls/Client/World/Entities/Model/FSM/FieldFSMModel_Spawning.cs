namespace TiedanSouls.Client.Entities {

    public class FieldFSMModel_Spawning {

        bool isEntering;
        public bool IsEntering => isEntering;
        public void SetIsEntering(bool value) => isEntering = value;

        bool isRespawning;
        public bool IsRespawning => isRespawning;
        public void SetIsRespawning(bool value) => isRespawning = value;

        bool isSpawningPaused;
        public bool IsSpawningPaused => isSpawningPaused;
        public void SetIsSpawningPaused(bool value) => isSpawningPaused = value;

        public int curFrame;

        public int curSpawnedCount;
        public int aliveEnemyCount;
        public int totalSpawnCount;

        public FieldFSMModel_Spawning() { }

        public void Reset() {
            isEntering = false;
            isSpawningPaused = false;
            curFrame = 0;
        }

    }

}