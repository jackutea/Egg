using System;

namespace TiedanSouls.Infra {

    public class InfraEventCenter {

        public event Action OnStartGameHandle;
        internal void InvokeStartGame() => OnStartGameHandle.Invoke();

    }

}