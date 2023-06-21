using UnityEngine;

namespace TiedanSouls.Template {

    [CreateAssetMenu(fileName = "so_config_game", menuName = "TiedanSouls/Template/GameConfigSO", order = 0)]
    public class GameConfigSO : ScriptableObject {

        public GameConfigTM tm;

    }

}