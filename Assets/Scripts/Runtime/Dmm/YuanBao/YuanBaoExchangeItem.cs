using System;
using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.Clipboard;
using Dmm.Dialog;
using Dmm.Util;
using Dmm.Widget;
using UnityEngine.UI;
using Zenject;

namespace Dmm.YuanBao
{
    public class YuanBaoExchangeItem : Item<YuanBaoExchange>
    {
        #region Inject

        public class Factory : Factory<YuanBaoExchangeItem>
        {
        }

        private IDialogManager _dialogManager;

        private IAnalyticManager _analyticManager;

        private IClipboardManager _clipboardManager;

        [Inject]
        public void Initialize(
            IDialogManager dialogManager,
            IAnalyticManager analyticManager,
            IClipboardManager clipboardManager)
        {
            _dialogManager = dialogManager;
            _analyticManager = analyticManager;
            _clipboardManager = clipboardManager;
        }

        #endregion

        public Text Name;

        public Text Time;

        public Text Price;

        public Text ExchangeCode;

        public Text State;

        public Button DetailBtn;

        private YuanBaoExchange _data;

        public override YuanBaoExchange GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, YuanBaoExchange data)
        {
            _data = data;

            if (data == null)
            {
                Reset(currentIndex);
                return;
            }

            Name.text = data.item_display_name;

            if (data.exchange_time > 0)
            {
                var date = DateUtil.ParseJavaTime(data.exchange_time);
                Time.text = string.Format(
                    "{0}/{1}/{2}\n{3}:{4}",
                    date.Year, date.Month, date.Day,
                    date.Hour, date.Minute);
            }
            else
            {
                Time.text = "";
            }

            Price.text = "" + data.yuan_bao_price;
            ExchangeCode.text = data.exchange_code;
            State.text = data.posted ? "已发货" : "未发货";
            DetailBtn.interactable = !string.IsNullOrEmpty(data.note);
        }

        public void ShowDetail()
        {
            if (_data == null)
                return;

            if (string.IsNullOrEmpty(_data.note))
                return;

            _dialogManager.ShowConfirmBox(_data.note);

            _analyticManager.Event("yuanbao_record_show_detail");
        }

        public override void Reset(int currentIndex)
        {
            Name.text = "";
            Time.text = "";
            Price.text = "";
            ExchangeCode.text = "";
            State.text = "";
            DetailBtn.interactable = false;
        }

        public override void Select(bool selected)
        {
        }

        public override Button GetClickButton()
        {
            return null;
        }

        public void CopyCode()
        {
            var code = _data.exchange_code;
            if (string.IsNullOrEmpty(code))
            {
                _dialogManager.ShowToast("兑换码异常", 3, true);
                return;
            }
            try
            {
                _clipboardManager.CopyToClipboard(code);
            }
            catch (Exception e)
            {
                _dialogManager.ShowToast("复制兑换码失败", 3, true);
                return;
            }

            _dialogManager.ShowToast("已复制兑换码到粘贴板", 3);
        }
    }
}