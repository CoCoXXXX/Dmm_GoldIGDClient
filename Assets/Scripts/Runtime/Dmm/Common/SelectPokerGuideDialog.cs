using Dmm.Constant;
using Dmm.Dialog;
using Dmm.Util;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class SelectPokerGuideDialog : MyDialog
    {
        public Toggle ZhengLieToggle;

        public Toggle DanZhangToggle;

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
            var xuanDanZhang = PrefsUtil.GetBool(PrefsKeys.XuanDanZhangKey, false);
            DanZhangToggle.isOn = xuanDanZhang;
            ZhengLieToggle.isOn = !xuanDanZhang;

            DanZhangToggle.onValueChanged.AddListener(OnDanZhangToggleChange);
            ZhengLieToggle.onValueChanged.AddListener(OnZhengLieToggleChange);
        }

        public void Confirm()
        {
            PrefsUtil.SetBool(PrefsKeys.HasGuideSelectPoker, true);
            PrefsUtil.Flush();
            Hide();
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}