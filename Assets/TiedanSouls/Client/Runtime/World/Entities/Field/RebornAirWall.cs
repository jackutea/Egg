using UnityEngine;

namespace TiedanSouls.World.Entities {
    public class RebornAirWall : MonoBehaviour {
        // Start is called before the first frame update
        int damage=50;
        public int Damage => damage;
        Vector2 rebornPos = new Vector2(3, 3);
        public Vector2 RebornPos => rebornPos;

    }
}

