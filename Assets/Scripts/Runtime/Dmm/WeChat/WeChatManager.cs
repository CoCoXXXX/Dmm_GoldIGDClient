using System.Collections;
using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Network;
using Dmm.Res;
using Dmm.Sdk;
using Dmm.Task;
using Dmm.Util;
using Dmm.ZXing;
using UnityEngine;
using Zenject;

namespace Dmm.WeChat
{
    public class WeChatManager : MonoBehaviour, IWeChatManager
    {
        #region Inject

        private ConfigHolder _configHolder;

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        private AndroidSDK _android;

        private IosSDK _ios;

        private ITaskManager _taskManager;

        private IDataContainer<NotifyDoShareResult> _notifyDoShareResult;

        private IDataContainer<InGameConfig> _inGameConfig;

        private IFilePicManager _filePicManager;

        private IAnalyticManager _analyticManager;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            ConfigHolder configHolder,
            AndroidSDK android,
            IosSDK ios,
            IDialogManager dialogManager,
            ITaskManager taskManager,
            IFilePicManager filePicManager,
            IAnalyticManager analyticManager,
            RemoteAPI remoteAPI)
        {
            _configHolder = configHolder;
            _android = android;
            _ios = ios;
            _remoteAPI = remoteAPI;
            _dialogManager = dialogManager;
            _taskManager = taskManager;
            _filePicManager = filePicManager;
            _analyticManager = analyticManager;
            _notifyDoShareResult = dataRepository.GetContainer<NotifyDoShareResult>(DataKey.NotifyDoShareResult);
            _inGameConfig = dataRepository.GetContainer<InGameConfig>(DataKey.InGameConfig);
        }

        #endregion

        #region Wexin Share

        private NotifyDoShare _currentNotifyDoShare;

        private WechatShareSeq _seq;

        private void Update()
        {
            if (_doShares.Count <= 0)
            {
                return;
            }

            if (_seq == null)
            {
                _seq = new WechatShareSeq(PeekNotiftDoShare(), _notifyDoShareResult, _remoteAPI, _dialogManager,
                    () =>
                    {
                        _doShares.Dequeue();
                        _seq.Cancel();
                        _seq = null;
                        MyLog.InfoWithFrame(name, "Wechat share finish");
                    });

                _taskManager.ExecuteSeq(_seq);
            }
        }

        #region 截图贴上二维码分享微信

        public void WechatShareScreenShot(int offset)
        {
            StartCoroutine(StartShare(offset));
        }

        public const string WechatShareFile = "WechatShare.png";

        private IEnumerator StartShare(int offset)
        {
            yield return new WaitForEndOfFrame();
            var texture = TextureHelper.CaptureByRect(new Rect(0, 0, Screen.width, Screen.height));
            var inGameConmfig = _inGameConfig.Read();
            if (inGameConmfig == null)
            {
                _dialogManager.ShowToast("数据发生错误，无法分享T_T", 2, true);
                _analyticManager.Event("game_wx_share_capture_screen_fail");
                yield break;
            }

            var inviteUrl = inGameConmfig.wx_invite_url;
            if (string.IsNullOrEmpty(inviteUrl))
            {
                _dialogManager.ShowToast("数据发生错误，无法分享T_T", 2, true);
                _analyticManager.Event("game_wx_share_capture_screen_fail");
                yield break;
            }

            var qrCodeTexture = GenerateQRCode.GenerateQRCodeTexture2DFromUrl(inviteUrl);

            var startX = Screen.width - offset;
            var startY = Screen.height - offset;
            //融合图片
            TextureHelper.ComposeTwoTexture(texture, qrCodeTexture, startX, startY);

            var bytes = texture.EncodeToPNG();
            var path = _filePicManager.SavePic(bytes, WechatShareFile);

            if (!string.IsNullOrEmpty(path))
            {
                _dialogManager.ShowDialog<WeChatShareDialog>(DialogName.WeChatShareDialog, false, true,
                    (dialog) =>
                    {
                        dialog.ApplyData("截图分享", null, null, path, null, null, null);
                        dialog.Show();
                    });

                _analyticManager.Event("game_wx_share");
            }
            else
            {
                _dialogManager.ShowToast("截屏失败了T_T", 2, true);
                _analyticManager.Event("game_wx_share_capture_screen_fail");
            }
        }

        #endregion

        /// <summary>
        /// 请求分享结果的消息队列
        /// </summary>
        private readonly Queue<NotifyDoShare> _doShares = new Queue<NotifyDoShare>();

        private void EnqueueDoShare(NotifyDoShare doShare)
        {
            if (_doShares == null)
            {
                return;
            }

            foreach (var k in _doShares)
            {
                if (doShare.id == k.id)
                {
                    return;
                }
            }

            _doShares.Enqueue(doShare);
        }

        private NotifyDoShare PeekNotiftDoShare()
        {
            if (_doShares.Count <= 0)
            {
                return null;
            }

            return _doShares.Peek();
        }

