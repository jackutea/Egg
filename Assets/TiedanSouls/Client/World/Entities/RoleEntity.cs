using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleEntity : MonoBehaviour, IEntity {

        RoleAIStrategy aiStrategy;
        public RoleAIStrategy AIStrategy => aiStrategy;
        public void SetAIStrategy(RoleAIStrategy value) => this.aiStrategy = value;

        #region [Component]

        public EntityIDComponent IDCom { get; private set; }
        public InputComponent InputCom { get; private set; }
        public RoleAttributeComponent RoleAttributeCom { get; private set; }
        public MoveComponent MoveCom { get; private set; }
        public RoleFSMComponent FSMCom { get; private set; }
        public WeaponSlotComponent WeaponSlotCom { get; private set; }
        public SkillSlotComponent SkillSlotCom { get; private set; }
        public BuffSlotComponent BuffSlotCom { get; private set; }
        public HUDSlotComponent HudSlotCom { get; private set; }
        public FootComponent FootCom { get; private set; }
        public BodyComponent BodyCom { get; private set; }
        public RoleRendererComponent RendererModCom { get; private set; }

        #endregion

        #region [Root]

        public Transform LogicRoot { get; private set; }
        public Transform RendererRoot { get; private set; }

        public Vector3 LogicPos => LogicRoot.position;
        public float LogicAngleZ => LogicRoot.rotation.z;
        public Quaternion LogicRotation => LogicRoot.rotation;
        public void SetLogicPos(Vector2 pos) => LogicRoot.position = pos;

        public Vector2 RendererPos => RendererRoot.position;

        public Transform WeaponRoot { get; private set; }
        public Vector2 GetPos_WeaponRoot() => WeaponRoot.position;

        public Rigidbody2D RB_LogicRoot { get; private set; }
        public CapsuleCollider2D Coll_LogicRoot { get; private set; }

        #endregion

        #region [Locomotion]

        sbyte faceDirX;
        public sbyte FaceDirX => faceDirX;

        #endregion

        #region [出生点 & 是否为Boss]

        Vector2 bornPos;
        public Vector2 BornPos => bornPos;
        public void SetBornPos(Vector2 value) => this.bornPos = value;

        bool isBoss;
        public bool IsBoss => isBoss;
        public void SetIsBoss(bool value) => this.isBoss = value;

        #endregion

        public void Ctor() {
            faceDirX = 1;

            // - Root
            LogicRoot = transform.Find("logic_root");
            RendererRoot = transform.Find("renderer_root");
            RB_LogicRoot = LogicRoot.GetComponent<Rigidbody2D>();
            Coll_LogicRoot = LogicRoot.GetComponent<CapsuleCollider2D>();
            TDLog.Assert(RB_LogicRoot != null);
            TDLog.Assert(Coll_LogicRoot != null);

            // - Move
            MoveCom = new MoveComponent();
            MoveCom.Inject(RB_LogicRoot);

            // - ID
            IDCom = new EntityIDComponent();
            IDCom.SetEntityType(EntityType.Role);

            // - Input
            InputCom = new InputComponent();

            // - Weapon
            WeaponRoot = LogicRoot.Find("weapon_root");
            WeaponSlotCom = new WeaponSlotComponent();
            WeaponSlotCom.Inject(WeaponRoot);

            // - RoleAttribute
            RoleAttributeCom = new RoleAttributeComponent();

            // - FSM
            FSMCom = new RoleFSMComponent();

            // - Skill
            SkillSlotCom = new SkillSlotComponent();

            // - Buff
            BuffSlotCom = new BuffSlotComponent();

            // - HUD
            var hudRoot = RendererRoot.Find("hud_root");
            TDLog.Assert(hudRoot != null);
            HudSlotCom = new HUDSlotComponent();
            HudSlotCom.Inject(hudRoot);

            RendererModCom = new RoleRendererComponent();

            // - Foot
            FootCom = LogicRoot.Find("foot").GetComponent<FootComponent>();
            FootCom.Ctor();
            FootCom.footTriggerEnter = OnFootTriggerEnter;
            FootCom.footTriggerExit = OnFootTriggerExit;

            // - Body
            BodyCom = LogicRoot.Find("body").GetComponent<BodyComponent>();
            BodyCom.Ctor();
        }

        public void TearDown() {
            GameObject.Destroy(gameObject);
        }

        public void Reset() {
            // - Foot
            FootCom.Reset();
            // - Body
            BodyCom.Reset();
            // - Weapon
            WeaponSlotCom.Reset();
            // - RoleAttribute
            RoleAttributeCom.Reset();
            // - FSM
            FSMCom.Reset();
            // - Input
            InputCom.Reset();
            // - Movement
            MoveCom.Reset();
            // - HUD
            HudSlotCom.Reset();
            HudSlotCom.HpBarHUD.SetHpBar(RoleAttributeCom.HP, RoleAttributeCom.HPMax);
        }

        public void SetMod(GameObject mod) {
            RendererModCom.SetMod(mod);
        }

        public void SetLogicFaceTo(float dirX) {
            if (Mathf.Abs(dirX) < 0.01f) return;

            var rot = LogicRoot.localRotation;
            bool isRight = dirX > 0;
            if (isRight) rot.y = 0;
            else rot.y = 180;
            LogicRoot.localRotation = rot;
        }

        public void SetFromFieldTypeID(int fieldTypeID) {
            IDCom.SetFromFieldTypeID(fieldTypeID);

            var idArgs = IDCom.ToArgs();
            SkillSlotCom.SetFather(idArgs);
            BuffSlotCom.SetFather(idArgs);
            Coll_LogicRoot.GetComponent<ColliderModel>().SetFather(idArgs);
        }

        #region [角色物理事件]

        public Action<RoleEntity, Collider2D> OnStandInGround;
        public Action<RoleEntity, Collider2D> OnStandInPlatform;
        public Action<RoleEntity, Collider2D> OnStandInWater;

        public Action<RoleEntity, Collider2D> OnLeaveGround;
        public Action<RoleEntity, Collider2D> OnLeavePlatform;
        public Action<RoleEntity, Collider2D> OnLeaveWater;

        void OnFootTriggerEnter(Collider2D other) {
            if (other.gameObject.layer == LayerMask.NameToLayer(LayerCollection.GROUND)) {
                OnStandInGround.Invoke(this, other);
            } else if (other.gameObject.layer == LayerMask.NameToLayer(LayerCollection.PLATFORM)) {
                OnStandInPlatform.Invoke(this, other);
            }
        }

        void OnFootTriggerExit(Collider2D other) {
            if (other.gameObject.layer == LayerMask.NameToLayer(LayerCollection.GROUND)) {
                OnLeaveGround.Invoke(this, other);
            } else if (other.gameObject.layer == LayerMask.NameToLayer(LayerCollection.PLATFORM)) {
                OnLeavePlatform.Invoke(this, other);
            } else if (other.gameObject.layer == LayerMask.NameToLayer(LayerCollection.WATER)) {
                OnLeaveWater.Invoke(this, other);
            }
        }
        #endregion

    }

}