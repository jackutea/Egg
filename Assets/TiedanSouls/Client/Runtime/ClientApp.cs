using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Infra.Controller;

namespace TiedanSouls.Main.Entry {

    public class ClientApp : MonoBehaviour {

        InfraController infraController;

        bool isInit;

        void Awake() {

            InfraContext infraContext = new InfraContext();
            infraController = new InfraController();

            infraController.Inject(GetComponentInChildren<Canvas>(), infraContext);

            Action action = async () => {

                await infraController.Init();

                isInit = true;
            };
            action.Invoke();

        }

        void FixedUpdate() {

            if (!isInit) {
                return;
            }

        }

        void Update() {

            if (!isInit) {
                return;
            }

            float dt = Time.deltaTime;
            infraController.Tick(dt);

        }

        void LateUpdate() {

            if (!isInit) {
                return;
            }

            float dt = Time.deltaTime;
            infraController.LateTick(dt);

        }

    }

}