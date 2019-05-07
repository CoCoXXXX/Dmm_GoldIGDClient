using System.Collections;
using DG.Tweening;
using Dmm.Analytic;
using Dmm.App;
using Dmm.Clipboard;
using Dmm.Common;
using Dmm.Data;
using Dmm.DataContainer;
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
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Dialog
{
    public class MyDialog : UIWindow, IAppContext
    {
        #region Inject

        private IAppContext _context;

        private IDataRepository _dataRepository;

        [Inject]
        public void Inject(IAppContext context, IDataRepository dataRepository)
        {
            _context = context;
            _dataRepository = dataRepository;
        }

        #endregion

        #region 状态

        public const int StatusHide = 0;

        public const int StatusShow = 1;

        public const int StatusAnim = 2;

        private int _status = StatusHide;

        /// <summary>
        /// 状态。
        /// </summary>
        public int Status
        {
            get { return _status; }
            private set { _status = value; }
        }

        #endregion

        /// <summary>
        /// 背景图片。
        /// </summary>
        public Image BgCover;

        /// <summary>
        /// 内容容器。
        /// </summary>
        public RectTransform Content;

        public float BgAlpha = 150f;

        /// <summary>
        /// 显示动画的时间。
        /// </summary>
        public float ShowAnimationTime = 0.2f;

        /// <summary>
        /// 隐藏动画的时间。
        /// </summary>
        public float HideAnimationTime = 0.1f;

        private Tweener _bgTweener;

        private Tweener _contentTweener;

        public override void Show()
        {
            StartCoroutine(ShowCoroutine());
        }

        private IEnumerator ShowCoroutine()
        {
            Status = StatusAnim;

            // 显示背景。
            if (_bgTweener != null)
            {
                _bgTweener.Kill();
                _bgTweener = null;
            }

            // 显示内容。
            if (_contentTweener != null)
            {
                _contentTweener.Kill();
                _contentTweener = null;
            }

            if (BgCover)
            {
                if (!BgCover.gameObject.activeSelf)
                    BgCover.gameObject.SetActive(true);

                BgCover.color = new Color(0, 0, 0, 0);
            }

            if (Content)
            {
                if (!Content.gameObject.activeSelf)
                    Content.gameObject.SetActive(true);

                Content.transform.localScale = new Vector3(0.1f, 0.1f, 1);
            }

            BeforeShow();

            yield return null;

            if (BgCover)
            {
                _bgTweener = BgCover
                    .DOFade(BgAlpha / 255f, ShowAnimationTime)
                    .SetEase(Ease.Linear);
            }

            if (Content)
            {
                _contentTweener = Content
                    .DOScale(new Vector3(1, 1, 1), ShowAnimationTime)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        Status = StatusShow;
                        AfterShow();
                    });
            }
            else
            {
                Status = StatusShow;
            }
        }

        public virtual void BeforeShow()
        {
        }

        public virtual void AfterShow()
        {
        }

        public override void Hide()
        {
            Status = StatusAnim;

            // 隐藏背景图片。
            if (_bgTweener != null)
            {
                _bgTweener.Kill();
                _bgTweener = null;
            }

            if (BgCover)
            {
                _bgTweener = BgCover
                    .DOFade(0, HideAnimationTime)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        if (BgCover.gameObject.activeSelf)
                            BgCover.gameObject.SetActive(false);
                    });
            }

            // 隐藏内容。
            if (_contentTweener != null)
            {
                _contentTweener.Kill();
                _contentTweener = null;
            }

            if (Content && Content.gameObject.activeSelf)
            {
                _contentTweener = Content
                    .DOScale(new Vector3(0.1f, 0.1f, 1), HideAnimationTime)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        if (Content.gameObject.activeSelf)
                            Content.gameObject.SetActive(false);

                        Status = StatusHide;
                        AfterHide();
                    });
            }
            else
            {
                Status = StatusHide;
            }
        }

        public virtual void AfterHide()
        {
            // 默认情况下对话框隐藏了，就销毁了。
            Destroy(gameObject);
        }

        #region AppContext

        protected IDataContainer<T> GetContainer<T>(string key)
        {
            return _dataRepository.GetContainer<T>(key);
        }

        public IAppContext GetContext()
        {
            return _context;
        }

        public IAppController GetAppController()
        {
            return _context.GetAppController();
        }

        public IGameCanvas GetGameCanvas()
        {
            return _context.GetGameCanvas();
        }

        public INetworkManager GetNetworkManager()
        {
            return _context.GetNetworkManager();
        }

        public IPayManager GetPayManager()
        {
            return _context.GetPayManager();
        }

        public ITaskManager GetTaskManager()
        {
            return _context.GetTaskManager();
        }

        public IDialogManager GetDialogManager()
        {
            return _context.GetDialogManager();
        }

        public IAnalyticManager GetAnalyticManager()
        {
            return _context.GetAnalyticManager();
        }

        public ISystemMsgController GetSystemMsgController()
        {
            return _context.GetSystemMsgController();
        }

        public IWeChatManager GetWeChatManager()
        {
            return _context.GetWeChatManager();
        }

        public ConfigHolder GetConfigHolder()
        {
            return _context.GetConfigHolder();
        }

        public RemoteAPI GetRemoteAPI()
        {
            return _context.GetRemoteAPI();
        }

        public ISoundController GetSoundController()
        {
            return _context.GetSoundController();
        }

        public IosSDK GetIosSDK()
        {
            return _context.GetIosSDK();
        }

        public IPIPLogic GetPIPLogic()
        {
            return _context.GetPIPLogic();
        }

        public ISocketFactory GetSocketFactory()
        {
            return _context.GetSocketFactory();
        }

        public IMessageRouter GetMessageRouter()
        {
            return _context.GetMessageRouter();
        }

        public IMsgRepo GetMsgRepo()
        {
            return _context.GetMsgRepo();
        }

        public XiaoMiManager GetXiaoMiManager()
        {
            return _context.GetXiaoMiManager();
        }

        public IDataRepository GetDataRepository()
        {
            return _context.GetDataRepository();
        }

        public IClipboardManager GetClipboardManager()
        {
            return _context.GetClipboardManager();
        }

        public IResourceManager GetResourceManager()
        {
            return _context.GetResourceManager();
        }

        #endregion
    }
}