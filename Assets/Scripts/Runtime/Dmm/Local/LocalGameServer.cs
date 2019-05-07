using System;
using System.Collections;
using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Log;
using Dmm.Msg;
using Dmm.PokerLogic;
using Dmm.Util;
using UnityEngine;
using Zenject;
using PokerNumType = Dmm.PokerLogic.PokerNumType;
using PokerPattern = Dmm.PokerLogic.PokerPattern;
using PokerSuitType = Dmm.PokerLogic.PokerSuitType;
using Random = UnityEngine.Random;

namespace Dmm.Local
{
    public class LocalGameServer : MonoBehaviour
    {
        private const int SeatCount = 4;

        #region Inject

        private IMsgRepo _msgRepo;

        private IAppController _appController;

        [Inject]
        public void Initialize(
            IMsgRepo msgRepo,
            IAppController appController,
            IDataRepository dataRepository)
        {
            _msgRepo = msgRepo;
            _appController = appController;
        }

        #endregion

        #region 处理消息

        public void LateUpdate()
        {
            if (!_appController.IsSingleGameMode())
            {
                return;
            }

            var msg = _msgRepo.DequeueWriteMessage();
            while (msg != null)
            {
                HandleMessage(msg);
                msg = _msgRepo.DequeueWriteMessage();
            }
        }

        private void HandleMessage(ProtoMessage msg)
        {
            switch (msg.Type)
            {
                case CmdType.GU.READY_V6:
                    HandleReady(msg);
                    break;

                case CmdType.GU.CHUPAI_V6:
                    HandleChuPai(msg);
                    break;

                case CmdType.GU.JINGONG_V6:
                    HandleJinGong(msg);
                    break;

                case CmdType.GU.HUANGONG_V6:
                    HandleHuanGong(msg);
                    break;
            }
        }

        private void HandleReady(ProtoMessage msg)
        {
            // 收到玩家的准备命令，则开始游戏。
            var content = msg.Content as Ready;
            if (content == null)
            {
                return;
            }

            SendResponse(CmdUtil.GU.ReadyResult(ResultCode.OK, null));

            FaPai();
        }

        private void HandleChuPai(ProtoMessage msg)
        {
            // 收到玩家的出牌命令，执行之后的服务器。
            var content = msg.Content as ChuPai;
            if (content == null)
            {
                return;
            }

            if (_autoChuPaiCoroutine == null)
            {
                PlayerChuPai(_seatList[0], content);
            }
        }

        private void HandleJinGong(ProtoMessage msg)
        {
            var content = msg.Content as JinGong;
            if (content == null)
            {
                return;
            }

            PlayerJinGong(content);
        }

        private void HandleHuanGong(ProtoMessage msg)
        {
            var content = msg.Content as HuanGong;
            if (content == null)
            {
                return;
            }

            PlayerHuanGong(content);
        }

        private void SendToSeat(Seat seat, ProtoMessage msg)
        {
            if (seat == null || msg == null)
            {
                return;
            }

            if (seat.Number == 0)
            {
                SendResponse(msg);
            }
        }

        private void SendResponse(ProtoMessage msg)
        {
            _msgRepo.ReceiveMsg(msg);
        }

        #endregion

        #region 桌子逻辑

        #region 桌子属性

        private PatternMatcher _matcher;

        private PatternValue _value;

        private User _myUser;

        private int _team1Host;

        private int _team2Host;

        private bool _team1Win;

        private int _hostTeam;

        private int _winLevel;

        private bool _isFirstChuPai;

        private int _shiftResult;

        private int _endState;

        private int _roundCount;

        private PokerAI _pokerAi;

        private readonly List<Seat> _lastRank = new List<Seat>(SeatCount);

        private readonly List<Seat> _currentRank = new List<Seat>(SeatCount);

        private readonly List<Seat> _seatList = new List<Seat>(SeatCount);

        private readonly List<JinHuanGongRecord> _jinHuanGongRecordList = new List<JinHuanGongRecord>(2);

        private Seat _curChuPaiSeat;

        private Seat _lastChuPaiSeat;

        private readonly PokerPattern[] _lastChuPai = new PokerPattern[4];

        private readonly int[] _unusedPokerCount = new int[15];

        private int _targetATimes1;

        private int _targetATimes2;

        private int _fanShanCount;

        private int _currentHost;

        private int _period;

        private const int ShiftNext = 0;
        private const int ShiftForceChuPai = 1;
        private const int ShiftJieFeng = 2;

        private const int NoEndState = 0;
        private const int EndRoundState = 1;

        #endregion

        #region 初始化数据

        public void Init()
        {
            _appController.ClearAppStateData();

            ResetTable();
            ResetRound();

            InitLoginData();
            InitRoomData();
            InitTableData();

            _value = new PatternValue(() => _currentHost);
            _matcher = new PatternMatcher(_value);

            _pokerAi = new PokerAI(_matcher, _value);
        }

        /// <summary>
        /// 初始化登陆数据。
        /// </summary>
        private void InitLoginData()
        {
            var user = new User();
            user.username = "MySelf";
            user.nickname = "单机玩家";
            user.sex = 0;
            user.body = "bodyGirlClassical";
            user.hair = "hairGirlD";
            user.item_show.Add("deskSunflower");

            _myUser = user;

            var bagItems = new List<BagItem>();
            bagItems.Add(CmdUtil.Model.BagItem("hairBoyHama", 1, 0, 1));
            bagItems.Add(CmdUtil.Model.BagItem("bodyBoyHama", 1, 0, 1));

            var commodities = new List<Commodity>();
            var c = new Commodity();
            c.name = "hairBoyHama";
            c.pic = "hairBoyHama";
            c.pic_bg = "hairBoyHama_bg";
            commodities.Add(c);

            c = new Commodity();
            c.name = "bodyBoyHama";
            c.pic = "bodyBoyHama";
            commodities.Add(c);

            c = new Commodity();
            c.name = "bodyBoySuit";
            c.pic = "bodyBoySuit";
            commodities.Add(c);

            c = new Commodity();
            c.name = "bodyBoyCowboy";
            c.pic = "bodyBoyCowboy";
            commodities.Add(c);

            c = new Commodity();
            c.name = "hairBoyCapXjcJan";
            c.pic = "hairBoyCapXjcJan";
            commodities.Add(c);

            c = new Commodity();
            c.name = "bodyGirlXjcSep1";
            c.pic = "bodyGirlXjcSep1";
            commodities.Add(c);

            c = new Commodity();
            c.name = "hairGirlC";
            c.pic = "hairGirlC";
            commodities.Add(c);

            var loginResult = CmdUtil.HU.LoginResult(
                ResultCode.OK,
                _myUser,
                CmdUtil.HU.HallData(
                    null,
                    commodities,
                    "chat_server_addr",
                    CmdUtil.Model.Bag(bagItems),
                    null,
                    null,
                    null,
                    null,
                    null),
                null
            );
            SendResponse(loginResult);
        }

