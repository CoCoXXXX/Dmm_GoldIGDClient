using com.morln.game.gd.command;
using Dmm.Background;
using Dmm.DataContainer;
using Dmm.Login;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;

namespace Dmm.MsgLogic.PU
{
    public class ClientVersionResultHandler : MessageHandlerAdapter<VersionResult>
    {
        private readonly IDataContainer<VersionResult> _versionResult;

        public ClientVersionResultHandler(IDataRepository dataRepository) :
            base(Server.PServer, Msg.CmdType.PU.CLIENT_VERSION_RESULT)
        {
            _versionResult = dataRepository.GetContainer<VersionResult>(DataKey.VersionResult);
        }

        protected override void DoHandle(VersionResult versionResult)
        {
            _versionResult.Write(versionResult, Time.time);

            // 如果成功获取VersionResult数据，则保存Launcher大图。
            if (versionResult.result != ResultCode.OK)
            {
                return;
            }

            // version >= 6.4.0 不再处理SplashScreen的广告位。

            // 保存游戏界面背景。
            var inGameConfig = versionResult.in_game_config;
            if (inGameConfig != null)
            {
                PrefsUtil.SetString(BgConstant.GameBgKey, inGameConfig.game_bg);
                PrefsUtil.SetString(BgConstant.GameBgUrlKey, inGameConfig.game_bg_url);
                PrefsUtil.Flush();
            }

            // 保存公告。
            var billboard = versionResult.billboard_6_2;
            if (billboard != null)
            {
                PrefsUtil.SetString(BillboardPanel.BillboardContentKey, billboard.content);
                PrefsUtil.SetLong(BillboardPanel.BillboardTimestampKey, billboard.timestamp);
                PrefsUtil.Flush();
            }
        }
    }
}