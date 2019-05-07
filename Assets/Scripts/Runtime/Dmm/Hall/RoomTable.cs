using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.HintItemLogic;
using Dmm.Network;
using Dmm.Sdk;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Hall
{
    /// <summary>
    /// 房间列表。
    /// 绑定HallData中的房间列表。
    /// </summary>
    public class RoomTable : ItemList<Room>
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        private IosSDK _ios;

        private RoomBtn.Factory _itemFactory;

        private IDataContainer<Queue<BRoomInOut>> _roomInOut;

        private IDataContainer<List<Room>> _roomList;

        private IDataContainer<int> _currenGameMode;

        private bool _initialized = false;

        [Inject]
        public void Initialize(
            IDialogManager dialogManager,
            IosSDK ios,
            RemoteAPI remoteAPI,
            IDataRepository dataRepository,
            RoomBtn.Factory itemFactory)
        {
            _dialogManager = dialogManager;
            _ios = ios;
            _remoteAPI = remoteAPI;
            _itemFactory = itemFactory;
            _roomList = dataRepository.GetContainer<List<Room>>(DataKey.RoomList);
            _roomInOut = dataRepository.GetContainer<Queue<BRoomInOut>>(DataKey.BRoomInOut);
            _currenGameMode = dataRepository.GetContainer<int>(DataKey.CurrentGameMode);
        }

        #endregion

        public float PlayerCountRefreshTime { get; private set; }

        public void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            RefreshContent();
            SetRefreshTime(Time.time);
            RefreshRoomPlayerCount();
        }

        public override void OnUpdate()
        {
            RefreshRoomPlayerCount();
        }

        private void RefreshRoomPlayerCount()
        {
            if (PlayerCountRefreshTime >= _roomInOut.Timestamp)
                return;

            PlayerCountRefreshTime = _roomInOut.Timestamp;

            var roomInOutData = _roomInOut.Read();
            if (roomInOutData == null)
            {
                return;
            }
            while (roomInOutData.Count > 0)
            {
                var inOut = roomInOutData.Dequeue();
                for (int i = 0; i < DataCount(); i++)
                {
                    var item = GetItem(i) as RoomBtn;
                    if (item && item.RoomId == inOut.room_id)
                    {
                        item.UpdatePlayerCount(inOut.room_player_count);
                    }
                }
            }
        }

        private readonly List<Room> _rooms = new List<Room>();

        private void ExtractRooms()
        {
            _rooms.Clear();
            var gameMode = _currenGameMode.Read();
            if (gameMode == GameMode.Null || gameMode == GameMode.Single)
            {
                return;
            }
            var roomList = _roomList.Read();

            for (int i = 0; i < roomList.Count; i++)
            {
                if (i < 0 || i > roomList.Count)
                {
                    continue;
                }
                var room = roomList[i];
                if (room == null)
                {
                    continue;
                }

                if (gameMode == room.game_mode)
                {
                    _rooms.Add(room);
                }
            }
        }

        public override void BeforeRefresh()
        {
            ExtractRooms();
            SelectEmpty();
        }

        public override int SlotCount()
        {
            return _rooms.Count;
        }

        public override float DataUpdateTime()
        {
            return _roomList.Timestamp;
        }

        public override int DataCount()
        {
            return _rooms.Count;
        }

        public override Room GetData(int index)
        {
            if (index < 0 || index >= _rooms.Count)
            {
                return null;
            }

            return _rooms[index];
        }

        public override void OnItemSelected(Item<Room> item)
        {
            if (item == null)
            {
                {
                    return;
                }
            }

            var data = item.GetData();
            if (data == null)
            {
                return;
            }

            if (data.type != RoomType.Ad)
            {
                _remoteAPI.ChooseRoom((int) data.room_id);
            }
            else
            {
                if (data.action != null)
                {
                    var action = data.action;
                    switch (action.type)
                    {
                        case ExtraActionType.SHOW_DIALOG:
                            _dialogManager.ShowDialog<UIWindow>(action.dialog_name);
                            break;

                        case ExtraActionType.AWARD:
                            _remoteAPI.RequestAward(action.award_type, action.award_code);
                            break;

                        case ExtraActionType.GOTO_ROOM:
                            _remoteAPI.ChooseRoom((int) action.room_id);
                            break;

                        case ExtraActionType.URL:
                            if (!string.IsNullOrEmpty(action.url))
                            {
                                _ios.OpenUrl(action.url);
                            }
                            break;
                    }
                }
                else if (data.hint_item != null)
                {
                    _dialogManager.ShowDialog<HintItemDialog>(DialogName.HintItemDialog, false, false,
                        (dialog) =>
                        {
                            dialog.ApplyData(data.hint_item);
                            dialog.Show();
                        });
                }
            }
        }

        public override Item<Room> CreateItem()
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
    }
}