        private void InitRoomData()
        {
            var chooseRoomResult = CmdUtil.HU.ChooseRoomResult(ResultCode.OK, "", 1, null, null, null, null);
            SendResponse(chooseRoomResult);

            var msg = new GLoginResult();
            msg.result = ResultCode.OK;
            msg.room = new Room();
            msg.room.room_id = 0;
            msg.room.base_money = 0;

            var gLoginResult = CmdUtil.GU.LoginResult(ResultCode.OK, new Room(), null);
            SendResponse(gLoginResult);
        }

        /// <summary>
        /// 初始化桌子的数据。
        /// </summary>
        private void InitTableData()
        {
            var user1 = _myUser;

            var user2 = new User();
            user2.username = "user1";
            user2.nickname = "掼蛋老板";
            user2.sex = 1;
            user2.body = "bodyBoySuit";
            user2.hair = "hairBoyDefault";
            user2.item_show.Add("deskNoddle");

            var user3 = new User();
            user3.username = "user2";
            user3.nickname = "掼蛋圣手";
            user3.sex = 1;
            user3.body = "bodyBoyCowboy";
            user3.hair = "hairBoyCapXjcJan";
            user3.item_show.Add("deskMooncake");

            var user4 = new User();
            user4.username = "user3";
            user4.nickname = "掼蛋老板娘";
            user4.sex = 0;
            user4.body = "bodyGirlXjcSep1";
            user4.hair = "hairGirlC";
            user4.item_show.Add("deskIceCone");

            _seatList.Clear();

            for (int i = 0; i < SeatCount; i++)
            {
                _seatList.Add(new Seat());
            }

            // 初始化座位的关系。
            for (int i = 0; i < SeatCount; i++)
            {
                _seatList[i].Number = i;
            }

            _seatList[0].User = user1;
            _seatList[1].User = user2;
            _seatList[2].User = user3;
            _seatList[3].User = user4;

            _period = TablePeriod.BetweenRound;

            var chooseTableResult = CmdUtil.GU.ChooseTableResult(
                ResultCode.OK,
                PlayerType.Player,
                CmdUtil.Model.Table(
                    0, 0, RoomType.Normal,
                    4,
                    PokerLogicUtil.ToSessionNumType(PokerNumType.P2),
                    user1, user2, user3, user4,
                    // 1： Waiting
                    1,
                    0,
                    BuildPlayingData(),
                    PokerLogicUtil.ToSessionNumType(PokerNumType.P2),
                    PokerLogicUtil.ToSessionNumType(PokerNumType.P2), 1)
            );

            SendResponse(chooseTableResult);
        }

        #endregion

        #region 开局

        private void ResetRound()
        {
            _jinHuanGongRecordList.Clear();

            _curChuPaiSeat = null;
            _lastChuPaiSeat = null;

            for (int i = 0; i < 15; i++)
            {
                _unusedPokerCount[i] = 0;
            }

            _isFirstChuPai = true;

            _endState = NoEndState;

            for (int i = 0; i < _lastChuPai.Length; i++)
            {
                _lastChuPai[i] = null;
            }

            _currentRank.Clear();
        }

        private void ResetTable()
        {
            _team1Host = PokerNumType.P2;
            _team2Host = PokerNumType.P2;
            _currentHost = PokerNumType.P2;

            _roundCount = 0;
            _fanShanCount = 0;

            _team1Win = false;
            _winLevel = 0;

            _targetATimes1 = 0;
            _targetATimes2 = 0;

            _lastRank.Clear();
        }

        private void FaPai()
        {
            _period = TablePeriod.StartRound;

            var pool = new List<int>();
            for (int i = 1; i < 109; i++)
            {
                pool.Add(i);
            }

            var pai1 = new List<Poker>();
            var pai2 = new List<Poker>();
            var pai3 = new List<Poker>();
            var pai4 = new List<Poker>();

            var myPokers = new byte[27];
            for (int i = 0; i < 27; i++)
            {
                var idx = Random.Range(0, pool.Count);
                var pokerNumber = pool[idx];
                pai1.Add(new Poker(pokerNumber));
                pool.RemoveAt(idx);

                myPokers[i] = (byte) pokerNumber;
            }

            for (int i = 0; i < 27; i++)
            {
                var idx = Random.Range(0, pool.Count);
                pai2.Add(new Poker(pool[idx]));
                pool.RemoveAt(idx);
            }

            for (int i = 0; i < 27; i++)
            {
                var idx = Random.Range(0, pool.Count);
                pai3.Add(new Poker(pool[idx]));
                pool.RemoveAt(idx);
            }

            for (int i = 0; i < 27; i++)
            {
                var idx = Random.Range(0, pool.Count);
                pai4.Add(new Poker(pool[idx]));
                pool.RemoveAt(idx);
            }

            _seatList[0].Pokers = pai1;
            _seatList[1].Pokers = pai2;
            _seatList[2].Pokers = pai3;
            _seatList[3].Pokers = pai4;

            for (int i = 0; i < 13; i++)
            {
                _unusedPokerCount[i] = 8;
            }

            _unusedPokerCount[13] = 2;
            _unusedPokerCount[14] = 2;

            _shiftResult = ShiftForceChuPai;

            var msg = CmdUtil.GU.StartRound(
                PokerLogicUtil.ToSessionNumType(_team1Host),
                PokerLogicUtil.ToSessionNumType(_team2Host),
                _team1Win ? 1 : 2,
                myPokers,
                _seatList[0].User,
                _seatList[1].User,
                _seatList[2].User,
                _seatList[3].User,
                _roundCount,
                BuildPlayingData()
            );

            SendResponse(msg);

            DoStartRound();
        }

