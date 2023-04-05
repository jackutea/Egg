using System;
using System.Collections.Generic;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client {

    public class RoleRepo {

        Dictionary<int, List<RoleEntity>> allAIRoles_Sorted;
        List<RoleEntity> allAIRoles;

        RoleEntity playerRole;
        public RoleEntity PlayerRole => playerRole;

        List<RoleEntity> roleList_temp;

        public RoleRepo() {
            allAIRoles_Sorted = new Dictionary<int, List<RoleEntity>>();
            allAIRoles = new List<RoleEntity>();
            roleList_temp = new List<RoleEntity>();
        }

        public void Set_Player(RoleEntity role) {
            if (playerRole != null) {
                TDLog.Warning("玩家角色已经被设置过了!");
                return;
            }
            playerRole = role;
        }

        #region [增]

        public void Add_ToAI(RoleEntity role) {
            var fromFieldTypeID = role.IDCom.FromFieldTypeID;
            if (!allAIRoles_Sorted.TryGetValue(fromFieldTypeID, out var list)) {
                list = new List<RoleEntity>();
                allAIRoles_Sorted.Add(fromFieldTypeID, list);
            }

            list.Add(role);
            allAIRoles.Add(role);
            TDLog.Log($"添加角色: {role.IDCom.EntityName} ");
        }

        #endregion

        #region [删]

        public bool TryRemove_FromAll(RoleEntity role) {
            var roleIDCom = role.IDCom;
            if (playerRole != null && playerRole.IDCom.EntityID == roleIDCom.EntityID) {
                playerRole = null;
                return true;
            }

            var len = allAIRoles.Count;
            for (int i = 0; i < len; i++) {
                var r = allAIRoles[i];
                if (r.IDCom.EntityID == roleIDCom.EntityID) {
                    allAIRoles.RemoveAt(i);
                    var fromFieldTypeID = role.IDCom.FromFieldTypeID;
                    if (allAIRoles_Sorted.TryGetValue(fromFieldTypeID, out var list)) {
                        list.Remove(role);
                    }
                    return true;
                }
            }

            return false;
        }

        public void Remove_AI(RoleEntity role) {
            allAIRoles_Sorted.Remove(role.IDCom.EntityID);
            allAIRoles.Remove(role);
        }

        public void Remove_AI(int id) {
            allAIRoles_Sorted.Remove(id);
            var len = allAIRoles.Count;
            for (int i = 0; i < len; i++) {
                var role = allAIRoles[i];
                if (role.IDCom.EntityID == id) {
                    allAIRoles.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion

        #region [查]

        public bool TryGet_TrackEntity(int fieldTypeID,
                                       RelativeTargetGroupType relativeTargetGroupType,
                                       in EntityIDArgs compareIDArgs,
                                       in AttributeSelectorModel attributeSelectorModel,
                                       out RoleEntity role) {
            var list = GetRoleList_RelativeTargetGroupType(fieldTypeID, relativeTargetGroupType, compareIDArgs);
            var count = list.Count;
            for (int i = 0; i < count; i++) {
                var r = list[i];
                if (r.FSMCom.ActionState == RoleActionState.Dying) continue;                                    // 状态过滤 - 已经退出
                if (!r.AttributeCom.IsMatch(attributeSelectorModel)) continue;      // 选择器过滤 - 属性
                role = r;
                return true;
            }

            role = null;
            return false;
        }

        public bool TryGet_FromAll(int entityID, out RoleEntity role) {
            role = null;
            if (playerRole != null && playerRole.IDCom.EntityID == entityID) {
                role = playerRole;
                return true;
            }

            var len = allAIRoles.Count;
            for (int i = 0; i < len; i++) {
                var r = allAIRoles[i];
                if (r.IDCom.EntityID == entityID) {
                    role = r;
                    return true;
                }
            }
            return false;
        }

        public bool HasAliveEnemy(int fieldTypeID, AllyType allyType) {
            bool hasAliveEnemy = false;
            Foreach_ByFieldTypeID(fieldTypeID, (role) => {
                var idCom = role.IDCom;
                var roleAllyType = idCom.AllyStatus;
                if (!roleAllyType.IsEnemy(allyType)) return;
                if (role.FSMCom.ActionState != RoleActionState.Dying) hasAliveEnemy = true;
            });

            return hasAliveEnemy;
        }

        public bool HasAI(int fieldTypeID) {
            if (fieldTypeID == -1) {
                return allAIRoles.Count > 0;
            } else {
                return allAIRoles_Sorted.TryGetValue(fieldTypeID, out var list) && list.Count > 0;
            }
        }

        /// <summary>
        /// 遍历指定关卡的所有角色 -1代表查找范围为所有关卡
        /// </summary>
        public void Foreach_All(int fieldTypeID, Action<RoleEntity> action) {
            Foreach_ByFieldTypeID(fieldTypeID, action);
            if (playerRole != null) action.Invoke(playerRole);
        }

        /// <summary>
        /// 遍历所有角色
        /// </summary>
        public void Foreach_All(Action<RoleEntity> action) {
            Foreach_ByFieldTypeID(-1, action);
            if (playerRole != null) action.Invoke(playerRole);
        }

        /// <summary>
        /// 遍历指定关卡的所有角色(除玩家角色) -1代表查找范围为所有关卡
        /// </summary>
        public void Foreach_AI(int fieldTypeID, Action<RoleEntity> action) {
            Foreach_ByFieldTypeID(fieldTypeID, action);
        }

        /// <summary>
        /// 遍历指定角色的友军角色 -1代表查找范围为所有关卡
        /// </summary>
        public void Foreach_Ally(int fieldTypeID, in EntityIDArgs iDArgs, Action<RoleEntity> action) {
            var selfEntityID = iDArgs.entityID;
            if (TryGet_FromAll(selfEntityID, out var selfRole)) return;
            var selfAllyType = selfRole.IDCom.AllyStatus;

            Foreach_ByFieldTypeID(fieldTypeID, (role) => {
                var roleAllyType = role.IDCom.AllyStatus;
                if (roleAllyType.IsAlly(selfAllyType)) action.Invoke(role);
            });
        }

        /// <summary>
        /// 遍历玩家角色的敌对角色 -1代表查找范围为所有关卡
        /// </summary>
        public void Foreach_EnemyOfPlayer(int fieldTypeID, Action<RoleEntity> action) {
            var playerAllyType = playerRole.IDCom.AllyStatus;
            Foreach_Enemy(fieldTypeID, playerAllyType, action);
        }

        /// <summary>
        /// 遍历敌对角色 -1代表查找范围为所有关卡
        /// </summary>
        public void Foreach_Enemy(int fieldTypeID, AllyType selfAllyType, Action<RoleEntity> action) {
            Foreach_ByFieldTypeID(fieldTypeID,
            (role) => {
                var roleAllyType = role.IDCom.AllyStatus;
                if (roleAllyType.IsEnemy(selfAllyType)) action.Invoke(role);
            });
        }

        /// <summary>
        /// 遍历中立角色 -1代表查找范围为所有关卡
        /// </summary>
        public void Foreach_Neutral(int fieldTypeID, AllyType selfAllyType, Action<RoleEntity> action) {
            Foreach_ByFieldTypeID(fieldTypeID, (role) => {
                var roleAllyType = role.IDCom.AllyStatus;
                if (roleAllyType == AllyType.Neutral) action.Invoke(role);
            });
        }

        /// <summary>
        /// 获取所有指定相对阵营类型的角色
        /// </summary>
        public List<RoleEntity> GetRoleList_RelativeTargetGroupType(int fieldTypeID, RelativeTargetGroupType relativeTargetGroupType, in EntityIDArgs compareIDArgs) {
            roleList_temp.Clear();
            Foreach_RelativeTargetGroupType(fieldTypeID, relativeTargetGroupType, compareIDArgs, (role) => {
                roleList_temp.Add(role);
            });
            return roleList_temp;
        }

        /// <summary>
        /// 遍历所有指定相对阵营类型的角色
        /// </summary>
        public void Foreach_RelativeTargetGroupType(int fieldTypeID, RelativeTargetGroupType relativeTargetGroupType, in EntityIDArgs compareIDArgs, Action<RoleEntity> action) {
            if (relativeTargetGroupType == RelativeTargetGroupType.None) return;

            var compareEntityType = compareIDArgs.entityType;
            var compareAllyType = compareIDArgs.allyType;

            // 若比较的是角色，且角色不存在，则返回
            bool isCompareRole = compareEntityType == EntityType.Role;
            bool hasRole = TryGet_FromAll(compareIDArgs.entityID, out var selfRole);
            if (isCompareRole && !hasRole) {
                return;
            }

            if (relativeTargetGroupType.Contains(RelativeTargetGroupType.Self)) {
                if (isCompareRole) action.Invoke(selfRole);
            }

            if (relativeTargetGroupType.Contains(RelativeTargetGroupType.Ally)) {
                Foreach_Ally(fieldTypeID, compareIDArgs, action);
            }

            if (relativeTargetGroupType.Contains(RelativeTargetGroupType.Enemy)) {
                Foreach_Enemy(fieldTypeID, compareAllyType, action);
            }

            if (relativeTargetGroupType.Contains(RelativeTargetGroupType.Neutral)) {
                Foreach_Neutral(fieldTypeID, compareAllyType, action);
            }
        }

        /// <summary>
        ///  根据fieldTypeID遍历角色 -1代表查找范围为所有关卡
        /// </summary>
        public void Foreach_ByFieldTypeID(int fieldTypeID, Action<RoleEntity> action) {
            if (fieldTypeID == -1) {
                Foreach_List(action);
            } else {
                Foreach_SortedDic(fieldTypeID, action);
            }
        }

        void Foreach_List(Action<RoleEntity> action) {
            allAIRoles.ForEach(action);
        }

        void Foreach_SortedDic(int fieldTypeID, Action<RoleEntity> action) {
            if (!allAIRoles_Sorted.TryGetValue(fieldTypeID, out var list)) return;
            list.ForEach(action);
        }

        /// <summary>
        /// 遍历所有相对目标组角色, 根据'属性'选择器过滤
        /// </summary>sd
        public void Foreach_AttributeSelector(int fieldTypeID,
                                              RelativeTargetGroupType relativeTargetGroupType,
                                              in EntityIDArgs self,
                                              in AttributeSelectorModel attributeSelectorModel,
                                              Action<RoleEntity> action) {
            var list = GetRoleList_RelativeTargetGroupType(fieldTypeID, relativeTargetGroupType, self);
            var count = list.Count;
            for (int i = 0; i < count; i++) {
                var role = list[i];
                if (!role.AttributeCom.IsMatch(attributeSelectorModel)) continue;   // 选择器过滤 - 属性
                action.Invoke(role);
            }
        }

        #endregion

    }
}