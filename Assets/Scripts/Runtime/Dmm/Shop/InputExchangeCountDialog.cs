using System;
using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.Constant;
using Dmm.Dialog;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Shop
{
    public class InputExchangeCountDialog : MyDialog
    {
        #region Inject

        private SpriteHolder _spriteHolder;

        public class Factory : PrefabFactory<InputExchangeCountDialog>
        {
        }

        [Inject]
        public void Initialize(SpriteHolder spriteHolder)
        {
            _spriteHolder = spriteHolder;
        }

        #endregion

        public Text Title;
        public Text Desc;
        public Image CurrencyIcon;
        public InputField CountEdt;

        private Exchange _data;
        private Action<long> _confirmAction;

        public void ApplyData(Exchange data, Action<long> confirmAction)
        {
            _data = data;
            if (data == null)
            {
                Reset();
                return;
            }

            _confirmAction = confirmAction;

            Title.text = string.Format("请输入要兑换的<color=green>{0}</color>的数量：", CurrencyType.LabelOf(data.target_type));
            Desc.text = string.Format(
                "<color=green>{0}{1}</color>可兑换<color=green>{2}{3}</color>",
                data.source_amount, CurrencyType.LabelOf(data.source_type),
                data.target_amount, CurrencyType.LabelOf(data.target_type));
            var sprite = _spriteHolder.GetCurrency(data.target_type);
            if (sprite)
            {
                if (!CurrencyIcon.gameObject.activeSelf)
                {
                    CurrencyIcon.gameObject.SetActive(true);
                }

                CurrencyIcon.sprite = sprite;
            }
            else
            {
                if (CurrencyIcon.gameObject.activeSelf)
                {
                    CurrencyIcon.gameObject.SetActive(false);
                }
            }
        }

        public void OnValueChange()
        {
            if (_data == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(CountEdt.text))
            {
                return;
            }

            try
            {
                var count = int.Parse(CountEdt.text);
                var exchangeCount = (int) (count / _data.target_amount);
                Desc.text = string.Format(
                    "<color=green>{0}{1}</color>兑换<color=green>{2}{3}</color>",
                    _data.source_amount * exchangeCount, CurrencyType.LabelOf(_data.source_type),
                    _data.target_amount * exchangeCount, CurrencyType.LabelOf(_data.target_type)
                );
            }
            catch (Exception e)
            {
            }
        }

        public void Reset()
        {
            if (CurrencyIcon.gameObject.activeSelf)
            {
                CurrencyIcon.gameObject.SetActive(false);
            }

            Title.text = "请输入要兑换的数量：";
            Desc.text = "";
            CountEdt.text = "0";
        }

        public void Confirm()
        {
            if (string.IsNullOrEmpty(CountEdt.text))
            {
                CountEdt.text = "0";
            }

            if (_confirmAction != null)
            {
                try
                {
                    var exchangeCount = int.Parse(CountEdt.text);
                    _confirmAction(exchangeCount);
                }
                catch (Exception e)
                {
                }
            }

            Hide();
        }

        public void Cancel()
        {
            Hide();
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}