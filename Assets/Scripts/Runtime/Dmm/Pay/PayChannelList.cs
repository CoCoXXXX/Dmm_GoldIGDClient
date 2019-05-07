using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Pay
{
    public class PayChannelList : ItemList<int>
    {
        #region Inject

        [Inject]
        public void Initialize(IDataRepository dataRepository)
        {
            _comparer = new PayChannelComparer(dataRepository.GetContainer<List<PayChannel>>(DataKey.PayChannelList));
        }

        #endregion

        private PayChannelComparer _comparer;

        public PayChannelPanel Parent;

        private List<int> _channelList;

        private float _dataTime;

        public void ApplyData(List<int> channels)
        {
            _channelList = channels;
            _dataTime = Time.time;

            if (_channelList != null && _channelList.Count > 0)
            {
                _channelList.Sort(_comparer);
            }
        }

        public PayChannelItem PayChannelItemPrefab;

        public override int SlotCount()
        {
            if (_channelList == null)
            {
                return 0;
            }

            return _channelList.Count;
        }

        public override Item<int> CreateItem()
        {
            if (PayChannelItemPrefab)
            {
                return Instantiate(PayChannelItemPrefab) as PayChannelItem;
            }
            else
            {
                return null;
            }
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
            return _dataTime;
        }

        public override int DataCount()
        {
            if (_channelList == null)
            {
                return 0;
            }

            return _channelList.Count;
        }

        public override int GetData(int index)
        {
            var list = _channelList;
            if (list == null)
            {
                return 0;
            }

            if (index < 0 || index >= list.Count)
            {
                return 0;
            }

            return list[index];
        }

        public override void OnItemSelected(Item<int> item)
        {
            var payChannel = item.GetData();
            if (Parent)
            {
                Parent.StartPay(payChannel);
            }
        }

        public override void BeforeRefresh()
        {
            SelectEmpty();
        }

        private class PayChannelComparer : Comparer<int>
        {
            private readonly IDataContainer<List<PayChannel>> _payChannelListContainer;

            public PayChannelComparer(IDataContainer<List<PayChannel>> payChannelListContainer)
            {
                _payChannelListContainer = payChannelListContainer;
            }

            public override int Compare(int x, int y)
            {
                var pX = GetPayChannel(x);
                var pY = GetPayChannel(y);
                var orderX = pX != null ? pX.order : 0;
                var orderY = pY != null ? pY.order : 0;
                return orderX - orderY;
            }

            private PayChannel GetPayChannel(int channelId)
            {
                var list = _payChannelListContainer.Read();
                if (list == null || list.Count <= 0)
                {
                    return null;
                }

                for (int i = 0; i < list.Count; i++)
                {
                    var p = list[i];
                    if (p.pay_channel_id == channelId)
                    {
                        return p;
                    }
                }

                return null;
            }
        }
    }
}