        private PlayingData BuildPlayingData()
        {
            var res = new PlayingData();
            res.ResetAll();

            res.chupai_key = "";
            res.chupai_key_owner_seat = _curChuPaiSeat != null ? _curChuPaiSeat.Number : -1;
            res.must_chupai = _shiftResult == ShiftForceChuPai || _shiftResult == ShiftJieFeng ? 1 : 2;
            res.my_pokers = _seatList[0].PokerBytes();
            res.left_time = 30;

            res.period = _period;

            res.win_level = _winLevel;

            var curRank = new List<int>();
            if (_currentRank != null && _currentRank.Count > 0)
            {
                for (int i = 0; i < _currentRank.Count; i++)
                {
                    curRank.Add(_currentRank[i].Number);
                }
            }

            res.current_rank.AddRange(curRank);

            res.jie_feng = _shiftResult == ShiftJieFeng;
            res.last_chu_pai_seat = _lastChuPaiSeat != null ? _lastChuPaiSeat.Number : -1;
            res.fanbei = 1;

            res.left_count1 = _seatList[0].PokerCount;
            res.left_count2 = _seatList[1].PokerCount;
            res.left_count3 = _seatList[2].PokerCount;
            res.left_count4 = _seatList[3].PokerCount;

            res.last_chupai1 = PokerLogicUtil.ConvertToSessionPattern(_lastChuPai[0]);
            res.last_chupai2 = PokerLogicUtil.ConvertToSessionPattern(_lastChuPai[1]);
            res.last_chupai3 = PokerLogicUtil.ConvertToSessionPattern(_lastChuPai[2]);
            res.last_chupai4 = PokerLogicUtil.ConvertToSessionPattern(_lastChuPai[3]);

            var pokerPeeperData = GetMyPokerPeeperData();
            res.poker_peeper1 = pokerPeeperData.PokerPeeper1;
            res.poker_peeper2 = pokerPeeperData.PokerPeeper2;
            res.poker_peeper3 = pokerPeeperData.PokerPeeper3;
            res.poker_peeper4 = pokerPeeperData.PokerPeeper4;

            var pokerRecorder = GetMyPokerRecorderResult(_seatList[0]);
            if (pokerRecorder != null)
            {
                res.poker_recorder.AddRange(pokerRecorder);
            }

            var rec1 = GetJinHuanGongRecord(0);
            if (rec1 != null)
            {
                res.jg_seat1 = rec1.JgSeatNumber;
                res.jg_poker1 = rec1.JgPokerNumber;
                res.jg_dest1 = rec1.HgSeatNumber;

                res.hg_seat1 = rec1.HgSeatNumber;
                res.hg_poker1 = rec1.HgPokerNumber;
                res.hg_dest1 = rec1.JgSeatNumber;
            }

            var rec2 = GetJinHuanGongRecord(1);
            if (rec2 != null)
            {
                res.jg_seat2 = rec2.JgSeatNumber;
                res.jg_poker2 = rec2.JgPokerNumber;
                res.jg_dest2 = rec2.HgSeatNumber;

                res.hg_seat2 = rec2.HgSeatNumber;
                res.hg_poker2 = rec2.HgPokerNumber;
                res.hg_dest2 = rec2.JgSeatNumber;
            }

            return res;
        }

        private void DoStartRound()
        {
            _roundCount++;

            if (_winLevel == 0)
            {
                // 如果是第一局的话，就从我自己开始出。
                _curChuPaiSeat = _seatList[0];
                StartChuPai();
            }
            else if (CheckIsKangGong())
            {
                _curChuPaiSeat = _lastRank[0];
                StartChuPai();
            }
            else
            {
                StartJinGong();
            }
        }

        #endregion

        #region 出牌

        private PokerPattern GetLastValidChuPai()
        {
            if (_lastChuPaiSeat == null)
            {
                return null;
            }

            for (int i = 0; i < SeatCount - 1; i++)
            {
                var seat = (_lastChuPaiSeat.Number - i + SeatCount) % SeatCount;

                var chuPai = _lastChuPai[seat];
                if (chuPai == null)
                {
                    continue;
                }

                if (chuPai.Type != PatternType.BUCHU)
                {
                    return chuPai;
                }
            }

            return null;
        }

        private int GetValidBuChuCount()
        {
            if (_lastChuPaiSeat == null)
            {
                return 0;
            }

            var count = 0;
            for (var i = 0; i < SeatCount - 1; i++)
            {
                var seat = (_lastChuPaiSeat.Number - i + SeatCount) % SeatCount;

                var chuPai = _lastChuPai[seat];
                if (chuPai == null)
                {
                    continue;
                }

                if (chuPai.Type != PatternType.BUCHU)
                {
                    break;
                }

                count++;
            }

            return count;
        }

        private int GetValidChuPaiCount()
        {
            var count = 0;
            for (var i = 0; i < SeatCount - 1; i++)
            {
                if (_lastChuPai[i] != null)
                {
                    count++;
                }
            }

            return count;
        }

        private Seat GetCurrentRankBottom()
        {
            if (_currentRank.Count <= 0)
            {
                return null;
            }

            return _currentRank[_currentRank.Count - 1];
        }

        private IEnumerator _autoChuPaiCoroutine;

        private IEnumerator AutoChuPaiCoroutine()
        {
            if (_curChuPaiSeat == null)
                yield break;

            // 结算的时候会将_curChuPaiSeat = null，所以此处应该检查。
            while (_curChuPaiSeat != null && _curChuPaiSeat.Number != 0)
            {
                yield return new WaitForSeconds(1);
                AutoChuPaiLogic(_curChuPaiSeat);

                if (_endState == EndRoundState)
                {
                    break;
                }
            }
        }

        private void AutoChuPaiLogic(Seat seat)
        {
            if (seat == null) return;

            _pokerAi.SetMyPokers(seat.Pokers);

            PokerPattern chuPai;
            if (_lastChuPaiSeat != null)
            {
                chuPai = _pokerAi.SelectChuPai(
                    GetLastValidChuPai(),
                    !IsMate(_lastChuPaiSeat, seat),
                    _lastChuPaiSeat.PokerCount);
            }
            else
            {
                chuPai = _pokerAi.SelectChuPai();
            }

            var msg = new ChuPai();
            msg.chupai_key = "ChuPaiKey";
            msg.chupai = PokerLogicUtil.ConvertToSessionPattern(chuPai);

            MyLog.InfoWithFrame(
                name,
                string.Format(
                    "Seat {0} chupai {1}",
                    seat,
                    PokerLogicUtil.FormatPokers(chuPai.Pokers)
                )
            );

            PlayerChuPai(seat, msg);
        }

        private void StartChuPai()
        {
            _period = TablePeriod.ChuPai;

            SendToSeat(
                _curChuPaiSeat,
                CmdUtil.GU.ChuPaiKey(
                    "ChuPaiKey",
                    30,
                    true,
                    BuildPlayingData()));

            Broadcast(
                CmdUtil.GU.BChuPaiKeyOwner(_curChuPaiSeat.Number, 30, BuildPlayingData()),
                _curChuPaiSeat.Number);

            _shiftResult = ShiftForceChuPai;

            if (_curChuPaiSeat.Number != 0)
            {
                StartCoroutine(AutoChuPaiCoroutine());
            }
        }

        private List<Poker> ToPokers(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0)
            {
                return null;
            }

            var pokers = new List<Poker>();
            for (int i = 0; i < bytes.Length; i++)
            {
                pokers.Add(new Poker(bytes[i]));
            }

            return pokers;
        }

        private PokerPeeperData GetMyPokerPeeperData()
        {
            var data = new PokerPeeperData();
            if (_period != TablePeriod.ChuPai && _period != TablePeriod.EndRound && _period != TablePeriod.BetweenRound)
            {
                return data;
            }

            if (_endState == EndRoundState)
            {
                for (int i = 0; i < _seatList.Count; i++)
                {
                    var seat = _seatList[i];
                    data.SetPokerPeeperAt(i, seat.PokerBytes());
                }

                return data;
            }

            if (_seatList[0].PokerCount <= 0)
            {
                data.SetPokerPeeperAt(2, _seatList[2].PokerBytes());
            }

            return data;
        }

