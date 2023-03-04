using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleBeHitStateModel {

        public Vector2 fromPos;

        public float knockbackForce;
        public int knockbackFrame;

        public int hitStunFrame;

        public int curFrame;

        public bool isEnter;

        public RoleBeHitStateModel() {}

    }

}