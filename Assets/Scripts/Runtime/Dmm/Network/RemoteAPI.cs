using System;
using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.App;
using Dmm.Clipboard;
using Dmm.Common;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Sound;
using Dmm.Task;
using Dmm.Util;
using Zenject;

namespace Dmm.Network
{
    public class RemoteAPI
    {
        #region Inject

        private IMsgRepo _msgRepo;

        private IAppController _appController;

        private IMessageRouter _messageRouter;

        private INetworkManager _network;

        private IDialogManager _dialogManager;

        private ISoundController _soundController;

        private ITaskManager _taskManager;

        private IAnalyticManager _analyticManager;

        private ConfigHolder _configHolder;

        private IClipboardManager _clipboardManager;

        private IDataRepository _dataRepository;

        private IDataContainer<User> _myUser;

        private IDataContainer<RequestAwardResult> _requestAwardResult;

        private IDataContainer<ChooseRoomResult> _chooseRoomResult;

        private IDataContainer<ChooseRoomFail> _chooseRoomFail;

        private IDataContainer<LeaveRoomResult> _leaveRoomResult;

        private IDataContainer<ChooseTableResult> _chooseTableResult;

        private IDataContainer<LeaveTableResult> _leaveTableResult;

        private IDataContainer<Room> _currentRoom;

        private IDataContainer<PlayingData> _playingData;

        private IDataContainer<TableUserData> _tableUserData;

        private IDataContainer<ExchangeResult> _exchangeResult;

        private IDataContainer<RequestExchangeVipResult> _requestExchangeVipResult;

        private IDataContainer<TradeNoResult> _tradeNoResult;

        private IDataContainer<CheckTradeResult> _checkTradeResult;

        private IDataContainer<CUSearchUserResult> _searchUserResult;

        private IDataContainer<SFriendDetailResult> _friendDetailResult;

        private IDataContainer<SRemoveFriendResultToSender> _removeFriendResultToSender;

        private IDataContainer<TraceUserResult> _traceUserResult;

        private IDataContainer<ExchangeYuanBaoResult> _exchangeYuanBaoResult;

        [Inject]
        public void Initialize(
            IMsgRepo msgRepo,
            IAppController appController,
            ISoundController soundController,
            IMessageRouter messageRouter,
            INetworkManager network,
            ITaskManager taskManager,
            IDialogManager dialogManager,
            IAnalyticManager analyticManager,
            ConfigHolder configHolder,
            IDataRepository dataRepository,
            IClipboardManager clipboardManager)
        {
            _msgRepo = msgRepo;
            _appController = appController;
            _soundController = soundController;
            _messageRouter = messageRouter;
            _network = network;
            _taskManager = taskManager;
            _analyticManager = analyticManager;
            _dialogManager = dialogManager;
            _configHolder = configHolder;
            _clipboardManager = clipboardManager;

            _dataRepository = dataRepository;
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
            _requestAwardResult = dataRepository.GetContainer<RequestAwardResult>(DataKey.RequestAwardResult);
            _chooseRoomResult = dataRepository.GetContainer<ChooseRoomResult>(DataKey.ChooseRoomResult);
            _chooseRoomFail = dataRepository.GetContainer<ChooseRoomFail>(DataKey.ChooseRoomFail);
            _leaveRoomResult = dataRepository.GetContainer<LeaveRoomResult>(DataKey.LeaveRoomResult);
            _chooseTableResult = dataRepository.GetContainer<ChooseTableResult>(DataKey.ChooseTableResult);
            _leaveTableResult = dataRepository.GetContainer<LeaveTableResult>(DataKey.LeaveTableResult);
            _currentRoom = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
            _tableUserData = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
            _exchangeResult = dataRepository.GetContainer<ExchangeResult>(DataKey.ExchangeResult);
            _requestExchangeVipResult =
                dataRepository.GetContainer<RequestExchangeVipResult>(DataKey.RequestExchangeVipResult);
            _tradeNoResult = dataRepository.GetContainer<TradeNoResult>(DataKey.TradeNoResult);
            _checkTradeResult = dataRepository.GetContainer<CheckTradeResult>(DataKey.CheckTradeResult);
            _searchUserResult = dataRepository.GetContainer<CUSearchUserResult>(DataKey.SearchUserResult);
            _friendDetailResult = dataRepository.GetContainer<SFriendDetailResult>(DataKey.SFriendDetailResult);
            _removeFriendResultToSender =
                dataRepository.GetContainer<SRemoveFriendResultToSender>(DataKey.SRemoveFriendResultToSender);
            _traceUserResult = dataRepository.GetContainer<TraceUserResult>(DataKey.TraceUserResult);
            _exchangeYuanBaoResult = dataRepository.GetContainer<ExchangeYuanBaoResult>(DataKey.ExchangeYuanBaoResult);
        }