        private void PlayerChuPai(Seat seat, ChuPai msg)
        {
            var pattern = _matcher.Match(ToPokers(msg.chupai.pokers));
            if (pattern.IsNull)
            {
                SendToSeat(seat,
                    CmdUtil.GU.ChuPaiResult(ResultCode.G_CHUPAI_NO_MATCHED_PATTERN, null, MyPokers,
                        BuildPlayingData()));
                return;
            }

            if (_isFirstChuPai && pattern.Type == PatternType.BUCHU)
            {
                SendToSeat(seat, CmdUtil.GU.ChuPaiResult(ResultCode.G_FIRST_BUCHU, null, MyPokers, BuildPlayingData()));
                return;
            }

            if (_shiftResult == ShiftForceChuPai &&
                pattern.Type == PatternType.BUCHU)
            {
                SendToSeat(seat,
                    CmdUtil.GU.ChuPaiResult(ResultCode.G_FORCE_CHUPAI_BUCHU, null, MyPokers, BuildPlayingData()));
                return;
            }

            if (_shiftResult == ShiftJieFeng &&
                pattern.Type == PatternType.BUCHU)
            {
                SendToSeat(seat,
                    CmdUtil.GU.ChuPaiResult(ResultCode.G_JIEFENG_BUCHU, null, MyPokers, BuildPlayingData()));
                return;
            }

            if (!_isFirstChuPai &&
                _shiftResult != ShiftForceChuPai &&
                _shiftResult != ShiftJieFeng &&
                pattern.Type != PatternType.BUCHU)
            {
                var compareRes = CanChuPai(pattern);
                if (compareRes != ResultCode.OK)
                {
                    SendToSeat(seat, CmdUtil.GU.ChuPaiResult(compareRes, null, MyPokers, BuildPlayingData()));
                    return;
                }
            }

            _endState = ChuPaiLogic(seat, pattern);

            if (_endState == EndRoundState)
            {
                SendToSeat(seat, CmdUtil.GU.ChuPaiResult(ResultCode.OK, msg.chupai, MyPokers, BuildPlayingData()));
                BroadcastChuPai(seat, pattern);

                EndRound();
                return;
            }

            SendToSeat(seat, CmdUtil.GU.ChuPaiResult(ResultCode.OK, msg.chupai, MyPokers, BuildPlayingData()));
            BroadcastChuPai(seat, pattern);

            ShiftChuPaiRight(pattern.Type == PatternType.BUCHU);

            var nextSeat = _curChuPaiSeat;
            if (_shiftResult == ShiftForceChuPai)
            {
                // 如果下一个出牌的座位是我，则发送chuPaiKey，如果下一个座位不是我，则发送BChuPaiKeyOwner。
                SendToSeat(nextSeat, CmdUtil.GU.ChuPaiKey("ChuPaiKey", 30, true, BuildPlayingData()));
                Broadcast(CmdUtil.GU.BChuPaiKeyOwner(_curChuPaiSeat.Number, 30, BuildPlayingData()), nextSeat.Number);
            }
            else if (_shiftResult == ShiftJieFeng)
            {
                Broadcast(CmdUtil.GU.BJieFeng(_curChuPaiSeat.Number, BuildPlayingData()), -1);
                SendToSeat(nextSeat, CmdUtil.GU.ChuPaiKey("ChuPaiKey", 30, true, BuildPlayingData()));
                Broadcast(CmdUtil.GU.BChuPaiKeyOwner(_curChuPaiSeat.Number, 30, BuildPlayingData()), nextSeat.Number);
            }
            else
            {
                SendToSeat(nextSeat, CmdUtil.GU.ChuPaiKey("ChuPaiKey", 30, false, BuildPlayingData()));
                Broadcast(CmdUtil.GU.BChuPaiKeyOwner(_curChuPaiSeat.Number, 30, BuildPlayingData()), nextSeat.Number);
            }

            // 出牌的座位是玩家的话，则开始自动出牌流程。
            if (seat.Number == 0)
            {
                StartCoroutine(AutoChuPaiCoroutine());
            }
        }

        private void BroadcastChuPai(Seat chuPaiSeat, PokerPattern chuPaiPattern)
        {
            if (chuPaiSeat.Number != 0)
            {
                // 如果出牌的不是自己。
                SendResponse(CmdUtil.GU.BChuPai(
                    chuPaiSeat.Number,
                    PokerLogicUtil.ConvertToSessionPattern(chuPaiPattern),
                    chuPaiSeat.PokerCount,
                    BuildPlayingData()));
            }

            // 向我发送记牌器。
            var myRecorder = GetMyPokerRecorderResult(_seatList[0]);
            SendResponse(CmdUtil.GU.PokerRecorderResult(
                ResultCode.OK,
                myRecorder));

            var mate = GetMateSeat(chuPaiSeat);

            if (chuPaiSeat.PokerCount == 0)
            {
                // 出牌座位已经出完了，向他发送对家的牌。
                SendToSeat(chuPaiSeat, CmdUtil.GU.PokerPeeperResult(
                    ResultCode.OK,
                    mate.Number,
                    mate.PokerBytes()));
            }

            if (mate.IsFinished)
            {
                // 如果对家已经出完牌了，则向他发送自己的牌。
                SendToSeat(mate, CmdUtil.GU.PokerPeeperResult(
                    ResultCode.OK,
                    chuPaiSeat.Number,
                    chuPaiSeat.PokerBytes()));
            }
        }

        private void Broadcast(ProtoMessage msg, int excludeSeat)
        {
            if (excludeSeat != 0)
                SendResponse(msg);
        }

        private void ShiftChuPaiRight(bool isBuChu)
        {
            _shiftResult = ShiftNext;

            var buChuCount = GetValidBuChuCount();
            var unfinishedCount = UnfinishedPlayerCount();
            var chuPaiCount = GetValidChuPaiCount();

            if (buChuCount >= unfinishedCount)
            {
                var rankBottom = GetCurrentRankBottom();
                var partner = GetMateSeat(rankBottom);
                // 接风的情况重置所有lastChuPai。
                _lastChuPaiSeat = null;
                for (int i = 0; i < _lastChuPai.Length; i++)
                {
                    _lastChuPai[i] = null;
                }

                _curChuPaiSeat = partner;
                _shiftResult = ShiftJieFeng;
                return;
            }

            var next = (_curChuPaiSeat.Number + 1) % SeatCount;
            while (_seatList[next].IsFinished)
            {
                _lastChuPai[next] = null;
                next = (next + 1) % SeatCount;
            }

            _lastChuPaiSeat = _curChuPaiSeat;
            _curChuPaiSeat = _seatList[next];

            var mustChuPai = false;
            var chuPai = _lastChuPai[next];
            if (chuPai != null && chuPai.Type != PatternType.BUCHU)
            {
                mustChuPai = buChuCount >= chuPaiCount;
            }

            _shiftResult = mustChuPai ? ShiftForceChuPai : ShiftNext;

            // 完成判定以后，需要清空下个出牌位置的出牌。
            _lastChuPai[next] = null;
        }

