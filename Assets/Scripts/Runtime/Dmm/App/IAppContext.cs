using Dmm.Analytic;
using Dmm.Clipboard;
using Dmm.Common;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Network;
using Dmm.Pay;
using Dmm.PIP;
using Dmm.Res;
using Dmm.Sdk;
using Dmm.Session;
using Dmm.Sound;
using Dmm.Task;
using Dmm.WeChat;

namespace Dmm.App
{
    public interface IAppContext
    {
        IAppController GetAppController();
        IGameCanvas GetGameCanvas();

        INetworkManager GetNetworkManager();
        IPayManager GetPayManager();
        ITaskManager GetTaskManager();
        IDialogManager GetDialogManager();
        IAnalyticManager GetAnalyticManager();
        ISystemMsgController GetSystemMsgController();
        ISoundController GetSoundController();
        IWeChatManager GetWeChatManager();

        ConfigHolder GetConfigHolder();

        RemoteAPI GetRemoteAPI();

        IosSDK GetIosSDK();

        IPIPLogic GetPIPLogic();

        ISocketFactory GetSocketFactory();
        IMessageRouter GetMessageRouter();
        IMsgRepo GetMsgRepo();

        XiaoMiManager GetXiaoMiManager();

        IDataRepository GetDataRepository();

        IClipboardManager GetClipboardManager();

        IResourceManager GetResourceManager();
    }
}