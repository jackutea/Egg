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
        public RoleAttributeComponent AttributeCom { get; private set; }
        public MoveComponent MoveCom { get; private set; }
        public RoleFSMComponent FSMCom { get; private set; }
        public WeaponSlotComponent WeaponSlotCom { get; private set; }
        public SkillSlotComponent SkillSlotCom { get; private set; }
        public BuffSlotComponent BuffSlotCom { get; private set; }
        public RoleCtrlEffectSlotComponent CtrlEffectSlotCom { get; private set; }

        RoleRendererComponent rendererCom;
        public RoleRendererComponent RendererCom => rendererCom;

        #endregion

        #region [Root]

        Transform logicRoot;
        public Transform LogicRoot => logicRoot;
        public Vector3 LogicRootPos => LogicRoot.position;
        public float LogicAngleZ => LogicRoot.rotation.z;
        public Quaternion LogicRotation => LogicRoot.rotation;
        public void SetLogicPos(Vector2 pos) => LogicRoot.position = pos;
        public void SetLogicRot(Quaternion rot) => LogicRoot.rotation = rot;


        Transform weaponRoot;
        public Transform WeaponRoot => weaponRoot;

        Rigidbody2D rb;
        public Rigidbody2D RB => rb;

        CapsuleCollider2D coll_LogicRoot;
        public CapsuleCollider2D Coll_LogicRoot => coll_LogicRoot;

        public void SetTrigger(bool isTrigger) => Coll_LogicRoot.isTrigger = isTrigger;

        #endregion

        #region [Locomotion]

        sbyte faceDirX;
        public sbyte FaceDirX => faceDirX;

        public int groundCount;

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

            MoveCom = new MoveComponent();
            IDCom = new EntityIDComponent();
            IDCom.SetEntityType(EntityType.Role);
            InputCom = new InputComponent();
            WeaponSlotCom = new WeaponSlotComponent();
            AttributeCom = new RoleAttributeComponent();
            FSMCom = new RoleFSMComponent();
            SkillSlotCom = new SkillSlotComponent();
            BuffSlotCom = new BuffSlotComponent();
            CtrlEffectSlotCom = new RoleCtrlEffectSlotComponent();
            rendererCom = new RoleRendererComponent();
            logicRoot = transform.Find("logic_root");
            var rendererRoot = transform.Find("renderer_root");

            rb = logicRoot.GetComponent<Rigidbody2D>();
            coll_LogicRoot = logicRoot.GetComponent<CapsuleCollider2D>();
            weaponRoot = rendererRoot.Find("weapon_root");

            MoveCom.Inject(RB);
            WeaponSlotCom.Inject(weaponRoot);
            rendererCom.Inject(rendererRoot);

        }

        public void TearDown() {
            GameObject.Destroy(gameObject);
        }

        public void Reset() {
            // - Weapon
            WeaponSlotCom.Reset();
            // - Attribute
            AttributeCom.Reset();
            // - FSM
            FSMCom.ResetAll();
            // - Input
            InputCom.Reset();
            // - Movement
            MoveCom.Reset();
            // - Renderer
            rendererCom.Reset(AttributeCom.GP, AttributeCom.HP, AttributeCom.HPMax);
        }

        public void SetFromFieldTypeID(int fieldTypeID) {
            IDCom.SetFromFieldTypeID(fieldTypeID);

            var idArgs = IDCom.ToArgs();
            SkillSlotCom.SetFather(idArgs);
            BuffSlotCom.SetFather(idArgs);
            Coll_LogicRoot.GetComponent<EntityCollider>().SetFather(idArgs);
        }

        public void Hide() {
            LogicRoot.gameObject.SetActive(false);
            rendererCom.Hide();
        }

        public void TryMoveByInput() {
            if (!InputCom.HasMoveOpt) return;

            Vector2 moveAxis = InputCom.MoveAxis;
            MoveCom.MoveHorizontal(moveAxis.x, AttributeCom.MoveSpeed);
        }

        public bool TryJumpByInput() {
            if (!InputCom.InputJump) return false;
            if (!FSMCom.PositionStatus.Contains(RolePositionStatus.OnGround)
            && !FSMCom.PositionStatus.Contains(RolePositionStatus.OnCrossPlatform)) return false;

            var rb = MoveCom.RB;
            var velo = rb.velocity;
            var jumpSpeed = AttributeCom.JumpSpeed;
            velo.y = jumpSpeed;
            MoveCom.SetVelocity(velo);

            FSMCom.Enter_JumpingUp();

            return true;
        }

        public void Fall(float dt) {
            var fallSpeed = AttributeCom.FallSpeed;
            var fallSpeedMax = AttributeCom.FallSpeedMax;

            var vel = MoveCom.Velocity;
            vel.y = Mathf.Max(vel.y - fallSpeed * dt, -fallSpeedMax);
            MoveCom.SetVerticalVelocity(vel);
        }

        public void HorizontalFaceTo(float dirX) {
            if (Mathf.Abs(dirX) < 0.01f) return;

            var rot = LogicRoot.localRotation;
            bool isRight = dirX > 0;
            if (isRight) rot.y = 0;
            else rot.y = 180;
            LogicRoot.localRotation = rot;
        }

        public void Stop() {
            MoveCom.Stop();
        }

    }

}