        private int UnfinishedPlayerCount()
        {
            var count = 0;
            for (int i = 0; i < _seatList.Count; i++)
            {
                if (!_seatList[i].IsFinished)
                {
                    count++;
                }
            }

            return count;
        }

        private int ChuPaiLogic(Seat seat, PokerPattern chuPaiPattern)
        {
            // 出牌的情况下记录出牌玩家和所出的牌。
            _lastChuPaiSeat = _curChuPaiSeat;
            _lastChuPai[_curChuPaiSeat.Number] = chuPaiPattern;

            if (chuPaiPattern.Type != PatternType.BUCHU)
            {
                // 出牌成功的情况下，要将这次出的牌从用户的牌中间删除掉。
                foreach (var poker in chuPaiPattern.Pokers)
                {
                    seat.RemovePoker(poker);
                    // 更新剩余的牌数。
                    UpdateUnusedPokerCount(_unusedPokerCount, poker);
                }

                if (_isFirstChuPai)
                {
                    _isFirstChuPai = false;
                }
            }

            // ------------检查是不是已经结束一局------------

            var endState = NoEndState;

            if (seat.IsFinished)
            {
                _currentRank.Add(seat);

                // 设置一下座位之间的关系。

                // 如果排名中已经有两个人了，就需要判断是否是双下的情况。如果是双下的情况就已经结束了。
                if (_currentRank.Count == 2)
                {
                    var firstSeat = _currentRank[0];
                    var secondSeat = _currentRank[1];

                    if (IsMate(firstSeat, secondSeat))
                    {
                        // 第一名、第二名是对家，则是双上的情况。

                        // 将剩下的两个人按照剩余牌数的顺序添加到排名列表里面。
                        for (int i = 0; i < _seatList.Count; i++)
                        {
                            if (!_currentRank.Contains(_seatList[i]))
                            {
                                var last = _currentRank[_currentRank.Count - 1].PokerCount;
                                var cur = _seatList[i].PokerCount;

                                if (cur <= last)
                                {
                                    _currentRank.Insert(_currentRank.Count - 1, _seatList[i]);
                                }
                                else
                                {
                                    _currentRank.Add(_seatList[i]);
                                }
                            }
                        }

                        endState = EndRoundState;
                    }
                }
                else if (_currentRank.Count == 3)
                {
                    // 排名中已经有三个人了，把最后一个人添加到排名中，直接结束。
                    for (int i = 0; i < 4; i++)
                    {
                        if (!_currentRank.Contains(_seatList[i]))
                        {
                            _currentRank.Add(_seatList[i]);
                        }
                    }

                    endState = EndRoundState;
                }
            }

            return endState;
        }

        private int CanChuPai(PokerPattern chuPaiPattern)
        {
            if (chuPaiPattern == null || chuPaiPattern.IsNull)
            {
                return -1;
            }

            var lastChuPai = GetLastValidChuPai();
            if (lastChuPai == null || lastChuPai.IsNull)
            {
                return ResultCode.OK;
            }

            int res = _value.Compare(chuPaiPattern, lastChuPai);

            if (res == 1)
            {
                return ResultCode.OK;
            }
            else
            {
                return ResultCode.G_CHUPAI_NO_GREATER;
            }
        }

        #endregion

        #region 进贡

        private void StartJinGong()
        {
            _period = TablePeriod.JinGong;

            Seat jinGongSeat1 = null;
            Seat jinGongSeat2 = null;

            if (_winLevel == 3)
            {
                _jinHuanGongRecordList.Add(new JinHuanGongRecord
                {
                    JgSeat = _lastRank[2]
                });
                _jinHuanGongRecordList.Add(new JinHuanGongRecord
                {
                    JgSeat = _lastRank[3]
                });

                jinGongSeat1 = _lastRank[2];
                jinGongSeat2 = _lastRank[3];
            }
            else
            {
                _jinHuanGongRecordList.Add(new JinHuanGongRecord
                {
                    JgSeat = _lastRank[3]
                });

                jinGongSeat1 = _lastRank[3];
            }

            Broadcast(
                CmdUtil.GU.BJinGongRequest(
                    jinGongSeat1.Number,
                    jinGongSeat2 != null ? jinGongSeat2.Number : -1,
                    30,
                    BuildPlayingData()),
                -1
            );

            StartCoroutine(AutoJinGongCoroutine(jinGongSeat1, jinGongSeat2));
        }

        private IEnumerator AutoJinGongCoroutine(Seat seat1, Seat seat2 = null)
        {
            if (seat1 != null && seat1.Number != 0)
            {
                yield return new WaitForSeconds(3);
                var selected = AutoSelectJinGong(seat1.Pokers);
                JinGongLogic(seat1, selected);
            }

            if (seat2 != null && seat2.Number != 0)
            {
                yield return new WaitForSeconds(3);
                var selected = AutoSelectJinGong(seat2.Pokers);
                JinGongLogic(seat2, selected);
            }
        }

        private bool CheckIsKangGong()
        {
            if (_winLevel == 3)
            {
                var rank4 = _lastRank[3];
                var rank3 = _lastRank[2];

                var haspD4 = haspD(rank4.Pokers);
                var haspD3 = haspD(rank3.Pokers);

                ProtoMessage msg;
                if (haspD4 + haspD3 >= 2)
                {
                    if (haspD4 > haspD3)
                    {
                        msg = CmdUtil.GU.BKangGong(rank4.Number, -1, BuildPlayingData());
                    }
                    else if (haspD4 < haspD3)
                    {
                        msg = CmdUtil.GU.BKangGong(rank3.Number, -1, BuildPlayingData());
                    }
                    else
                    {
                        msg = CmdUtil.GU.BKangGong(rank4.Number, rank3.Number, BuildPlayingData());
                    }

                    SendResponse(msg);
                    return true;
                }
            }
            else
            {
                var rank4 = _lastRank[3];
                var haspD4 = haspD(rank4.Pokers);

                if (haspD4 >= 2)
                {
                    SendResponse(CmdUtil.GU.BKangGong(rank4.Number, -1, BuildPlayingData()));
                    return true;
                }
            }

            return false;
        }

        private bool IsJinGongRequested(int seat)
        {
            if (_jinHuanGongRecordList.Count <= 0)
            {
                return false;
            }

            foreach (var rec in _jinHuanGongRecordList)
            {
                if (rec == null)
                {
                    continue;
                }

                if (rec.JgSeatNumber == seat)
                {
                    return true;
                }
            }

            return false;
        }

