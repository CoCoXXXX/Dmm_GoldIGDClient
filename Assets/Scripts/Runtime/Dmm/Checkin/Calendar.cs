using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Checkin
{
    public class Calendar : ItemList<CheckinDayData>
    {
        #region Inject

        private IDataContainer<CheckinConfig> _checkinConfig;

        [Inject]
        public void Initialize(IDataRepository dataRepository)
        {
            _checkinConfig = dataRepository.GetContainer<CheckinConfig>(DataKey.CheckinConfig);
        }

        #endregion

        public const int CalendarDayCount = 7 * 6;

        private readonly List<CheckinDayData> _data = new List<CheckinDayData>();

        public DayItem ItemPrefab;

        private void OnEnable()
        {
            // 初始化的时候先重置当前的数据。
            _data.Clear();

            for (int i = 0; i < CalendarDayCount; i++)
            {
                _data.Add(new CheckinDayData());
            }
        }

        public override void BeforeRefresh()
        {
            var config = _checkinConfig.Read();
            if (config == null)
            {
                return;
            }

            var list = config.checkin_item;
            for (int i = 0; i < CalendarDayCount; i++)
            {
                var data = _data[i];
                if (data == null)
                {
                    continue;
                }

                data.Day = i < config.day_start_offset || i >= config.day_start_offset + config.month_length
                    ? -1
                    : i - config.day_start_offset + 1;

                data.IsToday = data.Day == config.current_day;
                data.Enabled = i >= config.day_start_offset &&
                               i < config.day_start_offset + config.month_length;
                data.Status =
                    data.Day < config.current_day
                        ? CheckinStatus.Passed
                        : CheckinStatus.UnChecked;

                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if (data.Day == item.day)
                        {
                            data.Status =
                                data.Day <= config.current_day
                                    ? item.status
                                    : CheckinStatus.UnChecked;
                            break;
                        }
                    }
                }
            }
        }

        public override int SlotCount()
        {
            return CalendarDayCount;
        }

        public override Item<CheckinDayData> CreateItem()
        {
            if (!ItemPrefab)
                return null;

            return Instantiate(ItemPrefab) as DayItem;
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
            return _checkinConfig.Timestamp;
        }

        public override int DataCount()
        {
            return CalendarDayCount;
        }

        public override CheckinDayData GetData(int index)
        {
            if (index < 0 || index >= _data.Count)
            {
                return null;
            }

            return _data[index];
        }

        public override void OnItemSelected(Item<CheckinDayData> item)
        {
            // Nothing.
        }
    }
}