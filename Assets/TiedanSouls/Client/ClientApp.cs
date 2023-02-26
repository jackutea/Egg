using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Infra.Controller;
using TiedanSouls.Main.Controller;
using TiedanSouls.World.Controller;

namespace TiedanSouls.Main.Entry {

    public class ClientApp : MonoBehaviour {

        InfraController infraController;

        MainController mainController;

        WorldController worldController;

        bool isClientReady;

        void Awake() {

            GameObject.DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 120;

            // ==== Instantiate ====
            InfraContext infraContext = new InfraContext();
            infraController = new InfraController();

            mainController = new MainController();

            worldController = new WorldController();

            // ==== Inject ====
            infraController.Inject(GetComponentInChildren<Canvas>(), infraContext);
            mainController.Inject(infraContext);
            worldController.Inject(infraContext);

            // ==== Init ====
            Action action = async () => {

                await infraController.Init();
                worldController.Init();
                mainController.Init();

                isClientReady = true;

            };
            action.Invoke();
        }

        void Update() {
            if (!isClientReady) {
                return;
            }

            var dt = Time.deltaTime;
            infraController.Tick(dt);
            worldController.Tick(dt);
        }

        void LateUpdate() {
            if (!isClientReady) {
                return;
            }

            float dt = Time.deltaTime;
            infraController.LateTick(dt);
        }

    }

}