using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Game;
using Dmm.Msg;
using Dmm.Network;
using Dmm.Res;
using Dmm.Sound;
using Dmm.Task;
using Dmm.Util;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Shop
{
    public class CommodityTab : MonoBehaviour
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private ISoundController _soundController;

        private IDialogManager _dialogManager;

        private ITaskManager _taskManager;

        private IAnalyticManager _analyticManager;

        private IDataContainer<Bag> _bag;

        private IDataContainer<User> _user;

        private IDataContainer<List<Commodity>> _commodityList;

        private IDataContainer<BuyCommodityResult> _buyCommodityResult;

        private IDataContainer<UseCommodityResult> _useCommodityResult;


        [Inject]
        public void Initialize(
            RemoteAPI remoteAPI,
            ISoundController soundController,
            IDialogManager dialogManager,
            ITaskManager taskManager,
            IDataRepository dataRepository,
            IAnalyticManager analyticManager)
        {
            _remoteAPI = remoteAPI;
            _soundController = soundController;
            _dialogManager = dialogManager;
            _taskManager = taskManager;
            _analyticManager = analyticManager;

            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
            _bag = dataRepository.GetContainer<Bag>(DataKey.MyBag);
            _commodityList = dataRepository.GetContainer<List<Commodity>>(DataKey.CommodityList);
            _buyCommodityResult = dataRepository.GetContainer<BuyCommodityResult>(DataKey.BuyCommodityResult);
            _useCommodityResult = dataRepository.GetContainer<UseCommodityResult>(DataKey.UseCommodityResult);
        }

        #endregion

        #region Unity方法

        private void OnEnable()
        {
            SwitchToHairTab();
        }

        public void Update()
        {
            RefreshContent();
        }

        #endregion

        private float _refreshTime;

        private void RefreshContent()
        {
            if (_refreshTime >= _bag.Timestamp)
            {
                return;
            }

            _refreshTime = _bag.Timestamp;

            SetDetailContent(_currentSelected);
        }

        public CommodityList CommodityList;

        private Commodity _currentSelected;

        #region 商品类型

        public Image HairTabSelected;

        public Image BodyTabSelected;

        public Image DeskItemTabSelected;

        #endregion

        #region 商品详细信息

        public GameObject VIPTag;

        public AsyncImage DetailBgImage;

        public AsyncImage DetailImage;

        public Text DetailName;

        public CurrencyValue DetailPrice;

        public Button BuyBtn;

        public Button UseBtn;

        public Button UnUseBtn;

        public Button SaleBtn;

        public Text SaleTip;

        public void SelectCommodity(Commodity data)
        {
            _currentSelected = data;
            SetDetailContent(data);
        }

        private void SetDetailContent(Commodity data)
        {
            if (data != null)
            {
                var vip = data.vip_level > 0;
                if (VIPTag.activeSelf != vip)
                {
                    VIPTag.SetActive(vip);
                }

                if (DetailImage && !DetailImage.gameObject.activeSelf)
                {
                    DetailImage.gameObject.SetActive(true);
                }

                if (DetailBgImage && DetailBgImage.gameObject.activeSelf)
                {
                    DetailBgImage.gameObject.SetActive(true);
                }

                var pic = data.pic;
                if (DetailImage)
                {
                    if (!string.IsNullOrEmpty(pic))
                    {
                        DetailImage.SetTargetPic(pic, ResourcePath.CommodityPath, null, true);
                    }
                    else
                    {
                        DetailImage.Reset();
                    }
                }

                if (DetailBgImage)
                {
                    var picBg = data.pic_bg;
                    if (!string.IsNullOrEmpty(picBg))
                    {
                        DetailBgImage.SetTargetPic(picBg, ResourcePath.CommodityPath, null, true);
                    }
                    else
                    {
                        DetailBgImage.Reset();
                    }
                }

                if (DetailName)
                {
                    if (!DetailName.gameObject.activeSelf)
                    {
                        DetailName.gameObject.SetActive(true);
                    }

                    DetailName.text = data.display_name;
                }

                if (DetailPrice)
                {
                    if (!DetailPrice.gameObject.activeSelf)
                    {
                        DetailPrice.gameObject.SetActive(true);
                    }

                    DetailPrice.SetCurrency(CommodityHelper.GetPrice(data), CommodityHelper.GetCurrencyType(data));
                }

                var bag = _bag.Read();
                var user = _user.Read();
                var hasCommodity = GameUtil.HasCommodity(bag, data);
                var equiped = hasCommodity && GameUtil.IsCommodityEquiped(bag, user, data);

                if (hasCommodity)
                {
                    // 隐藏购买按钮。
                    if (BuyBtn && BuyBtn.gameObject.activeSelf)
                    {
                        BuyBtn.gameObject.SetActive(false);
                    }

                    // 使用按钮与是否装备相反。
                    if (UseBtn)
                    {
                        if (!UseBtn.gameObject.activeSelf)
                        {
                            UseBtn.gameObject.SetActive(true);
                        }

                        UseBtn.interactable = !equiped;
                    }

                    if (UnUseBtn)
                    {
                        if (!UnUseBtn.gameObject.activeSelf)
                        {
                            UnUseBtn.gameObject.SetActive(true);
                        }

                        UnUseBtn.interactable = equiped;
                    }

                    // 6.2版本不再显示典当按钮。
                    if (SaleBtn && SaleBtn.gameObject.activeSelf)
                    {
                        SaleBtn.gameObject.SetActive(false);
                    }

                    // 6.2版本不再显示典当提示。
                    if (SaleTip && SaleTip.gameObject.activeSelf)
                    {
                        SaleTip.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (BuyBtn && !BuyBtn.gameObject.activeSelf)
                    {
                        BuyBtn.gameObject.SetActive(true);
                    }

                    if (UseBtn && UseBtn.gameObject.activeSelf)
                    {
                        UseBtn.gameObject.SetActive(false);
                    }

                    if (UnUseBtn && UnUseBtn.gameObject.activeSelf)
                    {
                        UnUseBtn.gameObject.SetActive(false);
                    }

                    if (SaleBtn && SaleBtn.gameObject.activeSelf)
                    {
                        SaleBtn.gameObject.SetActive(false);
                    }

                    if (SaleTip && SaleTip.gameObject.activeSelf)
                    {
                        SaleTip.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (DetailBgImage)
                {
                    DetailBgImage.Reset();
                }

                if (DetailImage)
                {
                    DetailImage.Reset();
                }

                if (DetailName && DetailName.gameObject.activeSelf)
                {
                    DetailName.gameObject.SetActive(false);
                }

                if (DetailPrice && DetailPrice.gameObject.activeSelf)
                {
                    DetailPrice.gameObject.SetActive(false);
                }

                if (BuyBtn && BuyBtn.gameObject.activeSelf)
                {
                    BuyBtn.gameObject.SetActive(false);
                }

                if (UseBtn && UseBtn.gameObject.activeSelf)
                {
                    UseBtn.gameObject.SetActive(false);
                }

                if (UnUseBtn && UnUseBtn.gameObject.activeSelf)
                {
                    UnUseBtn.gameObject.SetActive(false);
                }

                if (SaleBtn && SaleBtn.gameObject.activeSelf)
                {
                    SaleBtn.gameObject.SetActive(false);
                }

                if (SaleTip && SaleTip.gameObject.activeSelf)
                {
                    SaleTip.gameObject.SetActive(false);
                }
            }
        }

        #endregion

        #region 切换商品列表

        public void SwitchToHairTab()
        {
            if (HairTabSelected && !HairTabSelected.gameObject.activeSelf)
            {
                HairTabSelected.gameObject.SetActive(true);
            }

            if (BodyTabSelected && BodyTabSelected.gameObject.activeSelf)
            {
                BodyTabSelected.gameObject.SetActive(false);
            }

            if (DeskItemTabSelected && DeskItemTabSelected.gameObject.activeSelf)
            {
                DeskItemTabSelected.gameObject.SetActive(false);
            }

            if (CommodityList)
            {
                CommodityList.CommodityType = CommodityType.Hair;
                CommodityList.RefreshContent();
            }
        }

        public void SwitchToBodyTab()
        {
            if (BodyTabSelected && !BodyTabSelected.gameObject.activeSelf)
            {
                BodyTabSelected.gameObject.SetActive(true);
            }

            if (HairTabSelected && HairTabSelected.gameObject.activeSelf)
            {
                HairTabSelected.gameObject.SetActive(false);
            }

            if (DeskItemTabSelected && DeskItemTabSelected.gameObject.activeSelf)
            {
                DeskItemTabSelected.gameObject.SetActive(false);
            }

            if (CommodityList)
            {
                CommodityList.CommodityType = CommodityType.Body;
                CommodityList.RefreshContent();
            }
        }

        public void SwitchToDeskItemTab()
        {
            if (DeskItemTabSelected && !DeskItemTabSelected.gameObject.activeSelf)
            {
                DeskItemTabSelected.gameObject.SetActive(true);
            }

            if (BodyTabSelected && BodyTabSelected.gameObject.activeSelf)
            {
                BodyTabSelected.gameObject.SetActive(false);
            }

            if (HairTabSelected && HairTabSelected.gameObject.activeSelf)
            {
                HairTabSelected.gameObject.SetActive(false);
            }

            if (CommodityList)
            {
                CommodityList.CommodityType = CommodityType.DeskItem;
                CommodityList.RefreshContent();
            }
        }

        #endregion

        #region 购买商品

        public void BuyCommodity()
        {
            _dialogManager.ShowWaitingDialog(true);

            _buyCommodityResult.ClearAndInvalidate(Time.time);
            _remoteAPI.BuyCommodity(_currentSelected);

            _taskManager.ExecuteTask(
                CheckBuyCommodityResult,
                () => _dialogManager.ShowWaitingDialog(false));
        }

        private bool CheckBuyCommodityResult()
        {
            var res = _buyCommodityResult.Read();
            if (res == null)
                return false;

            _dialogManager.ShowWaitingDialog(false);

            if (res.result == ResultCode.OK)
            {
                // 购买成功。
                AddCommodity(res.name);

                // 更新玩家的钱。
                var user = _user.Read();
                GameUtil.SetMyCurrency(user, CurrencyType.GOLDEN_EGG, res.current_money);
                GameUtil.SetMyCurrency(user, CurrencyType.YIN_PIAO, res.current_second_money);
                _user.Invalidate(Time.time);

                // 播放用钱的声音。
                _soundController.PlayUseGoldSound();
                _dialogManager.ShowConfirmBox(
                    "恭喜您，成功购买商品^_^",
                    true, "立即使用", () => UseCommodity(res.name, true),
                    false, null, null,
                    true, true, true);

                // 统计
                var commodityList = _commodityList.Read();
                var commodity = GameUtil.GetCommodity(commodityList, res.name);
                if (commodity != null)
                {
                    var type = CommodityHelper.GetCurrencyType(commodity);
                    var price = CommodityHelper.GetPrice(commodity);
                    var count = DataUtil.CalculateGeValue(type, price);
                    _analyticManager.Buy(res.name, 1, count);
                }
            }
            else
            {
                // 购买失败，显示错误信息。
                switch (res.result)
                {
                    case ResultCode.COMMODITY_NOT_FOUND:
                        _dialogManager.ShowToast("购买失败，商品不存在！", 2, true);
                        break;

                    case ResultCode.COMMODITY_BUY_LEVEL_LIMIT:
                        _dialogManager.ShowToast("购买失败，您的等级不够！", 2, true);
                        break;

                    case ResultCode.COMMODITY_BUY_VIP_LIMIT:
                        _dialogManager.ShowToast("购买失败，您的VIP等级不够！", 2, true);
                        break;

                    case ResultCode.COMMODITY_BUY_MONEY_LIMIT:
                        _dialogManager.ShowToast("购买失败，您的钱不够！", 2, true);
                        break;

                    case ResultCode.CURRENCY_NOT_SUPPORTED:
                        _dialogManager.ShowToast("购买失败，商品数据错误！", 2, true);
                        break;

                    // 默认情况下等待服务器端发送的Toast。
                }
            }

            // 读取过结果之后，就可以清空这个结果了。
            _buyCommodityResult.ClearAndInvalidate(Time.time);
            return true;
        }

        public void AddCommodity(string cname)
        {
            var bag = _bag.Read();
            if (bag == null)
                return;

            if (!GameUtil.HasCommodity(bag, cname))
            {
                var item = new BagItem();
                item.name = cname;
                item.count = 1;
                item.level = 1;
                item.state = 0;
                bag.item.Add(item);

                _bag.Invalidate(Time.time);
            }
        }

        #endregion

        #region 使用商品

        public void UseCommodity(bool use)
        {
            if (_currentSelected != null)
            {
                UseCommodity(_currentSelected.name, use);
            }
        }

        private void UseCommodity(string cname, bool use)
        {
            if (string.IsNullOrEmpty(cname))
                return;

            _dialogManager.ShowWaitingDialog(true);

            _useCommodityResult.ClearAndInvalidate(Time.time);
            _taskManager.ExecuteTask(
                CheckUseCommodityResult,
                () => _dialogManager.ShowWaitingDialog(false));

            _remoteAPI.UseCommodity(cname, use);
        }

        private bool CheckUseCommodityResult()
        {
            var res = _useCommodityResult.Read();
            if (res == null)
                return false;

            _dialogManager.ShowWaitingDialog(false);

            if (res.result == ResultCode.OK)
            {
                // 使用成功。
                UseCommodityFromBag(res.cname, res.use_or_not == 1);
            }
            else
            {
                var use = res.use_or_not == 1;
                // 使用失败，提示玩家。
                switch (res.result)
                {
                    case ResultCode.COMMODITY_NOT_FOUND:
                        _dialogManager.ShowToast(string.Format("{0}商品失败！", use ? "使用" : "卸下"), 2, true);
                        break;

                    case ResultCode.COMMODITY_VALIDITY_EXPIRED:
                        _dialogManager.ShowToast(string.Format("您的商品已经过期，无法{0}！", use ? "使用" : "卸下"), 2, true);
                        break;

                    case ResultCode.COMMODITY_ALREADY_IN_USE:
                        // 返回这个错误，证明客户端的状态是错的，因此需要纠正一下客户端的状态。
                        UseCommodityFromBag(res.cname, true);
                        break;

                    case ResultCode.COMMODITY_ALREADY_NOT_USE:
                        // 返回这个错误，证明客户端的状态是错的，因此需要纠正一下客户端的状态。
                        UseCommodityFromBag(res.cname, false);
                        break;

                    default:
                        _dialogManager.ShowToast(string.Format("{0}商品失败！", use ? "使用" : "卸下"), 2, true);
                        break;
                }
            }

            _useCommodityResult.ClearAndInvalidate(Time.time);
            return true;
        }

        public void UseCommodityFromBag(string cname, bool use)
        {
            var user = _user.Read();
            var commodityList = _commodityList.Read();
            DataUtil.UseCommodity(user, GameUtil.GetCommodity(commodityList, cname), use);
            _user.Invalidate(Time.time);
            _bag.Invalidate(Time.time);
        }

        #endregion
    }
}