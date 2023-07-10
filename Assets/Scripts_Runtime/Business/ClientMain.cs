using System;
using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Infra.Controller;
using TiedanSouls.Main.Controller;
using TiedanSouls.Client.Controller;
using TiedanSouls.Generic;

namespace TiedanSouls.Main.Entry {

    public class ClientMain : MonoBehaviour {

        InfraController infraController;

        MainController mainController;

        WorldController worldController;

        bool isClientReady;

        void Awake() {

            GameObject.DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 120;
            Physics2D.simulationMode = SimulationMode2D.Script;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(LayerCollection.ROLE), LayerMask.NameToLayer(LayerCollection.ROLE));

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

        void OnDestory() {
            TearDown();
        }

        void OnApplicationQuit() {
            TearDown();
        }

        void TearDown() {
            if (isClientReady) {
                return;
            }

            isClientReady = true;

            worldController.TearDown();
        }

    }

}