        private bool AlreadyJinGong(int seat)
        {
            if (_jinHuanGongRecordList.Count <= 0)
            {
                return false;
            }

            foreach (var rec in _jinHuanGongRecordList)
            {
                if (rec == null)
                {
                    continue;
                }

                if (rec.JgSeatNumber == seat && rec.JgPoker != null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsJinGongFinished()
        {
            if (_jinHuanGongRecordList.Count <= 0)
            {
                return false;
            }

            foreach (var rec in _jinHuanGongRecordList)
            {
                if (rec == null)
                {
                    continue;
                }

                if (rec.JgPoker == null)
                {
                    return false;
                }
            }

            return true;
        }

        private JinHuanGongRecord GetJinHuanGongRecord(int index)
        {
            if (index < 0 || index >= _jinHuanGongRecordList.Count)
            {
                return null;
            }

            return _jinHuanGongRecordList[index];
        }

        private JinHuanGongRecord GetJinHuanGongRecordByJgSeat(int seat)
        {
            foreach (var rec in _jinHuanGongRecordList)
            {
                if (rec == null)
                {
                    continue;
                }

                if (rec.JgSeatNumber == seat)
                {
                    return rec;
                }
            }

            return null;
        }

        private void PlayerJinGong(JinGong msg)
        {
            // 处理玩家的进贡命令。
            if (!IsJinGongRequested(_seatList[0].Number))
            {
                SendResponse(CmdUtil.GU.JinGongResult(
                    ResultCode.G_INVALID_JINGONG, msg.poker_id, MyPokers, BuildPlayingData()));
                return;
            }

            if (AlreadyJinGong(_seatList[0].Number))
            {
                SendResponse(CmdUtil.GU.JinGongResult(
                    ResultCode.G_INVALID_JINGONG, msg.poker_id, MyPokers, BuildPlayingData()));
                return;
            }

            var poker = new Poker(msg.poker_id);
            if (IsHeartHost(poker))
            {
                SendResponse(CmdUtil.GU.JinGongResult(
                    ResultCode.G_INVALID_JINGONG, msg.poker_id, MyPokers, BuildPlayingData()));
                return;
            }

            var autoJinGongPoker = AutoSelectJinGong(_seatList[0].Pokers);
            if (poker.NumType != autoJinGongPoker.NumType)
            {
                SendResponse(CmdUtil.GU.JinGongResult(
                    ResultCode.G_INVALID_JINGONG, msg.poker_id, MyPokers, BuildPlayingData()));
                return;
            }

            JinGongLogic(_seatList[0], poker);
        }

        private Poker AutoSelectJinGong(List<Poker> pokers)
        {
            if (pokers == null || pokers.Count <= 0)
                return null;

            Poker found = null;
            pokers.Sort(_value.Compare);
            for (int i = pokers.Count - 1; i >= 0; i--)
            {
                var p = pokers[i];
                if (!IsHeartHost(p))
                {
                    found = p;
                    break;
                }
            }

            return found;
        }

        private void JinGongLogic(Seat seat, Poker poker)
        {
            if (IsJinGongFinished())
            {
                SendToSeat(seat,
                    CmdUtil.GU.JinGongResult(ResultCode.G_INVALID_JINGONG, poker.Number, MyPokers, BuildPlayingData()));
                return;
            }

            var rec = GetJinHuanGongRecordByJgSeat(seat.Number);
            if (rec == null)
            {
                SendToSeat(seat,
                    CmdUtil.GU.JinGongResult(ResultCode.G_INVALID_JINGONG, poker.Number, MyPokers, BuildPlayingData()));
                return;
            }

            if (rec.JgPoker != null)
            {
                SendToSeat(seat,
                    CmdUtil.GU.JinGongResult(ResultCode.G_INVALID_JINGONG, poker.Number, MyPokers, BuildPlayingData()));
                return;
            }

            rec.JgPoker = poker;
            seat.RemovePoker(poker);

            if (seat.Number == 0)
            {
                SendToSeat(seat, CmdUtil.GU.JinGongResult(ResultCode.OK, poker.Number, MyPokers, BuildPlayingData()));
            }

            Broadcast(CmdUtil.GU.BJinGongPoker(seat.Number, poker.Number, BuildPlayingData()), -1);

            if (IsJinGongFinished())
            {
                var rec1 = GetJinHuanGongRecord(0);
                var rec2 = GetJinHuanGongRecord(1);

                if (_jinHuanGongRecordList.Count >= 2)
                {
                    var res = _value.Compare(rec1.JgPoker, rec2.JgPoker);

                    if (res == CompareResult.BIGGER)
                    {
                        rec1.HgSeat = _lastRank[0];
                        rec1.HgSeat.AddPoker(rec1.JgPoker);
                        rec2.HgSeat = _lastRank[1];
                        rec2.HgSeat.AddPoker(rec2.JgPoker);
                    }
                    else
                    {
                        rec1.HgSeat = _lastRank[1];
                        rec1.HgSeat.AddPoker(rec1.JgPoker);
                        rec2.HgSeat = _lastRank[0];
                        rec2.HgSeat.AddPoker(rec2.JgPoker);
                    }
                }
                else
                {
                    rec1.HgSeat = _lastRank[0];
                    rec1.HgSeat.AddPoker(rec1.JgPoker);
                }

                Broadcast(
                    CmdUtil.GU.BJinGongResult(
                        rec1 != null ? rec1.JgSeatNumber : -1,
                        rec1 != null ? rec1.HgSeatNumber : -1,
                        rec1 != null ? rec1.JgPokerNumber : -1,
                        rec2 != null ? rec2.JgSeatNumber : -1,
                        rec2 != null ? rec2.HgSeatNumber : -1,
                        rec2 != null ? rec2.JgPokerNumber : -1,
                        BuildPlayingData()),
                    -1);

                if (rec1 != null && rec1.HgSeatNumber == 0)
                {
                    SendToSeat(rec1.HgSeat,
                        CmdUtil.GU.BeenJinGong(rec1.JgSeatNumber, rec1.JgPokerNumber, MyPokers, BuildPlayingData()));
                }

                if (rec2 != null && rec2.HgSeatNumber == 0)
                {
                    SendToSeat(rec2.HgSeat,
                        CmdUtil.GU.BeenJinGong(rec2.JgSeatNumber, rec2.JgPokerNumber, MyPokers, BuildPlayingData()));
                }

                _period = TablePeriod.HuanGong;

                Broadcast(
                    CmdUtil.GU.BHuanGongRequest(
                        rec1 != null ? rec1.HgSeatNumber : -1,
                        rec2 != null ? rec2.HgSeatNumber : -1,
                        30,
                        BuildPlayingData()
                    ),
                    -1
                );

                // 开始自动还贡的流程。
                StartCoroutine(
                    AutoHuanGongCoroutine(
                        rec1 != null ? rec1.HgSeat : null,
                        rec2 != null ? rec2.HgSeat : null)
                );
            }
        }

        #endregion

        #region 还贡

        private bool IsHuanGongRequested(int seat)
        {
            foreach (var rec in _jinHuanGongRecordList)
            {
                if (rec == null)
                {
                    continue;
                }

                if (rec.HgSeatNumber == seat)
                {
                    return true;
                }
            }

            return false;
        }

        private bool AlreadyHuanGong(int seat)
        {
            foreach (var rec in _jinHuanGongRecordList)
            {
                if (rec == null)
                {
                    continue;
                }

                if (rec.HgSeatNumber == seat && rec.HgPoker != null)
                {
                    return true;
                }
            }

            return false;
        }

        private JinHuanGongRecord GetJinHuanGongRecordByHgSeat(int seat)
        {
            foreach (var rec in _jinHuanGongRecordList)
            {
                if (rec == null)
                {
                    continue;
                }

                if (rec.HgSeatNumber == seat)
                {
                    return rec;
                }
            }

            return null;
        }

        private bool IsHuanGongFinished()
        {
            foreach (var rec in _jinHuanGongRecordList)
            {
                if (rec == null)
                {
                    continue;
                }

                if (rec.HgPoker == null)
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerator AutoHuanGongCoroutine(Seat seat1, Seat seat2 = null)
        {
            if (seat1 != null && seat1.Number != 0)
            {
                yield return new WaitForSeconds(1);
                var selected = AutoSelectHuanGong(seat1.Pokers);
                HuanGongLogic(seat1, selected);
            }

            if (seat2 != null && seat2.Number != 0)
            {
                yield return new WaitForSeconds(1);
                var selected = AutoSelectHuanGong(seat2.Pokers);
                HuanGongLogic(seat2, selected);
            }
        }

        private Poker AutoSelectHuanGong(List<Poker> pokers)
        {
            if (pokers == null || pokers.Count <= 0)
                return null;

            pokers.Sort(_value.Compare);
            return pokers[0];
        }

        private void PlayerHuanGong(HuanGong msg)
        {
            if (!IsHuanGongRequested(0))
            {
                SendResponse(CmdUtil.GU.HuanGongResult(
                    ResultCode.G_INVALID_HUANGONG, msg.poker_id, null, BuildPlayingData()));
                return;
            }

            if (AlreadyHuanGong(0))
            {
                SendResponse(CmdUtil.GU.HuanGongResult(
                    ResultCode.G_INVALID_HUANGONG, msg.poker_id, null, BuildPlayingData()));
                return;
            }

            var poker = new Poker(msg.poker_id);
            if (IsHeartHost(poker))
            {
                SendResponse(CmdUtil.GU.HuanGongResult(
                    ResultCode.G_INVALID_HUANGONG, msg.poker_id, null, BuildPlayingData()));
                return;
            }

            HuanGongLogic(_seatList[0], poker);
        }

        private void HuanGongLogic(Seat from, Poker poker)
        {
            var rec = GetJinHuanGongRecordByHgSeat(from.Number);
            if (rec == null)
            {
                return;
            }

            var to = rec.JgSeat;
            if (to == null)
            {
                return;
            }

            rec.HgPoker = poker;
            to.AddPoker(poker);
            from.RemovePoker(poker);

            if (from.Number == 0)
            {
                SendToSeat(from, CmdUtil.GU.HuanGongResult(ResultCode.OK, poker.Number, MyPokers, BuildPlayingData()));
            }

            Broadcast(CmdUtil.GU.BHuanGongPoker(from.Number, to.Number, poker.Number, BuildPlayingData()), -1);

            if (to.Number == 0)
            {
                SendToSeat(to, CmdUtil.GU.BeenHuanGong(from.Number, poker.Number, MyPokers, BuildPlayingData()));
            }

            if (IsHuanGongFinished())
            {
                // 进贡较大的那个人出牌。
                // 如果两个人进贡的牌是一样的。那么应该就是最后一名的那个人出牌。
                JinHuanGongRecord maxJinGong = null;
                Seat maxJinGongSeat = null;

                foreach (var r in _jinHuanGongRecordList)
                {
                    if (maxJinGong == null)
                    {
                        maxJinGong = r;
                        maxJinGongSeat = r.JgSeat;
                    }
                    else
                    {
                        var result = _value.Compare(rec.JgPoker, maxJinGong.JgPoker);
                        if (result == CompareResult.BIGGER)
                        {
                            maxJinGong = r;
                            maxJinGongSeat = r.JgSeat;
                        }
                        else if (result == CompareResult.EQUAL)
                        {
                            maxJinGong = r;
                            // 如果两个进贡是一样的话，则出牌权给最后输的那个人。
                            maxJinGongSeat = _lastRank[_lastRank.Count - 1];
                        }
                    }
                }

                _curChuPaiSeat = maxJinGongSeat;
                StartChuPai();
            }
        }

        #endregion

        #region 结算

        private void EndRound()
        {
            _period = TablePeriod.EndRound;

            for (int i = 0; i < _seatList.Count; i++)
            {
                var seat = _seatList[i];
                if (seat.Number != 0)
                {
                    SendResponse(
                        CmdUtil.GU.PokerPeeperResult(
                            ResultCode.OK,
                            seat.Number,
                            seat.PokerBytes()
                        ));
                }
            }

            // 先记录一下上一次是哪一队赢了。
            var lastWinTeam = _team1Win ? 1 : 2;

            // 首先确定是哪个队赢了。
            var firstSeat = _currentRank[0].Number;

            _team1Win = firstSeat == 0 || firstSeat == 2;

            // 赢的一队的对家的座位
            var mate = GetMateSeat(_currentRank[0]);

            // 确定对家的名次
            var mateRank = _currentRank.IndexOf(mate);

            // 判断赢的等级。
            if (mateRank == 1)
                _winLevel = 3;
            else if (mateRank == 2)
                _winLevel = 2;
            else
                _winLevel = 1;

            var isFanshan = DetermineNextHost(_winLevel, _team1Win, lastWinTeam);

            var table = new Table();
            table.room_id = 0;
            table.table_id = 0;
            table.player_count = 4;

            table.round_count = _roundCount;

            table.target_host = com.morln.game.gd.command.PokerNumType.p2;
            table.host_team = _team1Win ? 1 : 2;
            table.team1_host = PokerLogicUtil.ToSessionNumType(_team1Host);
            table.team2_host = PokerLogicUtil.ToSessionNumType(_team2Host);

            table.user1 = _seatList[0].User;
            table.user2 = _seatList[1].User;
            table.user3 = _seatList[2].User;
            table.user4 = _seatList[3].User;

            // 返回金蛋和天梯积分的结算结果
            var roundEnd = CmdUtil.GU.BRoundEnd(
                _currentRank[0].Username,
                _currentRank[1].Username,
                _currentRank[2].Username,
                _currentRank[3].Username,
                PokerLogicUtil.ToSessionNumType(_team1Host),
                PokerLogicUtil.ToSessionNumType(_team2Host),
                _team1Win ? 1 : 2,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                -1,
                0, 0, 0, 0,
                table,
                1,
                BuildPlayingData()
            );

            // 广播结算信息。
            SendResponse(roundEnd);
            // 广播一下当前的host信息。
            SendResponse(CmdUtil.GU.HostInfoResult(
                PokerLogicUtil.ToSessionNumType(_team1Host),
                PokerLogicUtil.ToSessionNumType(_team2Host),
                _team1Win ? 1 : 2, BuildPlayingData()));

            if (isFanshan)
            {
                var r = _roundCount;
                var f = _fanShanCount;
                ResetTable();
                _roundCount = r;
                _fanShanCount = f;
            }

            _lastRank.Clear();
            _lastRank.AddRange(_currentRank);

            // 重置一局的状态。
            ResetRound();
        }

        public bool DetermineNextHost(int winLevel, bool team1Win, int lastWinTeam)
        {
            var isFanshan = false;

            // 计算下一个主牌
            if (team1Win)
            {
                var nextHost = _team1Host + winLevel;

                if (nextHost >= PokerNumType.PA)
                {
                    nextHost = PokerNumType.PA;
                    _targetATimes1++;
                }

                _team1Host = nextHost;
                _currentHost = _team1Host;
            }
            else
            {
                var nextHost = _team2Host + winLevel;

                if (nextHost >= PokerNumType.PA)
                {
                    nextHost = PokerNumType.PA;
                    _targetATimes2++;
                }

                _team2Host = nextHost;
                _currentHost = _team2Host;
            }

            // TIP isFanshan 必须要在结算前设置，因为会影响结算结果。
            // 当且仅当某队伍在台上打A、赢了、没有最后一名，才认为是翻山，三次没翻过就回2。
            if ((team1Win && lastWinTeam == 1 && _targetATimes1 >= 2 && winLevel >= 2) ||
                (!team1Win && lastWinTeam == 2 && _targetATimes2 >= 2 && winLevel >= 2))
            {
                isFanshan = true;
                _fanShanCount++;
            }
            else
            {
                if (_targetATimes2 > 3)
                {
                    _team2Host = PokerNumType.P2;
                    _currentHost = PokerNumType.P2;
                    _targetATimes2 = 0;
                }

                if (_targetATimes1 > 3)
                {
                    _team1Host = PokerNumType.P2;
                    _currentHost = PokerNumType.P2;
                    _targetATimes1 = 0;
                }
            }

            return isFanshan;
        }

        #endregion

        #region 工具方法

        private int haspD(List<Poker> list)
        {
            if (list == null || list.Count <= 0)
                return 0;

            var count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].NumType == PokerNumType.PD)
                    count++;
            }

            return count;
        }

        private bool IsHeartHost(Poker poker)
        {
            return poker.NumType == _currentHost && poker.SuitType == PokerSuitType.HEART;
        }

        private List<int> GetMyPokerRecorderResult(Seat seat)
        {
            if (_period != TablePeriod.ChuPai)
            {
                return null;
            }

            var pokers = seat.Pokers;
            var myRecorder = new int[_unusedPokerCount.Length];
            Array.Copy(_unusedPokerCount, 0, myRecorder, 0, _unusedPokerCount.Length);

            for (int i = 0; i < pokers.Count; i++)
            {
                UpdateUnusedPokerCount(myRecorder, pokers[i]);
            }

            var result = new List<int>();
            for (int i = 0; i < myRecorder.Length; i++)
            {
                result.Add(myRecorder[i]);
            }

            return result;
        }

        private bool IsMate(Seat s1, Seat s2)
        {
            return (s1.Number + s2.Number) % 2 == 0;
        }

        private Seat GetMateSeat(Seat seat)
        {
            if (seat == null)
            {
                return null;
            }

            switch (seat.Number)
            {
                case 0:
                    return _seatList[2];

                case 1:
                    return _seatList[3];

                case 2:
                    return _seatList[0];

                case 3:
                    return _seatList[1];

                default:
                    return null;
            }
        }

        private void UpdateUnusedPokerCount(int[] unusedPokerCount, Poker used)
        {
            if (used.NumType >= PokerNumType.P2 &&
                used.NumType <= PokerNumType.PA)
            {
                unusedPokerCount[used.NumType - 2]--;
            }
            else if (used.NumType == PokerNumType.PX)
            {
                unusedPokerCount[13]--;
            }
            else if (used.NumType == PokerNumType.PD)
            {
                unusedPokerCount[14]--;
            }
        }

        private byte[] MyPokers
        {
            get
            {
                if (_seatList.Count <= 0)
                    return null;

                var seat = _seatList[0];
                return seat.PokerBytes();
            }
        }

        #endregion

        public class Seat
        {
            public int Number;

            public List<Poker> Pokers;

            public User User;

            public string Username
            {
                get
                {
                    if (User == null)
                        return null;

                    return User.username;
                }
            }

            public int PokerCount
            {
                get
                {
                    if (Pokers == null)
                        return 0;

                    return Pokers.Count;
                }
            }

            public bool IsFinished
            {
                get { return Pokers == null || Pokers.Count <= 0; }
            }

            public void AddPoker(Poker poker)
            {
                if (poker == null)
                    return;

                if (Pokers == null)
                    Pokers = new List<Poker>();

                if (!Pokers.Contains(poker))
                    Pokers.Add(poker);
            }

            public void RemovePoker(Poker poker)
            {
                if (poker == null)
                    return;

                if (Pokers == null || Pokers.Count <= 0)
                    return;

                Pokers.Remove(poker);
            }

            public void RemovePokers(List<Poker> list)
            {
                if (list == null || list.Count <= 0)
                    return;

                if (Pokers == null || Pokers.Count <= 0)
                    return;

                Pokers.RemoveAll(list.Contains);
            }

            public byte[] PokerBytes()
            {
                if (Pokers == null || Pokers.Count <= 0)
                    return null;

                var bytes = new byte[Pokers.Count];
                for (int i = 0; i < Pokers.Count; i++)
                {
                    bytes[i] = (byte) Pokers[i].Number;
                }

                return bytes;
            }

            public override string ToString()
            {
                return string.Format("Seat {0}", Number);
            }
        }

        public class JinHuanGongRecord
        {
            public Seat JgSeat;

            public Poker JgPoker;

            public Seat HgSeat;

            public Poker HgPoker;

            public int JgSeatNumber
            {
                get { return JgSeat != null ? JgSeat.Number : -1; }
            }

            public int JgPokerNumber
            {
                get { return JgPoker != null ? JgPoker.Number : -1; }
            }

            public int HgSeatNumber
            {
                get { return HgSeat != null ? HgSeat.Number : -1; }
            }

            public int HgPokerNumber
            {
                get { return HgPoker != null ? HgPoker.Number : -1; }
            }
        }

        public class PokerPeeperData
        {
            public byte[] PokerPeeper1;
            public byte[] PokerPeeper2;
            public byte[] PokerPeeper3;
            public byte[] PokerPeeper4;

            public void SetPokerPeeperAt(int seat, byte[] data)
            {
                switch (seat)
                {
                    case 0:
                        PokerPeeper1 = data;
                        break;

                    case 1:
                        PokerPeeper2 = data;
                        break;

                    case 2:
                        PokerPeeper3 = data;
                        break;

                    case 3:
                        PokerPeeper4 = data;
                        break;
                }
            }
        }

        #endregion
    }
}