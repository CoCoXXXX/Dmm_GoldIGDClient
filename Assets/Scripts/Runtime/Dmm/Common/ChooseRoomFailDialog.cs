using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Dialog;
using Dmm.Pay;
using Dmm.Shop;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class ChooseRoomFailDialog : MyDialog
    {
        public Text Title;

        public Image Image;

        public Text Text;

        public Button ChargeBtn;

        public Button VipBtn;

        public Button SwitchRoomBtn;

        private ChooseRoomFail _data;

        public void ApplyData(ChooseRoomFail data)
        {
            _data = data;

            if (data == null)
            {
                // TODO 如果光是选房失败，其他什么都没有的时候，显示什么？
                return;
            }

            if (Text)
            {
                if (!Text.gameObject.activeSelf)
                    Text.gameObject.SetActive(true);

                Text.text = data.description;
            }

            var charge = data.code == ChooseRoomFailType.NEED_GE ||
                         data.code == ChooseRoomFailType.NEED_YP;

            if (ChargeBtn && ChargeBtn.gameObject.activeSelf != charge)
                ChargeBtn.gameObject.SetActive(charge);

            var vip = data.code == ChooseRoomFailType.VIP_LOW;
            if (VipBtn && VipBtn.gameObject.activeSelf != vip)
                VipBtn.gameObject.SetActive(vip);

            var switchRoom = data.code == ChooseRoomFailType.MONEY_OVERFLOW;
            if (SwitchRoomBtn && SwitchRoomBtn.gameObject.activeSelf != switchRoom)
                SwitchRoomBtn.gameObject.SetActive(switchRoom);

            var analyticManager = GetAnalyticManager();
            // 统计显示的选房失败的原因。
            switch (data.code)
            {
                case ChooseRoomFailType.NEED_GE:
                    analyticManager.Event("choose_room_fail_need_ge");
                    break;

                case ChooseRoomFailType.NEED_YP:
                    analyticManager.Event("choose_room_fail_need_yp");
                    break;

                case ChooseRoomFailType.VIP_LOW:
                    analyticManager.Event("choose_room_fail_vip_low");
                    break;

                case ChooseRoomFailType.MONEY_OVERFLOW:
                    analyticManager.Event("choose_room_fail_money_overflow");
                    break;

                default:
                    analyticManager.Event("choose_room_fail");
                    break;
            }
        }

        public void ExchangeVip()
        {
            var analyticManager = GetAnalyticManager();
            var dialogManager = GetDialogManager();
            analyticManager.Event("choose_room_fail_vip_go");
            dialogManager.ShowDialog<ShopPanel>(DialogName.ShopPanel, false, false,
                (shop) => { shop.Show(ShopPanel.ShopType.Vip); });

            Hide();
        }

        public void Charge()
        {
            var analyticManager = GetAnalyticManager();
            var dialogManager = GetDialogManager();
            analyticManager.Event("choose_room_fail_charge_go");

            if (_data != null && _data.prepayment != null)
            {
                dialogManager.ShowDialog<PayChannelPanel>(DialogName.PayChannelPanel, false, false,
                    (dialog) =>
                    {
                        dialog.ApplyData("choose_room_fail", _data.prepayment);
                        dialog.Show();
                    });
            }
            else
            {
                dialogManager.ShowMessageBox("没有找到支付数据！");
            }

            Hide();
        }

        public void SwitchRoom()
        {
            var analyticManager = GetAnalyticManager();
            analyticManager.Event("choose_room_fail_switch_room");

            if (_data != null && _data.suitable_room_id != -1)
            {
                var remoteAPI = GetRemoteAPI();
                remoteAPI.ChooseRoom((int) _data.suitable_room_id);
            }

            Hide();
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}