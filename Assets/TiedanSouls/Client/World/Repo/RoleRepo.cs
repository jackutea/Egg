using System;
using System.Collections.Generic;
using TiedanSouls.World.Entities;

namespace TiedanSouls.World {

    public class RoleRepo {

        Dictionary<int, List<RoleEntity>> allAIRoles_sortedByField;
        List<RoleEntity> allAIRoles;

        RoleEntity playerRole;
        public RoleEntity PlayerRole => playerRole;

        public RoleRepo() {
            allAIRoles_sortedByField = new Dictionary<int, List<RoleEntity>>();
            allAIRoles = new List<RoleEntity>();
        }

        public void AddAIRole(RoleEntity role, int fromFieldTypeID) {
            if (!allAIRoles_sortedByField.TryGetValue(fromFieldTypeID, out var list)) {
                list = new List<RoleEntity>();
                allAIRoles_sortedByField.Add(fromFieldTypeID, list);
            }

            list.Add(role);
            allAIRoles.Add(role);
            TDLog.Log($"添加角色: EntityD={role.EntityD}, fromFieldTypeID={fromFieldTypeID} ");
        }

        public void RemoveAIRole(RoleEntity role) {
            allAIRoles_sortedByField.Remove(role.EntityD);
            allAIRoles.Remove(role);
        }

        public void RemoveAIRoleByID(int id) {
            allAIRoles_sortedByField.Remove(id);
            var len = allAIRoles.Count;
            for (int i = 0; i < len; i++) {
                var role = allAIRoles[i];
                if (role.EntityD == id) {
                    allAIRoles.RemoveAt(i);
                    break;
                }
            }
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

        public void HideAllAIRolesInField(int fieldTypeID) {
            TDLog.Log($"隐藏场景中的角色: fieldTypeID={fieldTypeID}");
            if (!allAIRoles_sortedByField.TryGetValue(fieldTypeID, out var list)) {
                return;
            }

            var len = list.Count;
            for (int i = 0; i < len; i++) {
                var role = list[i];
                role.Hide();
            }
        }

        public void ShowAllAIRolesInField(int fieldTypeID) {
            if (!allAIRoles_sortedByField.TryGetValue(fieldTypeID, out var list)) {
                return;
            }

            var len = list.Count;
            for (int i = 0; i < len; i++) {
                var role = list[i];
                role.Show();
            }
        }

        public void SetPlayerRole(RoleEntity role) {
            if (playerRole != null) {
                TDLog.Error("玩家角色已经被设置过了!");
                return;
            }
            playerRole = role;
        }
    }
}