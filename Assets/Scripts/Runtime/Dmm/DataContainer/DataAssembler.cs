using System;
using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataRelation;
using Dmm.Help;
using Dmm.MoreFunction;
using Dmm.Race;
using Dmm.Report;
using Dmm.Res;

namespace Dmm.DataContainer
{
    public class DataAssembler : IDataAssembler
    {
        private readonly IDataContainerFactory _dcFactory;

        public DataAssembler()
        {
            _dcFactory = new DataContainerFactory();
        }

        public void AssembleData(IDataRepository dataRepository)
        {
            AddPServerContainer(dataRepository);

            #region HLoginResult

            var hLoginResult = _dcFactory.CreateRootDataContainer<HLoginResult>();

            //用户数据更新的时间 myUserContainer.TimeStamp
            var myUser =
                _dcFactory.CreateChildDataContainer(new UserToHLoginResultRelation(hLoginResult));

            var hallData =
                _dcFactory.CreateChildDataContainer(new HallDataToHLoginResultRelation(hLoginResult));

            //大厅房间列表数据
            var roomList =
                _dcFactory.CreateChildDataContainer(new RoomListToHallDataRelation(hallData));

            var commodityList =
                _dcFactory.CreateChildDataContainer(new CommodityToHallDataRelation(hallData));

            //商品数据更新的时间 玩家的背包 玩家的背包物品发生变化时刷新时间
            var myBag =
                _dcFactory.CreateChildDataContainer(new BagToHallDataRelation(hallData));

            var levelList =
                _dcFactory.CreateChildDataContainer(new LevelListToHallDataRelation(hallData));
            var payChannelList =
                _dcFactory.CreateChildDataContainer(new PayChannelToHallDataRelation(hallData));
            var prepaymentList =
                _dcFactory.CreateChildDataContainer(new PrepaymentListToHallDataRelation(hallData));
            var vipConfigList =
                _dcFactory.CreateChildDataContainer(new VipConfigListToHallDataRelation(hallData));
            var exchangeList =
                _dcFactory.CreateChildDataContainer(new ExchangeListToHallDataRelation(hallData));
            var interactionPrice =
                _dcFactory.CreateChildDataContainer(new InteractionPriceToHallDataRelation(hallData));
            var interactionDescription =
                _dcFactory.CreateChildDataContainer(
                    new InteractionDescriptionToHallDataRelation(hallData));
            //微信广告 WeChatAds >= 6.4.0
            var wechatAdsList =
                _dcFactory.CreateChildDataContainer(new WeChatAdsToHallDataRelation(hallData));
            //贱萌表情 JianMengItem >=6.4.0
            var jianMengItemList =
                _dcFactory.CreateChildDataContainer(
                    new JianMengItemListToHallDataRelation(hallData));
            var inviteConfig =
                _dcFactory.CreateChildDataContainer(new InviteConfigToHallDataRelation(hallData));
            var activityConfigList =
                _dcFactory.CreateChildDataContainer(
                    new ActivityConfigListToHallDataRelation(hallData));
            var latestMailTimestamp =
                _dcFactory.CreateChildDataContainer(
                    new LatestMailTimestampToHallDataRelation(hallData));
            var beenInvited =
                _dcFactory.CreateChildDataContainer(new BeenInvitedToHallDataRelation(hallData));
            var featureSwitch =
                _dcFactory.CreateChildDataContainer(new FeatureSwitchToHallDataRelation(hallData));
            var hintItemList =
                _dcFactory.CreateChildDataContainer(new HintItemListToHallDataRelation(hallData));
            var chatServerAddr =
                _dcFactory.CreateChildDataContainer(new ChatServerAddrToHallDataRelation(hallData));

            dataRepository.AddContainer(DataKey.HLoginResult, hLoginResult);
            dataRepository.AddContainer(DataKey.MyUser, myUser);

            dataRepository.AddContainer(DataKey.HallData, hallData);

            dataRepository.AddContainer(DataKey.RoomList, roomList);
            dataRepository.AddContainer(DataKey.PrepaymentList, prepaymentList);


            dataRepository.AddContainer(DataKey.CommodityList, commodityList);
            dataRepository.AddContainer(DataKey.MyBag, myBag);
            dataRepository.AddContainer(DataKey.LevelList, levelList);
            dataRepository.AddContainer(DataKey.PayChannelList, payChannelList);
            dataRepository.AddContainer(DataKey.VipConfigList, vipConfigList);
            dataRepository.AddContainer(DataKey.InteractionPrice, interactionPrice);
            dataRepository.AddContainer(DataKey.InteractionDescription,
                interactionDescription);
            dataRepository.AddContainer(DataKey.WeChatAdsList, wechatAdsList);
            dataRepository.AddContainer(DataKey.JianMengItemList, jianMengItemList);
            dataRepository.AddContainer(DataKey.ActivityConfigList, activityConfigList);
            dataRepository.AddContainer(DataKey.InviteConfig, inviteConfig);
            dataRepository.AddContainer(DataKey.LatestMailTimestamp, latestMailTimestamp);
            dataRepository.AddContainer(DataKey.BeenInvited, beenInvited);
            dataRepository.AddContainer(DataKey.FeatureSwitch, featureSwitch);
            dataRepository.AddContainer(DataKey.HintItemList, hintItemList);
            dataRepository.AddContainer(DataKey.ChatServerAddr, chatServerAddr);
            dataRepository.AddContainer(DataKey.ExchangeList, exchangeList);

            #endregion

            #region EditUserInfoResult、ChooseNicknameResult、EditPasswordResult、VisitorRegularizeResult、HRegisterResult

            var editUserInfoResult = _dcFactory.CreateRootDataContainer<EditUserInfoResult>();
            var chooseNicknameResult = _dcFactory.CreateRootDataContainer<ChooseNicknameResult>();
            var editPasswordResult = _dcFactory.CreateRootDataContainer<EditPasswordResult>();
            var visitorRegularizeResult = _dcFactory.CreateRootDataContainer<VisitorRegularizeResult>();
            var hRegisterResult = _dcFactory.CreateRootDataContainer<HRegisterResult>();

            dataRepository.AddContainer(DataKey.EditUserInfoResult, editUserInfoResult);
            dataRepository.AddContainer(DataKey.ChooseNicknameResult, chooseNicknameResult);
            dataRepository.AddContainer(DataKey.EditPasswordResult, editPasswordResult);
            dataRepository.AddContainer(DataKey.VisitorRegularizeResult, visitorRegularizeResult);
            dataRepository.AddContainer(DataKey.HRegisterResult, hRegisterResult);

            #endregion

            #region 宝箱数据

            var treasureChestData = _dcFactory.CreateRootDataContainer<TreasureChestData>();
            dataRepository.AddContainer(DataKey.TreasureChestData, treasureChestData);

            #endregion

            #region 商品数据

            var buyCommodityResult = _dcFactory.CreateRootDataContainer<BuyCommodityResult>();
            var saleCommodityResult = _dcFactory.CreateRootDataContainer<SaleCommodityResult>();
            var useCommodityResult = _dcFactory.CreateRootDataContainer<UseCommodityResult>();

            dataRepository.AddContainer(DataKey.BuyCommodityResult, buyCommodityResult);
            dataRepository.AddContainer(DataKey.SaleCommodityResult, saleCommodityResult);
            dataRepository.AddContainer(DataKey.UseCommodityResult, useCommodityResult);

            #endregion

            #region 兑换包数据

            var exchangeResult = _dcFactory.CreateRootDataContainer<ExchangeResult>();
            dataRepository.AddContainer(DataKey.ExchangeResult, exchangeResult);

            #endregion

            #region 邀请 Invite >= 6.4.0

            var inviteDataResult = _dcFactory.CreateRootDataContainer<InviteDataResult>();
            var inviteData =
                _dcFactory.CreateChildDataContainer(
                    new InviteDataToInviteDataResultRelation(inviteDataResult));

            var beenInvitedAwardResult = _dcFactory.CreateRootDataContainer<BeenInvitedAwardResult>();
            var inviteAwardResult = _dcFactory.CreateRootDataContainer<InviteAwardResult>();

            dataRepository.AddContainer(DataKey.InviteDataResult, inviteDataResult);
            dataRepository.AddContainer(DataKey.InviteDataResultInviteData, inviteData);

            dataRepository.AddContainer(DataKey.BeenInvitedAwardResult, beenInvitedAwardResult);
            dataRepository.AddContainer(DataKey.InviteAwardResult, inviteAwardResult);

            #endregion

            #region 签到 Checkin >= 6.4.0

            var checkinConfigResult =
                _dcFactory.CreateRootDataContainer<CheckinConfigResult>();

            var checkinConfig =
                _dcFactory.CreateChildDataContainer(
                    new CheckinConfigToCheckinConfigResultRelation(checkinConfigResult));

            var checkinAwardResult = _dcFactory.CreateRootDataContainer<CheckinAwardResult>();

            var checkinResult = _dcFactory.CreateRootDataContainer<CheckinResult>();
            var reCheckinResult = _dcFactory.CreateRootDataContainer<ReCheckinResult>();

            dataRepository.AddContainer(DataKey.CheckinConfigResult, checkinConfigResult);
            dataRepository.AddContainer(DataKey.CheckinConfig,
                checkinConfig);
            dataRepository.AddContainer(DataKey.CheckinAwardResult, checkinAwardResult);
            dataRepository.AddContainer(DataKey.CheckinResult, checkinResult);
            dataRepository.AddContainer(DataKey.ReCheckinResult, reCheckinResult);

            #endregion

            #region 活动 Activity >= 6.4.0

            var activityStatusResult = _dcFactory.CreateRootDataContainer<ActivityStatusResult>();
            var activityAwardResult = _dcFactory.CreateRootDataContainer<ActivityAwardResult>();

            dataRepository.AddContainer(DataKey.ActivityStatusResult, activityStatusResult);
            dataRepository.AddContainer(DataKey.ActivityAwardResult, activityAwardResult);

            #endregion

            #region YuanBao 元宝 >= 6.4.0

            var yuanBaoConfigResult = _dcFactory.CreateRootDataContainer<YuanBaoConfigResult>();
            var exchangeYuanBaoResult = _dcFactory.CreateRootDataContainer<ExchangeYuanBaoResult>();
            var myYuanBaoExchangeResult = _dcFactory.CreateRootDataContainer<MyYuanBaoExchangeResult>();

            dataRepository.AddContainer(DataKey.YuanBaoConfigResult, yuanBaoConfigResult);
            dataRepository.AddContainer(DataKey.ExchangeYuanBaoResult, exchangeYuanBaoResult);
            dataRepository.AddContainer(DataKey.MyYuanBaoExchangeResult, myYuanBaoExchangeResult);

            #endregion

            #region VipExchange VipExchangeData是独立于HallData的，需要每次打开Vip面板的时候请求。

            var vipExchangeListResult = _dcFactory.CreateRootDataContainer<VipExchangeListResult>();
            var requestExchangeVipResult = _dcFactory.CreateRootDataContainer<RequestExchangeVipResult>();

            dataRepository.AddContainer(DataKey.VipExchangeListResult, vipExchangeListResult);
            dataRepository.AddContainer(DataKey.RequestExchangeVipResult, requestExchangeVipResult);

            #endregion

            #region 房间人数

            var bRoomInOut = _dcFactory.CreateRootDataContainer<Queue<BRoomInOut>>();
            dataRepository.AddContainer(DataKey.BRoomInOut, bRoomInOut);

            #endregion

            #region 选房结果数据

            //ChooseRoomResult 根据 HLoginResult的刷新而刷新
            var chooseRoomResult =
                _dcFactory.CreateChildDataContainer(new ChooseRoomResultToHLoginResultRelation(hLoginResult));
            var chooseRoomFail = _dcFactory.CreateRootDataContainer<ChooseRoomFail>();
            var leaveRoomResult = _dcFactory.CreateRootDataContainer<LeaveRoomResult>();

            var gameServerAddress =
                _dcFactory.CreateChildDataContainer(new GameServerAddressToChooseRoomResultRelation(chooseRoomResult));

            dataRepository.AddContainer(DataKey.ChooseRoomResult, chooseRoomResult);
            dataRepository.AddContainer(DataKey.ChooseRoomFail, chooseRoomFail);
            dataRepository.AddContainer(DataKey.LeaveRoomResult, leaveRoomResult);
            dataRepository.AddContainer(DataKey.GameServerAddress, gameServerAddress);

            #endregion

            #region GLoginResult GServer登陆结果

            var gLoginResult = _dcFactory.CreateChildDataContainer(
                new GLoginResultToChooseRoomResultRelation(chooseRoomResult));
            dataRepository.AddContainer(DataKey.GLoginResult, gLoginResult);

            #endregion

            #region 当前房间数据

            var currentRoom =
                _dcFactory.CreateChildDataContainer(new CurrentRoomToGLoginResultRelation(gLoginResult));
            dataRepository.AddContainer(DataKey.CurrentRoom, currentRoom);

            #endregion

            #region ChooseTableResult 当前的桌子

            var chooseTableResult =
                _dcFactory.CreateChildDataContainer(new ChooseTableResultToCurrentRoomRelation(currentRoom));

            //桌子数据更新的时间 chooseTableResultTableContainer.TimeStamp
            var currentTable =
                _dcFactory.CreateChildDataContainer(new TableToChooseTableResultRelation(chooseTableResult));
            //playingData
            var playingData =
                _dcFactory.CreateChildDataContainer(new PlayingDataToTableRelation(currentTable));

            //离桌结果
            var leaveTableResult = _dcFactory.CreateRootDataContainer<LeaveTableResult>();

            // 准备结果。
            var readyResult = _dcFactory.CreateRootDataContainer<ReadyResult>();

            // 桌子上玩家数据变化。
            var tableUserData = _dcFactory.CreateChildDataContainer(new TableUserDataRelation(
                currentTable
                , myUser));

            var hostInfo =
                _dcFactory.CreateChildDataContainer(new HostInfoToTableRelation(currentTable));

            var bTableInOut = _dcFactory.CreateRootDataContainer<BTableInOut>();

            dataRepository.AddContainer(DataKey.ChooseTableResult, chooseTableResult);
            dataRepository.AddContainer(DataKey.CurrentTable, currentTable);
            dataRepository.AddContainer(DataKey.PlayingData,
                playingData);
            dataRepository.AddContainer(DataKey.LeaveTableResult, leaveTableResult);
            dataRepository.AddContainer(DataKey.ReadyResult, readyResult);
            dataRepository.AddContainer(DataKey.BTableInOutContainer, bTableInOut);
            dataRepository.AddContainer(DataKey.HostInfo, hostInfo);
            dataRepository.AddContainer(DataKey.TableUserData, tableUserData);

            #endregion

            #region 开局

            var startRound =
                _dcFactory.CreateChildDataContainer(new StartRoundToPlayingDataRelation(playingData));
            dataRepository.AddContainer(DataKey.StartRound, startRound);

            #endregion

            #region 大结算

            var bRoundEnd =
                _dcFactory.CreateChildDataContainer(new BRoundEndToPlayingDataRelation(playingData));
            dataRepository.AddContainer(DataKey.BRoundEnd, bRoundEnd);

            #endregion

            #region 比赛房结算

            var newRoundEnd =
                _dcFactory.CreateChildDataContainer(new RaceRoundEndToPlayingDataRelation(playingData));
            dataRepository.AddContainer(DataKey.RaceRoundEnd, newRoundEnd);

            #endregion

            #region 选座界面踢人倒计时 KickOutCounter

            var bKickOutCounter = _dcFactory.CreateRootDataContainer<BKickOutCounter>();
            dataRepository.AddContainer(DataKey.BKickOutCounter, bKickOutCounter);

            #endregion

            #region 当前玩家选择的游戏模式。

            var currentGameMode = _dcFactory.CreateRootDataContainer<int>();
            dataRepository.AddContainer(DataKey.CurrentGameMode, currentGameMode);

            #endregion

            #region T通用倒计时 BCounter

            var bCounter = _dcFactory.CreateRootDataContainer<BCounter>();
            dataRepository.AddContainer(DataKey.BCounter, bCounter);

            #endregion

            #region TTZ StartBroadcast

            var ttzStartBroadcast = _dcFactory.CreateRootDataContainer<TTZStartBroadcast>();
            dataRepository.AddContainer(DataKey.TTZStartBroadcast, ttzStartBroadcast);

            #endregion

            #region 进贡还贡

            var beenJinGong =
                _dcFactory.CreateChildDataContainer(new BeenJinGongToPlayingDataRelation(playingData));
            var beenHuanGong =
                _dcFactory.CreateChildDataContainer(new BeenHuanGongToPlayingDataRelation(playingData));
            var bKangGong = _dcFactory.CreateRootDataContainer<BKangGong>();

            dataRepository.AddContainer(DataKey.BKangGong, bKangGong);
            dataRepository.AddContainer(DataKey.BeenJinGong, beenJinGong);
            dataRepository.AddContainer(DataKey.BeenHuanGong, beenHuanGong);

            #endregion

            #region 我出牌

            var chuPaiKey =
                _dcFactory.CreateChildDataContainer(new ChuPaiKeyToPlayingDataRelation(playingData));
            dataRepository.AddContainer(DataKey.ChuPaiKey, chuPaiKey);

            #endregion

            #region PokerPeeperPokerPeeperData

            var pokerPeeperData = _dcFactory.CreateChildDataContainer(
                new PokerPeeperDataToPlayingDataRelation(playingData, tableUserData));
            dataRepository.AddContainer(DataKey.PokerPeeperData, pokerPeeperData);

            #endregion

            #region 记牌器

            var pokerRecorder =
                _dcFactory.CreateChildDataContainer(new PokerRecorderToPlayingDataRelation(playingData));
            dataRepository.AddContainer(DataKey.PokerRecorder, pokerRecorder);

            #endregion

            #region 牌桌互动

            var bInteraction = _dcFactory.CreateRootDataContainer<BInteraction>();
            dataRepository.AddContainer(DataKey.BInteraction, bInteraction);

            #endregion

            #region 小结算

            var bMiddleRoundEnd = _dcFactory.CreateRootDataContainer<BMiddleRoundEnd>();
            dataRepository.AddContainer(DataKey.BMiddleRoundEndContainer, bMiddleRoundEnd);

            #endregion

            #region 好友

            var friendListResult = _dcFactory.CreateRootDataContainer<SFriendListResult>();
            // 请求的好友信息。
            var friendDetailResult = _dcFactory.CreateRootDataContainer<SFriendDetailResult>();
            // 删除好友的结果。
            var removeFriendResultToSender =
                _dcFactory.CreateRootDataContainer<SRemoveFriendResultToSender>();
            // 跟踪好友的结果。
            var traceUserResult = _dcFactory.CreateRootDataContainer<TraceUserResult>();
            // 查找好友的结果。
            var searchUserResult = _dcFactory.CreateRootDataContainer<CUSearchUserResult>();

            var friendRequesterList = _dcFactory.CreateRootDataContainer<List<string>>();
            var friendIgnoreList = _dcFactory.CreateRootDataContainer<List<string>>();

            dataRepository.AddContainer(DataKey.SFriendListResult, friendListResult);
            dataRepository.AddContainer(DataKey.SFriendDetailResult, friendDetailResult);
            dataRepository.AddContainer(DataKey.SRemoveFriendResultToSender,
                removeFriendResultToSender);
            dataRepository.AddContainer(DataKey.TraceUserResult, traceUserResult);
            dataRepository.AddContainer(DataKey.SearchUserResult, searchUserResult);
            dataRepository.AddContainer(DataKey.FriendRequesterList, friendRequesterList);
            dataRepository.AddContainer(DataKey.FriendIgnoreList, friendIgnoreList);

            #endregion

            #region 快捷工具

            var actionPriceResult = _dcFactory.CreateRootDataContainer<ActionPriceResult>();
            var editNicknameResult = _dcFactory.CreateRootDataContainer<EditNicknameResult>();
            var resetWinRateResult = _dcFactory.CreateRootDataContainer<ResetWinRateResult>();
            var changeSexResult = _dcFactory.CreateRootDataContainer<ChangeSexResult>();

            dataRepository.AddContainer(DataKey.ActionPriceResult, actionPriceResult);
            dataRepository.AddContainer(DataKey.EditNicknameResult, editNicknameResult);
            dataRepository.AddContainer(DataKey.ResetWinRateResult, resetWinRateResult);
            dataRepository.AddContainer(DataKey.ChangeSexResult, changeSexResult);

            #endregion

            #region 聊天

            var bTextMsg = _dcFactory.CreateRootDataContainer<BTextMsg>();
            var bJianMeng = _dcFactory.CreateRootDataContainer<BJianMeng>();

            dataRepository.AddContainer(DataKey.BTextMsg, bTextMsg);
            dataRepository.AddContainer(DataKey.BJianMeng, bJianMeng);

            #endregion

            #region 屏蔽聊天

            var shieldChat = _dcFactory.CreateRootDataContainer<bool>();
            dataRepository.AddContainer(DataKey.ShieldChat, shieldChat);

            #endregion

            #region 奖励

            var requestAwardResult = _dcFactory.CreateRootDataContainer<RequestAwardResult>();
            dataRepository.AddContainer(DataKey.RequestAwardResult, requestAwardResult);

            #endregion

            #region 支付

            var tradeNoResult = _dcFactory.CreateRootDataContainer<TradeNoResult>();
            var checkTradeResult = _dcFactory.CreateRootDataContainer<CheckTradeResult>();

            dataRepository.AddContainer(DataKey.TradeNoResult, tradeNoResult);
            dataRepository.AddContainer(DataKey.CheckTradeResult, checkTradeResult);

            #endregion

            #region 微信绑定

            var wechatBindResult = _dcFactory.CreateRootDataContainer<WechatBindResult>();

            dataRepository.AddContainer(DataKey.WechatBindResult, wechatBindResult);

            #endregion

            #region 举报玩家

            var reportResult = _dcFactory.CreateRootDataContainer<ReportResult>();

            dataRepository.AddContainer(DataKey.ReportResult, reportResult);

            #endregion

            #region 反馈

            //反馈结果
            var commitIssueResult = _dcFactory.CreateRootDataContainer<CommitIssueResult>();
            //反馈历史
            var historyRecordResult = _dcFactory.CreateRootDataContainer<HistoryRecordResult>();

            dataRepository.AddContainer(DataKey.CommitIssueResult, commitIssueResult);
            dataRepository.AddContainer(DataKey.HistoryRecordResult, historyRecordResult);

            #endregion

            #region 比赛

            //比赛列表
            var raceConfigList = _dcFactory.CreateRootDataContainer<RaceConfigList>();
            //单个比赛 通过单个比赛更新比赛列表
            var raceConfig = _dcFactory.CreateRootDataContainer<RaceConfig>();

            dataRepository.AddContainer(DataKey.RaceConfigList, raceConfigList);
            dataRepository.AddContainer(DataKey.RaceConfig, raceConfig);

            #endregion

            #region 报名比赛

            var applyRaceResult = _dcFactory.CreateRootDataContainer<ApplyRaceResult>();

            dataRepository.AddContainer(DataKey.ApplyRaceResult, applyRaceResult);

            #endregion

            #region 比赛介绍

            var raceDescriptionResult = _dcFactory.CreateRootDataContainer<RaceDescriptionResult>();

            dataRepository.AddContainer(DataKey.RaceDescriptionResult, raceDescriptionResult);

            #endregion

            #region 比赛历史排名

            var historyRaceRankResult = _dcFactory.CreateRootDataContainer<HistoryRaceRankResult>();

            dataRepository.AddContainer(DataKey.HistoryRaceRankResult, historyRaceRankResult);

            #endregion

            #region List<RaceData>

            var raceDataList = _dcFactory.CreateRootDataContainer<List<RaceData>>();

            dataRepository.AddContainer(DataKey.RaceDataList, raceDataList);

            #endregion

            #region BFanbei

            var bFanbeiContainer = _dcFactory.CreateRootDataContainer<BFanbei>();

            dataRepository.AddContainer(DataKey.BFanbei, bFanbeiContainer);

            #endregion

            #region 比赛奖励

            var raceAwardContainer = _dcFactory.CreateRootDataContainer<Queue<RaceAward>>();

            dataRepository.AddContainer(DataKey.RaceAwardQueue, raceAwardContainer);

            #endregion

            var appStateContainer = _dcFactory.CreateChildDataContainer(
                new AppStateRelation(
                    hLoginResult,
                    currentRoom,
                    currentTable,
                    currentGameMode,
                    playingData));

            dataRepository.AddContainer(DataKey.AppState, appStateContainer);
        }

