using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.App;
using Dmm.DataContainer;
using Dmm.Msg;
using Dmm.MsgLogic.CU;
using Dmm.MsgLogic.GU;
using Dmm.MsgLogic.HU;
using Dmm.MsgLogic.PU;
using Zenject;

namespace Dmm.MsgLogic
{
    public class MessageLogicFactory : IMessageLogicFactory
    {
        private IAppContext _context;

        [Inject]
        public void Inject(IAppContext context)
        {
            _context = context;
        }

        public List<IMessageHandler> GetMessageHandlerList()
        {
            var appController = _context.GetAppController();
            var dataRepository = _context.GetDataRepository();
            var dialogManager = _context.GetDialogManager();
            var remoteAPI = _context.GetRemoteAPI();
            var analyticManager = _context.GetAnalyticManager();
            var systemMsgController = _context.GetSystemMsgController();
            var networkManager = _context.GetNetworkManager();

            var handlers = new List<IMessageHandler>();

            // PU
            handlers.Add(new ClientVersionResultHandler(dataRepository));
            handlers.Add(new PLoginResultHandler(dataRepository));
            handlers.Add(new SimpleMessageHandler<PRegisterResult>(
                Server.PServer, CmdType.PU.REGISTER_RESULT,
                dataRepository.GetContainer<PRegisterResult>(DataKey.PRegisterResult)));
            handlers.Add(new SimpleMessageHandler<WechatAuthResult>(
                Server.PServer, CmdType.PU.WECHAT_AUTH_RESULT,
                dataRepository.GetContainer<WechatAuthResult>(DataKey.WechatAuthResult)));
            handlers.Add(new SimpleMessageHandler<WechatLoginResult>(
                Server.PServer, CmdType.PU.WECHAT_LOGIN_RESULT,
                dataRepository.GetContainer<WechatLoginResult>(DataKey.WechatLoginResult)));
            handlers.Add(new PToastHandler(dialogManager));

            // HU
            handlers.Add(new HLoginResultHandler(dataRepository));
            handlers.Add(new RequestAwardResultHandler(dataRepository, dialogManager));
            handlers.Add(new BeenReplacedHandler(appController, dialogManager));
            handlers.Add(new ChooseRoomResultHandler(dataRepository, networkManager));
            handlers.Add(new ChooseRoomFailHandler(dataRepository, dialogManager));
            handlers.Add(new LeaveRoomResultHandler(dataRepository, dialogManager, remoteAPI, networkManager));
            handlers.Add(new BRoomInOutHandler(dataRepository));
            handlers.Add(new SimpleMessageHandler<BuyCommodityResult>(
                Server.HServer, CmdType.HU.BUY_COMMODITY_RESULT,
                dataRepository.GetContainer<BuyCommodityResult>(DataKey.BuyCommodityResult)));
            handlers.Add(new SimpleMessageHandler<UseCommodityResult>(
                Server.HServer, CmdType.HU.USE_COMMODITY_RESULT,
                dataRepository.GetContainer<UseCommodityResult>(DataKey.UseCommodityResult)));
            handlers.Add(new SimpleMessageHandler<SaleCommodityResult>(
                Server.HServer, CmdType.HU.SALE_COMMODITY_RESULT,
                dataRepository.GetContainer<SaleCommodityResult>(DataKey.SaleCommodityResult)));
            handlers.Add(new SimpleMessageHandler<ActionPriceResult>(
                Server.HServer, CmdType.HU.ACTION_PRICE_RESULT,
                dataRepository.GetContainer<ActionPriceResult>(DataKey.ActionPriceResult)));
            handlers.Add(new SimpleMessageHandler<ResetWinRateResult>(
                Server.HServer, CmdType.HU.RESET_WIN_RATE_RESULT,
                dataRepository.GetContainer<ResetWinRateResult>(DataKey.ResetWinRateResult)));
            handlers.Add(new SimpleMessageHandler<ChangeSexResult>(
                Server.HServer, CmdType.HU.CHANGE_SEX_RESULT,
                dataRepository.GetContainer<ChangeSexResult>(DataKey.ChangeSexResult)));
            handlers.Add(new SimpleMessageHandler<EditNicknameResult>(
                Server.HServer, CmdType.HU.EDIT_NICKNAME_RESULT,
                dataRepository.GetContainer<EditNicknameResult>(DataKey.EditNicknameResult)));
            handlers.Add(new SimpleMessageHandler<VipExchangeListResult>(
                Server.HServer, CmdType.HU.VIP_EXCHANGE_LIST_RESULT,
                dataRepository.GetContainer<VipExchangeListResult>(DataKey.VipExchangeListResult)));
            handlers.Add(new SimpleMessageHandler<RequestExchangeVipResult>(
                Server.HServer, CmdType.HU.REQUEST_EXCHANGE_VIP_RESULT,
                dataRepository.GetContainer<RequestExchangeVipResult>(DataKey.RequestExchangeVipResult)));
            handlers.Add(new SimpleMessageHandler<ExchangeResult>(
                Server.HServer, CmdType.HU.EXCHANGE_RESULT,
                dataRepository.GetContainer<ExchangeResult>(DataKey.ExchangeResult)));
            handlers.Add(new UserInfoResultHandler(dataRepository));
            handlers.Add(new SimpleMessageHandler<EditUserInfoResult>(
                Server.HServer, CmdType.HU.EDIT_USER_INFO_RESULT,
                dataRepository.GetContainer<EditUserInfoResult>(DataKey.EditUserInfoResult)));
            handlers.Add(new SimpleMessageHandler<EditPasswordResult>(
                Server.HServer, CmdType.HU.EDIT_PASSWORD_RESULT,
                dataRepository.GetContainer<EditPasswordResult>(DataKey.EditPasswordResult)));
            handlers.Add(new SimpleMessageHandler<ChooseNicknameResult>(
                Server.HServer, CmdType.HU.CHOOSE_NICKNAME_RESULT,
                dataRepository.GetContainer<ChooseNicknameResult>(DataKey.ChooseNicknameResult)));
            handlers.Add(new SimpleMessageHandler<TreasureChestData>(
                Server.HServer, CmdType.HU.TREASURE_CHEST_DATA,
                dataRepository.GetContainer<TreasureChestData>(DataKey.TreasureChestData)));
            handlers.Add(new TraceUserResultHandler(dataRepository, dialogManager, remoteAPI));
            handlers.Add(new SimpleMessageHandler<VisitorRegularizeResult>(
                Server.HServer, CmdType.HU.VISITOR_REGULARIZE_RESULT,
                dataRepository.GetContainer<VisitorRegularizeResult>(DataKey.VisitorRegularizeResult)));
            handlers.Add(new SimpleMessageHandler<HRegisterResult>(
                Server.HServer, CmdType.HU.REGISTER_RESULT,
                dataRepository.GetContainer<HRegisterResult>(DataKey.HRegisterResult)));
            handlers.Add(new SimpleMessageHandler<TradeNoResult>(
                Server.HServer, CmdType.HU.TRADE_NO_RESULT,
                dataRepository.GetContainer<TradeNoResult>(DataKey.TradeNoResult)));
            handlers.Add(new CheckTradeResultHandler(dataRepository, dialogManager));
            handlers.Add(new HToastHandler(dialogManager));
            handlers.Add(new HU.PushItemHandler(dialogManager));
            handlers.Add(new CheckinConfigResultHandler(dataRepository));
            handlers.Add(new CheckinResultHandler(dataRepository));
            handlers.Add(new ReCheckinResultHandler(dataRepository));
            handlers.Add(new CheckinAwardResultHandler(dataRepository, dialogManager));
            handlers.Add(new SimpleMessageHandler<InviteDataResult>(
                Server.HServer, CmdType.HU.MY_INVITE_DATA_RESULT,
                dataRepository.GetContainer<InviteDataResult>(DataKey.InviteDataResult)));
            handlers.Add(new SimpleMessageHandler<BeenInvitedAwardResult>(
                Server.HServer, CmdType.HU.BEEN_INVITED_AWARD_RESULT,
                dataRepository.GetContainer<BeenInvitedAwardResult>(DataKey.BeenInvitedAwardResult)));
            handlers.Add(new SimpleMessageHandler<InviteAwardResult>(
                Server.HServer, CmdType.HU.INVITE_AWARD_RESULT,
                dataRepository.GetContainer<InviteAwardResult>(DataKey.InviteAwardResult)));
            handlers.Add(new SimpleMessageHandler<ActivityStatusResult>(
                Server.HServer, CmdType.HU.ACTIVITY_STATUS_RESULT,
                dataRepository.GetContainer<ActivityStatusResult>(DataKey.ActivityStatusResult)));
            handlers.Add(new SimpleMessageHandler<ActivityAwardResult>(
                Server.HServer, CmdType.HU.ACTIVITY_AWARD_RESULT,
                dataRepository.GetContainer<ActivityAwardResult>(DataKey.ActivityAwardResult)));
            handlers.Add(new ExchangeYuanBaoResultHandler(dataRepository));
            handlers.Add(new SimpleMessageHandler<MyYuanBaoExchangeResult>(
                Server.HServer, CmdType.HU.MY_YUANBAO_EXCHANGE_RESULT,
                dataRepository.GetContainer<MyYuanBaoExchangeResult>(DataKey.MyYuanBaoExchangeResult)));
            handlers.Add(new SimpleMessageHandler<YuanBaoConfigResult>(
                Server.HServer, CmdType.HU.YUANBAO_CONFIG_RESULT,
                dataRepository.GetContainer<YuanBaoConfigResult>(DataKey.YuanBaoConfigResult)));
            handlers.Add(new SimpleMessageHandler<WechatBindResult>(
                Server.HServer, CmdType.HU.WECHAT_BIND_RESULT,
                dataRepository.GetContainer<WechatBindResult>(DataKey.WechatBindResult)));
            handlers.Add(new SimpleMessageHandler<RaceConfigList>(
                Server.HServer, CmdType.HU.RACE_CONFIG_LIST_RESULT,
                dataRepository.GetContainer<RaceConfigList>(DataKey.RaceConfigList)));
            handlers.Add(new RaceConfigHandler(dataRepository));
            handlers.Add(new SimpleMessageHandler<ApplyRaceResult>(
                Server.HServer, CmdType.HU.APPLY_RACE_RESULT,
                dataRepository.GetContainer<ApplyRaceResult>(DataKey.ApplyRaceResult)));
            handlers.Add(new RaceAwardHandler(dataRepository, dialogManager));
            handlers.Add(new SimpleMessageHandler<UserTaskListResult>(Server.HServer, CmdType.HU.USER_TASK_LIST_RESULT,
                dataRepository.GetContainer<UserTaskListResult>(DataKey.UserTaskListResult)));
            handlers.Add(new UserTaskTipHandler(dataRepository, dialogManager, remoteAPI));
            handlers.Add(new SimpleMessageHandler<GetUserTaskAwardResult>(Server.HServer,
                CmdType.HU.GET_USER_TASK_AWARD_RESULT,
                dataRepository.GetContainer<GetUserTaskAwardResult>(DataKey.GetUserTaskAwardResult)));
            handlers.Add(new SimpleMessageHandler<NotifyDoShareResult>(Server.HServer,
                CmdType.HU.NOTIFY_DO_SHARE_RESULT,
                dataRepository.GetContainer<NotifyDoShareResult>(DataKey.NotifyDoShareResult)));

            // GU
            handlers.Add(new GLoginResultHandler(analyticManager, dataRepository));
            handlers.Add(new MatchResultHandler(remoteAPI, dialogManager, dataRepository));
            handlers.Add(new BTableInOutHandler(dataRepository));
            handlers.Add(new BTableChangedHandler(dataRepository));
            handlers.Add(new ChooseTableResultHandler(dataRepository));
            handlers.Add(new LeaveTableResultHandler(dataRepository, remoteAPI, dialogManager));
            handlers.Add(new UserReadyHandler(dataRepository));
            handlers.Add(new SimpleMessageHandler<ReadyResult>(
                Server.GServer, CmdType.GU.READY_RESULT_V6,
                dataRepository.GetContainer<ReadyResult>(DataKey.ReadyResult)));
            handlers.Add(new SimpleMessageHandler<BKickOutCounter>(
                Server.GServer, CmdType.GU.B_KICK_OUT_COUNTER_V6,
                dataRepository.GetContainer<BKickOutCounter>(DataKey.BKickOutCounter)));
            handlers.Add(new SimpleMessageHandler<BCounter>(
                Server.GServer, CmdType.GU.B_COUNTER_V6,
                dataRepository.GetContainer<BCounter>(DataKey.BCounter)));
            handlers.Add(new BPlayerChooseSeatHandler(dataRepository));
            handlers.Add(new HostInfoResultHandler(dataRepository));

            handlers.Add(new TTZStartBroadcastHandler(dataRepository));
            handlers.Add(new StartRoundHandler(dataRepository));

            handlers.Add(new BJinGongRequestHandler(dataRepository));
            handlers.Add(new JinGongResultHandler(dataRepository));
            handlers.Add(new BJinGongPokerHandler(dataRepository));
            handlers.Add(new BJinGongResultHandler(dataRepository));
            handlers.Add(new BeenJinGongHandler(dataRepository));

            handlers.Add(new BHuanGongRequestHandler(dataRepository));
            handlers.Add(new HuanGongResultHandler(dataRepository));
            handlers.Add(new BeenHuanGongHandler(dataRepository));
            handlers.Add(new BHuanGongPokerHandler(dataRepository));
            handlers.Add(new BKangGongHandler(dataRepository));

            handlers.Add(new ChuPaiKeyHandler(dataRepository));
            handlers.Add(new BChuPaiKeyOwnerHandler(dataRepository));
            handlers.Add(new ChuPaiResultHandler(dataRepository));
            handlers.Add(new BChuPaiHandler(dataRepository));
            handlers.Add(new BJieFengHandler(dataRepository));
            handlers.Add(new BFanBeiHandler(dataRepository));
            handlers.Add(new BRoundEndHandler(dataRepository, analyticManager, appController));
            handlers.Add(new RoundEndHandler(dataRepository, analyticManager, appController));

            handlers.Add(new BTempLeaveHandler(dataRepository));
            handlers.Add(new BUseCommodityHandler(dataRepository));
            handlers.Add(new SimpleMessageHandler<TreasureChestData>(
                Server.GServer, CmdType.GU.TREASURE_CHEST_DATA_V6,
                dataRepository.GetContainer<TreasureChestData>(DataKey.TreasureChestData)));
            handlers.Add(new TreasureChestRewardHandler(dialogManager));
            handlers.Add(new BEscapeHandler(dialogManager, dataRepository, analyticManager));
            handlers.Add(new PunishHandler(dialogManager, dataRepository, analyticManager));
            handlers.Add(new ToastHandler(dialogManager));
            handlers.Add(new GU.PushItemHandler(dialogManager));
            handlers.Add(new InteractionResultHandler(dialogManager, dataRepository));
            handlers.Add(new SimpleMessageHandler<BInteraction>(
                Server.GServer, CmdType.GU.INTERACTION_BROADCAST_V6,
                dataRepository.GetContainer<BInteraction>(DataKey.BInteraction)));
            
            handlers.Add(new BSysTextMsgHandler(dialogManager, systemMsgController));
            handlers.Add(new SimpleMessageHandler<BTextMsg>(
                Server.GServer, CmdType.CU.B_TEXT_MSG_V6,
                dataRepository.GetContainer<BTextMsg>(DataKey.BTextMsg)));
            handlers.Add(new STextMsgResultHandler(dataRepository));
            handlers.Add(new SimpleMessageHandler<BJianMeng>(
                Server.GServer, CmdType.CU.B_JIAN_MENG_V6,
                dataRepository.GetContainer<BJianMeng>(DataKey.BJianMeng)));
            handlers.Add(new JianMengResultHandler(dataRepository));
            handlers.Add(new SimpleMessageHandler<SFriendDetailResult>(
                Server.GServer, CmdType.CU.FRIEND_DETAIL_RESULT_V6,
                dataRepository.GetContainer<SFriendDetailResult>(DataKey.SFriendDetailResult)));
            handlers.Add(new SimpleMessageHandler<SFriendListResult>(
                Server.GServer, CmdType.CU.FRIEND_LIST_RESULT_V6,
                dataRepository.GetContainer<SFriendListResult>(DataKey.SFriendListResult)));
            handlers.Add(new SimpleMessageHandler<SRemoveFriendResultToSender>(
                Server.GServer, CmdType.CU.REMOVE_FRIEND_RESULT_TO_SENDER_V6,
                dataRepository.GetContainer<SRemoveFriendResultToSender>(DataKey.SRemoveFriendResultToSender)));
            handlers.Add(new SFriendRemovedToReceiverHandler(remoteAPI));
            handlers.Add(new SAddFriendFailHandler(dialogManager));
            handlers.Add(new SAddFriendRequestToReceiverHandler(dataRepository, dialogManager));
            handlers.Add(new SAddFriendResponseToSenderHandler(dialogManager));
            handlers.Add(new SNewFriendHanlder(remoteAPI));
            handlers.Add(new CUToastHandler(dialogManager));
            handlers.Add(new SimpleMessageHandler<CUSearchUserResult>(
                Server.GServer, CmdType.CU.SEARCH_USER_RESULT_V6,
                dataRepository.GetContainer<CUSearchUserResult>(DataKey.SearchUserResult)));

            return handlers;
        }

        public List<IMessageFilter> GetMessageFilterList()
        {
            var heatbeatFilter = new HeartBeatMessageFilter(_context.GetRemoteAPI());

            var result = new List<IMessageFilter>();
            result.Add(heatbeatFilter);

            return result;
        }
    }
}