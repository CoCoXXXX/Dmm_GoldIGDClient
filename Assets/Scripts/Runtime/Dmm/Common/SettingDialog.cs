using Dmm.Constant;
using Dmm.Dialog;
using Dmm.Util;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class SettingDialog : MyDialog
    {
        public Toggle ZhengLieToggle;

        public Toggle DanZhangToggle;

        public Toggle EffectToggle;

        public void OnEffectToggleChanged()
        {
            if (EffectToggle)
            {
                GetSoundController().SetEffectEnable(EffectToggle.isOn);
            }
        }

        public void OnDanZhangToggleChange(bool isSelected)
        {
            if (isSelected)
            {
                PrefsUtil.SetBool(PrefsKeys.XuanDanZhangKey, true);
                PrefsUtil.Flush();
            }
        }

        public void OnZhengLieToggleChange(bool isSelected)
        {
            if (isSelected)
            {
                PrefsUtil.SetBool(PrefsKeys.XuanDanZhangKey, false);
                PrefsUtil.Flush();
            }
        }

        public override void BeforeShow()
        {
            EffectToggle.isOn = PrefsUtil.GetBool(PrefsKeys.EffectEnable, true);

            var xuanDanZhang = PrefsUtil.GetBool(PrefsKeys.XuanDanZhangKey, false);
            DanZhangToggle.isOn = xuanDanZhang;

            ZhengLieToggle.isOn = !xuanDanZhang;

            DanZhangToggle.onValueChanged.AddListener(OnDanZhangToggleChange);
            ZhengLieToggle.onValueChanged.AddListener(OnZhengLieToggleChange);
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}