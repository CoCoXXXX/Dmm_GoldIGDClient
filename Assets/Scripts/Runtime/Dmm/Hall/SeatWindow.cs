using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;
using UnityEngine;
using Zenject;

namespace Dmm.Hall
{
    public class SeatWindow : MonoBehaviour
    {
        #region Inject 

        private IDataContainer<Room> _currentRoom;

        private IDataContainer<Table> _currentTable;

        [Inject]
        public void Inject(IDataRepository dataRepository)
        {
            _currentRoom = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
            _currentTable = dataRepository.GetContainer<Table>(DataKey.CurrentTable);
        }

        public class Factory : PrefabFactory<SeatWindow>
        {
        }

        #endregion

        public SeatPanel SeatPanel;

        public TablePanel TablePanel;

        private float _refreshTime;

        private void Update()
        {
            var time = Mathf.Max(_currentRoom.Timestamp, _currentTable.Timestamp);
            if (_refreshTime >= time)
            {
                return;
            }

            _refreshTime = time;

            var room = _currentRoom.Read();
            var table = _currentTable.Read();

            if (room == null)
            {
                // 没有房间数据，则两个panel都不显示。
                if (SeatPanel.gameObject.activeSelf)
                {
                    SeatPanel.gameObject.SetActive(false);
                }

                if (TablePanel.gameObject.activeSelf)
                {
                    TablePanel.gameObject.SetActive(false);
                }

                return;
            }

            if (table == null)
            {
                // 尚未选桌成功。
                var chooseTable = room.type == RoomType.Normal;
                if (SeatPanel.gameObject.activeSelf)
                {
                    SeatPanel.gameObject.SetActive(false);
                }

                if (TablePanel.gameObject.activeSelf != chooseTable)
                {
                    TablePanel.gameObject.SetActive(chooseTable);
                }

                return;
            }

            // 当前存在桌子数据。
            if (!SeatPanel.gameObject.activeSelf)
            {
                SeatPanel.gameObject.SetActive(true);
            }

            if (TablePanel.gameObject.activeSelf)
            {
                TablePanel.gameObject.SetActive(false);
            }
        }
    }
}