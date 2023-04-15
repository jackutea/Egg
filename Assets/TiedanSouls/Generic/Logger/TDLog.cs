using UnityEngine;

namespace TiedanSouls.Generic {

    public static class TDLog {

        public static void Log(object message) {
            Debug.Log(message);
        }

        public static void Warning(object message) {
            Debug.LogWarning(message);
        }

        public static void Error(object message) {
            Debug.LogError(message);
        }

        public static void Assert(bool condition, string msg) {
            Debug.Assert(condition, msg);
        }

    }
}