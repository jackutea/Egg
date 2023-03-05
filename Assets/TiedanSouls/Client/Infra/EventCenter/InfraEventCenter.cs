using System;

namespace TiedanSouls.Infra {

    public class InfraEventCenter {

        event Action OnStartGameAct;
        public void Listen_OnStartGameAct(Action action) => OnStartGameAct += action;
        public void Invoke_OnStartGameAct() => OnStartGameAct.Invoke();

    }

}