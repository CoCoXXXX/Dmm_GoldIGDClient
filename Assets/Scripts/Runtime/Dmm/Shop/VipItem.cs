using System.Collections.Generic;
using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.Constant;
using Dmm.Network;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Shop
{
    public class VipItem : Item<VipExchange>
    {
        #region inject

        private RemoteAPI _remoteAPI;

        [Inject]
        public void Initialize(RemoteAPI remoteAPI)
        {
            _remoteAPI = remoteAPI;
        }

        public class Factory : Factory<VipItem>
        {
        }

        #endregion

        public List<Sprite> VipNameList;

        public LayoutElement Layout;

        public float FoldHeight = 58;

        public float ExpandHeight = 120;

        public Image NameImg;

        public Image ExtendIndicator;

        public Image NoNeedBuy;

        private void EnableNoNeedBuy(bool enable)
        {
            if (NoNeedBuy && NoNeedBuy.gameObject.activeSelf != enable)
                NoNeedBuy.gameObject.SetActive(enable);
        }

        public Button ContinueBtn;

        private void EnableContinueBtn(bool enable)
        {
            if (ContinueBtn && ContinueBtn.gameObject.activeSelf != enable)
                ContinueBtn.gameObject.SetActive(enable);
        }

        public Button BuyBtn;

        private void EnableBuyBtn(bool enable)
        {
            if (BuyBtn && BuyBtn.gameObject.activeSelf != enable)
                BuyBtn.gameObject.SetActive(enable);
        }

        public Button UpgradeBtn;

        private void EnableUpgradeBtn(bool enable)
        {
            if (UpgradeBtn && UpgradeBtn.gameObject.activeSelf != enable)
                UpgradeBtn.gameObject.SetActive(enable);
        }

        public CurrencyValue Price;

        private void EnablePrice(bool enable, Currency currency = null)
        {
            if (!Price) return;

            if (Price.gameObject.activeSelf != enable)
                Price.gameObject.SetActive(enable);

            if (enable)
            {
                if (currency != null)
                    Price.SetCurrency(currency.count, currency.type);
                else
                    Price.Clear();
            }
        }

        public GameObject DescriptionGroup;

        public Text Description;

        private void EnableDescription(bool enable, string content = null)
        {
            if (!DescriptionGroup)
                return;

            if (DescriptionGroup.activeSelf != enable)
                DescriptionGroup.SetActive(enable);

            if (enable && Description)
                Description.text = content;
        }

        public Button Button;

        private VipExchange _data;

        public override VipExchange GetData()
        {
            return _data;
        }

        private bool _expand;

        public override void BindData(int currentIndex, VipExchange data)
        {
            _data = data;

            if (data != null)
            {
                if (NameImg)
                {
                    if (VipNameList != null && data.target_level >= 1 && data.target_level <= VipNameList.Count)
                    {
                        if (!NameImg.gameObject.activeSelf)
                            NameImg.gameObject.SetActive(true);

                        NameImg.sprite = VipNameList[data.target_level - 1];
                    }
                    else
                    {
                        if (NameImg.gameObject.activeSelf)
                            NameImg.gameObject.SetActive(false);
                    }
                }

                if (ExtendIndicator)
                {
                    if (!ExtendIndicator.gameObject.activeSelf)
                        ExtendIndicator.gameObject.SetActive(true);

                    ExtendIndicator.rectTransform.localRotation = Quaternion.Euler(0, 0, _expand ? 90 : 0);
                }

                SetExchangeState(data, _expand);

                if (Layout)
                    Layout.preferredHeight = _expand ? ExpandHeight : FoldHeight;

                if (_expand)
                    EnableDescription(true, data.description);
                else
                    EnableDescription(false);
            }
            else
            {
                Reset(currentIndex);
            }
        }

        private void SetExchangeState(VipExchange data, bool expand)
        {
            if (data == null)
                return;

            switch (data.type)
            {
                case VipExchangeType.CanNotBuy:
                    EnableNoNeedBuy(true);
                    EnablePrice(false);
                    EnableBuyBtn(false);
                    EnableContinueBtn(false);
                    EnableUpgradeBtn(false);
                    break;

                case VipExchangeType.BuyOk:
                    EnableNoNeedBuy(false);
                    EnablePrice(true, data.price);
                    EnableBuyBtn(expand);
                    EnableContinueBtn(false);
                    EnableUpgradeBtn(false);
                    break;

                case VipExchangeType.Renew:
                    EnableNoNeedBuy(false);
                    EnablePrice(true, data.price);
                    EnableBuyBtn(false);
                    EnableContinueBtn(expand);
                    EnableUpgradeBtn(false);
                    break;

                case VipExchangeType.Upgrade:
                    EnableNoNeedBuy(false);
                    EnablePrice(true, data.price);
                    EnableBuyBtn(false);
                    EnableContinueBtn(false);
                    EnableUpgradeBtn(expand);
                    break;
            }

            EnableDescription(expand, expand ? data.description : null);
        }

        public override void Reset(int currentIndex)
        {
            if (Layout)
                Layout.preferredHeight = FoldHeight;

            if (NameImg && NameImg.gameObject.activeSelf)
                NameImg.gameObject.SetActive(false);

            if (ExtendIndicator && ExtendIndicator.gameObject.activeSelf)
                ExtendIndicator.gameObject.SetActive(false);

            EnableNoNeedBuy(false);
            EnableDescription(false);
            EnablePrice(false);
            EnableBuyBtn(false);
            EnableContinueBtn(false);
            EnableUpgradeBtn(false);
        }

        public float ExpandTime = 0.2f;

        private Tweener _expandTweener;

        private Tweener _indicatorTweener;

        public override void Select(bool selected)
        {
            // 非选中的时候，什么都不做。
            if (!selected) return;

            if (_expandTweener != null)
            {
                _expandTweener.Kill();
                _expandTweener = null;
            }

            if (_indicatorTweener != null)
            {
                _indicatorTweener.Kill();
                _indicatorTweener = null;
            }

            if (_expand)
            {
                _expand = false;

                SetExchangeState(_data, false);

                if (Layout)
                    _expandTweener = Layout
                        .DOPreferredSize(new Vector2(Layout.preferredWidth, FoldHeight), ExpandTime);

                if (ExtendIndicator)
                {
                    if (!ExtendIndicator.gameObject.activeSelf)
                        ExtendIndicator.gameObject.SetActive(true);

                    _indicatorTweener = ExtendIndicator.rectTransform.DORotate(new Vector3(0, 0, 0), ExpandTime);
                }
            }
            else
            {
                _expand = true;

                SetExchangeState(_data, true);

                if (Layout)
                    _expandTweener =
                        Layout.DOPreferredSize(new Vector2(Layout.preferredWidth, ExpandHeight), ExpandTime);

                if (ExtendIndicator)
                {
                    if (!ExtendIndicator.gameObject.activeSelf)
                        ExtendIndicator.gameObject.SetActive(true);

                    _indicatorTweener = ExtendIndicator.rectTransform.DORotate(new Vector3(0, 0, -90), ExpandTime);
                }
            }
        }

        public override Button GetClickButton()
        {
            return Button;
        }

        public void RequestExchangeVip()
        {
            if (_data != null)
                _remoteAPI.RequestExchangeVip(_data);
        }
    }
}