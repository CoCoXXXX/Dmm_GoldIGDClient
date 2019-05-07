using System;
using System.IO;
using DG.Tweening;
using Dmm.Constant;
using Dmm.Dialog;
using Dmm.Network;
using Dmm.Res;
using Dmm.Sdk;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Common
{
    public class ApkDownloadDialog : UIWindow
    {
        #region inject

        private IDialogManager _dialogManager;

        private IResourceCache _resourceCache;

        private RemoteAPI _remoteAPI;

        private AndroidSDK _android;

        [Inject]
        public void Initialize(
            AndroidSDK android,
            IDialogManager dialogManager,
            IResourceCache resourceCache,
            RemoteAPI remoteAPI)
        {
            _android = android;
            _dialogManager = dialogManager;
            _resourceCache = resourceCache;
            _remoteAPI = remoteAPI;
        }

        public class Factory : PrefabFactory<ApkDownloadDialog>
        {
        }

        #endregion

        public const string AppFileName = "app.apk";

        public RectTransform Content;

        public Slider DownloadProgress;

        public Text DownloadTip;

        public float HidePosition = 60;

        public float ShowPosition = 25;

        public float AnimationTime = 0.5f;

        private Tweener _tweener;

        public override void Show()
        {
            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }

            if (!Content) return;

            Content.anchoredPosition = new Vector2(0, HidePosition);
            _tweener = Content
                .DOAnchorPos(new Vector2(0, ShowPosition), AnimationTime)
                .OnComplete(() =>
                {
                    if (!string.IsNullOrEmpty(Url))
                        _downloader = _resourceCache.StartDownload(
                            AppFileName,
                            Url,
                            ContentType.Bytes,
                            ProcessDownloadedFile);
                });
        }

        public override void Hide()
        {
            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }

            if (!Content) return;

            _tweener = Content
                .DOAnchorPos(new Vector2(0, HidePosition), AnimationTime)
                .OnComplete(() => Destroy(gameObject));
        }

        public string Url { get; private set; }

        public string AppName { get; private set; }

        public string AwardCode { get; private set; }

        private DownloadTask _downloader;

        public void ApplyData(string url, string appName, string awardCode)
        {
            Url = url;
            AppName = appName;
            AwardCode = awardCode;

            if (DownloadTip)
                DownloadTip.text = appName + "下载中";
        }

        public void OnEnable()
        {
            if (DownloadProgress)
            {
                DownloadProgress.maxValue = 1;
                DownloadProgress.value = 0;
            }
        }

        public void Update()
        {
            if (_downloader == null) return;

            if (_downloader.Finished)
            {
                Hide();
                return;
            }

            if (DownloadProgress)
                DownloadProgress.value = _downloader.Progress;
        }

        private void ProcessDownloadedFile(DownloadTask task)
        {
            // 打开安装对话框。
            var path = FilePath.BinaryFilePath() + AppFileName;
            try
            {
                if (File.Exists(path))
                {
                    _dialogManager.ShowConfirmBox(
                        string.Format("{0}下载完成", AppName),
                        true, "安装", () =>
                        {
#if UNITY_ANDROID
                            _android.InstallApk(path);
                            if (!string.IsNullOrEmpty(AwardCode))
                                _remoteAPI.RequestAward(AwardCode);
#endif
                        },
                        false, null, null,
                        true, false, true);
                }
            }
            catch (Exception e)
            {
                _dialogManager.ShowToast("发生错误", 2, true);
            }
        }
    }
}