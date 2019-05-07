using System.Collections;
using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.Analytic;
using Dmm.Common;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Network;
using Dmm.Task;
using Dmm.UI;
using Dmm.Widget;
using Dmm.YuanBao;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Shop
{
    public class ShopPanel : UIWindow
    {
        #region Inject

        private IDialogManager _dialogManager;

        private ITaskManager _taskManager;

        private RemoteAPI _remoteAPI;

        private IUIController _uiController;

        private IAnalyticManager _analyticManager;

        private IDataContainer<User> _myUser;

        private IDataContainer<MyYuanBaoExchangeResult> _myYuanBaoExchangeResult;

        private IDataContainer<FeatureSwitch> _featureSwitch;

        [Inject]
        public void Initialize(
            IUIController uiController,
            IDialogManager dialogManager,
            ITaskManager taskManager,
            IAnalyticManager analyticManager,
            IDataRepository dataRepository,
            RemoteAPI remoteAPI)
        {
            _uiController = uiController;
            _dialogManager = dialogManager;
            _taskManager = taskManager;
            _analyticManager = analyticManager;
            _remoteAPI = remoteAPI;

            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
            _myYuanBaoExchangeResult =
                dataRepository.GetContainer<MyYuanBaoExchangeResult>(DataKey.MyYuanBaoExchangeResult);
            _featureSwitch = dataRepository.GetContainer<FeatureSwitch>(DataKey.FeatureSwitch);
        }

        public class Factory : PrefabFactory<ShopPanel>
        {
        }

        #endregion

        #region Unity方法

        private void Update()
        {
            RefreshMyMoney();
        }

        #endregion

        #region 子组件

        public CommodityTab CommodityPanel;

        public VipTab VipPanel;

        public ChargeTab ChargePanel;

        #endregion

        #region 商店界面的显示和隐藏

        public RectTransform Content;
        public Image BgCover;

        private Tweener _contentTweener;
        private Tweener _bgTweener;

        /// <summary>
        /// 动画执行的时间。
        /// </summary>
        public float ShowAnimationTime = 0.5f;

        /// <summary>
        /// 隐藏动画的时间。
        /// </summary>
        public float HideAnimationTime = 0.1f;

        public override void Show()
        {
            StartCoroutine(ShowCoroutine());
        }

        private IEnumerator ShowCoroutine()
        {
            if (_contentTweener != null)
            {
                _contentTweener.Kill();
                _contentTweener = null;
            }

            if (_bgTweener != null)
            {
                _bgTweener.Kill();
                _bgTweener = null;
            }

            if (Content)
            {
                if (!Content.gameObject.activeSelf)
                    Content.gameObject.SetActive(true);

                Content.localScale = new Vector3(0, 0, 1);
            }

            if (BgCover)

            {
                if (!BgCover.gameObject.activeSelf)
                    BgCover.gameObject.SetActive(true);

                BgCover.color = new Color(0, 0, 0, 0);
            }

            BeforeShow();

            yield return null;

            if (Content)
            {
                _contentTweener = Content
                    .DOScale(new Vector3(1, 1, 1), ShowAnimationTime)
                    .SetEase(Ease.OutBack, 1.1f)
                    .OnComplete(AfterShow);
            }

            if (BgCover)
            {
                _bgTweener = BgCover
                    .DOFade(150f / 255f, ShowAnimationTime)
                    .SetEase(Ease.Linear);
            }
        }

        private void BeforeShow()
        {
            SetTypeHideState(_hideList);
        }

        private void AfterShow()
        {
            SwitchToType(_showType);
        }

        public override void Hide()
        {
            if (_contentTweener != null)
            {
                _contentTweener.Kill();
                _contentTweener = null;
            }

            if (_bgTweener != null)
            {
                _bgTweener.Kill();
                _bgTweener = null;
            }

            if (Content && Content.gameObject.activeSelf)
            {
                _contentTweener = Content
                    .DOScale(new Vector3(0, 0, 1), HideAnimationTime)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        Content.gameObject.SetActive(false);
                        // 销毁商店面板。
                        Destroy(gameObject);
                        // 卸载不用的资源。
                        _uiController.NeedUnloadAsset();
                    });
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

        #region 充值标签页

        public Image ChargeTab;
        public Image ChargeSelect;
        public RectTransform ChargeContent;

        private void EnableChargeTab(bool enable)
        {
            if (ChargeSelect.gameObject.activeSelf != enable)
                ChargeSelect.gameObject.SetActive(enable);

            var enableContent = ChargeTab.gameObject.activeSelf && enable;
            if (ChargeContent.gameObject.activeSelf != enableContent)
                ChargeContent.gameObject.SetActive(enableContent);
        }

        public void ShowChargeTab()
        {
            EnableChargeTab(true);
            EnableCommodityTab(false);
            EnableExchangeTab(false);
            EnableVipTab(false);
            EnableYuanBaoTab(false);
        }

        #endregion

        #region 兑换标签页

        public Image ExchangeTab;
        public Image ExchangeSelect;
        public RectTransform ExchangeContent;

        private void EnableExchangeTab(bool enable)
        {
            if (ExchangeSelect.gameObject.activeSelf != enable)
                ExchangeSelect.gameObject.SetActive(enable);

            var enableContent = ExchangeTab.gameObject.activeSelf && enable;
            if (ExchangeContent.gameObject.activeSelf != enableContent)
                ExchangeContent.gameObject.SetActive(enableContent);
        }

        public void ShowExchangeTab()
        {
            EnableChargeTab(false);
            EnableCommodityTab(false);
            EnableExchangeTab(true);
            EnableVipTab(false);
            EnableYuanBaoTab(false);
        }

        #endregion

        #region 装饰品标签页

        public Image CommodityTab;
        public Image CommoditySelect;
        public RectTransform CommodityContent;

        private void EnableCommodityTab(bool enable)
        {
            if (CommoditySelect.gameObject.activeSelf != enable)
                CommoditySelect.gameObject.SetActive(enable);

            var enableContent = CommodityTab.gameObject.activeSelf && enable;
            if (CommodityContent.gameObject.activeSelf != enableContent)
                CommodityContent.gameObject.SetActive(enableContent);
        }

        public void ShowCommodityTab()
        {
            EnableChargeTab(false);
            EnableCommodityTab(true);
            EnableExchangeTab(false);
            EnableVipTab(false);
            EnableYuanBaoTab(false);
        }

        #endregion

        #region VIP标签页

        public Image VipTab;
        public Image VipSelect;
        public RectTransform VipContent;

        private void EnableVipTab(bool enable)
        {
            if (VipSelect.gameObject.activeSelf != enable)
                VipSelect.gameObject.SetActive(enable);

            var enableContent = VipTab.gameObject.activeSelf && enable;
            if (VipContent.gameObject.activeSelf != enableContent)
                VipContent.gameObject.SetActive(enableContent);
        }

        public void ShowVipTab()
        {
            EnableChargeTab(false);
            EnableCommodityTab(false);
            EnableExchangeTab(false);
            EnableVipTab(true);
            EnableYuanBaoTab(false);
        }

        #endregion

        #region 元宝标签页

        public Image YuanBaoTab;
        public Image YuanBaoSelect;
        public RectTransform YuanBaoContent;

        private void EnableYuanBaoTab(bool enable)
        {
            if (YuanBaoSelect.gameObject.activeSelf != enable)
                YuanBaoSelect.gameObject.SetActive(enable);

            var enableContent = YuanBaoTab.gameObject.activeSelf && enable;
            if (YuanBaoContent.gameObject.activeSelf != enableContent)
                YuanBaoContent.gameObject.SetActive(enableContent);

            ShowMyYuanBao(enable);
        }

        public void ShowYuanBaoTab()
        {
            EnableChargeTab(false);
            EnableCommodityTab(false);
            EnableExchangeTab(false);
            EnableVipTab(false);
            EnableYuanBaoTab(true);
        }

        #endregion

        #region 底部我的信息

        public GameObject MyYinPiaoGroup;
        public GameObject MyGoldEggGroup;
        public GameObject MyYuanBaoGroup;
        public GameObject FreeEggBtn;
        public GameObject MyYuanBaoBtn;

        public Text MyGoldEgg;
        public Text MyYinPiao;
        public Text MyYuanBao;

        private float _myMoneyRefreshTime;

        private void RefreshMyMoney()
        {
            if (_myMoneyRefreshTime >= _myUser.Timestamp)
                return;

            _myMoneyRefreshTime = _myUser.Timestamp;

            var user = _myUser.Read();
            MyGoldEgg.text = user.money + "";
            MyYinPiao.text = user.second_money + "";
            MyYuanBao.text = user.yuan_bao + "";
        }

        private void ShowMyYuanBao(bool show)
        {
            if (MyYuanBaoGroup.activeSelf != show)
                MyYuanBaoGroup.SetActive(show);

            if (MyYinPiaoGroup.activeSelf == show)
                MyYinPiaoGroup.SetActive(!show);

            if (MyGoldEggGroup.activeSelf == show)
                MyGoldEggGroup.SetActive(!show);

            //屏蔽免费金蛋
            FreeEggBtn.SetActive(false);
        }

        #endregion

        #region 商店的类型

        private ShopType _showType;

        private ShopType[] _hideList;

        public void Show(ShopType type, params ShopType[] hideList)
        {
            _showType = type;
            _hideList = hideList;

            Show();
        }

        private void SwitchToType(ShopType type)
        {
            switch (type)
            {
                case ShopType.Charge:
                    ShowChargeTab();
                    break;

                case ShopType.Exchange:
                    ShowExchangeTab();
                    break;

                case ShopType.Commodity:
                    ShowCommodityTab();
                    break;

                case ShopType.Vip:
                    ShowVipTab();
                    break;

                case ShopType.YuanBao:
                    ShowYuanBaoTab();
                    break;
            }
        }

        private readonly ShopType[] _allTypes =
        {
            ShopType.Charge,
            ShopType.Exchange,
            ShopType.Commodity,
            ShopType.Vip,
            ShopType.YuanBao
        };

        /// <summary>
        /// 设置商店类型的隐藏状态。
        /// 隐藏HideList中的商店类型。
        /// 显示非HideList中的商店类型的Tab。
        /// 具体Content是否显示，还要看SwitchToType来决定。
        /// 由Tab作为判断本商店类型是否显示的标准。
        /// </summary>
        /// <param name="hideList"></param>
        public void SetTypeHideState(params ShopType[] hideList)
        {
            for (int i = 0; i < _allTypes.Length; i++)
            {
                var t = _allTypes[i];
                ShowType(t, !Contains(hideList, t));
            }

            var feature = _featureSwitch.Read();
            var vip = feature != null && feature.vip;
            var yuanBao = feature != null && feature.yuanbao;

            if (!vip)
            {
                ShowType(ShopType.Vip, false);
            }

            if (!yuanBao)
            {
                ShowType(ShopType.YuanBao, false);
            }
        }

        private bool Contains(ShopType[] list, ShopType item)
        {
            if (list == null || list.Length <= 0)
                return false;

            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == item)
                    return true;
            }

            return false;
        }

        private void ShowType(ShopType type, bool show)
        {
            Image tab = null;
            RectTransform content = null;
            switch (type)
            {
                case ShopType.Charge:
                    tab = ChargeTab;
                    content = ChargeContent;
                    break;

                case ShopType.Commodity:
                    tab = CommodityTab;
                    content = CommodityContent;
                    break;

                case ShopType.Exchange:
                    tab = ExchangeTab;
                    content = ExchangeContent;
                    break;

                case ShopType.Vip:
                    tab = VipTab;
                    content = VipContent;
                    break;

                case ShopType.YuanBao:
                    tab = YuanBaoTab;
                    content = YuanBaoContent;
                    break;
            }

            if (tab && tab.gameObject.activeSelf != show)
            {
                tab.gameObject.SetActive(show);
            }

            // 不显示的情况下，需要连内容一起隐藏。
            if (!show && content && content.gameObject.activeSelf)
            {
                content.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 商店的类型。
        /// </summary>
        public enum ShopType
        {
            /// <summary>
            /// 充值面板。
            /// </summary>
            Charge,

            /// <summary>
            /// 兑换。
            /// </summary>
            Exchange,

            /// <summary>
            /// 商品。
            /// </summary>
            Commodity,

            /// <summary>
            /// Vip。
            /// </summary>
            Vip,

            /// <summary>
            /// 元宝。
            /// </summary>
            YuanBao
        }

        #endregion

        #region 元宝兑换记录

        public void ShowMyYuanBaoRecordDialog()
        {
            _dialogManager.ShowWaitingDialog(true);

            _myYuanBaoExchangeResult.ClearAndInvalidate(Time.time);
            _taskManager.ExecuteTask(CheckMyYuanBaoExchangeResult, () => _dialogManager.ShowWaitingDialog(false));
            _remoteAPI.RequestMyYuanBaoExchange();

            _analyticManager.Event("yuanbao_record_show");
        }

        private bool CheckMyYuanBaoExchangeResult()
        {
            if (_myYuanBaoExchangeResult.Read() == null)
                return false;

            _dialogManager.ShowWaitingDialog(false);
            var data = _myYuanBaoExchangeResult.Read();
            if (data.res.code == ResultCode.OK)
            {
                var list = data.exchange;
                if (list == null || list.Count <= 0)
                {
                    _dialogManager.ShowToast("没有找到兑奖券兑换数据", 2);
                }
                else
                {
                    _dialogManager.ShowDialog<YuanBaoRecordDialog>(DialogName.YuanBaoRecordDialog);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(data.res.msg))
                {
                    _dialogManager.ShowToast(data.res.msg, 2, true);
                }
            }

            return true;
        }

        #endregion

        #region 规则说明

        public void ShowRuleDescription()
        {
            _dialogManager.ShowDialog<WelfareDescriptionDialog>(DialogName.WelfareDescriptionDialog, true, true);
        }

        #endregion
    }
}