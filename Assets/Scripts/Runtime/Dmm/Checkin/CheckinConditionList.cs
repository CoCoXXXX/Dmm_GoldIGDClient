using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Checkin
{
    public class CheckinConditionList : ItemList<CheckinCondition>
    {
        #region Inject

        private IDataContainer<CheckinConfig> _container;

        private CheckinConditionItem.Factory _itemFactory;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            CheckinConditionItem.Factory itemFactory)
        {
            _container = dataRepository.GetContainer<CheckinConfig>(DataKey.CheckinConfig);
            _itemFactory = itemFactory;
        }

        #endregion

        private List<CheckinCondition> GetCheckinConditionList()
        {
            var config = _container.Read();
            if (config == null)
            {
                return null;
            }

            return config.condition;
        }

        public override int SlotCount()
        {
            var list = GetCheckinConditionList();
            if (list == null)
            {
                return 0;
            }

            return list.Count;
        }

        public override Item<CheckinCondition> CreateItem()
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
            return _container.Timestamp;
        }

        public override int DataCount()
        {
            var list = GetCheckinConditionList();
            if (list == null)
            {
                return 0;
            }

            return list.Count;
        }

        public override CheckinCondition GetData(int index)
        {
            var list = GetCheckinConditionList();
            if (list == null)
            {
                return null;
            }

            if (index < 0 || index >= list.Count)
            {
                return null;
            }

            return list[index];
        }

        public override void OnItemSelected(Item<CheckinCondition> item)
        {
            // Nothing.
        }
    }
}