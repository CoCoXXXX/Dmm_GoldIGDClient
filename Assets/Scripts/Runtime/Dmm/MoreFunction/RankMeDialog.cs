using System;
using Dmm.Dialog;
using Dmm.Util;
using UnityEngine.UI;

namespace Dmm.MoreFunction
{
    public class RankMeDialog : MyDialog
    {
        public Text Description;

        private string _appId;

        private void OnEnable()
        {
            Description.text = string.Format(
                "觉得我们的游戏不错\n就给我们评个分吧^_^");
        }

        // 更新了key是为了所有装过新版本的人都能再评一次分。

        public const string RankMeShowKey = "RankMeShow700";

        public const string RankMeShowTimeKey = "RankMeShowTime700";

        public void GoRank()
        {
            _appId = GetConfigHolder().AppId;
            if (string.IsNullOrEmpty(_appId))
            {
                return;
            }

            GetIosSDK().OpenUrl(string.Format("itms-apps://itunes.apple.com/app/id{0}", _appId));
            // _ios.OpenProductPage(_appId, null);

            PrefsUtil.SetBool(RankMeShowKey, true);
            PrefsUtil.Flush();

            GetAnalyticManager().Event("rank_me_panel_go");

            Hide();
        }

        public override void AfterShow()
        {
            PrefsUtil.SetLong(RankMeShowTimeKey, DateTime.Now.CurrentTimeMillis());
            PrefsUtil.Flush();
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}