        #endregion

        #region 登陆

        public void GetVersionData(int clientVersion, string saleChannel, string product, int platform, int network,
            string device, string deviceId)
        {
            var msg = CmdUtil.PU.ClientVersion(
                clientVersion,
                saleChannel,
                product,
                platform,
                network,
                device,
                deviceId);
            _msgRepo.SendMsg(msg);
        }

        public void PLogin(string username, string password)
        {
            var msg = CmdUtil.PU.Login(username, password);
            _msgRepo.SendMsg(msg);
        }

        public void PVisitorLogin(string nickname, string deviceId, string username)
        {
            var msg = CmdUtil.PU.VisitorLogin(nickname, deviceId, username);
            _msgRepo.SendMsg(msg);
        }

        public void Login(string username, string token)
        {
            var msg = CmdUtil.HU.Login(username, token);
            _msgRepo.SendMsg(msg);
        }

        public void VisitorLogin(string visitor, string visitorUsername)
        {
            var msg = CmdUtil.PU.VisitorLogin("", visitor, visitorUsername);
            _msgRepo.SendMsg(msg);
        }

        public void WechatLogin(string openId)
        {
            var msg = CmdUtil.PU.WechatLogin(openId);
            _msgRepo.SendMsg(msg);
        }

        public void WechatAuth(string authCode)
        {
            var msg = CmdUtil.PU.WechatAuth(authCode);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 微信绑定

        public void WechatBind(string authCode)
        {
            var msg = CmdUtil.PU.WechatBind(authCode);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 登陆奖励

        public void RequestLoginReward()
        {
            _requestAwardResult.ClearNotInvalidate();
            // 从6.1版本开始，已经不区别登陆奖励的类型了。
            var msg = CmdUtil.HU.RequestLoginReward(0);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 奖励

        public void RequestAward(int awardType, string awardCode)
        {
            if (string.IsNullOrEmpty(awardCode))
                return;

            if (awardType == 1)
            {
                RequestAward(awardCode);
            }
            else if (awardType == 2)
            {
                var myUser = _myUser.Read();
                RequestUserAward(myUser.Username(), awardCode);
            }
        }

        public void RequestAward(string awardCode)
        {
            if (string.IsNullOrEmpty(awardCode))
            {
                return;
            }

            Waiting(true);

            _requestAwardResult.ClearNotInvalidate();
            _taskManager.ExecuteTask(CheckAwardResult, () => Waiting(false));

            var msg = CmdUtil.HU.RequestAward(awardCode);
            _msgRepo.SendMsg(msg);
        }

        public void RequestUserAward(string username, string awardCode)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(awardCode))
            {
                return;
            }

            Waiting(true);

            _requestAwardResult.ClearNotInvalidate();
            _taskManager.ExecuteTask(CheckAwardResult, () => Waiting(false));

            var msg = CmdUtil.HU.RequestUserAward(username, awardCode);
            _msgRepo.SendMsg(msg);
        }

        private bool CheckAwardResult()
        {
            var res = _requestAwardResult.Read(true);
            if (res == null)
            {
                return false;
            }

            Waiting(false);
            return true;
        }

        #endregion

        #region 注册

        public void Register(string username, string nickname, string password, int sex)
        {
            var seq = new RegisterSeq(username, nickname, password, sex, _appController, _messageRouter,
                _network, _dialogManager, this, _dataRepository);
            _taskManager.ExecuteSeq(seq);
        }

        public void PRegister(string username, string password, string nickname, int sex)
        {
            var msg = CmdUtil.PU.Register(username, password, nickname, sex);
            _msgRepo.SendMsg(msg);
        }

        public void HRegister(string username, string password, string nickname, int sex)
        {
            var msg = CmdUtil.HU.HRegister(username, password, sex, nickname, null);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region User

        public void RequestUserInfo()
        {
            var user = _myUser.Read();
            if (user != null)
            {
                var msg = CmdUtil.HU.RequestUserInfo(user.username, null);
                _msgRepo.SendMsg(msg);
            }
        }

        public void EditUserInfo(string nickname, int sex, string email, string description, string city)
        {
            var msg = CmdUtil.HU.EditUserInfo(nickname, sex, email, description, city);
            _msgRepo.SendMsg(msg);
        }

        public void VisitorChooseNickname(string nickname, int sex)
        {
            var msg = CmdUtil.HU.ChooseNickname(nickname, sex);
            _msgRepo.SendMsg(msg);
        }

        public void VisitorRegularize(string username, string nickname, string password, int sex)
        {
            var msg = CmdUtil.HU.VisitorRegularize(username, nickname, password, sex);
            _msgRepo.SendMsg(msg);
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            var msg = CmdUtil.HU.EditPassword(oldPassword, newPassword);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region ChooseRoom

        /// <summary>
        /// 执行选房操作
        /// </summary>
        /// <param name="roomId"></param>
        public void ChooseRoom(int roomId)
        {
            // 显示等待对话框。
            Waiting(true);

            _chooseRoomResult.ClearNotInvalidate();
            _chooseRoomFail.ClearNotInvalidate();
            _taskManager.ExecuteTask(
                CheckChooseRoomResult,
                () => Waiting(false));

            // 发送选房的命令。
            var msg = CmdUtil.HU.ChooseRoom(roomId, 0);
            _msgRepo.SendMsg(msg);
        }

        public const int ChooseRoomOk = 0;

        private bool HasChooseRoomResult()
        {
            var chooseRoomResult = _chooseRoomResult.Read();
            var chooseRoomFail = _chooseRoomFail.Read();
            return chooseRoomResult != null || chooseRoomFail != null;
        }

        private bool CheckChooseRoomResult()
        {
            if (!HasChooseRoomResult())
            {
                return false;
            }

            // 有结果数据的情况下，先结束等待对话框。
            Waiting(false);

            // 现在选房失败会直接弹出对话框，不再需要什么逻辑了。
            return true;
        }

        #endregion

        #region LeaveRoom

        public void LeaveRoom(bool confirmLeave)
        {
            Waiting(true);

            _leaveRoomResult.ClearNotInvalidate();
            _taskManager.ExecuteTask(
                CheckLeaveRoomResult,
                () => Waiting(false));

            var msg = CmdUtil.HU.LeaveRoom(confirmLeave);
            _msgRepo.SendMsg(msg);
        }

        private bool CheckLeaveRoomResult()
        {
            var res = _leaveRoomResult.Read(true);
            if (res == null)
            {
                return false;
            }

            Waiting(false);
            return true;
        }

        #endregion

        #region ChooseTable

        public void ChooseTable(long tableId)
        {
            Waiting(true);

            _chooseTableResult.ClearNotInvalidate();
            _taskManager.ExecuteTask(CheckChooseTableResult, () => Waiting(false));

            var msg = CmdUtil.GU.ChooseTable(tableId, PlayerType.Player);
            _msgRepo.SendMsg(msg);
        }

        public void MatchTable(bool confirmLeave)
        {
            Waiting(true);

            var msg = CmdUtil.GU.Match(confirmLeave);
            _msgRepo.SendMsg(msg);

            _taskManager.ExecuteTask(CheckChooseTableResult, () => Waiting(false));
        }

        private bool CheckChooseTableResult()
        {
            var res = _chooseTableResult.Read();
            if (res == null)
            {
                return false;
            }

            Waiting(false);

            if (res.result == ResultCode.OK)
            {
                // 选桌成功不需要做什么事情。
            }
            else
            {
                switch (res.result)
                {
                    case ResultCode.G_CHOOSE_TABLE_NOT_FOUND:
                        Toast("选桌失败，桌子不存在！", true);
                        break;

                    case ResultCode.G_TABLE_PLAYING_CANNOT_LEAVE:
                        Toast("选桌失败，您当前还在其他桌子中打牌！", true);
                        break;

                    case ResultCode.G_TABLE_FULL:
                        Toast("选桌失败，桌子已满！", true);
                        break;

                    // 默认错误的情况下，服务器端会发toast过来。
                }
            }

            return true;
        }

        #endregion

        #region LeaveTable

        public void LeaveTable(bool force)
        {
            Waiting(true);

            // 直接退出桌子。
            _leaveTableResult.ClearNotInvalidate();
            _taskManager.ExecuteTask(
                CheckLeaveTableResult,
                () => Waiting(false));

            // 向服务器发送一个离桌的消息。
            var msg = CmdUtil.GU.LeaveTable(force);
            _msgRepo.SendMsg(msg);
        }

        private bool CheckLeaveTableResult()
        {
            var res = _leaveTableResult.Read(true);
            if (res != null)
            {
                return false;
            }

            Waiting(false);
            return true;
        }

        #endregion

        #region 选座界面逻辑

        public void Ready(bool ready = true)
        {
            var msg = CmdUtil.GU.Ready(ready);
            _msgRepo.SendMsg(msg);
        }

        public void ChooseSeat(int seat)
        {
            var msg = CmdUtil.GU.ChooseSeat(seat);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 踢人

        public void KickOut(int seat)
        {
            var msg = CmdUtil.GU.KickOut(seat);
            _msgRepo.SendMsg(msg);

            _analyticManager.Event("try_kickout_other");
        }

        #endregion

        #region 玩家交互

        public void DoInteraction(int code, string targetUsername)
        {
            var msg = CmdUtil.GU.DoInteraction(code, targetUsername);
            _msgRepo.SendMsg(msg);

            // 统计交互的事件：
            // 主要统计玩家在哪个房间，大概多少钱的时候使用动画表情。
            var room = _currentRoom.Read();
            if (room == null)
            {
                return;
            }

            var attrs = new Dictionary<string, string>();
            attrs.Add("room_id", "" + room.room_id);
            attrs.Add("base_money", "" + room.base_money);
            var user = _myUser.Read();

            if (user == null)
            {
                return;
            }

            var myMoney = (int) user.MyCurrency(CurrencyType.GOLDEN_EGG);

            switch (code)
            {
                case InteractionCode.Flower:
                    _analyticManager.EventValue("interaction_flower", attrs, myMoney);
                    break;

                case InteractionCode.Egg:
                    _analyticManager.EventValue("interaction_egg", attrs, myMoney);
                    break;
            }
        }

        #endregion

        #region 打牌逻辑

        public void ChuPai(PokerPattern pattern)
        {
            if (pattern == null)
            {
                return;
            }

            var playingData = _playingData.Read();
            if (playingData == null)
            {
                return;
            }

            var msg = CmdUtil.GU.ChuPai(playingData.chupai_key, pattern);
            _msgRepo.SendMsg(msg);
        }

        public void JinGong(int poker)
        {
            if (poker == -1)
            {
                return;
            }

            var msg = CmdUtil.GU.JinGong(poker);
            _msgRepo.SendMsg(msg);
        }

        public void HuanGong(int poker)
        {
            if (poker == -1)
            {
                return;
            }

            var msg = CmdUtil.GU.HuanGong(poker);
            _msgRepo.SendMsg(msg);
        }

        public void TempLeave(bool templeave)
        {
            var msg = CmdUtil.GU.TempLeave(templeave);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 聊天逻辑

        public void SendTextMsg(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            var tableUser = _tableUserData.Read();
            var user = _myUser.Read();
            if (user == null)
            {
                return;
            }

            var list = tableUser.GetTableOtherUsernameList();
            if (list == null)
            {
                return;
            }

            var msg = CmdUtil.CU.USendTextMsg(content, user.username, list);
            _msgRepo.SendMsg(msg);
        }

        public void SendJianMeng(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                return;
            }

            var tableUser = _tableUserData.Read();
            var user = _myUser.Read();
            if (user == null)
            {
                return;
            }

            var list = tableUser.GetTableOtherUsernameList();
            if (list == null)
            {
                return;
            }

            var msg = CmdUtil.CU.SendJianMeng(cmd, user.username, list);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 商品逻辑

        // 购买商品之后，会收到购买商品的结果。
        // 是不是直接添加一个购买商品的任务？
        // 添不添加购买的任务，主要看，是不是在没有发起的时候，就收到服务器端的消息。

        public void BuyCommodity(Commodity commodity)
        {
            var msg = CmdUtil.Shared.BuyCommodity(
                commodity.name,
                1,
                null,
                commodity.currency_type,
                Server.HServer);

            _msgRepo.SendMsg(msg);
        }

        public void SaleCommodity(Commodity commodity)
        {
            if (commodity == null)
            {
                return;
            }

            var msg = CmdUtil.HU.SaleCommodity(commodity.name, 1);
            _msgRepo.SendMsg(msg);
        }

        public void UseCommodity(string cname, bool use)
        {
            if (string.IsNullOrEmpty(cname))
            {
                return;
            }

            var msg = CmdUtil.Shared.UseCommodity(cname, 1, use ? 1 : 2, Server.HServer);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region Exchange

        public void Exchange(string exName, long count)
        {
            Waiting(true);

            _exchangeResult.ClearNotInvalidate();
            _taskManager.ExecuteTask(CheckExchangeResult, () => Waiting(false));

            var msg = CmdUtil.HU.RequestExchange(exName, count, null);
            _msgRepo.SendMsg(msg);
        }

        private bool CheckExchangeResult()
        {
            var res = _exchangeResult.Read(true);
            if (res == null)
            {
                return false;
            }

            Waiting(false);

            if (res.result == ResultCode.OK)
            {
                // 兑换成功的情况下，就不显示什么了。
                // AnalyticManager.EventValue("exchange", null, (int) res.source_amount);
            }
            else
            {
                // 兑换失败，提示用户。
                // TODO 确认，服务器端已经发送过Toast提示了，客户端是否还需要再提示一遍。
                switch (res.result)
                {
                    case ResultCode.H_EXCHANGE_NOT_FOUND:
                        // Toast("兑换失败，没有找到兑换包！", true);
                        break;

                    case ResultCode.H_EXCHANGE_FAIL:
                        // Toast("兑换失败！", true);
                        break;

                    case ResultCode.H_EXCHANGE_SOURCE_NOT_ENOUGH:
                        // Toast(string.Format("兑换失败，您的{0}不够！", CurrencyType.LabelOf(res.source_type)));
                        break;
                }
            }

            return true;
        }

        #endregion

        #region VipExchange

        public void RequestVipExchangeList()
        {
            var msg = CmdUtil.HU.RequestVipExchangeList();
            _msgRepo.SendMsg(msg);
        }

        private VipExchange _lastVipExchangeRequest;

        public void RequestExchangeVip(VipExchange data)
        {
            if (data == null)
            {
                return;
            }

            Waiting(true);

            _lastVipExchangeRequest = data;

            _requestExchangeVipResult.ClearNotInvalidate();
            _taskManager.ExecuteTask(CheckExchangeVipResult, () => Waiting(false));

            var msg = CmdUtil.HU.RequestExchangeVip(data.name);
            _msgRepo.SendMsg(msg);

            _analyticManager.Event("exchange_vip");
        }

        private bool CheckExchangeVipResult()
        {
            var res = _requestExchangeVipResult.Read(true);
            if (res == null)
            {
                return false;
            }

            Waiting(false);

            if (res.result.code == ResultCode.OK)
            {
                // 提示玩家成功购买VIP。
                string content;
                if (_lastVipExchangeRequest != null)
                    content = string.Format("恭喜您，成功{0}VIP！", VipExchangeType.LabelOf(_lastVipExchangeRequest.type));
                else
                    content = "恭喜您，成功购买VIP！";
                _dialogManager.ShowConfirmBox(content);

                // 重新请求一遍Vip列表。
                RequestVipExchangeList();
                // 这里重新请求玩家数据，会比较保险。
                // 重新请求一下用户数据。
                RequestUserInfo();

                // 播放一下使用金蛋的声音。
                _soundController.PlayUseGoldSound();

                var count = 0L;
                if (_lastVipExchangeRequest != null)
                {
                    var price = _lastVipExchangeRequest.price;
                    if (price != null)
                    {
                        count = DataUtil.CalculateGeValue(price.type, price.count);
                    }
                }

                _analyticManager.Buy("vip", 1, count);
                _analyticManager.Event("exchange_vip_ok");
            }
            else
            {
                if (!string.IsNullOrEmpty(res.result.msg))
                {
                    Toast(res.result.msg, true);
                }

                _analyticManager.Event("exchange_vip_fail");
            }

            _lastVipExchangeRequest = null;
            return true;
        }

        #endregion

        #region 支付

        public void RequestTradeNo(string pname, int payChannel)
        {
            _tradeNoResult.ClearNotInvalidate();
            var msg = CmdUtil.HU.RequestTradeNo(pname, 1, null, payChannel, _configHolder.ClientVersion);
            _msgRepo.SendMsg(msg);
        }

        public void CheckTrade(string tradeNo, string outTradeNo, string extra)
        {
            _checkTradeResult.ClearNotInvalidate();
            var msg = CmdUtil.HU.CheckTrade(tradeNo, outTradeNo, extra);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 快捷工具

        public void RequestActionPrice(int actionCode)
        {
            var msg = CmdUtil.Shared.RequestActionPrice(actionCode, Server.HServer);
            _msgRepo.SendMsg(msg);
        }

        public void EditNickname(string newNickname)
        {
            var msg = CmdUtil.HU.EditNickname(newNickname);
            _msgRepo.SendMsg(msg);
        }

        public void ResetWinRate()
        {
            var msg = CmdUtil.HU.ResetWinRate();
            _msgRepo.SendMsg(msg);
        }

        public void ChangeSex()
        {
            var msg = CmdUtil.HU.ChangeSex();
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region FindFriend

        public void FindFriend(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return;
            }

            _searchUserResult.ClearNotInvalidate();
            _taskManager.ExecuteTask(CheckSearchUserResult, SearchUserTimeout);

            Waiting(true);

            var msg = CmdUtil.CU.SearchUser(username);
            _msgRepo.SendMsg(msg);
        }

        private bool CheckSearchUserResult()
        {
            var res = _searchUserResult.Read(true);
            if (res == null)
            {
                return false;
            }

            Waiting(false);

            if (res.result.code == ResultCode.OK)
            {
                _dialogManager.ShowDialog<OtherInfoPanel>(DialogName.OtherInfoPanel, false, false,
                    (dialog) =>
                    {
                        dialog.ApplyData(res.user, true, false);
                        dialog.Show();
                    });
            }
            else
            {
                if (!string.IsNullOrEmpty(res.result.msg))
                    Toast(res.result.msg, true);
            }

            return true;
        }

        private void SearchUserTimeout()
        {
            Waiting(false);
            Toast("查找好友失败！", true);
        }

        #endregion

        #region FriendList

        public void RefreshFriendList()
        {
            var msg = CmdUtil.CU.URefreshFriendList();
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region FriendDetail

        public void RequestFriendDetail(string username)
        {
            _friendDetailResult.ClearNotInvalidate();
            _taskManager.ExecuteTask(CheckFriendDetailResult, RequestFriendDetailTimeout);

            var msg = CmdUtil.CU.UFriendDetail(username);
            _msgRepo.SendMsg(msg);

            Waiting(true, 10);

            _analyticManager.Event("friend_list_show_detail");
        }

        private bool CheckFriendDetailResult()
        {
            var res = _friendDetailResult.Read(true);
            if (res == null)
            {
                return false;
            }

            Waiting(false);

            if (res.result.code == ResultCode.OK)
            {
                _dialogManager.ShowDialog<OtherInfoPanel>(DialogName.OtherInfoPanel, false, false,
                    (dialog) =>
                    {
                        dialog.ApplyData(res.userbase, false, false, false);
                        dialog.Show();
                    });
            }
            else
            {
                if (!string.IsNullOrEmpty(res.result.msg))
                {
                    Toast(res.result.msg, true);
                }
            }

            return true;
        }

        private void RequestFriendDetailTimeout()
        {
            Waiting(false);
            Toast("向服务器请求好友数据失败！", true);
        }

        #endregion

        #region AddFriend

        public void AddFriend(string username)
        {
            var msg = CmdUtil.CU.UAddFriend(username);
            _msgRepo.SendMsg(msg);

            _analyticManager.Event("add_friend");
        }

        public void AddFriendResponse(bool accept, string senderUsername)
        {
            var msg = CmdUtil.CU.UAddFriendResponse(accept, senderUsername);
            _msgRepo.SendMsg(msg);

            var attrs = new Dictionary<string, string>();
            attrs.Add("accept", "" + accept);
            _analyticManager.Event("friend_response", attrs);
        }

        #endregion

        #region RemoveFriend

        public void RemoveFriend(string username)
        {
            _removeFriendResultToSender.ClearNotInvalidate();
            _taskManager.ExecuteTask(CheckRemoveFriendResult, null);

            var msg = CmdUtil.CU.URemoveFriend(username);
            _msgRepo.SendMsg(msg);
        }

        private bool CheckRemoveFriendResult()
        {
            var res = _removeFriendResultToSender.Read(true);
            if (res == null)
            {
                return false;
            }

            if (res.result.code == ResultCode.OK)
            {
                RefreshFriendList();
                Toast("成功删除好友");
                _analyticManager.Event("remove_friend");
            }
            else
            {
                if (!string.IsNullOrEmpty(res.result.msg))
                    Toast(res.result.msg, true);
            }

            return true;
        }

        #endregion

        #region TraceUser

        public void TraceUser(string username, bool confirmLeave)
        {
            _traceUserResult.ClearNotInvalidate();
            _taskManager.ExecuteTask(CheckTraceUserResult, null);
            var msg = CmdUtil.HU.TraceUser(username, confirmLeave);
            _msgRepo.SendMsg(msg);

            Waiting(true);

            _analyticManager.Event("trace_user");
        }

        private bool CheckTraceUserResult()
        {
            var res = _traceUserResult.Read(true);
            if (res == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(res.result.msg))
            {
                Toast(res.result.msg, res.result.code != ResultCode.OK, 1.5f);
            }

            Waiting(false);
            return true;
        }

        #endregion

        #region 邮件

        public void RequestMailBriefList(long timestamp)
        {
            var msg = CmdUtil.HU.RequestMailBriefList(timestamp);
            _msgRepo.SendMsg(msg);
        }

        public void RequestMailContent(string mailId)
        {
            var msg = CmdUtil.HU.RequestMailContent(mailId);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 签到

        public void RequestCheckinConfig()
        {
            var msg = CmdUtil.HU.RequestCheckinConfig();
            _msgRepo.SendMsg(msg);
        }

        public void Checkin()
        {
            var msg = CmdUtil.HU.Checkin();
            _msgRepo.SendMsg(msg);
        }

        public void ReCheckin()
        {
            var msg = CmdUtil.HU.ReCheckin();
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 邀请

        public void RequestMyInviteData()
        {
            var msg = CmdUtil.HU.RequestMyInviteData();
            _msgRepo.SendMsg(msg);
        }

        public void RequestBeenInvitedAward(string inviteCode)
        {
            var msg = CmdUtil.HU.RequestBeenInvitedAward(inviteCode);
            _msgRepo.SendMsg(msg);
        }

        public void RequestInviteAward(int inviteCount)
        {
            var msg = CmdUtil.HU.RequestInviteAward(inviteCount);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 活动

        public void RequestActivityAward(string activityCode, string conditionCode)
        {
            var msg = CmdUtil.HU.RequestActivityAward(activityCode, conditionCode);
            _msgRepo.SendMsg(msg);
        }

        public void RequestActivityStatus()
        {
            var msg = CmdUtil.HU.RequestActivityStatus();
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 元宝

        public void RequestExchangeYuanBao(string yuanBaoItemName)
        {
            Waiting(true);
            _exchangeYuanBaoResult.ClearNotInvalidate();

            var msg = CmdUtil.HU.RequestExchangeYuanBao(yuanBaoItemName);
            _msgRepo.SendMsg(msg);
            _taskManager.ExecuteTask(CheckExchangeYuanBaoResult, () => Waiting(false));

            var attrs = new Dictionary<string, string>();
            attrs.Add("item", yuanBaoItemName);
            _analyticManager.Event("yuanbao_request_exchange", attrs);
        }

        private bool CheckExchangeYuanBaoResult()
        {
            var res = _exchangeYuanBaoResult.Read(true);
            if (res == null)
            {
                return false;
            }

            Waiting(false);
            if (res.res.code == ResultCode.OK)
            {
                var code = res.exchange_code;
                var content = string.Format(
                    "您的兑换码为：<color=red>{0}</color>\n{1}", code, res.description);
                _dialogManager.ShowConfirmBox(
                    content,
                    true, "复制兑换码", () =>
                    {
                        if (string.IsNullOrEmpty(code))
                        {
                            _dialogManager.ShowToast("兑换码异常", 3, true);
                            return;
                        }

                        try
                        {
                            _clipboardManager.CopyToClipboard(code);
                        }
                        catch (Exception e)
                        {
                            _dialogManager.ShowToast("复制兑换码失败", 3, true);
                            return;
                        }

                        _dialogManager.ShowToast("已复制兑换码到粘贴板", 3);
                    },
                    false, null, null,
                    true, false, true);
            }
            else
            {
                if (!string.IsNullOrEmpty(res.res.msg))
                {
                    _dialogManager.ShowToast(res.res.msg, 3, true);
                }
            }

            return true;
        }

        public void RequestMyYuanBaoExchange()
        {
            var msg = CmdUtil.HU.RequestMyYuanBaoExchange();
            _msgRepo.SendMsg(msg);
        }

        public void RequestYuanBaoConfigData()
        {
            var msg = CmdUtil.HU.RequestYuanBaoConfig();
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 心跳

        public void HBRes(long timestamp, Server server)
        {
            var msg = CmdUtil.Shared.HBRes(timestamp, server);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 简化的功能方法

        private void Waiting(bool show, float timeout = 10)
        {
            _dialogManager.ShowWaitingDialog(show, timeout);
        }

        private void Toast(string content, bool error = false, float time = 2)
        {
            _dialogManager.ShowToast(content, time, error);
        }

        #endregion

        #region 比赛房

        public void RequestRaceConfigList()
        {
            var msg = CmdUtil.HU.RequestRaceConfigList();
            _msgRepo.SendMsg(msg);
        }

        public void ApplyRace(long raceId)
        {
            var msg = CmdUtil.HU.ApplyRace(raceId);
            _msgRepo.SendMsg(msg);
        }

        #endregion

        #region 每日任务

        public void RequestUserTaskList()
        {
            var msg = CmdUtil.HU.RequestUserTaskList();
            _msgRepo.SendMsg(msg);
        }

        public void GetUserTaskAward(string stateId)
        {
            var msg = CmdUtil.HU.GetUserTaskAward(stateId);
            _msgRepo.SendMsg(msg);
        }

        public void NotifyDoShare(string id, int type, string userTaskCode, string awardCode, string shareResult)
        {
            var msg = CmdUtil.HU.NotifyDoShare(id, type, userTaskCode, awardCode, shareResult);
            _msgRepo.SendMsg(msg);
        }

        #endregion
    }
}