using Dmm.Constant;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Pay
{
    public class PayChannelItem : Item<int>
    {
        #region 支付方式图标

        public Sprite PayChannelZhiFuBao;

        public Sprite PayChannelWeiXin;

        public Sprite PayChannelApple;

        public Sprite PayChannelXiaoMi;

        public Sprite PayChannelDefault;

        public Sprite GetPayChannelIcon(int payChannel)
        {
            switch (payChannel)
            {
                case PayChannelType.ALIPAY_CLIENT:
                case PayChannelType.ALIPAY_IOS:
                    return PayChannelZhiFuBao;

                case PayChannelType.IOS_IAP:
                    return PayChannelApple;

                case PayChannelType.WEI_XIN:
                    return PayChannelWeiXin;

                case PayChannelType.XIAOMI:
                    return PayChannelXiaoMi;

                default:
                    return PayChannelDefault;
            }
        }

        #endregion

        public Image Icon;

        public Text Name;

        public Button Button;

        private int _payChannel;

        public override int GetData()
        {
            return _payChannel;
        }

        public override void BindData(int currentIndex, int data)
        {
            _payChannel = data;

            if (!Icon.gameObject.activeSelf)
                Icon.gameObject.SetActive(true);

            Icon.sprite = GetPayChannelIcon(data);

            if (!Name.gameObject.activeSelf)
                Name.gameObject.SetActive(true);

            Name.text = PayChannelType.GetPayChannelName(data);
        }

        public override void Reset(int currentIndex)
        {
            /*if (Icon)
            {
                Icon.sprite = null;

                if (Icon.gameObject.activeSelf)
                    Icon.gameObject.SetActive(false);
            }

            if (Name && Name.gameObject.activeSelf)
                Name.gameObject.SetActive(false);*/
        }

        public override void Select(bool selected)
        {
        }

        public override Button GetClickButton()
        {
            return Button;
        }
    }
}