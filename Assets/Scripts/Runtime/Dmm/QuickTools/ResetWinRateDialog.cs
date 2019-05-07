using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine.UI;

namespace Dmm.QuickTools
{
    public class ResetWinRateDialog : MyDialog
    {
        public Text CostTip;

        public Button ConfirmBtn;

        private ActionPriceResult _price;

        private IDataContainer<string> _serviceQQ;

        private IDataContainer<ActionPriceResult> _actionPriceResult;

        private IDataContainer<ResetWinRateResult> _resetWinRateResult;

        private void OnEnable()
        {
            _serviceQQ = GetContainer<string>(DataKey.ServiceQQ);
            _actionPriceResult = GetContainer<ActionPriceResult>(DataKey.ActionPriceResult);
            _resetWinRateResult = GetContainer<ResetWinRateResult>(DataKey.ResetWinRateResult);
        }

        public override void BeforeShow()
        {
            CostTip.text = "正在与服务器通信中...";
            ConfirmBtn.interactable = false;

            _price = null;

            _actionPriceResult.ClearNotInvalidate();
            GetRemoteAPI().RequestActionPrice(ActionCode.ResetWinRate);
            GetTaskManager().ExecuteTask(CheckActionPriceResult, null);
        }

        private bool CheckActionPriceResult()
        {
            _price = _actionPriceResult.Read(true);
            if (_price == null)
            {
                return false;
            }

            if (_price.action_code == ActionCode.ResetWinRate)
            {
                ConfirmBtn.interactable = true;
                var p = _price.price;
                var ct = p != null ? p.type : CurrencyType.YIN_PIAO;
                var count = p != null ? p.count : 0;

                CostTip.text = string.Format(
                    "只需{0}{1}，即可将胜率恢复成50%",
                    count,
                    CurrencyType.LabelOf(ct));
            }
            else
            {
                var serviceQQ = _serviceQQ.Read();
                ConfirmBtn.interactable = false;
                CostTip.text = string.Format(
                    "数据错误，无法恢复胜率\n请联系客服:{0}",
                    serviceQQ);
            }

            return true;
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }

        public void DoResetWinRate()
        {
            GetDialogManager().ShowWaitingDialog(true);

            _resetWinRateResult.ClearNotInvalidate();
            GetRemoteAPI().ResetWinRate();

            GetTaskManager().ExecuteTask(
                CheckResetWinRateResult,
                () => GetDialogManager().ShowWaitingDialog(false));

            GetAnalyticManager().Event("reset_winrate_dialog_apply");
        }

        private bool CheckResetWinRateResult()
        {
            var res = _resetWinRateResult.Read(true);
            if (res == null)
            {
                return false;
            }

            var dialogManager = GetDialogManager();
            dialogManager.ShowWaitingDialog(false);

            if (res.result == ResultCode.OK)
            {
                dialogManager.ShowConfirmBox("恭喜您，重置胜率成功！");
                GetRemoteAPI().RequestUserInfo();

                var count = DataUtil.CalculateGeValue(res.cost);
                GetAnalyticManager().Buy("reset_winrate", 1, count);

                Hide();
            }
            else
            {
                if (!string.IsNullOrEmpty(res.msg))
                {
                    dialogManager.ShowToast(res.msg, 3, true);
                }
            }

            return true;
        }
    }
}