        public void WxShareResult(string content)
        {
            MyLog.InfoWithFrame(name, "WXShareResult: " + content);
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            var result = JsonUtility.FromJson<WxShareResult>(content);
            if (result.Res == WeChat.WxShareResult.Success)
            {
                var shareContent = JsonUtility.FromJson<ShareContent>(result.Content);
                if (shareContent == null)
                {
                    return;
                }

                var type = shareContent.Type;
                var code = shareContent.Content;

                if (string.IsNullOrEmpty(code))
                {
                    return;
                }

                var notifyDoShare = new NotifyDoShare();
                notifyDoShare.id = System.Guid.NewGuid().ToString("N");
                notifyDoShare.type = type;
                if (type == (int) ShareResultType.AwardCode)
                {
                    notifyDoShare.award_code = code;
                }
                else if (type == (int) ShareResultType.TaskCode)
                {
                    notifyDoShare.user_task_code = code;
                }

                EnqueueDoShare(notifyDoShare);
            }
            else
            {
                if (!string.IsNullOrEmpty(result.ErrMsg))
                {
                    _dialogManager.ShowToast(result.ErrMsg, 2, true);
                }
            }
        }

        public void WxSuccess(string awardCode)
        {
            MyLog.InfoWithFrame(name, "WxSuccess: " + awardCode);
            if (!string.IsNullOrEmpty(awardCode))
                _remoteAPI.RequestAward(awardCode);
        }

        public void WxFail(string errorMsg)
        {
            MyLog.InfoWithFrame(name, "WxFail: " + errorMsg);
            if (!string.IsNullOrEmpty(errorMsg))
                _dialogManager.ShowToast(errorMsg, 2, true);
        }

        #endregion

        /// <summary>
        /// 需要在正确获取微信参数后调用。
        /// 也就是在ClientVersionOk的时候调用。
        /// </summary>
        public void Init()
        {
#if UNITY_ANDROID
            var wxAppId = _configHolder.WxAppId;
            _android.WxInit(wxAppId);
#endif
#if UNITY_IOS // nothing.
#endif
        }

        public void WxShare(string url, string wxImgUrl, string wxImgPath, string wxTitle, string wxContent,
            string thumbUrl, string content = null)
        {
            MyLog.InfoWithFrame(name, "微信分享 url：" + url);
#if UNITY_ANDROID
            _android.WxShare(url, wxImgUrl, wxImgPath, wxTitle, wxContent, thumbUrl, content);
#endif
#if UNITY_IOS
            _ios.WxShare(url, wxImgUrl, wxImgPath, wxTitle, wxContent, thumbUrl, content);
#endif
        }

        public void WxCircle(string url, string wxImgUrl, string wxImgPath, string wxTitle, string wxContent,
            string thumbUrl, string content = null)
        {
            MyLog.InfoWithFrame(name, "微信分享朋友圈 url：" + url);
#if UNITY_ANDROID
            _android.WxCircle(url, wxImgUrl, wxImgPath, wxTitle, wxContent, thumbUrl, content);
#endif
#if UNITY_IOS
            _ios.WxCircle(url, wxImgUrl, wxImgPath, wxTitle, wxContent, thumbUrl, content);
#endif
        }

        public void WxIgdInvite()
        {
#if UNITY_ANDROID
            _android.WxIgdInvite();
#endif
#if UNITY_IOS
#endif
        }

        public bool IsWechatInstalled()
        {
#if UNITY_IOS
            return _ios.IsWechatInstalled();
#endif
#if UNITY_ANDROID
            return _android.IsWechatInstalled();
#endif
            return false;
        }

        #region Auth

        public void Auth(string deviceId)
        {
            MyLog.InfoWithFrame(name, string.Format("Get deviceId  {0}", deviceId));
            _authResult = null;
            LoginRecord.RemoveOpenId();
            LoginRecord.RemoveAuthCode();
            PrefsUtil.Flush();

#if UNITY_ANDROID
            _android.WxAuth(deviceId);
#endif
#if UNITY_IOS
            _ios.WxAuth(deviceId);
#endif
        }

        public void AuthResult(string param)
        {
            MyLog.InfoWithFrame(name, string.Format("Get auth result: {0}", param));
            if (string.IsNullOrEmpty(param))
            {
                _authResult = new WxAuthResult
                {
                    Result = WxAuthResult.Error,
                };
                return;
            }

            _authResult = JsonUtility.FromJson<WxAuthResult>(param);

            if (_authResult == null)
            {
                _authResult = new WxAuthResult()
                {
                    Result = WxAuthResult.Error
                };
            }

            MyLog.InfoWithFrame(name, string.Format("_authResult.Result: {0}", _authResult.Result));
            if (_authResult.Result == WxAuthResult.Ok)
            {
                LoginRecord.SaveAuthCode(_authResult.Code);
            }
        }

        private WxAuthResult _authResult;

        public WxAuthResult GetWxAuthResult()
        {
            return _authResult;
        }

        #endregion
    }
}