using System;
using System.Collections.Generic;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 角色控制效果槽, 包括禁锢, 眩晕, 沉默等. 
    /// 一个角色可以同时受到多种控制效果, 同种控制效果的优先级根据剩余控制帧数来判断
    /// </summary>
    public class RoleCtrlEffectSlotComponent {

        RoleCtrlEffect rootCtrlEffect;
        RoleCtrlEffect stunCtrlEffect;
        RoleCtrlEffect silenceCtrlEffect;

        public RoleCtrlEffectSlotComponent() {
        }

        public void Reset() {
        }

        public void AddCtrlEffect(in RoleCtrlEffectModel ctrlEffectModel) {
            var totalFrame = ctrlEffectModel.totalFrame;
            switch (ctrlEffectModel.ctrlEffectType) {
                case RoleCtrlEffectType.Root:
                    if (totalFrame >= rootCtrlEffect.curFrame) {
                        rootCtrlEffect.curFrame = totalFrame;
                        rootCtrlEffect.totalFrame = totalFrame;
                        TDLog.Log($"控制效果 - 禁锢\n{ctrlEffectModel}");
                    }
                    break;
                case RoleCtrlEffectType.Stun:
                    if (totalFrame >= stunCtrlEffect.curFrame) {
                        stunCtrlEffect.curFrame = totalFrame;
                        stunCtrlEffect.totalFrame = totalFrame;
                        TDLog.Log($"控制效果 - 眩晕\n{ctrlEffectModel}");
                    }
                    break;
                case RoleCtrlEffectType.Silence:
                    if (totalFrame >= silenceCtrlEffect.curFrame) {
                        silenceCtrlEffect.curFrame = totalFrame;
                        silenceCtrlEffect.totalFrame = totalFrame;
                        TDLog.Log($"控制效果 - 沉默\n{ctrlEffectModel}");
                    }
                    break;
                default:
                    TDLog.Log($"未处理的控制效果类型 {ctrlEffectModel.ctrlEffectType}");
                    break;
            }
        }

        public void Tick(){
            if (rootCtrlEffect.curFrame > 0) {
                rootCtrlEffect.curFrame--;
            }
            if (stunCtrlEffect.curFrame > 0) {
                stunCtrlEffect.curFrame--;
            }
            if (silenceCtrlEffect.curFrame > 0) {
                silenceCtrlEffect.curFrame--;
            }
        }

    }

}