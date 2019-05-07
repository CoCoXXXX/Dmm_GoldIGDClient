using System;
using Dmm.App;
using Dmm.Common;
using Dmm.Data;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.PIP;
using Dmm.StateLogic;
using UnityEngine;
using UnityEngine.Networking;

namespace Dmm.Network
{
    public class NetworkPIPState : StateAdapter<IAppContext>
    {
        private const string Tag = "NetworkPIPState";

        public override int GetStateCode()
        {
            return NetworkState.PIP;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        public override void Initialize(IAppContext context, float time)
        {
            MyLog.DebugWithFrame(Tag, "Start download pip data.");
            GetPIP(context, time);
        }

        public override bool Process(IAppContext context, float time)
        {
            var pip = context.GetPIPLogic();
            var status = pip.GetCurrentStatus();
            var pipFinished = status != PIPStatus.Downloading;

            if (pipFinished)
            {
                return true;
            }

            return false;
        }

        public override StateResult Finish(IAppContext context, float time)
        {
            var pip = context.GetPIPLogic();
            var status = pip.GetCurrentStatus();
            var stateResult = new StateResult();

            if (status == PIPStatus.Success)
            {
                MyLog.DebugWithFrame(Tag, "Pip success.");
                stateResult.NextStateCode = NetworkState.BuildFirstCache;
                stateResult.Result = StateResult.Ok;
                CheckVersionUpdate(context);
                return stateResult;
            }

            stateResult.NextStateCode = StateResult.Error;
            stateResult.Result = StateResult.Error;
            stateResult.ErrMsg = string.Format("登录发生错误，请重新登陆\n【错误码{0}】",
                NetworkStateErrorCode.PipFailCode);

            return stateResult;
        }

        private void GetPIP(IAppContext context, float time)
        {
            var pip = context.GetPIPLogic();
            pip.StartDownloadPIP();
        }

        private void CheckVersionUpdate(IAppContext context)
        {
            var pip = context.GetPIPLogic();
            var configHolder = context.GetConfigHolder();
            var dialogManager = context.GetDialogManager();

            var clientVersion = configHolder.ClientVersion;
            var newVersion = pip.GetNewVersion();
            var forceUpdate = pip.GetForceUpdate();
            var updateDescription = pip.GetDescription();

            if (string.IsNullOrEmpty(updateDescription))
            {
                updateDescription = configHolder.ProductDisplayName + "新版本发布啦！请更新到最新版本吧";
            }

            MyLog.InfoWithFrame(Tag, "newVersion is " + newVersion);
            if (clientVersion >= newVersion)
            {
                // 当前的版本高于新版本，则不显示。
                return;
            }

            MyLog.InfoWithFrame(Tag, "检测到有新版本");
            var currentdate = DateTime.Now.ToShortDateString();

            if (LoginRecord.DontUpdateDate == currentdate)
            {
                //如果今天点了暂不更新，则不弹更新对话框
                MyLog.InfoWithFrame(Tag, "当天暂不更新");
                return;
            }

            var androidUrl = pip.GetAndroidUrl();
            var iosUrl = pip.GetIosUrl();

            if (!forceUpdate)
            {
                dialogManager.ShowConfirmBox(updateDescription, true, "更新版本",
                    () => { UpdateNewVersion(context, androidUrl, iosUrl); }, true, "暂不更新",
                    () => { LoginRecord.DontUpdateDate = currentdate; }, true, false, false);
            }
            else
            {
                dialogManager.ShowConfirmBox(updateDescription, true, "更新版本",
                    () => { UpdateNewVersion(context, androidUrl, iosUrl); }, false, "", null, true, false, false);
            }
        }

        private void UpdateNewVersion(IAppContext context, string androidUrl, string iosUrl)
        {
            var dialogManager = context.GetDialogManager();
            var iosSdk = context.GetIosSDK();
            var configHolder = context.GetConfigHolder();

#if UNITY_ANDROID
            dialogManager.ShowDialog<ApkDownloadDialog>(DialogName.ApkDownloadDialog, false, true,
                (apkDownloadDialog) =>
                {
                    apkDownloadDialog.ApplyData(androidUrl, configHolder.ProductDisplayName, "");
                    apkDownloadDialog.Show();
                }
            );

#elif UNITY_IPHONE
            iosSdk.OpenUrl(iosUrl);
#endif
        }
    }
}