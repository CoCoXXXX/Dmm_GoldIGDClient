using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Pay;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Shop
{
    public class PrepaymentList : ItemList<Prepayment>
    {
        #region Inject

        private IDialogManager _dialogManager;

        private PrepaymentItem.Factory _itemFactory;

        private IDataContainer<List<Prepayment>> _prepayment;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            PrepaymentItem.Factory itemFactory)
        {
            _dialogManager = dialogManager;
            _itemFactory = itemFactory;
            _prepayment = dataRepository.GetContainer<List<Prepayment>>(DataKey.PrepaymentList);
        }

        #endregion

        public void OnEnable()
        {
            RefreshContent();
        }

        public override void BeforeRefresh()
        {
            SelectEmpty();
        }

        public override int SlotCount()
        {
            var prepayment = _prepayment.Read();
            if (prepayment == null)
            {
                return 0;
            }
            return prepayment.Count;
        }

        public override Item<Prepayment> CreateItem()
        {
            return _itemFactory.Create();
        }

        public override bool HasDivider()
        {
            return false;
        }

        public override GameObject CreateDivider()
        {
            return null;
        }

        public override float DataUpdateTime()
        {
            return _prepayment.Timestamp;
        }

        public override int DataCount()
        {
            var prepayment = _prepayment.Read();
            if (prepayment == null)
            {
                return 0;
            }
            return prepayment.Count;
        }

        public override Prepayment GetData(int index)
        {
            var prepayment = _prepayment.Read();
            if (prepayment == null)
            {
                return null;
            }
            if (index < 0 || index >= prepayment.Count)
            {
                return null;
            }
            return prepayment[index];
        }

        public override void OnItemSelected(Item<Prepayment> item)
        {
            if (item == null)
            {
                return;
            }
            var data = item.GetData();
            if (data == null)
            {
                return;
            }

            _dialogManager.ShowDialog<PayChannelPanel>(DialogName.PayChannelPanel, false, false,
                (dialog) =>
                {
                    dialog.ApplyData("shop", data);
                    dialog.Show();
                });
        }
    }
}