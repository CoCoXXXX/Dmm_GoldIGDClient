using System.Collections.Generic;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Network;
using Dmm.Task;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Race
{
    public class RaceHistoryRankList : ItemList<RaceData>
    {
        #region Inject

        private RaceHistoryRankItem.Factory _historyRankFactory;

        private IDataContainer<RaceDescriptionResult> _raceDescriptionResult;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            RemoteAPI remoteAPI,
            RaceHistoryRankItem.Factory historyRankFactory,
            ITaskManager task)
        {
            _historyRankFactory = historyRankFactory;
            _raceDescriptionResult =
                dataRepository.GetContainer<RaceDescriptionResult>(DataKey.RaceDescriptionResult);
        }

        #endregion

        public RaceIntroduceDialog Dialog;

        public void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            InitData();
            RefreshContent();
            SetRefreshTime(Time.time);
        }

        private readonly List<RaceData> _historyRankList = new List<RaceData>();

        private void InitData()
        {
            var historyRankCount = GetRaceHistoryListCount();

            _historyRankList.Clear();
            for (var i = 0; i < historyRankCount; i++)
            {
                var historyRank = GetRaceHistoryAt(i);
                if (historyRank != null)
                {
                    _historyRankList.Add(historyRank);
                }
            }

            var currnt = GetCurrentRaceData();
            if (currnt == null)
            {
                currnt = new RaceData();
            }
            currnt.signUpTime = "当前";
            currnt.IsCurrent = true;
            _historyRankList.Insert(0, currnt);
        }

        public int GetRaceHistoryListCount()
        {
            var raceDescriptionResult = _raceDescriptionResult.Read();
            if (raceDescriptionResult == null)
            {
                return 0;
            }

            var data = raceDescriptionResult.data;
            if (data == null)
            {
                return 0;
            }

            var historyList = data.historyList;
            if (historyList == null)
            {
                return 0;
            }

            return historyList.Length;
        }

        public RaceData GetRaceHistoryAt(int index)
        {
            var raceDescriptionResult = _raceDescriptionResult.Read();

            if (raceDescriptionResult == null)
            {
                return null;
            }

            var data = raceDescriptionResult.data;
            if (data == null)
            {
                return null;
            }

            var historyList = data.historyList;
            if (historyList == null)
            {
                return null;
            }

            var count = historyList.Length;
            if ((index < 0) || (index >= count))
            {
                return null;
            }

            return historyList[index];
        }

        public RaceData GetCurrentRaceData()
        {
            var raceDescriptionResult = _raceDescriptionResult.Read();

            if (raceDescriptionResult == null)
            {
                return null;
            }

            var data = raceDescriptionResult.data;
            if (data == null)
            {
                return null;
            }

            var current = data.current;
            return current;
        }

        public override int SlotCount()
        {
            var count = _historyRankList.Count;
            Dialog.SetHistoryRankNotExist(count <= 0);
            return count;
        }

        public override Item<RaceData> CreateItem()
        {
            return _historyRankFactory.Create();
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
            var count = _historyRankList.Count;
            Dialog.SetHistoryRankNotExist(count <= 0);
            return count;
        }

        public override RaceData GetData(int index)
        {
            if ((index < 0) || (index >= _historyRankList.Count))
            {
                return null;
            }
            return _historyRankList[index];
        }

        public override void OnItemSelected(Item<RaceData> selectedItem)
        {
            for (var i = 0; i < GetItemCount(); i++)
            {
                var item = GetItem(i) as RaceHistoryRankItem;
                if (item)
                {
                    item.ShowDate();
                }
            }
            var currentItem = selectedItem as RaceHistoryRankItem;

            if (!currentItem)
            {
                return;
            }
            currentItem.ShowMyRank();
            var data = currentItem.Data;
            if (data == null)
            {
                return;
            }
            Dialog.RefreshDisplayRankList(data.subRaceId, data.IsCurrent);
        }
    }
}