        private void AddPServerContainer(IDataRepository dataRepository)
        {
            #region PSever 注册

            var pRegisterResult = _dcFactory.CreateRootDataContainer<PRegisterResult>();

            dataRepository.AddContainer(DataKey.PRegisterResult, pRegisterResult);

            #endregion

            #region PServer登陆的结果。

            var pLoginResult = _dcFactory.CreateRootDataContainer<PLoginResult>();

            var hServerAddress =
                _dcFactory.CreateChildDataContainer(new HServerAddressToPLoginResultRelation(pLoginResult));

            dataRepository.AddContainer(DataKey.PLoginResult, pLoginResult);
            dataRepository.AddContainer(DataKey.HServerAddress, hServerAddress);

            #endregion

            #region 公告栏是否已读。

            var billboardRead = _dcFactory.CreateRootDataContainer<bool>();

            dataRepository.AddContainer(DataKey.BillboardRead, billboardRead);

            #endregion

            #region 持有从PServer收到的VersionResult数据。

            var versionResult = _dcFactory.CreateRootDataContainer<VersionResult>();
            dataRepository.AddContainer(DataKey.VersionResult, versionResult);

            var launcherPic =
                _dcFactory.CreateChildDataContainer(new LauncherPicToVersionResultRelation(versionResult));
            dataRepository.AddContainer(DataKey.LauncherPic, launcherPic);

            // 聊天文字预设。
            var textChatPresets =
                _dcFactory.CreateChildDataContainer(new TextChatPresetsToVersionResultRelation(versionResult));

            var gameTipList =
                _dcFactory.CreateChildDataContainer(new GameTipListToVersionResultRelation(versionResult));

            var treasureChestTipContent =
                _dcFactory.CreateChildDataContainer(
                    new TreasureChestTipContentToVersionResultRelation(versionResult));

            var hintPicUrls =
                _dcFactory.CreateChildDataContainer(new HintPicUrlsToVersionResultRelation(versionResult));

            var picUrls62 =
                _dcFactory.CreateChildDataContainer(new PicUrls62ToVersionResultRelation(versionResult));

            var serviceQQ =
                _dcFactory.CreateChildDataContainer(new ServiceQQToVersionResultRelation(versionResult));

            var serviceQQGroup =
                _dcFactory.CreateChildDataContainer(new ServiceQQGroupToVersionResultRelation(versionResult));

            var serviceContent =
                _dcFactory.CreateChildDataContainer(new ServiceContentToVersionResultRelation(versionResult));

            var aboutContent =
                _dcFactory.CreateChildDataContainer(new AboutContentToVersionResultRelation(versionResult));

            var inGameConfig =
                _dcFactory.CreateChildDataContainer(new InGameConfigToVersionResultRelation(versionResult));

            dataRepository.AddContainer(DataKey.TextChatPresets, textChatPresets);
            dataRepository.AddContainer(DataKey.GameTipListCount, gameTipList);
            dataRepository.AddContainer(DataKey.TreasureChestTipContent, treasureChestTipContent);
            dataRepository.AddContainer(DataKey.HintPicUrls, hintPicUrls);
            dataRepository.AddContainer(DataKey.PicUrls62, picUrls62);
            dataRepository.AddContainer(DataKey.ServiceQQ, serviceQQ);
            dataRepository.AddContainer(DataKey.ServiceQQGroup, serviceQQGroup);
            dataRepository.AddContainer(DataKey.ServiceContent, serviceContent);
            dataRepository.AddContainer(DataKey.AboutContent, aboutContent);
            dataRepository.AddContainer(DataKey.InGameConfig, inGameConfig);

            #endregion

            #region 微信登录

            var wechatAuthResult = _dcFactory.CreateRootDataContainer<WechatAuthResult>();
            var wechatLoginResult = _dcFactory.CreateRootDataContainer<WechatLoginResult>();

            dataRepository.AddContainer(DataKey.WechatAuthResult, wechatAuthResult);
            dataRepository.AddContainer(DataKey.WechatLoginResult, wechatLoginResult);

            #endregion

            #region 每日任务

            var userTaskListResult = _dcFactory.CreateRootDataContainer<UserTaskListResult>();

            var userTaskTip = _dcFactory.CreateRootDataContainer<UserTaskTip>();

            var getUserTaskAwardRResult = _dcFactory.CreateRootDataContainer<GetUserTaskAwardResult>();

            var notifyDoShareResult = _dcFactory.CreateRootDataContainer<NotifyDoShareResult>();

            dataRepository.AddContainer(DataKey.UserTaskListResult, userTaskListResult);
            dataRepository.AddContainer(DataKey.UserTaskTip, userTaskTip);
            dataRepository.AddContainer(DataKey.GetUserTaskAwardResult, getUserTaskAwardRResult);
            dataRepository.AddContainer(DataKey.NotifyDoShareResult, notifyDoShareResult);

            #endregion

            #region RealNameResult

            var realNameResult = _dcFactory.CreateRootDataContainer<RealNameResult>();

            dataRepository.AddContainer(DataKey.RealNameResult, realNameResult);

            #endregion

            #region Build AssetBundle Cache

            var buildFirstCacheResult = _dcFactory.CreateRootDataContainer<BuildFirstCacheResult>();

            dataRepository.AddContainer(DataKey.BuildFirstCacheResult, buildFirstCacheResult);

            var downloadResourceResult = _dcFactory.CreateRootDataContainer<DownloadResourceResult>();

            dataRepository.AddContainer(DataKey.DownloadResourceResult, downloadResourceResult);

            #endregion

            #region DownloadAssetBundleInfo

            var downloadAssetBundlwInfo = _dcFactory.CreateRootDataContainer<DownloadAssetBundleInfo>();

            dataRepository.AddContainer(DataKey.DownloadAssetBundleInfo, downloadAssetBundlwInfo);

            #endregion

            #region TestLogin

            var isSetTestPServer = _dcFactory.CreateRootDataContainer<bool>();

            dataRepository.AddContainer(DataKey.IsSetTestPServer, isSetTestPServer);

            var isSetTestClientVersion = _dcFactory.CreateRootDataContainer<bool>();

            dataRepository.AddContainer(DataKey.IsSetTestClientVersion, isSetTestClientVersion);

            #endregion
        }
    }
}