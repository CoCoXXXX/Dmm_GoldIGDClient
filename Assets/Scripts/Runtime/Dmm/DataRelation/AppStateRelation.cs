using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.DataRelation
{
    public class AppStateRelation : ChildRelationAdapter<AppState>
    {
        private readonly IDataContainer<HLoginResult> _hLoginResult;

        private readonly IDataContainer<Room> _currentRoom;

        private readonly IDataContainer<Table> _currentTable;

        private readonly IDataContainer<int> _gameMode;

        private readonly IDataContainer<PlayingData> _playingData;

        public AppStateRelation(
            IDataContainer<HLoginResult> hLoginResult,
            IDataContainer<Room> currentRoom,
            IDataContainer<Table> currentTable,
            IDataContainer<int> gameMode,
            IDataContainer<PlayingData> playingData)
        {
            _hLoginResult = hLoginResult;
            _currentRoom = currentRoom;
            _currentTable = currentTable;
            _gameMode = gameMode;
            _playingData = playingData;
        }

        private bool IsChooseRoomOk()
        {
            var room = _currentRoom.Read();
            return room != null;
        }

        private bool IsChooseTableOk()
        {
            var table = _currentTable.Read();
            return table != null;
        }

        private bool IsLoginOk()
        {
            var data = _hLoginResult.Read();
            if (data == null)
            {
                return false;
            }

            return data.result == ResultCode.OK;
        }

        private bool IsInPortal()
        {
            var mode = _gameMode.Read();
            return mode == GameMode.Null;
        }

        private bool IsWaiting()
        {
            var playingData = _playingData.Read();
            if (playingData == null)
            {
                return false;
            }

            return playingData.period == TablePeriod.Waiting;
        }

        private bool IsPlaying()
        {
            var playingData = _playingData.Read();
            if (playingData == null)
            {
                return false;
            }

            switch (playingData.period)
            {
                case TablePeriod.BeforeChuPai:
                case TablePeriod.StartRound:
                case TablePeriod.ChuPai:
                case TablePeriod.JinGong:
                case TablePeriod.HuanGong:
                    return true;

                default:
                    return false;
            }
        }

        public override AppState Data
        {
            get
            {
                // 未登陆。
                if (!IsLoginOk())
                {
                    return AppState.NoLogin;
                }

                // 已登录。

                if (!IsChooseRoomOk())
                {
                    // 没有选房，在大厅中。
                    return IsInPortal() ? AppState.LoginOk : AppState.ChooseRoom;
                }

                // 已经选房。

                if (IsChooseTableOk())
                {
                    if (IsWaiting())
                    {
                        return AppState.InTable;
                    }

                    if (IsPlaying())
                    {
                        return AppState.Playing;
                    }

                    return AppState.BetweenRound;
                }

                // 不在桌子中。
                var room = _currentRoom.Read();
                if (room != null)
                {
                    if (room.type == RoomType.Normal)
                    {
                        return AppState.InRoom;
                    }

                    return AppState.ChooseRoom;
                }

                // 按理说，不应该发生这种情况。
                return IsInPortal() ? AppState.LoginOk : AppState.ChooseRoom;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get
            {
                return Mathf.Max(
                    _hLoginResult.Timestamp,
                    _gameMode.Timestamp,
                    _currentRoom.Timestamp,
                    _currentTable.Timestamp
                );
            }
        }
    }
}