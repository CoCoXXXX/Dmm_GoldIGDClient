using System;
using System.CodeDom;
using System.Collections.Generic;
using com.morln.game.gd.command;

namespace Dmm.Msg
{
    /// <summary>
    /// 82号消息是心跳消息，会被过滤，其他消息不能是82
    /// </summary>
    public class CmdType
    {
        static CmdType()
        {
            // PU
            PServerTypes.Add(PU.CLIENT_VERSION, typeof(ClientVersion));
            PServerTypes.Add(PU.CLIENT_VERSION_RESULT, typeof(VersionResult));
            PServerTypes.Add(PU.LOGIN, typeof(PLogin));
            PServerTypes.Add(PU.LOGIN_RESULT, typeof(PLoginResult));
            PServerTypes.Add(PU.REGISTER_RESULT, typeof(PRegisterResult));
            PServerTypes.Add(PU.VISITOR_LOGIN, typeof(VisitorLogin));
            PServerTypes.Add(PU.VISITOR_LOGIN_RESULT, typeof(VisitorLoginResult));
            PServerTypes.Add(PU.DEVICE_INFO, typeof(DeviceInfo));
            PServerTypes.Add(PU.TOAST, typeof(Toast));
            PServerTypes.Add(PU.WECHAT_AUTH, typeof(WechatAuth));
            PServerTypes.Add(PU.WECHAT_AUTH_RESULT, typeof(WechatAuthResult));
            PServerTypes.Add(PU.WECHAT_LOGIN, typeof(WechatLogin));
            PServerTypes.Add(PU.WECHAT_LOGIN_RESULT, typeof(WechatLoginResult));

            // HU Type
            HServerAndGServerTypes.Add(HU.HB_REQ, typeof(HBReq));
            HServerAndGServerTypes.Add(HU.HB_RES, typeof(HBRes));
            HServerAndGServerTypes.Add(HU.LOGIN, typeof(HLogin));
            HServerAndGServerTypes.Add(HU.LOGIN_RESULT, typeof(HLoginResult));
            HServerAndGServerTypes.Add(HU.LOGIN_REWARD_CONFIG, typeof(LoginRewardConfig));
            HServerAndGServerTypes.Add(HU.REQUEST_LOGIN_REWARD, typeof(RequestLoginReward));
            HServerAndGServerTypes.Add(HU.GIFT_RESULT, typeof(GiftResult));
            HServerAndGServerTypes.Add(HU.REFILL_HINT, typeof(RefillHint));
            HServerAndGServerTypes.Add(HU.VIP_HINT, typeof(VipHint));
            HServerAndGServerTypes.Add(HU.CHAT_SERVER_ADDR, typeof(ChatServerAddr));
            HServerAndGServerTypes.Add(HU.SHOW_ROOMS_RESULT, typeof(ShowRoomsResult));
            HServerAndGServerTypes.Add(HU.CHOOSE_ROOM, typeof(ChooseRoom));
            HServerAndGServerTypes.Add(HU.CHOOSE_ROOM_RESULT, typeof(ChooseRoomResult));
            HServerAndGServerTypes.Add(HU.CHOOSE_ROOM_FAIL, typeof(ChooseRoomFail));
            HServerAndGServerTypes.Add(HU.QUICK_START, typeof(QuickStart));
            HServerAndGServerTypes.Add(HU.REQUEST_CHOOSE_ROOM, typeof(RequestChooseRoom));
            HServerAndGServerTypes.Add(HU.LEAVE_ROOM_RESULT, typeof(LeaveRoomResult));
            HServerAndGServerTypes.Add(HU.ROOM_IN_OUT, typeof(BRoomInOut));
            HServerAndGServerTypes.Add(HU.LEAVE_HALL_RESULT, typeof(LeaveHallResult));
            HServerAndGServerTypes.Add(HU.REQUEST_USER_INFO, typeof(RequestUserInfo));
            HServerAndGServerTypes.Add(HU.USER_INFO_RESULT, typeof(UserInfoResult));
            HServerAndGServerTypes.Add(HU.EDIT_USER_INFO, typeof(EditUserInfo));
            HServerAndGServerTypes.Add(HU.EDIT_USER_INFO_RESULT, typeof(EditUserInfoResult));
            HServerAndGServerTypes.Add(HU.CHOOSE_NICKNAME, typeof(ChooseNickname));
            HServerAndGServerTypes.Add(HU.CHOOSE_NICKNAME_RESULT, typeof(ChooseNicknameResult));
            HServerAndGServerTypes.Add(HU.EDIT_PASSWORD, typeof(EditPassword));
            HServerAndGServerTypes.Add(HU.EDIT_PASSWORD_RESULT, typeof(EditPasswordResult));
            HServerAndGServerTypes.Add(HU.MYBAG_RESULT, typeof(MyBagResult));
            HServerAndGServerTypes.Add(HU.BUY_COMMODITY, typeof(BuyCommodity));
            HServerAndGServerTypes.Add(HU.BUY_COMMODITY_RESULT, typeof(BuyCommodityResult));
            HServerAndGServerTypes.Add(HU.USE_COMMODITY, typeof(UseCommodity));
            HServerAndGServerTypes.Add(HU.USE_COMMODITY_RESULT, typeof(UseCommodityResult));
            HServerAndGServerTypes.Add(HU.B_USE_COMMODITY, typeof(BUseCommodity));
            HServerAndGServerTypes.Add(HU.UPGRADE_COMMODITY, typeof(UpgradeCommodity));
            HServerAndGServerTypes.Add(HU.UPGRADE_COMMODITY_RESULT, typeof(UpgradeCommodityResult));
            HServerAndGServerTypes.Add(HU.SALE_COMMODITY_RESULT, typeof(SaleCommodityResult));
            HServerAndGServerTypes.Add(HU.COMMODITY_LIST_RESULT, typeof(CommodityListResult));
            HServerAndGServerTypes.Add(HU.HALL_DATA, typeof(HallData));
            HServerAndGServerTypes.Add(HU.DAILY_LOGIN_REWARD, typeof(DailyLoginReward));
            HServerAndGServerTypes.Add(HU.ROOM_DATA, typeof(RoomData));
            HServerAndGServerTypes.Add(HU.DEVICE_INFO, typeof(DeviceInfo));
            HServerAndGServerTypes.Add(HU.SUBMIT_PRIVACY, typeof(SubmitPrivacy));
            HServerAndGServerTypes.Add(HU.CHANGE_PRIVACY_RESULT, typeof(ChangePrivacyResult));
            HServerAndGServerTypes.Add(HU.PRIVACY_RESULT, typeof(PrivacyResult));
            HServerAndGServerTypes.Add(HU.REQUEST_TRADE_NO, typeof(RequestTradeNo));
            HServerAndGServerTypes.Add(HU.TRADE_NO_RESULT, typeof(TradeNoResult));
            HServerAndGServerTypes.Add(HU.CHECK_TRADE, typeof(CheckTrade));
            HServerAndGServerTypes.Add(HU.CHECK_TRADE_RESULT, typeof(CheckTradeResult));
            HServerAndGServerTypes.Add(HU.SUBMIT_WEIBO_INFO, typeof(SubmitWeiboInfo));
            HServerAndGServerTypes.Add(HU.SUBMIT_WEIBO_RESULT, typeof(SubmitWeiboResult));
            HServerAndGServerTypes.Add(HU.REQUEST_REWARD, typeof(RequestReward));
            HServerAndGServerTypes.Add(HU.REQUEST_EXCHANGE, typeof(RequestExchange));
            HServerAndGServerTypes.Add(HU.EXCHANGE_RESULT, typeof(ExchangeResult));
            HServerAndGServerTypes.Add(HU.VIP_EXCHANGE_LIST_RESULT, typeof(VipExchangeListResult));
            HServerAndGServerTypes.Add(HU.REQUEST_EXCHANGE_VIP, typeof(RequestExchangeVip));
            HServerAndGServerTypes.Add(HU.REQUEST_EXCHANGE_VIP_RESULT, typeof(RequestExchangeVipResult));
            HServerAndGServerTypes.Add(HU.VISITOR_REGULARIZE, typeof(VisitorRegularize));
            HServerAndGServerTypes.Add(HU.VISITOR_REGULARIZE_RESULT, typeof(VisitorRegularizeResult));
            HServerAndGServerTypes.Add(HU.PUNISH, typeof(Punish));
            HServerAndGServerTypes.Add(HU.TOAST, typeof(Toast));
            HServerAndGServerTypes.Add(HU.SYS_TEXT_MSG, typeof(SysTextMsg));
            HServerAndGServerTypes.Add(HU.LEVEL_UP, typeof(LevelUp));
            HServerAndGServerTypes.Add(HU.ALLOWANCE, typeof(Allowance));
            HServerAndGServerTypes.Add(HU.TREASURE_CHEST_DATA, typeof(TreasureChestData));
            HServerAndGServerTypes.Add(HU.REGISTER, typeof(HRegister));
            HServerAndGServerTypes.Add(HU.REGISTER_RESULT, typeof(HRegisterResult));
            HServerAndGServerTypes.Add(HU.RESET_WIN_RATE_RESULT, typeof(ResetWinRateResult));
            HServerAndGServerTypes.Add(HU.EDIT_NICKNAME, typeof(EditNickname));
            HServerAndGServerTypes.Add(HU.EDIT_NICKNAME_RESULT, typeof(EditNicknameResult));
            HServerAndGServerTypes.Add(HU.CHANGE_SEX_RESULT, typeof(ChangeSexResult));
            HServerAndGServerTypes.Add(HU.REQUEST_ACTION_PRICE, typeof(RequestActionPrice));
            HServerAndGServerTypes.Add(HU.ACTION_PRICE_RESULT, typeof(ActionPriceResult));
            HServerAndGServerTypes.Add(HU.TRACE_USER, typeof(TraceUser));
            HServerAndGServerTypes.Add(HU.TRACE_USER_RESULT, typeof(TraceUserResult));
            HServerAndGServerTypes.Add(HU.REQUEST_AWARD, typeof(RequestAward));
            HServerAndGServerTypes.Add(HU.REQUEST_AWARD_RESULT, typeof(RequestAwardResult));
            HServerAndGServerTypes.Add(HU.PUSH_HINT, typeof(PushHint));
            HServerAndGServerTypes.Add(HU.PUSH_ITEM, typeof(PushItem));
            HServerAndGServerTypes.Add(HU.CHECKIN_CONFIG, typeof(CheckinConfigResult));
            HServerAndGServerTypes.Add(HU.CHECKIN_RESULT, typeof(CheckinResult));
            HServerAndGServerTypes.Add(HU.RECHECKIN_RESULT, typeof(ReCheckinResult));
            HServerAndGServerTypes.Add(HU.CHECKIN_AWARD_RESULT, typeof(CheckinAwardResult));
            HServerAndGServerTypes.Add(HU.MY_INVITE_DATA_RESULT, typeof(InviteDataResult));
            HServerAndGServerTypes.Add(HU.BEEN_INVITED_AWARD_RESULT, typeof(BeenInvitedAwardResult));
            HServerAndGServerTypes.Add(HU.INVITE_AWARD_RESULT, typeof(InviteAwardResult));
            HServerAndGServerTypes.Add(HU.ACTIVITY_STATUS_RESULT, typeof(ActivityStatusResult));
            HServerAndGServerTypes.Add(HU.ACTIVITY_AWARD_RESULT, typeof(ActivityAwardResult));
            HServerAndGServerTypes.Add(HU.ACTIVITY_CONFIG_RESULT, typeof(ActivityConfigResult));
            HServerAndGServerTypes.Add(HU.ACTIVITY_TIP, typeof(ActivityTip));
            HServerAndGServerTypes.Add(HU.MAIL_BRIEF_LIST_RESULT, typeof(MailBriefListResult));
            HServerAndGServerTypes.Add(HU.MAIL_CONTENT_RESULT, typeof(MailContentResult));
            HServerAndGServerTypes.Add(HU.EXCHANGE_YUANBAO_RESULT, typeof(ExchangeYuanBaoResult));
            HServerAndGServerTypes.Add(HU.MY_YUANBAO_EXCHANGE_RESULT, typeof(MyYuanBaoExchangeResult));
            HServerAndGServerTypes.Add(HU.YUANBAO_CONFIG_RESULT, typeof(YuanBaoConfigResult));
            HServerAndGServerTypes.Add(HU.WECHAT_BIND, typeof(WechatBind));
            HServerAndGServerTypes.Add(HU.WECHAT_BIND_RESULT, typeof(WechatBindResult));
            HServerAndGServerTypes.Add(HU.RACE_CONFIG_LIST_RESULT, typeof(RaceConfigList));
            HServerAndGServerTypes.Add(HU.RACE_CONFIG, typeof(RaceConfig));
            HServerAndGServerTypes.Add(HU.APPLY_RACE, typeof(ApplyRace));
            HServerAndGServerTypes.Add(HU.APPLY_RACE_RESULT, typeof(ApplyRaceResult));
            HServerAndGServerTypes.Add(HU.RACE_AWARD, typeof(RaceAward));
            HServerAndGServerTypes.Add(HU.USER_TASK_LIST_RESULT, typeof(UserTaskListResult));
            HServerAndGServerTypes.Add(HU.USER_TASK_TIP, typeof(UserTaskTip));
            HServerAndGServerTypes.Add(HU.GET_USER_TASK_AWARD_RESULT, typeof(GetUserTaskAwardResult));
            HServerAndGServerTypes.Add(HU.NOTIFY_DO_SHARE_RESULT, typeof(NotifyDoShareResult));

            //HU Server
            HServerAndGServerMsgServer.Add(HU.HB_REQ, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.HB_RES, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.LOGIN, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.LOGIN_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.LOGIN_REWARD_CONFIG, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REQUEST_LOGIN_REWARD, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.GIFT_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REFILL_HINT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.VIP_HINT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.CHAT_SERVER_ADDR, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.SHOW_ROOMS_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.CHOOSE_ROOM, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.CHOOSE_ROOM_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.CHOOSE_ROOM_FAIL, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.QUICK_START, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REQUEST_CHOOSE_ROOM, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.LEAVE_ROOM_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.ROOM_IN_OUT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.LEAVE_HALL_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REQUEST_USER_INFO, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.USER_INFO_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.EDIT_USER_INFO, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.EDIT_USER_INFO_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.CHOOSE_NICKNAME, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.CHOOSE_NICKNAME_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.EDIT_PASSWORD, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.EDIT_PASSWORD_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.MYBAG_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.BUY_COMMODITY, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.BUY_COMMODITY_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.USE_COMMODITY, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.USE_COMMODITY_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.B_USE_COMMODITY, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.UPGRADE_COMMODITY, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.UPGRADE_COMMODITY_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.SALE_COMMODITY_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.COMMODITY_LIST_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.HALL_DATA, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.DAILY_LOGIN_REWARD, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.ROOM_DATA, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.DEVICE_INFO, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.SUBMIT_PRIVACY, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.CHANGE_PRIVACY_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.PRIVACY_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REQUEST_TRADE_NO, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.TRADE_NO_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.CHECK_TRADE, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.CHECK_TRADE_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.SUBMIT_WEIBO_INFO, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.SUBMIT_WEIBO_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REQUEST_REWARD, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REQUEST_EXCHANGE, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.EXCHANGE_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.VIP_EXCHANGE_LIST_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REQUEST_EXCHANGE_VIP, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REQUEST_EXCHANGE_VIP_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.VISITOR_REGULARIZE, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.VISITOR_REGULARIZE_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.PUNISH, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.TOAST, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.SYS_TEXT_MSG, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.LEVEL_UP, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.ALLOWANCE, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.TREASURE_CHEST_DATA, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REGISTER, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REGISTER_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.RESET_WIN_RATE_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.EDIT_NICKNAME, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.EDIT_NICKNAME_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.CHANGE_SEX_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REQUEST_ACTION_PRICE, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.ACTION_PRICE_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.TRACE_USER, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.TRACE_USER_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REQUEST_AWARD, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.REQUEST_AWARD_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.PUSH_HINT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.PUSH_ITEM, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.CHECKIN_CONFIG, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.CHECKIN_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.RECHECKIN_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.CHECKIN_AWARD_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.MY_INVITE_DATA_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.BEEN_INVITED_AWARD_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.INVITE_AWARD_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.ACTIVITY_STATUS_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.ACTIVITY_AWARD_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.ACTIVITY_CONFIG_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.ACTIVITY_TIP, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.MAIL_BRIEF_LIST_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.MAIL_CONTENT_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.EXCHANGE_YUANBAO_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.MY_YUANBAO_EXCHANGE_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.YUANBAO_CONFIG_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.WECHAT_BIND, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.WECHAT_BIND_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.RACE_CONFIG_LIST_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.RACE_CONFIG, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.APPLY_RACE, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.APPLY_RACE_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.RACE_AWARD, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.USER_TASK_LIST_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.USER_TASK_TIP, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.GET_USER_TASK_AWARD_RESULT, Server.HServer);
            HServerAndGServerMsgServer.Add(HU.NOTIFY_DO_SHARE_RESULT, Server.HServer);

            // GU Type
            HServerAndGServerTypes.Add(GU.REFILL_HINT_V6, typeof(RefillHint));
            HServerAndGServerTypes.Add(GU.VIP_HINT_V6, typeof(VipHint));
            HServerAndGServerTypes.Add(GU.LOGIN_V6, typeof(GLogin));
            HServerAndGServerTypes.Add(GU.LOGIN_RESULT_V6, typeof(GLoginResult));
            HServerAndGServerTypes.Add(GU.SHOW_ROOM_RESULT_V6, typeof(ShowRoomResult));
            HServerAndGServerTypes.Add(GU.CHOOSE_TABLE_V6, typeof(ChooseTable));
            HServerAndGServerTypes.Add(GU.CHOOSE_TABLE_RESULT_V6, typeof(ChooseTableResult));
            HServerAndGServerTypes.Add(GU.LEAVE_TABLE_RESULT_V6, typeof(LeaveTableResult));
            HServerAndGServerTypes.Add(GU.ESCAPE_RESULT_V6, typeof(EscapeResult));
            HServerAndGServerTypes.Add(GU.B_ESCAPE_V6, typeof(BEscape));
            HServerAndGServerTypes.Add(GU.B_TABLE_IN_OUT_V6, typeof(BTableInOut));
            HServerAndGServerTypes.Add(GU.B_TABLE_CHANGED_V6, typeof(BTableChanged));
            HServerAndGServerTypes.Add(GU.SHOW_TABLE_RESULT_V6, typeof(ShowTableResult));
            HServerAndGServerTypes.Add(GU.KICK_OUT_V6, typeof(KickOut));
            HServerAndGServerTypes.Add(GU.B_KICK_OUT_V6, typeof(BKickOut));
            HServerAndGServerTypes.Add(GU.B_KICK_OUT_COUNTER_V6, typeof(BKickOutCounter));
            HServerAndGServerTypes.Add(GU.KICK_OUT_RESULT_V6, typeof(KickOutResult));
            HServerAndGServerTypes.Add(GU.B_COUNTER_V6, typeof(BCounter));
            HServerAndGServerTypes.Add(GU.CHOOSE_SEAT_V6, typeof(ChooseSeat));
            HServerAndGServerTypes.Add(GU.B_PLAYER_CHOOSE_SEAT_V6, typeof(BPlayerChooseSeat));
            HServerAndGServerTypes.Add(GU.READY_V6, typeof(Ready));
            HServerAndGServerTypes.Add(GU.B_USER_READY_V6, typeof(BUserReady));
            HServerAndGServerTypes.Add(GU.READY_RESULT_V6, typeof(ReadyResult));
            HServerAndGServerTypes.Add(GU.START_ROUND_V6, typeof(StartRound));
            HServerAndGServerTypes.Add(GU.TTZ_START_ROUND_V6, typeof(TTZStartBroadcast));
            HServerAndGServerTypes.Add(GU.B_VIP_EFFECT_V6, typeof(BVipEffect));
            HServerAndGServerTypes.Add(GU.TEMP_LEAVE_V6, typeof(TempLeave));
            HServerAndGServerTypes.Add(GU.B_TEMP_LEAVE_V6, typeof(BTempLeave));
            HServerAndGServerTypes.Add(GU.B_JINGONG_REQUEST_V6, typeof(BJinGongRequest));
            HServerAndGServerTypes.Add(GU.JINGONG_V6, typeof(JinGong));
            HServerAndGServerTypes.Add(GU.JINGONG_RESULT_V6, typeof(JinGongResult));
            HServerAndGServerTypes.Add(GU.B_JINGONG_POKER_V6, typeof(BJinGongPoker));
            HServerAndGServerTypes.Add(GU.B_JINGONG_RESULT_V6, typeof(BJinGongResult));
            HServerAndGServerTypes.Add(GU.BEEN_JINGONG_V6, typeof(BeenJinGong));
            HServerAndGServerTypes.Add(GU.B_HUANGONG_REQUEST_V6, typeof(BHuanGongRequest));
            HServerAndGServerTypes.Add(GU.HUANGONG_V6, typeof(HuanGong));
            HServerAndGServerTypes.Add(GU.HUANGONG_RESULT_V6, typeof(HuanGongResult));
            HServerAndGServerTypes.Add(GU.B_HUANGONG_POKER_V6, typeof(BHuanGongPoker));
            HServerAndGServerTypes.Add(GU.BEEN_HUANGONG_V6, typeof(BeenHuanGong));
            HServerAndGServerTypes.Add(GU.B_KANGGONG_V6, typeof(BKangGong));
            HServerAndGServerTypes.Add(GU.B_JIEFENG_V6, typeof(BJieFeng));
            HServerAndGServerTypes.Add(GU.CHUPAI_KEY_V6, typeof(ChuPaiKey));
            HServerAndGServerTypes.Add(GU.B_CHUPAI_KEY_OWNER_V6, typeof(BChuPaiKeyOwner));
            HServerAndGServerTypes.Add(GU.CHUPAI_V6, typeof(ChuPai));
            HServerAndGServerTypes.Add(GU.CHUPAI_RESULT_V6, typeof(ChuPaiResult));
            HServerAndGServerTypes.Add(GU.B_CHUPAI_V6, typeof(BChuPai));
            HServerAndGServerTypes.Add(GU.B_FANBEI_V6, typeof(BFanbei));
            HServerAndGServerTypes.Add(GU.B_ROUND_END_V6, typeof(BRoundEnd));
            HServerAndGServerTypes.Add(GU.R_ROUND_END_V6, typeof(com.morln.game.gd.command.RoundEnd));
            HServerAndGServerTypes.Add(GU.HOST_INFO_RESULT_V6, typeof(HostInfoResult));
            HServerAndGServerTypes.Add(GU.MATCH_RESULT_V6, typeof(MatchResult));
            HServerAndGServerTypes.Add(GU.CANCEL_MATCH_RESULT_V6, typeof(CancelMatchResult));
            HServerAndGServerTypes.Add(GU.MYBAG_RESULT_V6, typeof(MyBagResult));
            HServerAndGServerTypes.Add(GU.BUY_COMMODITY_V6, typeof(BuyCommodity));
            HServerAndGServerTypes.Add(GU.BUY_RESULT_V6, typeof(BuyCommodityResult));
            HServerAndGServerTypes.Add(GU.USE_COMMODITY_V6, typeof(UseCommodity));
            HServerAndGServerTypes.Add(GU.USE_COMMODITY_RESULT_V6, typeof(UseCommodityResult));
            HServerAndGServerTypes.Add(GU.B_USE_COMMODITY_V6, typeof(BUseCommodity));
            HServerAndGServerTypes.Add(GU.COMMODITY_LIST_RESULT_V6, typeof(CommodityListResult));
            HServerAndGServerTypes.Add(GU.POKER_PEEPER_V6, typeof(PokerPeeper));
            HServerAndGServerTypes.Add(GU.POKER_PEEPER_RESULT_V6, typeof(PokerPeeperResult));
            HServerAndGServerTypes.Add(GU.CLOSE_POKER_PEEPER_V6, typeof(ClosePokerPeeper));
            HServerAndGServerTypes.Add(GU.CLOSE_POKER_PEEPER_RESULT_V6, typeof(ClosePokerPeeperResult));
            HServerAndGServerTypes.Add(GU.POKER_RECORDER_RESULT_V6, typeof(PokerRecorderResult));
            HServerAndGServerTypes.Add(GU.CLOSE_POKER_RECORDER_RESULT_V6, typeof(ClosePokerRecorderResult));
            HServerAndGServerTypes.Add(GU.GIFT_V6, typeof(GiftResult));
            HServerAndGServerTypes.Add(GU.PUNISH_V6, typeof(Punish));
            HServerAndGServerTypes.Add(GU.TOAST_V6, typeof(Toast));
            HServerAndGServerTypes.Add(GU.HB_REQ_V6, typeof(HBReq));
            HServerAndGServerTypes.Add(GU.HB_RES_V6, typeof(HBRes));
            HServerAndGServerTypes.Add(GU.LEVEL_UP_V6, typeof(LevelUp));
            HServerAndGServerTypes.Add(GU.ALLOWANCE_V6, typeof(Allowance));
            HServerAndGServerTypes.Add(GU.TREASURE_CHEST_DATA_V6, typeof(TreasureChestData));
            HServerAndGServerTypes.Add(GU.TREASURE_CHEST_REWARD_V6, typeof(TreasureChestReward));
            HServerAndGServerTypes.Add(GU.REQUEST_ACTION_PRICE_V6, typeof(RequestActionPrice));
            HServerAndGServerTypes.Add(GU.ACTION_PRICE_RESULT_V6, typeof(ActionPriceResult));
            HServerAndGServerTypes.Add(GU.PUSH_ITEM_V6, typeof(PushItem));
            HServerAndGServerTypes.Add(GU.INTERACTION_BROADCAST_V6, typeof(BInteraction));
            HServerAndGServerTypes.Add(GU.INTERACTION_RESULT_V6, typeof(InteractionResult));

            //GU Server
            HServerAndGServerMsgServer.Add(GU.REFILL_HINT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.VIP_HINT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.LOGIN_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.LOGIN_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.SHOW_ROOM_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.CHOOSE_TABLE_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.CHOOSE_TABLE_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.LEAVE_TABLE_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.ESCAPE_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_ESCAPE_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_TABLE_IN_OUT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_TABLE_CHANGED_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.SHOW_TABLE_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.KICK_OUT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_KICK_OUT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_KICK_OUT_COUNTER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.KICK_OUT_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_COUNTER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.CHOOSE_SEAT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_PLAYER_CHOOSE_SEAT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.READY_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_USER_READY_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.READY_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.START_ROUND_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.TTZ_START_ROUND_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_VIP_EFFECT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.TEMP_LEAVE_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_TEMP_LEAVE_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_JINGONG_REQUEST_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.JINGONG_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.JINGONG_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_JINGONG_POKER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_JINGONG_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.BEEN_JINGONG_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_HUANGONG_REQUEST_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.HUANGONG_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.HUANGONG_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_HUANGONG_POKER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.BEEN_HUANGONG_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_KANGGONG_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_JIEFENG_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.CHUPAI_KEY_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_CHUPAI_KEY_OWNER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.CHUPAI_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.CHUPAI_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_CHUPAI_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_FANBEI_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_ROUND_END_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.R_ROUND_END_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.HOST_INFO_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.MATCH_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.CANCEL_MATCH_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.MYBAG_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.BUY_COMMODITY_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.BUY_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.USE_COMMODITY_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.USE_COMMODITY_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.B_USE_COMMODITY_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.COMMODITY_LIST_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.POKER_PEEPER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.POKER_PEEPER_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.CLOSE_POKER_PEEPER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.CLOSE_POKER_PEEPER_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.POKER_RECORDER_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.CLOSE_POKER_RECORDER_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.GIFT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.PUNISH_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.TOAST_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.HB_REQ_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.HB_RES_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.LEVEL_UP_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.ALLOWANCE_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.TREASURE_CHEST_DATA_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.TREASURE_CHEST_REWARD_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.REQUEST_ACTION_PRICE_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.ACTION_PRICE_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.PUSH_ITEM_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.INTERACTION_BROADCAST_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(GU.INTERACTION_RESULT_V6, Server.GServer);

            // CU Type
            HServerAndGServerTypes.Add(CU.LOGIN_V6, typeof(ULogin));
            HServerAndGServerTypes.Add(CU.LOGIN_RESULT_V6, typeof(SLoginResult));
            HServerAndGServerTypes.Add(CU.REFILL_HINT_V6, typeof(RefillHint));
            HServerAndGServerTypes.Add(CU.VIP_HINT_V6, typeof(VipHint));
            HServerAndGServerTypes.Add(CU.SEND_TEXT_MSG_V6, typeof(USendTextMsg));
            HServerAndGServerTypes.Add(CU.B_TEXT_MSG_V6, typeof(BTextMsg));
            HServerAndGServerTypes.Add(CU.TEXT_MSG_RESULT_V6, typeof(STextMsgResult));
            HServerAndGServerTypes.Add(CU.B_JIAN_MENG_V6, typeof(BJianMeng));
            HServerAndGServerTypes.Add(CU.JIAN_MENG_RESULT_V6, typeof(JianMengResult));
            HServerAndGServerTypes.Add(CU.SEND_VOICE_MSG_V6, typeof(USendVoiceMsg));
            HServerAndGServerTypes.Add(CU.VOICE_RESULT_V6, typeof(SVoiceResult));
            HServerAndGServerTypes.Add(CU.B_VOICE_MSG_V6, typeof(BVoiceMsg));
            HServerAndGServerTypes.Add(CU.B_SYS_TEXT_MSG_V6, typeof(BSysTextMsg));
            HServerAndGServerTypes.Add(CU.B_SYS_VOICE_MSG_V6, typeof(BSysVoiceMsg));
            HServerAndGServerTypes.Add(CU.HB_REQ_V6, typeof(HBReq));
            HServerAndGServerTypes.Add(CU.HB_RES_V6, typeof(HBRes));
            HServerAndGServerTypes.Add(CU.REWARD_RESULT_V6, typeof(RewardResult));
            HServerAndGServerTypes.Add(CU.FRIEND_LIST_RESULT_V6, typeof(SFriendListResult));
            HServerAndGServerTypes.Add(CU.B_FRIEND_ONLINE_STATE_V6, typeof(BFriendOnlineState));
            HServerAndGServerTypes.Add(CU.FRIEND_DETAIL_V6, typeof(UFriendDetail));
            HServerAndGServerTypes.Add(CU.FRIEND_DETAIL_RESULT_V6, typeof(SFriendDetailResult));
            HServerAndGServerTypes.Add(CU.UPLOAD_MY_STATES_V6, typeof(UUploadMyStates));
            HServerAndGServerTypes.Add(CU.ADD_FRIEND_REQUEST_FROM_SENDER_V6, typeof(UAddFriendRequestFromSender));
            HServerAndGServerTypes.Add(CU.ADD_FRIEND_FAIL_V6, typeof(SAddFriendFail));
            HServerAndGServerTypes.Add(CU.ADD_FRIEND_REQUEST_TO_RECEIVER_V6, typeof(SAddFriendRequestToReceiver));
            HServerAndGServerTypes.Add(CU.ADD_FRIEND_RESPONSE_FROM_RECEIVER_V6, typeof(UAddFriendResponseFromReceiver));
            HServerAndGServerTypes.Add(CU.ADD_FRIEND_RESPONSE_TO_SENDER_V6, typeof(SAddFriendResponseToSender));
            HServerAndGServerTypes.Add(CU.NEW_FRIEND_V6, typeof(SNewFriend));
            HServerAndGServerTypes.Add(CU.REMOVE_FRIEND_V6, typeof(URemoveFriend));
            HServerAndGServerTypes.Add(CU.REMOVE_FRIEND_RESULT_TO_SENDER_V6, typeof(SRemoveFriendResultToSender));
            HServerAndGServerTypes.Add(CU.FRIEND_REMOVED_TO_RECEIVER_V6, typeof(SFriendRemovedToReceiver));
            HServerAndGServerTypes.Add(CU.SEARCH_USER_V6, typeof(UCSearchUser));
            HServerAndGServerTypes.Add(CU.SEARCH_USER_RESULT_V6, typeof(CUSearchUserResult));
            HServerAndGServerTypes.Add(CU.TOAST_V6, typeof(Toast));

            // CU Server
            HServerAndGServerMsgServer.Add(CU.LOGIN_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.LOGIN_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.REFILL_HINT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.VIP_HINT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.SEND_TEXT_MSG_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.B_TEXT_MSG_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.TEXT_MSG_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.B_JIAN_MENG_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.JIAN_MENG_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.SEND_VOICE_MSG_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.VOICE_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.B_VOICE_MSG_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.B_SYS_TEXT_MSG_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.B_SYS_VOICE_MSG_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.HB_REQ_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.HB_RES_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.REWARD_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.FRIEND_LIST_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.B_FRIEND_ONLINE_STATE_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.FRIEND_DETAIL_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.FRIEND_DETAIL_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.UPLOAD_MY_STATES_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.ADD_FRIEND_REQUEST_FROM_SENDER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.ADD_FRIEND_FAIL_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.ADD_FRIEND_REQUEST_TO_RECEIVER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.ADD_FRIEND_RESPONSE_FROM_RECEIVER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.ADD_FRIEND_RESPONSE_TO_SENDER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.NEW_FRIEND_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.REMOVE_FRIEND_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.REMOVE_FRIEND_RESULT_TO_SENDER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.FRIEND_REMOVED_TO_RECEIVER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.SEARCH_USER_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.SEARCH_USER_RESULT_V6, Server.GServer);
            HServerAndGServerMsgServer.Add(CU.TOAST_V6, Server.GServer);
        }

        private static readonly Dictionary<int, Type> PServerTypes = new Dictionary<int, Type>();


        public static ProtoMessage CreatePServerEmptyMsg(int cmdType)
        {
            var msg = new ProtoMessage();
            msg.Type = cmdType;
            msg.Server = Server.PServer;

            if (PServerTypes.ContainsKey(cmdType))
            {
                msg.Model = PServerTypes[cmdType];
            }

            return msg;
        }

        private static readonly Dictionary<int, Type> HServerAndGServerTypes = new Dictionary<int, Type>();

        private static readonly Dictionary<int, Server> HServerAndGServerMsgServer = new Dictionary<int, Server>();

        public static ProtoMessage CreateHServerAndGServerEmptyMsg(int cmdType)
        {
            var msg = new ProtoMessage();
            msg.Type = cmdType;

            if (HServerAndGServerMsgServer.ContainsKey(cmdType))
            {
                msg.Server = HServerAndGServerMsgServer[cmdType];
            }

            if (HServerAndGServerTypes.ContainsKey(cmdType))
            {
                msg.Model = HServerAndGServerTypes[cmdType];
            }

            return msg;
        }

        // PServer和客户端的命令类型。
        public static class PU
        {
            public const int CLIENT_VERSION = 1;
            public const int CLIENT_VERSION_RESULT = 2;

            public const int REGISTER = 11;
            public const int REGISTER_RESULT = 12;

            public const int LOGIN = 21;
            public const int LOGIN_RESULT = 22;

            public const int VISITOR_LOGIN = 31;
            public const int VISITOR_LOGIN_RESULT = 32;

            public const int VISITOR_REGULARIZE = 41;
            public const int VISITOR_REGULARIZE_RESULT = 42;

            public const int DEVICE_INFO = 61;

            public const int TOAST = 71;

            public const int WECHAT_AUTH = 85;
            public const int WECHAT_AUTH_RESULT = 86;

            public const int WECHAT_LOGIN = 87;
            public const int WECHAT_LOGIN_RESULT = 88;
        }

        public static class HU
        {
            // ---------------- hserver 命令 ----------------

            public const int LOGIN = 1;
            public const int LOGIN_RESULT = 2;

            public const int REQUEST_LOGIN_REWARD_CONFIG = 3;
            public const int LOGIN_REWARD_CONFIG = 4;
            public const int REQUEST_LOGIN_REWARD = 5;

            public const int REQUEST_GIFT = 11;
            public const int GIFT_RESULT = 12;

            public const int RACE_REWARD = 13;
            public const int REFILL_HINT = 14;
            public const int VIP_HINT = 15;
            public const int FIRST_REFILL_HINT = 16;

            public const int REQUEST_CHAT_SERVER_ADDR = 21;
            public const int CHAT_SERVER_ADDR = 22;

            public const int BIND_USER_PUSH_INFO = 26;
            public const int BIND_USER_PUSH_INFO_OK = 27;
            public const int BIND_USER_PUSH_INFO_FAIL = 28;

            public const int REQUEST_SHOW_ROOMS = 31;
            public const int SHOW_ROOMS_RESULT = 32;

            public const int CHOOSE_ROOM = 41;
            public const int GAME_SERVER_ADDR = 42;
            public const int CHOOSE_ROOM_RESULT = 43;
            public const int QUICK_START = 44;
            public const int REQUEST_CHOOSE_ROOM = 45;

            // version >= 6.1
            public const int CHOOSE_ROOM_FAIL = 46;

            public const int LEAVE_ROOM = 51;
            public const int LEAVE_ROOM_RESULT = 52;
            public const int ROOM_IN_OUT = 53;

            public const int LEAVE_HALL = 61;
            public const int LEAVE_HALL_RESULT = 62;

            public const int REQUEST_USER_INFO = 71;
            public const int USER_INFO_RESULT = 72;

            public const int EDIT_USER_INFO = 73;
            public const int EDIT_USER_INFO_RESULT = 74;

            public const int CHOOSE_NICKNAME = 75;
            public const int CHOOSE_NICKNAME_RESULT = 76;

            public const int EDIT_PASSWORD = 77;
            public const int EDIT_PASSWORD_RESULT = 78;

            public const int HB_REQ = 81;
            public const int HB_RES = 82;

            public const int MYBAG = 91;
            public const int MYBAG_RESULT = 92;

            public const int BUY_COMMODITY = 101;
            public const int BUY_COMMODITY_RESULT = 102;

            public const int USE_COMMODITY = 111;
            public const int USE_COMMODITY_RESULT = 112;
            public const int B_USE_COMMODITY = 113;
            public const int UPGRADE_COMMODITY = 114;
            public const int UPGRADE_COMMODITY_RESULT = 115;

            public const int SALE_COMMODITY = 116;
            public const int SALE_COMMODITY_RESULT = 117;

            public const int COMMODITY_LIST = 121;
            public const int COMMODITY_LIST_RESULT = 122;

            public const int BEEN_REPLACED = 131;

            public const int REQUEST_HALL_DATA = 141;
            public const int HALL_DATA = 142;
            public const int DAILY_LOGIN_REWARD = 143;

            public const int REQUEST_ROOM_DATA = 151;
            public const int ROOM_DATA = 152;

            public const int DEVICE_INFO = 161;

            public const int SUBMIT_PRIVACY = 181;
            public const int CHANGE_PRIVACY_RESULT = 182;

            public const int GET_PRIVACY = 183;
            public const int PRIVACY_RESULT = 184;

            public const int REQUEST_TRADE_NO = 191;
            public const int TRADE_NO_RESULT = 192;

            public const int CHECK_TRADE = 201;
            public const int CHECK_TRADE_RESULT = 202;

            public const int SUBMIT_WEIBO_INFO = 221;
            public const int SUBMIT_WEIBO_RESULT = 222;

            public const int REQUEST_REWARD = 231;
            public const int REWARD_RESULT = 232;

            public const int REQUEST_EXCHANGE = 241;
            public const int EXCHANGE_RESULT = 242;

            public const int REQUEST_VIP_EXCHANGE_LIST = 245;
            public const int VIP_EXCHANGE_LIST_RESULT = 246;

            public const int REQUEST_EXCHANGE_VIP = 247;
            public const int REQUEST_EXCHANGE_VIP_RESULT = 248;

            public const int VISITOR_REGULARIZE = 251;
            public const int VISITOR_REGULARIZE_RESULT = 252;

            public const int PUNISH = 970;
            public const int TOAST = 980;
            public const int SYS_TEXT_MSG = 990;

            public const int ERROR_STATE = 1000;

            public const int LEVEL_UP = 1010;
            public const int ALLOWANCE = 1100;

            public const int TREASURE_CHEST_DATA = 1200;

            public const int REGISTER = 1300;
            public const int REGISTER_RESULT = 1301;

            public const int RESET_WIN_RATE = 1401;
            public const int RESET_WIN_RATE_RESULT = 1402;

            public const int EDIT_NICKNAME = 1411;
            public const int EDIT_NICKNAME_RESULT = 1412;

            /**
             * 改变性别。
             * version >= 6.4.0
             */
            public const int CHANGE_SEX = 1416;

            /**
             * 改变性别的结果。
             * version >= 6.4.0
             */
            public const int CHANGE_SEX_RESULT = 1417;

            public const int REQUEST_ACTION_PRICE = 1421;
            public const int ACTION_PRICE_RESULT = 1422;

            public const int TRACE_USER = 1431;
            public const int TRACE_USER_RESULT = 1432;

            public const int REQUEST_AWARD = 1441;
            public const int REQUEST_AWARD_RESULT = 1442;

            public const int PUSH_HINT = 1450;

            public const int PUSH_ITEM = 1451;

            #region 签到

            /// <summary>
            /// 申请签到配置数据。
            /// version >= 6.4.0
            /// </summary>
            public const int REQUEST_CHECKIN_CONFIG = 1471;

            /// <summary>
            /// 签到配置数据。
            /// version >= 6.4.0
            /// </summary>
            public const int CHECKIN_CONFIG = 1472;

            /**
             * 签到。
             * version >= 6.4.0
             */
            public const int CHECKIN = 1473;

            /**
             * 签到结果。
             * version >= 6.4.0
             */
            public const int CHECKIN_RESULT = 1474;

            /**
             * 补签。
             * version >= 6.4.0
             */
            public const int RECHECKIN = 1475;

            /**
             * 补签结果。
             * version >= 6.4.0
             */
            public const int RECHECKIN_RESULT = 1476;

            /**
             * 签到的奖励。
             * version >= 6.4.0
             */
            public const int CHECKIN_AWARD_RESULT = 1477;

            #endregion

            #region activity 活动

            /**
             * 请求活动配置数据。
             */
            public const int REQUEST_ACTIVITY_CONFIG = 1481;

            /**
             * 活动配置数据。
             * version >= 6.4.0
             */
            public const int ACTIVITY_CONFIG_RESULT = 1482;

            /**
             * 请求我的活动状态。
             * version >= 6.4.0
             */
            public const int REQUEST_ACTIVITY_STATUS = 1483;

            /**
             * 我的活动状态数据。
             * version >= 6.4.0
             */
            public const int ACTIVITY_STATUS_RESULT = 1484;

            /**
             * 请求活动奖励。
             * version >= 6.4.0
             */
            public const int REQUEST_ACTIVITY_AWARD = 1485;

            /**
             * 活动奖励的结果。
             * version >= 6.4.0
             */
            public const int ACTIVITY_AWARD_RESULT = 1486;

            /**
             * 活动完成的提示。
             * version >= 6.4.0
             */
            public const int ACTIVITY_TIP = 1487;

            #endregion

            #region 邮件

            /// <summary>
            /// 请求邮件摘要列表。
            /// </summary>
            public const int REQUEST_MAIL_BRIEF_LIST = 1491;

            ///<summary>
            /// 邮件摘要列表。
            /// </summary>
            public const int MAIL_BRIEF_LIST_RESULT = 1492;

            /// <summary>
            /// 请求邮件的内容。
            /// </summary>
            public const int REQUEST_MAIL_CONTENT = 1493;

            /// <summary>
            /// 邮件内容结果。
            /// </summary>
            public const int MAIL_CONTENT_RESULT = 1494;

            #endregion

            #region 邀请

            public const int REQUEST_MY_INVITE_DATA = 1501;

            public const int MY_INVITE_DATA_RESULT = 1502;

            public const int REQUEST_BEEN_INVITED_AWARD = 1503;

            public const int BEEN_INVITED_AWARD_RESULT = 1504;

            public const int REQUEST_INVITE_AWARD = 1505;

            public const int INVITE_AWARD_RESULT = 1506;

            #endregion

            #region 元宝

            /// <summary>
            /// 兑换元宝商品。
            /// </summary>
            public const int EXCHANGE_YUANBAO = 1666;

            /// <summary>
            /// 兑换元宝商品的结果。
            /// </summary>
            public const int EXCHANGE_YUANBAO_RESULT = 1667;

            /// <summary>
            /// 请求我的元宝兑换记录。
            /// </summary>
            public const int REQUEST_MY_YUANBAO_EXCHANGE = 1668;

            /// <summary>
            /// 我的元宝兑换记录结果。
            /// </summary>
            public const int MY_YUANBAO_EXCHANGE_RESULT = 1669;

            /// <summary>
            /// 请求元宝配置数据。
            /// </summary>
            public const int REQUEST_YUANBAO_CONFIG = 1671;

            /// <summary>
            /// 元宝配置数据。
            /// </summary>
            public const int YUANBAO_CONFIG_RESULT = 1672;

            #endregion

            #region 微信绑定

            public const int WECHAT_BIND = 1673;

            public const int WECHAT_BIND_RESULT = 1674;

            #endregion

            #region 比赛房

            public const int APPLY_RACE = 1700;

            public const int APPLY_RACE_RESULT = 1701;

            public const int RACE_AWARD = 1704;

            public const int REQUEST_RACE_CONFIG_LIST = 1705;

            public const int RACE_CONFIG_LIST_RESULT = 1706;

            public const int RACE_CONFIG = 1707;

            #endregion

            #region 每日任务

            public const int REQUEST_USER_TASK_LIST = 1801;

            public const int USER_TASK_LIST_RESULT = 1802;

            public const int USER_TASK_TIP = 1803;

            public const int GET_USER_TASK_AWARD = 1804;

            public const int GET_USER_TASK_AWARD_RESULT = 1805;

            public const int NOTIFY_DO_SHARE = 1806;

            public const int NOTIFY_DO_SHARE_RESULT = 1807;

            #endregion

            // ---------------- gserver 命令 ----------------
            // 在原gserver命令的基础上所有的值都+10000

            // ---------------- cserver 命令 ----------------
            // 在所有原cserver的值上+20000。
        }

        public static class GU
        {
            public const int REFILL_HINT = 0;
            public const int VIP_HINT = 1;

            public const int LOGIN = 11;
            public const int LOGIN_RESULT = 12;

            public const int SHOW_ROOM = 21;
            public const int SHOW_ROOM_RESULT = 22;

            public const int CHOOSE_TABLE = 31;
            public const int CHOOSE_TABLE_RESULT = 32;

            public const int LEAVE_TABLE = 41;
            public const int LEAVE_TABLE_RESULT = 42;

            public const int ESCAPE = 46;
            public const int ESCAPE_RESULT = 47;
            public const int B_ESCAPE = 48;

            public const int B_TABLE_IN_OUT = 51;
            public const int B_TABLE_CHANGED = 52;

            public const int SHOW_TABLE = 61;
            public const int SHOW_TABLE_RESULT = 62;

            public const int KICK_OUT = 71;
            public const int B_KICK_OUT = 72;
            public const int B_KICK_OUT_COUNTER = 73;
            public const int KICK_OUT_RESULT = 74;

            public const int CHOOSE_SEAT = 81;
            public const int B_PLAYER_CHOOSE_SEAT = 82;

            public const int READY = 91;
            public const int B_USER_READY = 92;
            public const int READY_RESULT = 93;

            public const int TTZ_START_ROUND = 110;
            public const int START_ROUND = 121;
            public const int B_VIP_EFFECT = 125;

            public const int TEMP_LEAVE = 131;
            public const int B_TEMP_LEAVE = 132;

            public const int B_ROTATE_SEAT = 134;

            public const int B_JINGONG_REQUEST = 141;
            public const int JINGONG = 142;
            public const int JINGONG_RESULT = 143;
            public const int B_JINGONG_POKER = 144;
            public const int B_JINGONG_RESULT = 145;
            public const int BEEN_JINGONG = 146;

            public const int B_HUANGONG_REQUEST = 151;
            public const int HUANGONG = 152;
            public const int HUANGONG_RESULT = 153;
            public const int B_HUANGONG_POKER = 154;
            public const int BEEN_HUANGONG = 155;

            public const int B_KANGGONG = 161;

            public const int B_JIEFENG = 171;

            public const int START_CHUPAI = 172;
            public const int CHUPAI_KEY = 181;
            public const int B_CHUPAI_KEY_OWNER = 182;

            public const int CHUPAI = 191;
            public const int CHUPAI_RESULT = 192;
            public const int B_CHUPAI = 193;

            public const int B_FANBEI = 195;

            public const int B_ROUND_END = 201;

            public const int REQUEST_HOST_INFO = 211;
            public const int HOST_INFO_RESULT = 212;

            public const int MATCH = 221;
            public const int MATCH_RESULT = 222;
            public const int B_MATCH_INFO = 223;

            public const int CANCEL_MATCH = 231;
            public const int CANCEL_MATCH_RESULT = 232;

            public const int MYBAG = 241;
            public const int MYBAG_RESULT = 242;

            public const int BUY_COMMODITY = 251;
            public const int BUY_RESULT = 252;

            public const int USE_COMMODITY = 261;
            public const int USE_COMMODITY_RESULT = 262;
            public const int B_USE_COMMODITY = 263;

            public const int COMMODITY_LIST = 271;
            public const int COMMODITY_LIST_RESULT = 272;

            public const int POKER_PEEPER = 281;
            public const int POKER_PEEPER_RESULT = 282;

            public const int CLOSE_POKER_PEEPER = 283;
            public const int CLOSE_POKER_PEEPER_RESULT = 284;

            public const int POKER_RECORDER = 291;
            public const int POKER_RECORDER_RESULT = 292;

            public const int CLOSE_POKER_RECORDER = 293;
            public const int CLOSE_POKER_RECORDER_RESULT = 294;

            public const int GIFT = 960;
            public const int PUNISH = 970;
            public const int TOAST = 980;

            public const int HB_REQ = 991;
            public const int HB_RES = 992;

            public const int LEVEL_UP = 1000;
            public const int ALLOWANCE = 1100;

            public const int TREASURE_CHEST_DATA = 1200;
            public const int TREASURE_CHEST_REWARD = 1201;

            public const int REQUEST_ACTION_PRICE = 1421;
            public const int ACTION_PRICE_RESULT = 1422;

            public const int PUSH_ITEM = 1451;

            public const int INTERACTION = 1460;
            public const int INTERACTION_RESULT = 1461;
            public const int INTERACTION_BROADCAST = 1462;

            // 新版V6版的cmdType。
            public const int REFILL_HINT_V6 = 10000;

            public const int VIP_HINT_V6 = 10001;

            public const int LOGIN_V6 = 10011;
            public const int LOGIN_RESULT_V6 = 10012;

            public const int SHOW_ROOM_V6 = 10021;
            public const int SHOW_ROOM_RESULT_V6 = 10022;

            public const int CHOOSE_TABLE_V6 = 10031;
            public const int CHOOSE_TABLE_RESULT_V6 = 10032;

            public const int LEAVE_TABLE_V6 = 10041;
            public const int LEAVE_TABLE_RESULT_V6 = 10042;

            public const int ESCAPE_V6 = 10046;
            public const int ESCAPE_RESULT_V6 = 10047;
            public const int B_ESCAPE_V6 = 10048;

            public const int B_TABLE_IN_OUT_V6 = 10051;
            public const int B_TABLE_CHANGED_V6 = 10052;

            public const int SHOW_TABLE_V6 = 10061;
            public const int SHOW_TABLE_RESULT_V6 = 10062;

            public const int KICK_OUT_V6 = 10071;
            public const int B_KICK_OUT_V6 = 10072;
            public const int B_KICK_OUT_COUNTER_V6 = 10073;
            public const int KICK_OUT_RESULT_V6 = 10074;

            public const int B_COUNTER_V6 = 10075;

            public const int CHOOSE_SEAT_V6 = 10081;
            public const int B_PLAYER_CHOOSE_SEAT_V6 = 10082;

            public const int READY_V6 = 10091;
            public const int B_USER_READY_V6 = 10092;
            public const int READY_RESULT_V6 = 10093;

            public const int TTZ_START_ROUND_V6 = 10110;
            public const int START_ROUND_V6 = 10121;
            public const int B_VIP_EFFECT_V6 = 10125;

            public const int TEMP_LEAVE_V6 = 10131;
            public const int B_TEMP_LEAVE_V6 = 10132;

            public const int B_ROTATE_SEAT_V6 = 10134;

            public const int B_JINGONG_REQUEST_V6 = 10141;
            public const int JINGONG_V6 = 10142;
            public const int JINGONG_RESULT_V6 = 10143;
            public const int B_JINGONG_POKER_V6 = 10144;
            public const int B_JINGONG_RESULT_V6 = 10145;
            public const int BEEN_JINGONG_V6 = 10146;

            public const int B_HUANGONG_REQUEST_V6 = 10151;
            public const int HUANGONG_V6 = 10152;
            public const int HUANGONG_RESULT_V6 = 10153;
            public const int B_HUANGONG_POKER_V6 = 10154;
            public const int BEEN_HUANGONG_V6 = 10155;

            public const int B_KANGGONG_V6 = 10161;

            public const int B_JIEFENG_V6 = 10171;
            public const int START_CHUPAI_V6 = 10172;

            public const int CHUPAI_KEY_V6 = 10181;
            public const int B_CHUPAI_KEY_OWNER_V6 = 10182;

            public const int CHUPAI_V6 = 10191;
            public const int CHUPAI_RESULT_V6 = 10192;
            public const int B_CHUPAI_V6 = 10193;

            public const int B_FANBEI_V6 = 10195;

            public const int B_ROUND_END_V6 = 10201;

            public const int R_ROUND_END_V6 = 10202;

            public const int REQUEST_HOST_INFO_V6 = 10211;
            public const int HOST_INFO_RESULT_V6 = 10212;

            public const int MATCH_V6 = 10221;
            public const int MATCH_RESULT_V6 = 10222;
            public const int B_MATCH_INFO_V6 = 10223;

            public const int CANCEL_MATCH_V6 = 10231;
            public const int CANCEL_MATCH_RESULT_V6 = 10232;

            public const int MYBAG_V6 = 10241;
            public const int MYBAG_RESULT_V6 = 10242;

            public const int BUY_COMMODITY_V6 = 10251;
            public const int BUY_RESULT_V6 = 10252;

            public const int USE_COMMODITY_V6 = 10261;
            public const int USE_COMMODITY_RESULT_V6 = 10262;
            public const int B_USE_COMMODITY_V6 = 10263;

            public const int COMMODITY_LIST_V6 = 10271;
            public const int COMMODITY_LIST_RESULT_V6 = 10272;

            public const int POKER_PEEPER_V6 = 10281;
            public const int POKER_PEEPER_RESULT_V6 = 10282;

            public const int CLOSE_POKER_PEEPER_V6 = 10283;
            public const int CLOSE_POKER_PEEPER_RESULT_V6 = 10284;

            public const int POKER_RECORDER_V6 = 10291;
            public const int POKER_RECORDER_RESULT_V6 = 10292;

            public const int CLOSE_POKER_RECORDER_V6 = 10293;
            public const int CLOSE_POKER_RECORDER_RESULT_V6 = 10294;

            public const int GIFT_V6 = 10960;
            public const int PUNISH_V6 = 10970;
            public const int TOAST_V6 = 10980;

            public const int HB_REQ_V6 = 10991;
            public const int HB_RES_V6 = 10992;

            public const int LEVEL_UP_V6 = 11000;
            public const int ALLOWANCE_V6 = 11100;

            public const int TREASURE_CHEST_DATA_V6 = 11200;
            public const int TREASURE_CHEST_REWARD_V6 = 11201;

            public const int REQUEST_ACTION_PRICE_V6 = 11421;
            public const int ACTION_PRICE_RESULT_V6 = 11422;

            public const int PUSH_ITEM_V6 = 11451;

            public const int INTERACTION_V6 = 11460;
            public const int INTERACTION_RESULT_V6 = 11461;
            public const int INTERACTION_BROADCAST_V6 = 11462;

            public static int TypeV6(int type)
            {
                return type + 10000;
            }
        }

        public static class CU
        {
            public const int LOGIN = 11;
            public const int LOGIN_RESULT = 12;

            public const int REFILL_HINT = 14;
            public const int VIP_HINT = 15;

            public const int SEND_TEXT_MSG = 21;
            public const int B_TEXT_MSG = 22;
            public const int TEXT_MSG_RESULT = 23;

            public const int SEND_VOICE_MSG = 31;
            public const int VOICE_RESULT = 32;
            public const int B_VOICE_MSG = 33;

            public const int B_SYS_TEXT_MSG = 41;

            public const int B_SYS_VOICE_MSG = 51;

            public const int HB_REQ = 61;
            public const int HB_RES = 62;

            public const int MAILBOX = 71;
            public const int MAILBOX_RESULT = 72;

            public const int READ_MAIL = 73;
            public const int MAIL_DETIAL = 74;

            // deprecated public const int U_SUBMIT_PROMT_CODE = 81;
            // deprecated public const int S_PROMT_CODE_RESULT = 82;
            // deprecated public const int U_REQUEST_REWARD = 91;

            public const int REWARD_RESULT = 92;

            public const int REFRESH_FRIEND_LIST = 101;
            public const int FRIEND_LIST_RESULT = 102;

            public const int B_FRIEND_ONLINE_STATE = 111;

            public const int FRIEND_DETAIL = 121;
            public const int FRIEND_DETAIL_RESULT = 122;

            public const int UPLOAD_MY_STATES = 131;

            public const int ADD_FRIEND_REQUEST_FROM_SENDER = 141;
            public const int ADD_FRIEND_FAIL = 142;
            public const int ADD_FRIEND_REQUEST_TO_RECEIVER = 143;
            public const int ADD_FRIEND_RESPONSE_FROM_RECEIVER = 144;
            public const int ADD_FRIEND_RESPONSE_TO_SENDER = 145;
            public const int NEW_FRIEND = 146;

            public const int REMOVE_FRIEND = 151;
            public const int REMOVE_FRIEND_RESULT_TO_SENDER = 152;
            public const int FRIEND_REMOVED_TO_RECEIVER = 153;

            public const int SEARCH_USER = 161;
            public const int SEARCH_USER_RESULT = 162;

            public const int TOAST = 171;

            // 新版V6的cmdType。
            public const int LOGIN_V6 = 20011;

            public const int LOGIN_RESULT_V6 = 20012;

            public const int REFILL_HINT_V6 = 20014;
            public const int VIP_HINT_V6 = 20015;

            public const int SEND_TEXT_MSG_V6 = 20021;
            public const int B_TEXT_MSG_V6 = 20022;
            public const int TEXT_MSG_RESULT_V6 = 20023;
            public const int SEND_JIAN_MENG_V6 = 20024;
            public const int B_JIAN_MENG_V6 = 20025;
            public const int JIAN_MENG_RESULT_V6 = 20026;

            public const int SEND_VOICE_MSG_V6 = 20031;
            public const int VOICE_RESULT_V6 = 20032;
            public const int B_VOICE_MSG_V6 = 20033;

            public const int B_SYS_TEXT_MSG_V6 = 20041;

            public const int B_SYS_VOICE_MSG_V6 = 20051;

            public const int HB_REQ_V6 = 20061;
            public const int HB_RES_V6 = 20062;

            public const int MAILBOX_V6 = 20071;
            public const int MAILBOX_RESULT_V6 = 20072;

            public const int READ_MAIL_V6 = 20073;
            public const int MAIL_DETIAL_V6 = 20074;

            public const int REWARD_RESULT_V6 = 20092;

            public const int REFRESH_FRIEND_LIST_V6 = 20101;
            public const int FRIEND_LIST_RESULT_V6 = 20102;

            public const int B_FRIEND_ONLINE_STATE_V6 = 20111;

            public const int FRIEND_DETAIL_V6 = 20121;
            public const int FRIEND_DETAIL_RESULT_V6 = 20122;

            public const int UPLOAD_MY_STATES_V6 = 20131;

            public const int ADD_FRIEND_REQUEST_FROM_SENDER_V6 = 20141;
            public const int ADD_FRIEND_FAIL_V6 = 20142;
            public const int ADD_FRIEND_REQUEST_TO_RECEIVER_V6 = 20143;
            public const int ADD_FRIEND_RESPONSE_FROM_RECEIVER_V6 = 20144;
            public const int ADD_FRIEND_RESPONSE_TO_SENDER_V6 = 20145;
            public const int NEW_FRIEND_V6 = 20146;

            public const int REMOVE_FRIEND_V6 = 20151;
            public const int REMOVE_FRIEND_RESULT_TO_SENDER_V6 = 20152;
            public const int FRIEND_REMOVED_TO_RECEIVER_V6 = 20153;

            public const int SEARCH_USER_V6 = 20161;
            public const int SEARCH_USER_RESULT_V6 = 20162;

            public const int TOAST_V6 = 20171;

            public static int TypeV6(int type)
            {
                return type + 20000;
            }
        }
    }
}