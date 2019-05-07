using Dmm.App;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Log;
using Dmm.Res;
using Dmm.StateLogic;
using Dmm.Util;

namespace Dmm.Network
{
    public class NetworkBuildFirstCacheState : StateAdapter<IAppContext>
    {
        public const string Tag = "NetworkBuildFirstCacheState";

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
            return NetworkState.BuildFirstCache;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        public override void Initialize(IAppContext context, float time)
        {
            _currentRetryTimes = 0;
            BuildFirstCache(context, time);
        }

        public override bool Process(IAppContext context, float time)
        {
            var dataReposity = context.GetDataRepository();
            var buildFirstCacheResult = dataReposity.GetContainer<BuildFirstCacheResult>(DataKey.BuildFirstCacheResult);
            var res = buildFirstCacheResult.Read();

            if (res == null)
            {
                return false;
            }

            if (res.result == BuildFirstCacheResult.Error)
            {
                return CheckBigReconnect(context, time);
            }

            return true;
        }

        public override StateResult Finish(IAppContext context, float time)
        {
            var dataReposity = context.GetDataRepository();
            var buildFirstCacheResult = dataReposity.GetContainer<BuildFirstCacheResult>(DataKey.BuildFirstCacheResult);
            var res = buildFirstCacheResult.Read();
            var assetBundleManager = context.GetResourceManager();
            var stateResult = new StateResult();
            stateResult.NextStateCode = StateResult.Error;
            stateResult.Result = StateResult.Error;

            if (res.result == BuildFirstCacheResult.Ok)
            {
                var initAssetKey = assetBundleManager.GetAssetBundleInitializedKey();
                PrefsUtil.SetInt(initAssetKey, 1);
                PrefsUtil.Flush();

                MyLog.InfoWithFrame(Tag, "BuildFirstCacheResult  ok.");
                stateResult.NextStateCode = NetworkState.DownloadResources;
                stateResult.Result = StateResult.Ok;
                return stateResult;
            }

            MyLog.InfoWithFrame(Tag, "BuildFirstCacheResult Fail!");

            stateResult.ErrMsg = res.error;

            return stateResult;
        }

        private bool CheckBigReconnect(IAppContext context, float time)
        {
            if (_currentRetryTimes < RetryTimes)
            {
                BuildFirstCache(context, time);
                return false;
            }

            return true;
        }

        private void BuildFirstCache(IAppContext context, float time)
        {
            _currentRetryTimes++;
            var _resource = context.GetResourceManager();
            var dataReposity = context.GetDataRepository();
            var buildFirstCacheResult = dataReposity.GetContainer<BuildFirstCacheResult>(DataKey.BuildFirstCacheResult);

            buildFirstCacheResult.ClearNotInvalidate();
            _resource.InitiateIfNeeded();
        }
    }
}