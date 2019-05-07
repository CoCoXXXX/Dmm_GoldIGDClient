using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Pay;
using Dmm.Shop;
using Dmm.Task;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class PushItemDialog : MyDialog
    {
        public Text ContentTxt;

        public Button ChargeBtn;

        public Button VipBtn;

        public Button ExchangeBtn;

        public Button ActivityBtn;

        private PushItem _data;

        public void ApplyData(PushItem data)
        {
            _data = data;

            if (data != null)
            {
                if (!ContentTxt.gameObject.activeSelf)
                    ContentTxt.gameObject.SetActive(true);

                ContentTxt.text = data.content;

                var prepayment = data.type == PushItemType.Prepayment;
                var vip = data.type == PushItemType.Vip;
                var exchange = data.type == PushItemType.RecheckinCard || data.type == PushItemType.CardRecorder;
                var activity = data.type == PushItemType.UserTask;

                var isEnableVip = false;
                var featureSwitch = GetDataRepository().GetContainer<FeatureSwitch>(DataKey.FeatureSwitch).Read();
                if (featureSwitch == null)
                {
                    isEnableVip = false;
                }
                else
                {
                    isEnableVip = featureSwitch.vip;
                }

                if (!isEnableVip)
                {
                    vip = false;
                }

                if (ChargeBtn.gameObject.activeSelf != prepayment)
                {
                    ChargeBtn.gameObject.SetActive(prepayment);
                }

                if (VipBtn.gameObject.activeSelf != vip)
                {
                    VipBtn.gameObject.SetActive(vip);
                }

                if (ExchangeBtn.gameObject.activeSelf != exchange)
                {
                    ExchangeBtn.gameObject.SetActive(exchange);
                }

                if (ActivityBtn.gameObject.activeSelf != activity)
                {
                    ActivityBtn.gameObject.SetActive(activity);
                }

                // 统计。
                var eventId = string.Format("pushitem_{0}_{1}_show", PushItemType.IdOf(data.type), data.code);
                Dictionary<string, string> attrs = null;
                if (prepayment)
                {
                    attrs = new Dictionary<string, string>();
                    var p = data.prepayment;
                    attrs.Add("prepayment", p != null ? p.name : "NULL");
                }

                var analyticManager = GetAnalyticManager();
                if (attrs != null)
                {
                    analyticManager.Event(eventId, attrs);
                }
                else
                {
                    analyticManager.Event(eventId);
                }
            }
            else
            {
                if (ContentTxt.gameObject.activeSelf)
                {
                    ContentTxt.gameObject.SetActive(false);
                }

                if (ChargeBtn.gameObject.activeSelf)
                {
                    ChargeBtn.gameObject.SetActive(false);
                }

                if (VipBtn.gameObject.activeSelf)
                {
                    VipBtn.gameObject.SetActive(false);
                }
            }
        }

        public void DoCharge()
        {
            var dialogManager = GetDialogManager();
            var p = _data != null ? _data.prepayment : null;
            if (p != null)
            {
                dialogManager.ShowDialog<PayChannelPanel>(DialogName.PayChannelPanel, false, false,
                    (dialog) =>
                    {
                        dialog.ApplyData(string.Format("pushitem_{0}", _data != null ? _data.code : ""), p);
                        dialog.Show();
                    });
            }
            else
            {
                dialogManager.ShowToast("发生错误，没有支付包", 2, true);
            }

            Hide();
        }

        public void DoExchangeVip()
        {
            var dialogManager = GetDialogManager();
            dialogManager.ShowDialog<ShopPanel>(DialogName.ShopPanel, false, false,
                (shop) => { shop.Show(ShopPanel.ShopType.Vip); });

            Hide();
        }

        public void DoExchange()
        {
            var dialogManager = GetDialogManager();
            dialogManager.ShowDialog<ShopPanel>(DialogName.ShopPanel, false, false,
                (shop) => { shop.Show(ShopPanel.ShopType.Exchange); });

            Hide();
        }

        public void DoActivity()
        {
            var dialogManager = GetDialogManager();
            var context = GetContext();
            dialogManager.ShowUserTaskDialog(context);

            Hide();
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}