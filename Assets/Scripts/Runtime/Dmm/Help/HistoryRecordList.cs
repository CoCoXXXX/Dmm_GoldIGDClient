using Dmm.DataContainer;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Help
{
    public class HistoryRecordList : ItemList<HistoryRecord>
    {
        #region Inject

        private HistoryRecordItem.Factory _itemFactory;

        private IDataContainer<HistoryRecordResult> _historyRecordResult;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            HistoryRecordItem.Factory itemFactory)
        {
            _itemFactory = itemFactory;
            _historyRecordResult =
                dataRepository.GetContainer<HistoryRecordResult>(DataKey.HistoryRecordResult);
        }

        #endregion

        public void Init()
        {
            RefreshContent();
            SetRefreshTime(Time.time);
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <returns></returns>
        public override int SlotCount()
        {
            var historyRecordResult = _historyRecordResult.Read();
            if (historyRecordResult == null)
            {
                return 0;
            }
            var data = historyRecordResult.data;
            if (data == null)
            {
                return 0;
            }
            return data.Length;
        }

        public override Item<HistoryRecord> CreateItem()
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
            return 0;
        }

        public override int DataCount()
        {
            var historyRecordResult = _historyRecordResult.Read();
            if (historyRecordResult == null)
            {
                return 0;
            }
            var data = historyRecordResult.data;
            if (data == null)
            {
                return 0;
            }
            return data.Length;
        }

        public override HistoryRecord GetData(int index)
        {
            var historyRecordResult = _historyRecordResult.Read();
            if (historyRecordResult == null)
            {
                return null;
            }
            var data = historyRecordResult.data;
            if (data == null)
            {
                return null;
            }

            if (index < 0 || index >= data.Length)
            {
                return null;
            }

            return data[index];
        }

        public override void OnItemSelected(Item<HistoryRecord> item)
        {
            //nothing
        }
    }
}