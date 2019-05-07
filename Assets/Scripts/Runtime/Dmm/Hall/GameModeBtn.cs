using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Hall
{
    public class GameModeBtn : MonoBehaviour
    {
        private IDataContainer<List<Room>> _roomList;

        private IDataContainer<Queue<BRoomInOut>> _roomInOut;

        [Inject]
        public void Inject(IDataRepository dataRepository)
        {
            _roomInOut = dataRepository.GetContainer<Queue<BRoomInOut>>(DataKey.BRoomInOut);

            _roomList = dataRepository.GetContainer<List<Room>>(DataKey.RoomList);
        }

        public Text PlayerCountText;

        public int GameMode;

        private void OnEnable()
        {
            SetPlayerCount();
        }

        private void Update()
        {
            if (_refreshTime >= _roomInOut.Timestamp)
            {
                return;
            }

            _refreshTime = _roomInOut.Timestamp;

            SetPlayerCount();
        }

        private float _refreshTime;

        private void SetPlayerCount()
        {
            var totalCount = 0L;
            var roomList = _roomList.Read();
            if (roomList == null)
            {
                return;
            }
            for (int i = 0; i < roomList.Count; i++)
            {
                var room = roomList[i];
                if (room == null)
                {
                    continue;
                }

                if (room.game_mode != GameMode)
                {
                    continue;
                }

                totalCount += room.current_player_num;
            }

            PlayerCountText.text = "" + totalCount;
        }
    }
}