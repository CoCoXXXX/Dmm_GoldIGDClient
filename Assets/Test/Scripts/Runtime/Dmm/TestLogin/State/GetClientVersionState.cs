using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Msg;
using Dmm.Network;
using Dmm.StateLogic;
using Dmm.Util;
using Test.Scripts.Runtime.Dmm.TestLogin.Record;
using UnityEngine;

namespace Test.Scripts.Runtime.Dmm.TestLogin.State
{
    public class GetClientVersionState : StateAdapter<IAppContext>
    {
        private const string Tag = "GetClientVersionState";

        public override int GetStateCode()
        {
            return TestLoginStateCode.GetClientVersionState;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        public override void Initialize(IAppContext context, float time)
        {
            var network = NetworkType.NetworkTypeOf(Application.internetReachability);
            var remoteAPI = context.GetRemoteAPI();
            var configHolder = context.GetConfigHolder();
            var dialog = context.GetDialogManager();
            
            var clientVersion = PrefsUtil.GetInt(TestLoginRecord.TestClientVersion,0);
            var platform = PrefsUtil.GetInt(TestLoginRecord.TestPlatform,0);
            var saleChannel = PrefsUtil.GetString(TestLoginRecord.TestSaleChannel,null);
            var product = PrefsUtil.GetString(TestLoginRecord.TestProduct, null);
            // 发送ClientVersion命令。
            remoteAPI.GetVersionData(
                clientVersion,
                // 如果传给服务器端的是null，则服务器端会设置成默认的渠道。
                // 所以不必担心，直接使用SaleChannel。
                saleChannel,
                product,
                platform,
                network,
                SystemInfo.deviceModel,
                GetDeviceId(context)
            );
        }
        
        private string GetDeviceId(IAppContext context)
        {
            var deviceId = SystemInfo.deviceUniqueIdentifier;
            var ios = context.GetIosSDK();
#if UNITY_IOS
            deviceId = ios.GetDeviceId();
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = SystemInfo.deviceUniqueIdentifier;
                ios.SaveDeviceId(deviceId);
                MyLog.InfoWithFrame(Tag, string.Format("save deviceId:{0}", deviceId));
            }
            else
            {
                MyLog.InfoWithFrame(Tag, string.Format("use deviceId:{0}", deviceId));
            }
#endif
#if UNITY_ANDROID // TODO 从安卓取出deviceId。
#endif
            return deviceId;
        }

        public override bool Process(IAppContext context, float time)
        {
            var dataRepository = context.GetDataRepository();
            var container = dataRepository.GetContainer<VersionResult>(DataKey.VersionResult);
            var res = container.Read();
            return res != null;
        }

        public override StateResult Finish(IAppContext context, float time)
        {
            var dataRepository = context.GetDataRepository();
            var container = dataRepository.GetContainer<VersionResult>(DataKey.VersionResult);
            var res = container.Read();
            var versionResult =  JsonUtility.ToJson(res);
            var stateResult = new StateResult();

            var dialog = context.GetDialogManager();

            var loginType = LoginRecord.NoLogin;
            LoginRecord.CurrentLoginType = loginType;
            LoginRecord.LastLoginType = loginType;
            
            if ((res != null) && (res.result == ResultCode.OK))
            {
                stateResult.NextStateCode = TestLoginStateCode.SelectLoginTypeSate;
                dialog.ShowConfirmBox("获取 versionResult 成功");
            }

            return stateResult;
        }
    }
}