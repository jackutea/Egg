using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleEntity : MonoBehaviour, IEntity {

        RoleAIStrategy aiStrategy;
        public RoleAIStrategy AIStrategy => aiStrategy;
        public void SetAIStrategy(RoleAIStrategy value) => this.aiStrategy = value;

        #region [Component]

        IDComponent idCom;
        public IDComponent IDCom => idCom;

        InputComponent inputCom;
        public InputComponent InputCom => inputCom;

        AttributeComponent attributeCom;
        public AttributeComponent AttributeCom => attributeCom;

        SkillSlotComponent skillSlotCom;
        public SkillSlotComponent SkillSlotCom => skillSlotCom;

        MoveComponent moveCom;
        public MoveComponent MoveCom => moveCom;

        WeaponSlotComponent weaponSlotCom;
        public WeaponSlotComponent WeaponSlotCom => weaponSlotCom;

        RoleFSMComponent fsmCom;
        public RoleFSMComponent FSMCom => fsmCom;

        RoleRendererModComponent rendererModCom;
        public RoleRendererModComponent RendererModCom => rendererModCom;

        HUDSlotComponent hudSlotCom;
        public HUDSlotComponent HudSlotCom => hudSlotCom;

        FootComponent footCom;
        BodyComponent bodyCom;

        #endregion

        #region [Root]

        Transform logicRoot;
        public Transform LogicRoot => logicRoot;

        public Vector3 LogicPos => logicRoot.position;
        public float LogicAngleZ => logicRoot.rotation.z;
        public Quaternion LogicRotation => logicRoot.rotation;
        public void SetLogicPos(Vector2 pos) => logicRoot.position = pos;

        Transform rendererRoot;
        public Transform RendererRoot => rendererRoot;
        public Vector2 GetPos_RendererRoot() => rendererRoot.position;

        Transform weaponRoot;
        public Transform WeaponRoot => weaponRoot;
        public Vector2 GetPos_WeaponRoot() => weaponRoot.position;

        Rigidbody2D rb_logicRoot;
        CapsuleCollider2D coll_logicRoot;

        #endregion

        #region [Event]

        public event Action<RoleEntity, Collider2D> FootTriggerEnterAction;
        public event Action<RoleEntity, Collider2D> FootTriggerExit;
        public event Action<RoleEntity, Collider2D> BodyTriggerExitAction;

        #endregion

        #region [Locomotion]

        bool isJumping;

        bool isGrounded;
        public bool IsGrounded => isGrounded;

        bool isStandCrossPlatform;
        public bool IsStandCrossPlatform => isStandCrossPlatform;

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
            logicRoot = transform.Find("logic_root");
            rendererRoot = transform.Find("renderer_root");

            rb_logicRoot = logicRoot.GetComponent<Rigidbody2D>();
            TDLog.Assert(rb_logicRoot != null);
            moveCom = new MoveComponent();
            moveCom.Inject(rb_logicRoot);

            coll_logicRoot = logicRoot.GetComponent<CapsuleCollider2D>();
            TDLog.Assert(coll_logicRoot != null);

            // - Foot
            footCom = logicRoot.Find("foot").GetComponent<FootComponent>();
            footCom.Ctor();

            // - Body
            bodyCom = logicRoot.Find("body").GetComponent<BodyComponent>();
            bodyCom.Ctor();

            // - Weapon
            weaponRoot = logicRoot.Find("weapon_root");
            weaponSlotCom = new WeaponSlotComponent();
            weaponSlotCom.Inject(weaponRoot);

            // - HUD
            var hudRoot = rendererRoot.Find("hud_root");
            TDLog.Assert(hudRoot != null);

            hudSlotCom = new HUDSlotComponent();
            hudSlotCom.Inject(hudRoot);

            // - Attribute
            attributeCom = new AttributeComponent();

            // - Component
            idCom = new IDComponent();
            idCom.SetEntityType(EntityType.Role);
            rendererModCom = new RoleRendererModComponent();
            fsmCom = new RoleFSMComponent();
            inputCom = new InputComponent();
            skillSlotCom = new SkillSlotComponent();

            // - Locomotion
            isJumping = false;
            isGrounded = false;

            // - Bind Event
            footCom.FootTriggerEnter += OnFootTriggerEnter;
            footCom.FootTriggerExit += OnFootCollisionExit;
        }

        public void TearDown() {
            footCom.FootTriggerEnter -= OnFootTriggerEnter;
            footCom.FootTriggerExit -= OnFootCollisionExit;
            GameObject.Destroy(gameObject);
        }

        public void Reset() {
            // - Foot
            footCom.Reset();

            // - Body
            bodyCom.Reset();

            // - Weapon
            weaponSlotCom.Reset();

            // - Attribute
            attributeCom.Reset();

            // - FSM
            fsmCom.Reset();

            // - Input
            inputCom.Reset();

            // - Movement
            moveCom.Reset();

            // - HUD
            hudSlotCom.Reset();
            hudSlotCom.HpBarHUD.SetHpBar(attributeCom.HP, attributeCom.HPMax);
        }

        public void Show() {
            logicRoot.gameObject.SetActive(true);
            rendererRoot.gameObject.SetActive(true);
            TDLog.Log($"显示角色: {idCom.EntityName} ");
        }

        public void SetMod(GameObject mod) {
            rendererModCom.SetMod(mod);
        }

        public void Hide() {
            logicRoot.gameObject.SetActive(false);
            rendererRoot.gameObject.SetActive(false);
            TDLog.Log($"隐藏角色: {idCom.EntityName} ");
        }

        public void ActivateCollider() {
            coll_logicRoot.enabled = true;
        }

        public void DeactivateCollider() {
            coll_logicRoot.enabled = false;
        }

        public void SetLogicFaceTo(float dirX) {
            if (Mathf.Abs(dirX) < 0.01f) return;

            var rot = logicRoot.localRotation;
            bool isRight = dirX > 0;
            if (isRight) rot.y = 0;
            else rot.y = 180;
            logicRoot.localRotation = rot;
        }

        public void SetFromFieldTypeID(int fieldTypeID) {
            idCom.SetFromFieldTypeID(fieldTypeID);
            skillSlotCom.Foreach_Origin((skill) => {
                skill.SetFromFieldTypeID(fieldTypeID);
            });
            skillSlotCom.Foreach_Combo((skill) => {
                skill.SetFromFieldTypeID(fieldTypeID);
            });
            var colliderModel = coll_logicRoot.GetComponent<ColliderModel>();
            colliderModel.SetFather(idCom.ToArgs());
        }

        #region [Locomotion]

        public void MoveByInput() {
            if (!inputCom.HasMoveOpt) return;
            Vector2 moveAxis = inputCom.MoveAxis;
            moveCom.Move_Horizontal(moveAxis.x, attributeCom.MoveSpeed);
        }

        public void Dash(Vector2 dir, Vector2 force) {
            moveCom.Dash(dir, force);
        }

        public void JumpByInput() {
            bool isJumpPress = inputCom.PressJump;
            if (!isJumpPress) return;
            if (isJumping) return;
            if (!isGrounded) return;

            this.isJumping = true;
            this.isGrounded = false;

            var rb = moveCom.RB;
            var velo = rb.velocity;
            var jumpSpeed = attributeCom.JumpSpeed;
            velo.y = jumpSpeed;
            moveCom.SetVelocity(velo);
        }

        public void TryCrossDown() {
            if (inputCom.MoveAxis.y < 0 && isStandCrossPlatform) {
                LeaveCrossPlatform();
            }
        }

        public void Falling(float dt) {
            if (isGrounded) return;

            var fallingAcceleration = attributeCom.FallingAcceleration;
            var fallingSpeedMax = attributeCom.FallingSpeedMax;

            var rb = moveCom.RB;
            var velo = rb.velocity;
            var offset = fallingAcceleration * dt;

            velo.y -= offset;
            if (velo.y < -fallingSpeedMax) {
                velo.y = -fallingSpeedMax;
            }
            moveCom.SetVelocity(velo);
        }

        public void EnterGround() {
            coll_logicRoot.isTrigger = false;
            isGrounded = true;
            isJumping = false;

            var rb = moveCom.RB;
            var velo = rb.velocity;
            velo.y = 0;
            moveCom.SetVelocity(velo);
        }

        public void LeaveGround() {
            isGrounded = false;
            isStandCrossPlatform = false;
        }

        public void EnterCrossPlatform() {
            isStandCrossPlatform = true;
        }

        public void LeaveCrossPlatform() {
            LeaveGround();
            coll_logicRoot.isTrigger = true;
        }

        #endregion

        #region [Attribute]

        public int Attribute_DecreaseHP(int atk) {
            TDLog.Log($"{idCom.EntityName} 受到伤害 - {atk}");
            var decrease = attributeCom.DecreaseHP(atk);
            hudSlotCom.HpBarHUD.SetHpBar(attributeCom.HP, attributeCom.HPMax);
            return decrease;
        }

        #endregion

        #region [Foot]

        void OnFootTriggerEnter(Collider2D other) {
            FootTriggerEnterAction.Invoke(this, other);
        }

        void OnFootCollisionExit(Collider2D other) {
            FootTriggerExit.Invoke(this, other);
        }

        #endregion

        #region [Renderer]

        public void Renderer_Sync() {
            var logicPos = logicRoot.position;
            rendererRoot.position = logicPos;
            weaponRoot.position = logicPos;

            rendererModCom.Mod.transform.rotation = logicRoot.rotation;
            weaponRoot.rotation = logicRoot.rotation;
        }

        public void Renderer_Easing(float dt) {
            var lerpPos = Vector3.Lerp(rendererRoot.position, logicRoot.position, dt * 30);
            rendererRoot.position = lerpPos;
            weaponRoot.position = lerpPos;

            rendererModCom.Mod.transform.rotation = logicRoot.rotation;
            weaponRoot.rotation = logicRoot.rotation;
        }

        #endregion

    }

}