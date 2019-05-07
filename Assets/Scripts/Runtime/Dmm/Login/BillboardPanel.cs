using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.DataContainer;
using Dmm.Util;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Login
{
    /// <summary>
    /// 公告栏。
    /// </summary>
    public class BillboardPanel : UIWindow
    {
        #region Inject

        private IDataContainer<bool> _billboardRead;

        private IDataContainer<VersionResult> _versionResult;

        [Inject]
        public void Initialize(IDataRepository dataRepository)
        {
            _billboardRead = dataRepository.GetContainer<bool>(DataKey.BillboardRead);
            _versionResult = dataRepository.GetContainer<VersionResult>(DataKey.VersionResult);
        }

        #endregion

        /// <summary>
        /// 标题。
        /// </summary>
        public Text Title;

        /// <summary>
        /// 内容文字。
        /// </summary>
        public Text ContentTxt;

        /// <summary>
        /// 内容Scroll容器。
        /// </summary>
        public ScrollRect ContentScroll;

        /// <summary>
        /// 内容。
        /// </summary>
        public RectTransform Content;

        public Image BgCover;

        public const string BillboardContentKey = "BillboardContent";

        public const string BillboardTimestampKey = "BillboardTimestamp";

        /*
        public void Update()
        {
            if (_refreshTime < _puData.VersionDataTime)
            {
                _refreshTime = _puData.VersionDataTime;
                RefreshContent();
            }
        }
        */

        private float _refreshTime;

        /// <summary>
        /// 刷新内容其实是刷新是否有新内容这个事情。
        /// </summary>
        private void RefreshContent()
        {
            var version = _versionResult.Read();
            if (version == null)
            {
                return;
            }

            var billboard = version.billboard_6_2;

            if (billboard == null)
            {
                ShowSavedBillboard();
                return;
            }

            var timestamp = PrefsUtil.GetLong(BillboardTimestampKey, 0);
            if (timestamp < billboard.timestamp)
            {
                // 更新公告栏的内容。
                PrefsUtil.SetString(BillboardContentKey, billboard.content);
                PrefsUtil.SetLong(BillboardTimestampKey, billboard.timestamp);
                PrefsUtil.Flush();
            }

            // 显示公告栏。
            ShowSavedBillboard();
        }

        /// <summary>
        /// 显示保存的公告。
        /// </summary>
        private void ShowSavedBillboard()
        {
            var read = _billboardRead.Read();
            var content = PrefsUtil.GetString(BillboardContentKey, null);
            if (!read && !string.IsNullOrEmpty(content))
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void ShowBillboard()
        {
            // 主动显示公告栏，证明玩家，希望看到公告栏。
            _billboardRead.Write(false, Time.time);
            Show();
        }

        public void CloseBillboard()
        {
            // 主动关闭公告栏，意味着玩家已经读过了。
            _billboardRead.Write(true, Time.time);
            Hide();
        }

        #region 显示和隐藏

        private Tweener _tweener;
        private Tweener _bgTweener;

        public float ShowAnimationTime = 0.2f;

        public override void Show()
        {
            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }

            if (Content)
            {
                if (!Content.gameObject.activeSelf)
                    Content.gameObject.SetActive(true);

                Content.localScale = new Vector3(0, 0, 1);
                if (ContentTxt)
                    ContentTxt.text = null;

                _tweener = Content
                    .DOScale(new Vector3(1, 1, 1), ShowAnimationTime)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        if (ContentTxt) ContentTxt.text = PrefsUtil.GetString(BillboardContentKey, "无内容");
                    });
            }

            if (BgCover)
            {
                if (!BgCover.gameObject.activeSelf)
                    BgCover.gameObject.SetActive(true);

                BgCover.color = new Color(0, 0, 0, 0);
            }

            if (BgCover)
            {
                _bgTweener = BgCover
                    .DOFade(150f / 255f, ShowAnimationTime)
                    .SetEase(Ease.Linear);
            }
        }

        public float HideAnimationTime = 0.1f;

        public override void Hide()
        {
            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }

            if (Content && Content.gameObject.activeSelf)
            {
                _tweener = Content
                    .DOScale(new Vector3(0, 0, 1), HideAnimationTime)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => { Destroy(gameObject); });
            }

            if (BgCover && BgCover.gameObject.activeSelf)
            {
                _bgTweener = BgCover
                    .DOFade(0, HideAnimationTime)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => { BgCover.gameObject.SetActive(false); });
            }
        }

        #endregion
    }
}