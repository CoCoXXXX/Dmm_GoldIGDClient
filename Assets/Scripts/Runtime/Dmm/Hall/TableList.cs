using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Network;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Hall
{
    public class TableList : ItemList<Table>
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IDataContainer<Room> _currentRoom;

        [Inject]
        public void Initialize(RemoteAPI remoteAPI, IDataRepository dataRepository)
        {
            _remoteAPI = remoteAPI;
            _currentRoom = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
        }

        #endregion

        public TableBtn TableBtnPrefab;

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
            var room = _currentRoom.Read();
            if (room == null || room.type != RoomType.Normal)
            {
                return 0;
            }

            return room.table.Count;
        }

        public override Item<Table> CreateItem()
        {
            if (TableBtnPrefab)
            {
                return Instantiate(TableBtnPrefab) as TableBtn;
            }

            return null;
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
            return _currentRoom.Timestamp;
        }

        public override int DataCount()
        {
            var room = _currentRoom.Read();
            if (room == null || room.type != RoomType.Normal)
            {
                return 0;
            }

            return room.table.Count;
        }

        public override Table GetData(int index)
        {
            var room = _currentRoom.Read();
            if (room == null || room.type != RoomType.Normal)
            {
                return null;
            }

            if (index < 0 || index >= room.table.Count)
            {
                return null;
            }

            return room.table[index];
        }

        public override void OnItemSelected(Item<Table> item)
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

            _remoteAPI.ChooseTable(data.table_id);
        }
    }
}