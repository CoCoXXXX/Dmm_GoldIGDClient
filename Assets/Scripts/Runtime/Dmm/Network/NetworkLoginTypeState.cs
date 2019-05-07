using Dmm.App;
using Dmm.Data;
using Dmm.Log;
using Dmm.StateLogic;
using UnityEngine;

namespace Dmm.Network
{
    public class NetworkLoginTypeState : StateAdapter<IAppContext>
    {
        private const string Tag = "NetworkChooseLoginTypeState";

        private float _bigReconnectTimeOut = 10;

        private float _startLoginTime;

        public override int GetStateCode()
        {
            return NetworkState.ChooseLoginType;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        public override void Initialize(IAppContext context, float time)
        {
            _startLoginTime = time;
            MyLog.DebugWithFrame(Tag, "Wait login type");
        }

        public override bool Process(IAppContext data, float time)
        {
            return LoginRecord.CurrentLoginType != LoginRecord.NoLogin;
        }

        public override StateResult Finish(IAppContext context, float time)
        {
            MyLog.DebugWithFrame(Tag, string.Format("Choose login type: {0}", LoginRecord.CurrentLoginType));
            var stateResult = new StateResult();
            stateResult.NextStateCode = NetworkState.LoginGateServer;
            stateResult.Result = StateResult.Ok;
            return stateResult;
        }

        public override void OnPause(IAppContext context, bool pause, float time)
        {
            MyLog.InfoWithFrame(Tag, string.Format("OnPause: {0}", pause));
            var appController = context.GetAppController();
            if (appController.IsSingleGameMode())
            {
                return;
            }

            if ((!pause) && ((Time.realtimeSinceStartup - _startLoginTime) > _bigReconnectTimeOut))
            {
                var network = context.GetNetworkManager();
                network.InitLogin();
            }
        }
    }
}