using Dmm.App;
using Dmm.DataContainer;
using Dmm.Log;
using Dmm.Res;
using Dmm.StateLogic;

namespace Dmm.Network
{
    public class NetworkDownloadResourcesState : StateAdapter<IAppContext>
    {
        public const string Tag = "NetworkDownloadResourcesState";

        /// <summary>
        /// 重连的次数
        /// </summary>
        public int RetryTimes = 3;

        /// <summary>
        /// 当前重连的次数
        /// </summary>
        private int _currentRetryTimes = 0;

        public override int GetStateCode()
        {
            return NetworkState.DownloadResources;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        public override void Initialize(IAppContext context, float time)
        {
            _currentRetryTimes = 0;
            DownloadResource(context, time);
        }


        public override bool Process(IAppContext context, float time)
        {
            var dataReposity = context.GetDataRepository();
            var downloadResourceResult =
                dataReposity.GetContainer<DownloadResourceResult>(DataKey.DownloadResourceResult);
            var res = downloadResourceResult.Read();

            if (res == null)
            {
                return false;
            }

            if (res.result == DownloadResourceResult.Error)
            {
                return CheckBigReconnect(context, time);
            }

            return true;
        }

        public override StateResult Finish(IAppContext context, float time)
        {
            var dataReposity = context.GetDataRepository();
            var downloadResourceResult =
                dataReposity.GetContainer<DownloadResourceResult>(DataKey.DownloadResourceResult);
            var res = downloadResourceResult.Read();
            var stateResult = new StateResult();
            stateResult.NextStateCode = StateResult.Error;
            stateResult.Result = StateResult.Error;

            if (res.result == DownloadResourceResult.Ok)
            {
                MyLog.InfoWithFrame(Tag, "DownloadResourceResult  ok.");
                stateResult.NextStateCode = NetworkState.ConnectGateServer;
                stateResult.Result = StateResult.Ok;
                return stateResult;
            }

            MyLog.InfoWithFrame(Tag, "DownloadResourceResult Fail!");

            stateResult.ErrMsg = res.error;

            return stateResult;
        }

        private bool CheckBigReconnect(IAppContext context, float time)
        {
            if (_currentRetryTimes < RetryTimes)
            {
                DownloadResource(context, time);
                return false;
            }

            return true;
        }

        private void DownloadResource(IAppContext context, float time)
        {
            _currentRetryTimes++;
            var _resource = context.GetResourceManager();
            var dataReposity = context.GetDataRepository();
            var downloadResource = dataReposity.GetContainer<DownloadResourceResult>(DataKey.DownloadResourceResult);

            downloadResource.ClearNotInvalidate();
            _resource.StartDownloadResource();
        }
    }
}