using System;
using System.Collections.Generic;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client {

    public class RoleRepo {

        Dictionary<int, List<RoleEntity>> allAIRoles_sortedByField;
        List<RoleEntity> allAIRoles;

        RoleEntity playerRole;
        public RoleEntity PlayerRole => playerRole;

        public RoleRepo() {
            allAIRoles_sortedByField = new Dictionary<int, List<RoleEntity>>();
            allAIRoles = new List<RoleEntity>();
        }

        public bool TryGet(int entityID, out RoleEntity role) {
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

        public void SetPlayerRole(RoleEntity role) {
            if (playerRole != null) {
                TDLog.Warning("玩家角色已经被设置过了!");
                return;
            }
            playerRole = role;
        }

        public void AddAIRole(RoleEntity role, int fromFieldTypeID) {
            if (!allAIRoles_sortedByField.TryGetValue(fromFieldTypeID, out var list)) {
                list = new List<RoleEntity>();
                allAIRoles_sortedByField.Add(fromFieldTypeID, list);
            }

            list.Add(role);
            allAIRoles.Add(role);
            TDLog.Log($"添加角色: {role.IDCom.EntityName} ");
        }

        public void RemoveAIRole(RoleEntity role) {
            allAIRoles_sortedByField.Remove(role.IDCom.EntityID);
            allAIRoles.Remove(role);
        }

        public void RemoveAIRoleByID(int id) {
            allAIRoles_sortedByField.Remove(id);
            var len = allAIRoles.Count;
            for (int i = 0; i < len; i++) {
                var role = allAIRoles[i];
                if (role.IDCom.EntityID == id) {
                    allAIRoles.RemoveAt(i);
                    break;
                }
            }
        }

        public void HideAllAIRolesInField(int fieldTypeID) {
            TDLog.Log($"隐藏关卡中的角色: fieldTypeID={fieldTypeID}");
            if (!allAIRoles_sortedByField.TryGetValue(fieldTypeID, out var list)) {
                return;
            }

            var len = list.Count;
            for (int i = 0; i < len; i++) {
                var role = list[i];
                role.Hide();
            }
        }

        public void ResetAllAIRolesInField(int fieldTypeID, bool isShow) {
            if (!allAIRoles_sortedByField.TryGetValue(fieldTypeID, out var list)) {
                return;
            }

            var len = list.Count;
            for (int i = 0; i < len; i++) {
                var role = list[i];
                role.Reset();
                role.SetPos_Logic(role.BornPos);
                if (isShow) {
                    role.Show();
                }
            }
        }

        public bool HasRoleFromFieldType(int fieldTypeID) {
            return allAIRoles_sortedByField.ContainsKey(fieldTypeID);
        }

        public void ForeachAll(Action<RoleEntity> action) {
            var len = allAIRoles.Count;
            for (int i = 0; i < len; i++) {
                var role = allAIRoles[i];
                action.Invoke(role);
            }

            if (playerRole != null) {
                action.Invoke(playerRole);
            }
        }

        public void Foreach_AI(Action<RoleEntity> action) {
            var len = allAIRoles.Count;
            for (int i = 0; i < len; i++) {
                var role = allAIRoles[i];
                action.Invoke(role);
            }
        }

        public void Foreach_EnemyOfPlayer(Action<RoleEntity> action) {
            var playerAllyType = playerRole.IDCom.AllyType;
            Foreach_Enemy(playerAllyType, action);
        }

        public void Foreach_Enemy(AllyType selfAllyType, Action<RoleEntity> action) {
            var len = allAIRoles.Count;
            for (int i = 0; i < len; i++) {
                var role = allAIRoles[i];
                var roleAllyType = role.IDCom.AllyType;
                if (roleAllyType != selfAllyType && roleAllyType != AllyType.Neutral) {
                    action.Invoke(role);
                }
            }
        }

        public void Foreach_AIFromField(int fieldTypeID, Action<RoleEntity> action) {
            if (!allAIRoles_sortedByField.TryGetValue(fieldTypeID, out var list)) {
                return;
            }

            var len = list.Count;
            for (int i = 0; i < len; i++) {
                var role = list[i];
                action.Invoke(role);
            }
        }

    }
}