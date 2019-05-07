using Dmm.Common;
using Dmm.Constant;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Widget
{
    public class CurrencyValue : MonoBehaviour
    {
        #region Inject

        private SpriteHolder _spriteHolder;

        [Inject]
        public void Initialize(SpriteHolder spriteHolder)
        {
            _spriteHolder = spriteHolder;
        }

        public class Factory : Factory<CurrencyValue>
        {
        }

        #endregion

        public Text AmountTxt;

        public Image IconImg;

        public void SetCurrency(long amount, int currencyType)
        {
            if (AmountTxt)
            {
                AmountTxt.text = BuildAmountText(amount);
                if (currencyType == CurrencyType.VIP)
                    AmountTxt.text += "个月VIP";
            }

            if (IconImg)
            {
                if (currencyType == CurrencyType.VIP)
                {
                    // VIP就不显示Icon了。
                    if (IconImg.gameObject.activeSelf)
                        IconImg.gameObject.SetActive(false);
                }
                else
                {
                    IconImg.sprite = _spriteHolder.GetCurrency(currencyType);
                    if (!IconImg.sprite)
                    {
                        if (IconImg.gameObject.activeSelf)
                            IconImg.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (!IconImg.gameObject.activeSelf)
                            IconImg.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void Clear()
        {
            if (AmountTxt)
                AmountTxt.text = "";

            if (IconImg && IconImg.gameObject.activeSelf)
                IconImg.gameObject.SetActive(false);
        }

        private string BuildAmountText(long amount)
        {
            // 按照单位来。
            // 大于100000000的时候，按照亿来计算。
            if (amount >= 100000000)
            {
                var res = amount / 100000000 + "亿";
                if (amount % 100000000 >= 10000)
                    res += amount % 100000000 / 10000 + "万";

                return res;
            }

            // 大于1000000的时候，按照万来计算。
            if (amount >= 1000000)
                return amount / 10000 + "万";

            return amount + "";
        }
    }
}