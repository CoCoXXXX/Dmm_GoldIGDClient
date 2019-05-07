using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Game;
using Dmm.Msg;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.QuickTools
{
    public class ChangeSexDialog : MyDialog
    {
        public Text PriceText;

        public Button ConfirmBtn;

        private IDataContainer<string> _serviceQQ;

        private IDataContainer<ChangeSexResult> _changeSexResult;

        private IDataContainer<User> _myUser;

        private IDataContainer<ActionPriceResult> _actionPriceResult;

        private void OnEnable()
        {
            _serviceQQ = GetContainer<string>(DataKey.ServiceQQ);
            _changeSexResult = GetContainer<ChangeSexResult>(DataKey.ChangeSexResult);
            _myUser = GetContainer<User>(DataKey.MyUser);
            _actionPriceResult = GetContainer<ActionPriceResult>(DataKey.ActionPriceResult);
        }

        public void DoChangeSex()
        {
            GetDialogManager().ShowWaitingDialog(true);

            _changeSexResult.ClearNotInvalidate();
            GetRemoteAPI().ChangeSex();

            GetTaskManager().ExecuteTask(
                CheckChangeSexResult,
                () => GetDialogManager().ShowWaitingDialog(false));
        }

        private bool CheckChangeSexResult()
        {
            var data = _changeSexResult.Read(true);
            if (data == null)
            {
                return false;
            }

            var dialogManager = GetDialogManager();
            dialogManager.ShowWaitingDialog(false);

            if (data.res.code == ResultCode.OK)
            {
                var user = _myUser.Read();
                if (user == null)
                {
                    return false;
                }

                user.sex = data.current_sex;

                var cur = data.current_currency;
                if (cur != null)
                {
                    GameUtil.SetMyCurrency(user, cur.type, cur.count);
                }

                _myUser.Invalidate(Time.time);
                dialogManager.ShowConfirmBox("恭喜您，成功变更性别T_T");

                Hide();
            }
            else
            {
                if (!string.IsNullOrEmpty(data.res.msg))
                {
                    dialogManager.ShowToast(data.res.msg, 2, true);
                }
                else
                {
                    // 如果错误了，且不填写errMsg，则会由服务器端提供错误信息。
                }
            }

            return true;
        }

        public override void BeforeShow()
        {
            PriceText.text = "正在与服务器通信中...";
            ConfirmBtn.interactable = false;

            _actionPriceResult.ClearNotInvalidate();
            GetRemoteAPI().RequestActionPrice(ActionCode.ChangeSex);
            GetTaskManager().ExecuteTask(CheckActionPriceResult, null);
        }

        private ActionPriceResult _price;

        private bool CheckActionPriceResult()
        {
            _price = _actionPriceResult.Read(true);
            if (_price == null)
            {
                return false;
            }

            if (_price.action_code == ActionCode.ChangeSex)
            {
                ConfirmBtn.interactable = true;
                var p = _price.price;
                var ct = p != null ? p.type : CurrencyType.YIN_PIAO;
                var count = p != null ? p.count : 0;

                PriceText.text = string.Format(
                    "只需{0}{1}，即可变更角色性别",
                    count,
                    CurrencyType.LabelOf(ct));
            }
            else
            {
                var serviceQQ = _serviceQQ.Read();
                ConfirmBtn.interactable = false;
                PriceText.text = string.Format(
                    "数据错误，无法变更性别\n请联系客服:{0}",
                    serviceQQ);
            }

            return true;
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}