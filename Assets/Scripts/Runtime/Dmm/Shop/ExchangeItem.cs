using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Network;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Shop
{
    public class ExchangeItem : Item<Exchange>
    {
        #region inject

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        private IDataContainer<User> _myUser;

        [Inject]
        public void Initialize(
            IDialogManager dialogManager,
            RemoteAPI remoteAPI,
            IDataRepository dataRepository)
        {
            _dialogManager = dialogManager;
            _remoteAPI = remoteAPI;
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        public class Factory : Factory<ExchangeItem>
        {
        }

        #endregion

        public Image TypeTag;

        public Sprite TypeGeSprite;

        public Sprite TypeCardRecorderSprite;

        public Sprite TypeCheckinCardSprite;

        public CurrencyValue SourceCount;

        public CurrencyValue TargetCount;

        public Text ExchangeRate;

        public Text CurrentCountText;

        private Exchange _data;

        public override Exchange GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, Exchange data)
        {
            _data = data;
            if (data == null)
            {
                Reset(currentIndex);
                return;
            }

            switch (data.target_type)
            {
                case CurrencyType.GOLDEN_EGG:
                    TypeTag.sprite = TypeGeSprite;
                    break;

                case CurrencyType.CARD_RECORDER:
                    TypeTag.sprite = TypeCardRecorderSprite;
                    break;

                case CurrencyType.RECHECKIN_CARD:
                    TypeTag.sprite = TypeCheckinCardSprite;
                    break;
            }

            UpdateSourceAndTargetValue();

            ExchangeRate.text = string.Format(
                "{0}{1}可兑换{2}{3}",
                data.source_amount,
                CurrencyType.LabelOf(data.source_type),
                data.target_amount,
                CurrencyType.LabelOf(data.target_type)
            );

            var myUser = _myUser.Read();

            switch (data.target_type)
            {
                case CurrencyType.GOLDEN_EGG:
                    CurrentCountText.text =
                        string.Format(
                            "您现有  <color=#F9CB51>{0}</color>  金蛋",
                            myUser.MyCurrency(data.target_type)
                        );
                    break;

                case CurrencyType.RECHECKIN_CARD:
                    CurrentCountText.text =
                        string.Format(
                            "您现有补签卡  <color=#F9CB51>{0}</color>  张",
                            myUser.MyCurrency(data.target_type)
                        );
                    break;

                case CurrencyType.CARD_RECORDER:
                    CurrentCountText.text =
                        string.Format(
                            "记牌器有效期  <color=#F9CB51>{0}</color>  天",
                            myUser.CardRecorderLeftDays()
                        );
                    break;

                default:
                    CurrentCountText.text =
                        string.Format(
                            "您现有 <color=#F9CB51>{0}</color>",
                            myUser.MyCurrency(data.target_type)
                        );
                    break;
            }
        }

        public override void Reset(int currentIndex)
        {
            _data = null;
            _exchangeCount = 0;
            UpdateSourceAndTargetValue();

            TypeTag.sprite = TypeGeSprite;
            ExchangeRate.text = "";
            CurrentCountText.text = "";
        }

        public override void Select(bool selected)
        {
        }

        public override Button GetClickButton()
        {
            return null;
        }

        #region 增加减少兑换数量

        /// <summary>
        /// 当前兑换的银票数量。
        /// </summary>
        private long _exchangeCount;

        /// <summary>
        /// 选择兑换所有的银票。
        /// </summary>
        public void ExchangeAll()
        {
            if (_data == null)
                return;

            var myUser = _myUser.Read();
            var currentMoney = myUser.MyCurrency(_data.source_type);
            _exchangeCount = _data.source_amount != 0 ? currentMoney / _data.source_amount : 0;

            UpdateSourceAndTargetValue();
        }

        public void AddExchangeCount()
        {
            IncExchangeCount(1);
        }

        public void MinusExchangeCount()
        {
            IncExchangeCount(-1);
        }

        private void IncExchangeCount(long delta)
        {
            if (!SourceCount || _data == null)
                return;

            _exchangeCount += delta;

            if (_exchangeCount < 0)
            {
                // 如果小于0，则闪红一下。
                _exchangeCount = 0;
                ShakeExchangeCount();
            }

            var myUser = _myUser.Read();
            var curAmount = myUser.MyCurrency(_data.source_type);

            if (_exchangeCount * _data.source_amount > curAmount)
            {
                // 如果超过了最大值，则返回0。
                var currency = CurrencyType.LabelOf(_data.source_type);
                var target = CurrencyType.LabelOf(_data.target_type);
                _dialogManager.ShowToast(string.Format("您的{0}不足，无法兑换更多{1}", currency, target), 2, true);
                return;
            }

            UpdateSourceAndTargetValue();
        }

        /// <summary>
        /// 刷新source和target的显示数量。
        /// </summary>
        private void UpdateSourceAndTargetValue()
        {
            var sourceType = _data != null ? _data.source_type : CurrencyType.YIN_PIAO;
            var sourceAmount = _data != null ? _data.source_amount : 0;
            var targetType = _data != null ? _data.target_type : CurrencyType.GOLDEN_EGG;
            var targetAmount = _data != null ? _data.target_amount : 0;

            SourceCount.SetCurrency(_exchangeCount * sourceAmount, sourceType);
            TargetCount.SetCurrency(_exchangeCount * targetAmount, targetType);
        }

        public float ShakeAnimationTime = 0.1f;

        public float ShakeStrength = 0.4f;

        private Tweener _shakeTweener;

        private void ShakeExchangeCount()
        {
            if (_shakeTweener != null)
            {
                _shakeTweener.Kill();
                _shakeTweener = null;
            }

            SourceCount.transform.localScale = new Vector3(1, 1, 1);
            _shakeTweener = SourceCount.transform
                .DOShakeScale(ShakeAnimationTime, ShakeStrength)
                .OnStart(() =>
                {
                    if (SourceCount.AmountTxt)
                        SourceCount.AmountTxt.color = new Color(1, 0, 0, 1);
                })
                .OnComplete(() =>
                {
                    if (SourceCount.AmountTxt)
                        SourceCount.AmountTxt.color = new Color(1, 1, 1, 1);
                });
        }

        /// <summary>
        /// 实际执行兑换逻辑。
        /// </summary>
        public void DoExchange()
        {
            if (_exchangeCount <= 0)
            {
                _dialogManager.ShowToast("请输入需要兑换的数量", 2);
                return;
            }

            if (_data != null)
                // 向服务器端发送兑换的请求。
                _remoteAPI.Exchange(_data.name, _exchangeCount);
        }

        #endregion

        #region 输入兑换数量

        public void ShowInputExchangeCountDialog()
        {
            if (_data == null)
            {
                return;
            }

            _dialogManager.ShowDialog<InputExchangeCountDialog>(DialogName.InputExchangeCountDialog, false, false,
                (dialog) =>
                {
                    dialog.ApplyData(_data, OnConfirmExchangeCount);
                    dialog.Show();
                });
        }

        public void OnConfirmExchangeCount(long count)
        {
            if (_data == null)
            {
                return;
            }

            _exchangeCount = count / _data.target_amount;
            UpdateSourceAndTargetValue();
        }

        #endregion
    }
}