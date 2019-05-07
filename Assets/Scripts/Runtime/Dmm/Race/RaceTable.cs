using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Network;
using Dmm.Task;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Race
{
    public class RaceTable : ItemList<RaceConfig>
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        private RaceItem.Factory _raceFactory;

        private ITaskManager _task;

        private IDataContainer<RaceConfigList> _raceConfigList;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            RemoteAPI remoteAPI,
            RaceItem.Factory raceFactory,
            ITaskManager task)
        {
            _dialogManager = dialogManager;
            _remoteAPI = remoteAPI;
            _raceFactory = raceFactory;
            _task = task;
            _raceConfigList = dataRepository.GetContainer<RaceConfigList>(DataKey.RaceConfigList);
        }

        #endregion

        public void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            ExtractRooms();
        }

        private readonly List<RaceConfig> _races = new List<RaceConfig>();

        private void ExtractRooms()
        {
            _dialogManager.ShowWaitingDialog(true);
            _raceConfigList.ClearAndInvalidate(0);
            _task.ExecuteTask(CheckRaceConfigListResult, GetRaceConfigListComplete,
                () => { _dialogManager.ShowWaitingDialog(false); }, 5);

            _remoteAPI.RequestRaceConfigList();
        }

        private bool CheckRaceConfigListResult()
        {
            var raceConfigList = _raceConfigList.Read();
            if (raceConfigList == null)
            {
                return false;
            }
            return true;
        }

        private void GetRaceConfigListComplete()
        {
            _dialogManager.ShowWaitingDialog(false);
        }

        public override void BeforeRefresh()
        {
            _races.Clear();
            var raceConfigList = _raceConfigList.Read();
            if (raceConfigList == null)
            {
                _dialogManager.ShowToast("当前没有比赛", 2, false);
                return;
            }
            var raceRoomList = raceConfigList.config_list;
            if (raceRoomList == null)
            {
                _dialogManager.ShowToast("当前没有比赛", 2, false);
                return;
            }

            _races.AddRange(raceRoomList);
        }

        public override int SlotCount()
        {
            return _races.Count;
        }

        public override Item<RaceConfig> CreateItem()
        {
            return _raceFactory.Create();
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
            return _raceConfigList.Timestamp;
        }

        public override int DataCount()
        {
            return _races.Count;
        }

        public override RaceConfig GetData(int index)
        {
            if (index < 0 || index >= _races.Count)
            {
                return null;
            }

            return _races[index];
        }

        public override void OnItemSelected(Item<RaceConfig> item)
        {
        }
    }
}