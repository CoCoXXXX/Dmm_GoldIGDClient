using System;
using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Session;

namespace Dmm.Msg
{
    /// <summary>
    /// 消息工具。
    /// </summary>
    public class CmdUtil
    {
        public class PU
        {
            #region ClientVersion

            private static readonly Type ClientVersionType = typeof(ClientVersion);

            /// <summary>
            /// ClientVersion命令。
            /// </summary>
            /// <param name="version"></param>
            /// <param name="saleChannel"></param>
            /// <param name="product"></param>
            /// <param name="platform"></param>
            /// <param name="networkType"></param>
            /// <param name="model"></param>
            /// <param name="deviceId"></param>
            /// <returns></returns>
            public static ProtoMessage ClientVersion(
                int version,
                string saleChannel,
                string product,
                int platform,
                int networkType,
                string model,
                string deviceId)
            {
                var res = new ClientVersion();
                res.version = version;
                res.sale_channel = saleChannel;
                res.product = product;
                res.platform = platform;
                res.network_type = networkType;
                res.model_string = model;
                res.device_id = deviceId;

                var msg = new ProtoMessage();
                msg.Type = CmdType.PU.CLIENT_VERSION;
                msg.Content = res;
                msg.Model = ClientVersionType;
                return msg;
            }

            #endregion

            #region ClientVersionResult

            private static readonly Type VersionResultType = typeof(VersionResult);

            /// <summary>
            /// ClientVersion的结果。
            /// </summary>
            /// <returns></returns>
            public static ProtoMessage ClientVersionResult(
                int result,
                int lowestVersion,
                int latestVersion,
                Billboard billboard,
                ReleaseConfig releaseConfig,
                List<AdsItem> adsItemList,
                PicUrls picUrls,
                TreasureChestInfo treasureChestInfo,
                TextChatPresets textChatPresets,
                InGameConfig inGameConfig,
                List<AwardConfig> awardConfigList,
                List<HintItem> hintItemList,
                HintPicUrls hintPicUrls)
            {
                var res = new VersionResult();
                res.result = result;
                res.lowest_version = lowestVersion;
                res.latest_version = latestVersion;
                res.billboard = billboard;
                res.release_config = releaseConfig;

                if (adsItemList != null)
                    res.ads_item.AddRange(adsItemList);

                res.pic_urls = picUrls;
                res.treasure_chest_info = treasureChestInfo;
                res.text_chat_presets = textChatPresets;
                res.in_game_config = inGameConfig;

                if (awardConfigList != null)
                    res.award_config.AddRange(awardConfigList);

                if (hintItemList != null)
                    res.hint_item.AddRange(hintItemList);

                res.hint_pic_urls = hintPicUrls;

                var msg = new ProtoMessage();
                msg.Type = CmdType.PU.CLIENT_VERSION_RESULT;
                msg.Content = res;
                msg.Model = VersionResultType;
                return msg;
            }

            /// <summary>
            /// 公告牌。
            /// </summary>
            /// <param name="content"></param>
            /// <param name="timestamp"></param>
            /// <param name="validity"></param>
            /// <returns></returns>
            public static Billboard Billboard(string content, long timestamp, long validity)
            {
                var billboard = new Billboard();
                billboard.content = content;
                billboard.timestamp = timestamp;
                billboard.validity = validity;
                return billboard;
            }

            /// <summary>
            /// 版本控制数据。
            /// </summary>
            /// <param name="enableCharge"></param>
            /// <param name="enableAlipay"></param>
            /// <param name="enableXianHua"></param>
            /// <param name="enableVip"></param>
            /// <param name="enableRating"></param>
            /// <returns></returns>
            public static ReleaseConfig ReleaseConfig(
                bool enableCharge,
                bool enableAlipay,
                bool enableXianHua,
                bool enableVip,
                bool enableRating)
            {
                var config = new ReleaseConfig();
                config.enable_charge = enableCharge;
                config.enable_alipay = enableAlipay;
                config.enable_yuanbao = enableXianHua;
                config.enable_vip = enableVip;
                config.enable_rating = enableRating;
                return config;
            }

            /// <summary>
            /// 广告项目。
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="picName"></param>
            /// <param name="url"></param>
            /// <returns></returns>
            public static AdsItem AdsItem(int pos, string picName, string url)
            {
                var item = new AdsItem();
                item.pos = pos;
                item.pic_name = picName;
                item.url = url;
                return item;
            }

            /// <summary>
            /// 延迟下载图片列表。
            /// </summary>
            /// <param name="urls"></param>
            /// <returns></returns>
            public static PicUrls PicUrls(List<string> urls)
            {
                var res = new PicUrls();
                if (urls != null)
                    res.url.AddRange(urls);

                return res;
            }

            /// <summary>
            /// 宝箱信息。
            /// </summary>
            /// <param name="content"></param>
            /// <returns></returns>
            public static TreasureChestInfo TreasureChestInfo(string content)
            {
                var res = new TreasureChestInfo();
                res.content = content;
                return res;
            }

            /// <summary>
            /// 聊天预设。
            /// </summary>
            /// <param name="chatMsgs"></param>
            /// <returns></returns>
            public static TextChatPresets TextChatPresets(List<string> chatMsgs)
            {
                var res = new TextChatPresets();
                if (chatMsgs != null)
                    res.chat_msg.AddRange(chatMsgs);

                return res;
            }

            /// <summary>
            /// 游戏配置。
            /// </summary>
            /// <param name="bgPic"></param>
            /// <param name="racePic"></param>
            /// <param name="ttzPic"></param>
            /// <param name="vipUrl"></param>
            /// <param name="taobaoUrl"></param>
            /// <param name="xianHuaUrl"></param>
            /// <param name="commoditySaleCutOff"></param>
            /// <param name="aliPid"></param>
            /// <param name="aliSid"></param>
            /// <param name="aliPkey"></param>
            /// <param name="serviceQq"></param>
            /// <param name="serviceQqGroup"></param>
            /// <param name="wxInviteUrl"></param>
            /// <param name="wxInviteTitle"></param>
            /// <param name="wxInviteDescription"></param>
            /// <param name="wxShareTitle"></param>
            /// <param name="wxShareDescription"></param>
            /// <returns></returns>
            public static InGameConfig InGameConfig(
                string bgPic,
                string racePic,
                string ttzPic,
                string vipUrl,
                string taobaoUrl,
                string xianHuaUrl,
                int commoditySaleCutOff,
                string aliPid,
                string aliSid,
                string aliPkey,
                string serviceQq,
                string serviceQqGroup,
                string wxInviteUrl,
                string wxInviteTitle,
                string wxInviteDescription,
                string wxShareTitle,
                string wxShareDescription)
            {
                var config = new InGameConfig();
                config.bg_pic = bgPic;
//                config.race_pic = racePic;
//                config.ttz_pic = ttzPic;
//                config.vip_url = vipUrl;
//                config.taobao_url = taobaoUrl;
//                config.yuanbao_url = xianHuaUrl;
                config.commodity_sale_cutoff = commoditySaleCutOff;

                config.ali_pid = aliPid;
                config.ali_sid = aliSid;
                config.ali_pkey = aliPkey;

                config.service_qq = serviceQq;
                config.service_qq_group = serviceQqGroup;

                config.wx_invite_url = wxInviteUrl;
                config.wx_invite_title = wxInviteTitle;
                config.wx_invite_description = wxInviteDescription;

                config.wx_share_title = wxShareTitle;
                config.wx_share_description = wxShareDescription;

                return config;
            }

            public static AwardConfig AwardConfig(string code, Award award)
            {
                var res = new AwardConfig();
                res.code = code;
                res.award = award;
                return res;
            }

            public static HintItem HintItem(
                int pos,
                string outerPic,
                string contentPic,
                int size,
                int type,
                string url,
                Prepayment prepayment,
                Commodity commodity)
            {
                var res = new HintItem();
                res.pos = pos;
                res.outer_pic = outerPic;
                res.content_pic = contentPic;
                res.size = size;
                res.type = type;
                res.url = url;
                res.prepayment = prepayment;
                res.commodity = commodity;

                return res;
            }

            public static HintPicUrls HintPicUrls(List<string> urls)
            {
                var res = new HintPicUrls();
                if (urls != null)
                    res.url.AddRange(urls);

                return res;
            }

            #endregion

            private static readonly Type LoginType = typeof(PLogin);

            /// <summary>
            /// 登陆PServer。
            /// </summary>
            /// <param name="username"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public static ProtoMessage Login(string username, string password)
            {
                var res = new PLogin();
                res.username = username;
                res.password = password;

                var msg = new ProtoMessage();
                msg.Type = CmdType.PU.LOGIN;
                msg.Content = res;
                msg.Model = LoginType;
                return msg;
            }

            private static readonly Type RegisterType = typeof(PRegister);

            public static ProtoMessage Register(string username, string password, string nickname, int sex)
            {
                var res = new PRegister();
                res.username = username;
                res.nickname = nickname;
                res.password = password;
                res.sex = sex;

                var msg = new ProtoMessage();
                msg.Type = CmdType.PU.REGISTER;
                msg.Content = res;
                msg.Model = RegisterType;
                return msg;
            }

            private static readonly Type LoginResultType = typeof(PLoginResult);

            public static ProtoMessage LoginResult(
                int result,
                string username,
                string token,
                string hallServerAddr)
            {
                var res = new PLoginResult();
                res.result = result;
                res.username = username;
                res.token = token;
                res.hall_server_addr = hallServerAddr;

                var msg = new ProtoMessage();
                msg.Type = CmdType.PU.LOGIN_RESULT;
                msg.Content = res;
                msg.Model = LoginResultType;
                return msg;
            }

            private static readonly Type VisitorLoginType = typeof(VisitorLogin);

            public static ProtoMessage VisitorLogin(string nickname, string deviceId, string username)
            {
                var res = new VisitorLogin();
                res.nickname = nickname;
                res.device_id = deviceId;
                res.username = username;

                var msg = new ProtoMessage();
                msg.Type = CmdType.PU.VISITOR_LOGIN;
                msg.Content = res;
                msg.Model = VisitorLoginType;
                return msg;
            }

            private static readonly Type VisitorLoginResultType = typeof(VisitorLoginResult);

            public static ProtoMessage VisitorLoginResult(
                int result,
                string username,
                string token,
                string hallServerAddr)
            {
                var res = new VisitorLoginResult();
                res.result = result;
                res.username = username;
                res.token = token;
                res.hall_server_addr = hallServerAddr;

                var msg = new ProtoMessage();
                msg.Type = CmdType.PU.VISITOR_LOGIN_RESULT;
                msg.Content = res;
                msg.Model = VisitorLoginResultType;
                return msg;
            }

            private static readonly Type WechatLoginType = typeof(WechatLogin);

            public static ProtoMessage WechatLogin(string openId)
            {
                var res = new WechatLogin();
                res.open_id = openId;

                var msg = new ProtoMessage();
                msg.Type = CmdType.PU.WECHAT_LOGIN;
                msg.Content = res;
                msg.Model = WechatLoginType;
                return msg;
            }

            private static readonly Type WechatAuthType = typeof(WechatAuth);

            public static ProtoMessage WechatAuth(string authCode)
            {
                var res = new WechatAuth();
                res.auth_code = authCode;

                var msg = new ProtoMessage();
                msg.Type = CmdType.PU.WECHAT_AUTH;
                msg.Content = res;
                msg.Model = WechatAuthType;
                return msg;
            }

            private static readonly Type WechatBindType = typeof(WechatBind);

            public static ProtoMessage WechatBind(string authCode)
            {
                var res = new WechatBind();
                res.auth_code = authCode;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.WECHAT_BIND;
                msg.Content = res;
                msg.Model = WechatBindType;
                return msg;
            }
        }

        /// <summary>
        /// 与HServer交互的命令。
        /// </summary>
        public class HU
        {
            private static readonly Type LoginType = typeof(HLogin);

            /// <summary>
            /// 登陆HServer。
            /// </summary>
            /// <param name="username"></param>
            /// <param name="token"></param>
            /// <returns></returns>
            public static ProtoMessage Login(string username, string token)
            {
                var res = new HLogin();
                res.username = username;
                res.token = token;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.LOGIN;
                msg.Content = res;
                msg.Model = LoginType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type LoginResultType = typeof(HLoginResult);

            public static ProtoMessage LoginResult(
                int result,
                User user,
                HallData hallData,
                RoomData roomData)
            {
                var res = new HLoginResult();
                res.result = result;
                res.user = user;
                res.hall_data = hallData;
                res.room_data = roomData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.LOGIN_RESULT;
                msg.Content = res;
                msg.Model = LoginResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type RequestUserInfoType = typeof(RequestUserInfo);

            public static ProtoMessage RequestUserInfo(string username, byte[] attr)
            {
                var res = new RequestUserInfo();
                res.username = username;
                res.attr = attr;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_USER_INFO;
                msg.Content = res;
                msg.Model = RequestUserInfoType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type UserInfoResultType = typeof(UserInfoResult);

            public static ProtoMessage UserInfoResult(int result, User user)
            {
                var res = new UserInfoResult();
                res.result = result;
                res.user = user;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.USER_INFO_RESULT;
                msg.Content = res;
                msg.Model = UserInfoResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            public static ProtoMessage RequestCheckinConfig()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_CHECKIN_CONFIG;
                return msg;
            }

            private static readonly Type EditUserInfoType = typeof(EditUserInfo);

            /// <summary>
            /// 编辑玩家的信息。
            /// </summary>
            /// <param name="newNickname"></param>
            /// <param name="newSex">如果==-1，则不修改性别。</param>
            /// <param name="email"></param>
            /// <param name="description"></param>
            /// <param name="city"></param>
            /// <returns></returns>
            public static ProtoMessage EditUserInfo(
                string newNickname,
                int newSex,
                string email,
                string description,
                string city)
            {
                var res = new EditUserInfo();
                res.new_nickname = newNickname;
                if (newSex != -1) res.new_sex = newSex;
                res.email = email;
                res.description = description;
                res.city = city;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.EDIT_USER_INFO;
                msg.Content = res;
                msg.Model = EditUserInfoType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type EditUserInfoResultType = typeof(EditUserInfoResult);

            public static ProtoMessage EditUserInfoResult(int result)
            {
                var res = new EditUserInfoResult();
                res.result = result;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.EDIT_USER_INFO_RESULT;
                msg.Content = res;
                msg.Model = EditUserInfoResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type ChooseNicknameType = typeof(ChooseNickname);

            public static ProtoMessage ChooseNickname(string nickname, int sex)
            {
                var res = new ChooseNickname();
                res.nickname = nickname;
                res.new_sex = sex;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.CHOOSE_NICKNAME;
                msg.Content = res;
                msg.Model = ChooseNicknameType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type ChooseNicknameResultType = typeof(ChooseNicknameResult);

            public static ProtoMessage ChooseNicknameResult(int result)
            {
                var res = new ChooseNicknameResult();
                res.result = result;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.CHOOSE_NICKNAME_RESULT;
                msg.Content = res;
                msg.Model = ChooseNicknameResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type EditPasswordType = typeof(EditPassword);

            public static ProtoMessage EditPassword(string password, string newPassword)
            {
                var res = new EditPassword();
                res.password = password;
                res.new_password = newPassword;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.EDIT_PASSWORD;
                msg.Content = res;
                msg.Model = EditPasswordType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type EditPasswordResultType = typeof(EditPasswordResult);

            public static ProtoMessage EditPasswordResult(int result)
            {
                var res = new EditPasswordResult();
                res.result = result;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.EDIT_PASSWORD_RESULT;
                msg.Content = res;
                msg.Model = EditPasswordResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            public static ProtoMessage RequestShowRooms()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_SHOW_ROOMS;
                return msg;
            }

            private static readonly Type ShowRoomsResultType = typeof(ShowRoomsResult);

            public static ProtoMessage ShowRoomsResult(int result, List<Room> rooms)
            {
                var res = new ShowRoomsResult();
                res.result = result;
                if (rooms != null)
                    res.room.AddRange(rooms);

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.SHOW_ROOMS_RESULT;
                msg.Content = res;
                msg.Model = ShowRoomsResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            public static ProtoMessage RequestChatServerAddr()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_CHAT_SERVER_ADDR;
                return msg;
            }

            private static readonly Type ChatServerAddrType = typeof(ChatServerAddr);

            public static ProtoMessage ChatServerAddr(string address)
            {
                var res = new ChatServerAddr();
                res.addr = address;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.CHAT_SERVER_ADDR;
                msg.Content = res;
                msg.Model = ChatServerAddrType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type ChooseRoomType = typeof(ChooseRoom);

            public static ProtoMessage ChooseRoom(int roomId, int ping)
            {
                var res = new ChooseRoom();
                res.room_id = roomId;
                res.ping = ping;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.CHOOSE_ROOM;
                msg.Content = res;
                msg.Model = ChooseRoomType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type ChooseRoomResultType = typeof(ChooseRoomResult);

            public static ProtoMessage ChooseRoomResult(
                int result,
                string gameServerAddr,
                long roomId,
                string title,
                string description,
                Currency needPrice,
                List<Field> needField)
            {
                var res = new ChooseRoomResult();
                res.result = result;
                res.game_server_addr = gameServerAddr;
                res.room_id = roomId;
                res.title = title;
                res.description = description;
                res.need_price = needPrice;
                if (needField != null)
                    res.need_field.AddRange(needField);

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.CHOOSE_ROOM_RESULT;
                msg.Content = res;
                msg.Model = ChooseRoomResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type ChooseRoomFailType = typeof(ChooseRoomFail);

            public static ProtoMessage ChooseRoomFail(
                long roomId,
                int code,
                string description,
                Prepayment prepayment,
                List<Field> needFields)
            {
                var res = new ChooseRoomFail();
                res.room_id = roomId;
                res.code = code;
                res.description = description;
                res.prepayment = prepayment;
                if (needFields != null)
                    res.need_field.AddRange(needFields);

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.CHOOSE_ROOM_FAIL;
                msg.Content = res;
                msg.Model = ChooseRoomFailType;
                msg.Server = Server.HServer;

                return msg;
            }

            private static readonly Type RequestChooseRoomType = typeof(RequestChooseRoom);

            public static ProtoMessage RequestChooseRoom(int roomId, List<Field> values)
            {
                var res = new RequestChooseRoom();
                res.room_id = roomId;
                if (values != null)
                    res.value.AddRange(values);

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_CHOOSE_ROOM;
                msg.Content = res;
                msg.Model = RequestChooseRoomType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type LeaveRoomType = typeof(LeaveRoom);

            public static ProtoMessage LeaveRoom(bool confirmLeave)
            {
                var res = new LeaveRoom();
                res.confirm_leave = confirmLeave;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.LEAVE_ROOM;
                msg.Content = res;
                msg.Model = LeaveRoomType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type LeaveRoomResultType = typeof(LeaveRoomResult);

            public static ProtoMessage LeaveRoomResult(
                int result,
                List<Room> rooms)
            {
                var res = new LeaveRoomResult();
                res.result = result;
                if (rooms != null)
                    res.room.AddRange(rooms);

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.LEAVE_ROOM_RESULT;
                msg.Content = res;
                msg.Model = LeaveRoomResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type BRoomInOutType = typeof(BRoomInOut);

            public static ProtoMessage BRoomInOut(
                int inOrOut,
                long roomId,
                int roomPlayerCount,
                string nickname)
            {
                var res = new BRoomInOut();
                res.in_or_out = inOrOut;
                res.room_id = roomId;
                res.room_player_count = roomPlayerCount;
                res.nickname = nickname;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.ROOM_IN_OUT;
                msg.Content = res;
                msg.Model = BRoomInOutType;
                msg.Server = Server.HServer;
                return msg;
            }

            public static ProtoMessage LeaveHall()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.LEAVE_HALL;
                return msg;
            }

            private static readonly Type LeaveHallResultType = typeof(LeaveHallResult);

            public static ProtoMessage LeaveHallResult(int result)
            {
                var res = new LeaveHallResult();
                res.result = result;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.LEAVE_HALL;
                msg.Content = res;
                msg.Model = LeaveHallResultType;
                return msg;
            }

            public static ProtoMessage BeanReplaced()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.BEEN_REPLACED;
                return msg;
            }

            public static ProtoMessage RequestHallData()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_HALL_DATA;
                return msg;
            }

            private static readonly Type HallDataType = typeof(HallData);

            public static HallData HallData(
                List<Room> rooms,
                List<Commodity> commodities,
                string chatServerAddr,
                Bag bag,
                List<Level> levels,
                List<Prepayment> prepayments,
                List<PayChannel> payChannels,
                List<VipConfig> vipConfigs,
                List<Exchange> exchanges)
            {
                var res = new HallData();
                if (rooms != null)
                    res.room.AddRange(rooms);

                if (commodities != null)
                    res.commodity.AddRange(commodities);

                res.chat_server_addr = chatServerAddr;
                res.bag = bag;
                // 这样在进入单机的时候，就不会显示兑换奖励的按钮。
                res.been_invited = true;

                if (levels != null)
                    res.level.AddRange(levels);

                if (prepayments != null)
                    res.prepayment.AddRange(prepayments);

                if (payChannels != null)
                    res.pay_channel.AddRange(payChannels);

                if (vipConfigs != null)
                    res.vip_config.AddRange(vipConfigs);

                if (exchanges != null)
                    res.exchange.AddRange(exchanges);

                return res;
            }

            public static ProtoMessage RequestRoomData()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_ROOM_DATA;
                return msg;
            }

            private static readonly Type RoomDataType = typeof(RoomData);

            public static ProtoMessage RoomData(int roomId, string gameServerAddr)
            {
                var res = new RoomData();
                res.room_id = roomId;
                res.game_server_addr = gameServerAddr;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.ROOM_DATA;
                msg.Content = res;
                msg.Model = RoomDataType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type SubmitPrivacyType = typeof(SubmitPrivacy);

            public static ProtoMessage SubmitPrivacy(UserPrivacy privacy)
            {
                var res = new SubmitPrivacy();
                res.privacy_data = privacy;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.SUBMIT_PRIVACY;
                msg.Content = res;
                msg.Model = SubmitPrivacyType;
                return msg;
            }

            private static readonly Type ChangePrivacyResultType = typeof(ChangePrivacyResult);

            public static ProtoMessage ChangePrivacyResult(int result, string errMsg)
            {
                var res = new ChangePrivacyResult();
                res.result = result;
                res.msg = errMsg;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.CHANGE_PRIVACY_RESULT;
                msg.Content = res;
                msg.Model = ChangePrivacyResultType;
                return msg;
            }

            public static ProtoMessage GetPrivacy()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.GET_PRIVACY;
                return msg;
            }

            private static readonly Type PrivacyResultType = typeof(PrivacyResult);

            public static ProtoMessage PrivacyResult(int result, UserPrivacy privacy)
            {
                var res = new PrivacyResult();
                res.result = result;
                res.privacy_data = privacy;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.PRIVACY_RESULT;
                msg.Content = res;
                msg.Model = PrivacyResultType;
                return msg;
            }

            private static readonly Type RequestTradeNoType = typeof(RequestTradeNo);

            public static ProtoMessage RequestTradeNo(
                string pname,
                int count,
                string receiver,
                int payChannel,
                int clientVersion)
            {
                var res = new RequestTradeNo();
                res.pname = pname;
                res.count = count;
                res.receiver = receiver;
                res.pay_channel = payChannel;
                res.client_version = clientVersion;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_TRADE_NO;
                msg.Content = res;
                msg.Model = RequestTradeNoType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type TradeNoResultType = typeof(TradeNoResult);

            public static ProtoMessage TradeNoResult(
                string pname,
                int count,
                int currencyType,
                long currencyCount,
                Trade trade,
                int result,
                string url,
                string extra)
            {
                var res = new TradeNoResult();
                res.pname = pname;
                res.count = count;
                res.currency_type = currencyType;
                res.currency_count = currencyCount;
                res.trade = trade;
                res.result = result;
                res.url = url;
                res.extra = extra;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.TRADE_NO_RESULT;
                msg.Content = res;
                msg.Model = TradeNoResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type CheckTradeType = typeof(CheckTrade);

            public static ProtoMessage CheckTrade(
                string tradeNo,
                string outerTradeNo,
                string extra)
            {
                var res = new CheckTrade();
                res.trade_no = tradeNo ?? "";
                res.outer_trade_no = outerTradeNo ?? "";
                res.extra = extra;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.CHECK_TRADE;
                msg.Content = res;
                msg.Model = CheckTradeType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type CheckTradeResultType = typeof(CheckTradeResult);

            public static ProtoMessage CheckTradeResult(
                int result,
                Trade trade,
                int currencyType,
                long addedCurrencyCount,
                int currentCurrencyCount,
                Gift gift)
            {
                var res = new CheckTradeResult();
                res.result = result;
                res.trade = trade;
                res.currency_type = currencyType;
                res.added_currency_count = addedCurrencyCount;
                res.current_currency_count = currentCurrencyCount;
                res.gift = gift;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.CHECK_TRADE_RESULT;
                msg.Content = res;
                msg.Model = CheckTradeResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type SubmitWeiboInfoType = typeof(SubmitWeiboInfo);

            public static ProtoMessage SubmitWeiboInfo(
                string uid,
                string token,
                int validity,
                string refreshToken,
                string weiboUsername,
                string weiboPassword,
                int weiboAuthType)
            {
                var res = new SubmitWeiboInfo();
                res.uid = uid;
                res.token = token;
                res.validity = validity;
                res.refresh_token = refreshToken;
                res.weibo_username = weiboUsername;
                res.weibo_password = weiboPassword;
                res.weibo_auth_type = weiboAuthType;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.SUBMIT_WEIBO_INFO;
                msg.Content = res;
                msg.Model = SubmitWeiboInfoType;
                return msg;
            }

            private static readonly Type SubmitWeiboResultType = typeof(SubmitWeiboResult);

            public static ProtoMessage SubmitWeiboResult(int result)
            {
                var res = new SubmitWeiboResult();
                res.result = result;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.SUBMIT_WEIBO_RESULT;
                msg.Content = res;
                msg.Model = SubmitWeiboResultType;
                return msg;
            }

            private static readonly Type RequestRewardType = typeof(RequestReward);

            public static ProtoMessage RequestReward(int money, int exp)
            {
                var res = new RequestReward();
                res.money = money;
                res.exp = exp;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_REWARD;
                msg.Content = res;
                msg.Model = RequestRewardType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type RewardResultType = typeof(RewardResult);

            public static ProtoMessage RewardResult(int result, Gift gift)
            {
                var res = new RewardResult();
                res.result = result;
                res.gift = gift;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REWARD_RESULT;
                msg.Content = res;
                msg.Model = RewardResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type RequestExchangeType = typeof(RequestExchange);

            public static ProtoMessage RequestExchange(
                string exName,
                long count,
                string receiver)
            {
                var res = new RequestExchange();
                res.ex_name = exName;
                res.count = count;
                res.receiver = receiver;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_EXCHANGE;
                msg.Content = res;
                msg.Model = RequestExchangeType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type ExchangeResultType = typeof(ExchangeResult);

            public static ProtoMessage ExchangeResult(
                int result,
                string exName,
                int sourceType,
                long sourceAmount,
                int targetType,
                long targetAmount,
                User newUserInfo,
                Gift gift)
            {
                var res = new ExchangeResult();
                res.result = result;
                res.ex_name = exName;
                res.source_type = sourceType;
                res.source_amount = sourceAmount;
                res.target_type = targetType;
                res.target_amount = targetAmount;
                res.new_user_info = newUserInfo;
                res.gift = gift;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.EXCHANGE_RESULT;
                msg.Content = res;
                msg.Model = ExchangeResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type VisitorRegularizeType = typeof(VisitorRegularize);

            public static ProtoMessage VisitorRegularize(
                string username,
                string nickname,
                string password,
                int sex)
            {
                var res = new VisitorRegularize();
                res.username = username;
                res.nickname = nickname;
                res.password = password;
                res.sex = sex;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.VISITOR_REGULARIZE;
                msg.Content = res;
                msg.Model = VisitorRegularizeType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type VisitorRegularizeResultType = typeof(VisitorRegularizeResult);

            public static ProtoMessage VisitorRegularizeResult(int result)
            {
                var res = new VisitorRegularizeResult();
                res.result = result;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.VISITOR_REGULARIZE_RESULT;
                msg.Content = res;
                msg.Model = VisitorRegularizeResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type DailyLoginRewardType = typeof(DailyLoginReward);

            public static ProtoMessage DailyLoginReward(
                int dayCount,
                Gift currentNormalGift,
                Gift currentVipGift,
                Gift nextVipGift)
            {
                var res = new DailyLoginReward();
                res.day_count = dayCount;
                res.current_normal_gift = currentNormalGift;
                res.current_vip_gift = currentVipGift;
                res.next_vip_gift = nextVipGift;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.DAILY_LOGIN_REWARD;
                msg.Content = res;
                msg.Model = DailyLoginRewardType;
                return msg;
            }

            private static readonly Type UpgradeCommodityType = typeof(UpgradeCommodity);

            public static ProtoMessage UpgradeCommodity(string cname)
            {
                var res = new UpgradeCommodity();
                res.cname = cname;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.UPGRADE_COMMODITY;
                msg.Content = res;
                msg.Model = UpgradeCommodityType;
                return msg;
            }

            private static readonly Type UpgradeCommodityResultType = typeof(UpgradeCommodityResult);

            public static ProtoMessage UpgradeCommodityResult(
                MsgResult result,
                string cname,
                int currentCount,
                int currentLevel,
                long money,
                long secondMoney)
            {
                var res = new UpgradeCommodityResult();
                res.result = result;
                res.cname = cname;
                res.current_count = currentCount;
                res.current_level = currentLevel;
                res.money = money;
                res.second_money = secondMoney;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.UPGRADE_COMMODITY_RESULT;
                msg.Content = res;
                msg.Model = UpgradeCommodityResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type SaleCommodityType = typeof(SaleCommodity);

            public static ProtoMessage SaleCommodity(string cname, int count)
            {
                var res = new SaleCommodity();
                res.cname = cname;
                res.count = count;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.SALE_COMMODITY;
                msg.Content = res;
                msg.Model = SaleCommodityType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type SaleCommodityResultType = typeof(SaleCommodityResult);

            public static ProtoMessage SaleCommodityResult(
                MsgResult result,
                List<Currency> profit,
                string cname)
            {
                var res = new SaleCommodityResult();
                res.result = result;
                if (profit != null)
                    res.profit.AddRange(profit);

                res.cname = cname;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.SALE_COMMODITY_RESULT;
                msg.Content = res;
                msg.Model = SaleCommodityResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type QuickStartType = typeof(QuickStart);

            public static ProtoMessage QuickStart(int ping)
            {
                var res = new QuickStart();
                res.ping = ping;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.QUICK_START;
                msg.Content = res;
                msg.Model = QuickStartType;
                return msg;
            }

            private static readonly Type LoginRewardConfigType = typeof(LoginRewardConfig);

            public static ProtoMessage LoginRewardConfig(
                int myLoginDays,
                List<int> rewardMoneys,
                int todayVipExtraMoney,
                List<int> dayRewardStates,
                int todayVipRewardState)
            {
                var res = new LoginRewardConfig();
                res.my_login_days = myLoginDays;
                if (rewardMoneys != null)
                    res.reward_money.AddRange(rewardMoneys);

                res.today_vip_extra_money = todayVipExtraMoney;

                if (dayRewardStates != null)
                    res.day_reward_state.AddRange(dayRewardStates);

                res.today_vip_reward_state = todayVipRewardState;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.LOGIN_REWARD_CONFIG;
                msg.Content = res;
                msg.Model = LoginRewardConfigType;
                return msg;
            }

            private static readonly Type RequestLoginRewardType = typeof(RequestLoginReward);

            public static ProtoMessage RequestLoginReward(int requestType)
            {
                var res = new RequestLoginReward();
                res.request_type = requestType;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_LOGIN_REWARD;
                msg.Content = res;
                msg.Model = RequestLoginRewardType;
                return msg;
            }

            private static readonly Type HRegisterType = typeof(HRegister);

            public static ProtoMessage HRegister(
                string username,
                string password,
                int sex,
                string nickname,
                string inviteCode)
            {
                var res = new HRegister();
                res.username = username;
                res.password = password;
                res.sex = sex;
                res.nickname = nickname;
                res.invite_code = inviteCode;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REGISTER;
                msg.Content = res;
                msg.Model = HRegisterType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type HRegisterReusltType = typeof(HRegisterResult);

            public static ProtoMessage HRegisterResult(int result)
            {
                var res = new HRegisterResult();
                res.result = result;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REGISTER_RESULT;
                msg.Content = res;
                msg.Model = HRegisterReusltType;
                msg.Server = Server.HServer;
                return msg;
            }

            public static ProtoMessage ResetWinRate()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.RESET_WIN_RATE;
                return msg;
            }

            private static readonly Type ResetWinRateResultType = typeof(ResetWinRateResult);

            public static ProtoMessage ResetWinRateResult(int result, string errMsg)
            {
                var res = new ResetWinRateResult();
                res.result = result;
                res.msg = errMsg;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.RESET_WIN_RATE_RESULT;
                msg.Content = res;
                msg.Model = ResetWinRateResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type EditNicknameType = typeof(EditNickname);

            public static ProtoMessage EditNickname(string newNickname)
            {
                var res = new EditNickname();
                res.new_nickname = newNickname;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.EDIT_NICKNAME;
                msg.Content = res;
                msg.Model = EditNicknameType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type EditNicknameResultType = typeof(EditNicknameResult);

            public static ProtoMessage EditNicknameResult(int result, string errMsg)
            {
                var res = new EditNicknameResult();
                res.result = result;
                res.msg = errMsg;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.EDIT_NICKNAME_RESULT;
                msg.Content = res;
                msg.Model = EditNicknameResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            public static ProtoMessage ChangeSex()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.CHANGE_SEX;
                return msg;
            }

            public static ProtoMessage RequestVipExchangeList()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_VIP_EXCHANGE_LIST;
                return msg;
            }

            public static VipExchange VipExchange(
                string name,
                string tag,
                int targetLevel,
                Currency price,
                Currency originPrice,
                string description,
                int type)
            {
                var res = new VipExchange();
                res.name = name;
                res.tag = tag;
                res.target_level = targetLevel;
                res.price = price;
                res.origin_price = originPrice;
                res.description = description;
                res.type = type;
                return res;
            }

            private static readonly Type VipExchangeListResultType = typeof(VipExchangeListResult);

            public static ProtoMessage VipExchangeListResult(
                int currentVipLevel,
                int leftVipDays,
                List<VipExchange> exchanges)
            {
                var res = new VipExchangeListResult();
                res.current_vip_level = currentVipLevel;
                res.left_vip_days = leftVipDays;
                if (exchanges != null)
                    res.exchange.AddRange(exchanges);

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.VIP_EXCHANGE_LIST_RESULT;
                msg.Content = res;
                msg.Model = VipExchangeListResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type RequestExchangeVipType = typeof(RequestExchangeVip);

            public static ProtoMessage RequestExchangeVip(string name)
            {
                var res = new RequestExchangeVip();
                res.name = name;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_EXCHANGE_VIP;
                msg.Content = res;
                msg.Model = RequestExchangeVipType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type RequestExchangeVipResultType = typeof(RequestExchangeVipResult);

            public static ProtoMessage RequestExchangeVipResult(MsgResult result)
            {
                var res = new RequestExchangeVipResult();
                res.result = result;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_EXCHANGE_VIP_RESULT;
                msg.Content = res;
                msg.Model = RequestExchangeVipResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type TraceUserType = typeof(TraceUser);

            public static ProtoMessage TraceUser(string username, bool confirmLeave)
            {
                var res = new TraceUser();
                res.username = username;
                res.confirm_leave = confirmLeave;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.TRACE_USER;
                msg.Content = res;
                msg.Model = TraceUserType;
                return msg;
            }

            private static readonly Type TraceUserResultType = typeof(TraceUserResult);

            public static ProtoMessage TraceUserResult(MsgResult result)
            {
                var res = new TraceUserResult();
                res.result = result;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.TRACE_USER_RESULT;
                msg.Content = res;
                msg.Model = TraceUserResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type SysTextMsgType = typeof(SysTextMsg);

            public static ProtoMessage SysTextMsg(int type, string content, long timestamp)
            {
                var res = new SysTextMsg();
                res.type = type;
                res.msg = content;
                res.timestamp = timestamp;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.SYS_TEXT_MSG;
                msg.Content = res;
                msg.Model = SysTextMsgType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type RequestAwardType = typeof(RequestAward);

            public static ProtoMessage RequestAward(string code)
            {
                var res = new RequestAward();
                res.code = code;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_AWARD;
                msg.Content = res;
                msg.Model = RequestAwardType;
                msg.Server = Server.HServer;
                return msg;
            }

            public static ProtoMessage RequestUserAward(string username, string code)
            {
                var res = new RequestAward();
                res.code = code;
                res.username = username;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_AWARD;
                msg.Content = res;
                msg.Model = RequestAwardType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type RequestAwardResultType = typeof(RequestAwardResult);

            public static ProtoMessage RequestAwardResult(
                int result,
                Award award,
                string errMsg)
            {
                var res = new RequestAwardResult();
                res.result = result;
                res.award = award;
                res.err_msg = errMsg;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_AWARD_RESULT;
                msg.Content = res;
                msg.Model = RequestAwardResultType;
                msg.Server = Server.HServer;
                return msg;
            }

            #region mail

            private static readonly Type RequestMailBriefListType = typeof(RequestMailBriefList);

            public static ProtoMessage RequestMailBriefList(long timestamp)
            {
                var res = new RequestMailBriefList();
                res.timestamp = timestamp;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_MAIL_BRIEF_LIST;
                msg.Content = res;
                msg.Model = RequestMailBriefListType;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type RequestMailContentType = typeof(RequestMailContent);

            public static ProtoMessage RequestMailContent(string mailId)
            {
                var res = new RequestMailContent();
                res.mail_id = mailId;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_MAIL_CONTENT;
                msg.Content = res;
                msg.Model = RequestMailContentType;
                msg.Server = Server.HServer;
                return msg;
            }

            #endregion

            #region checkin

            public static ProtoMessage Checkin()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.CHECKIN;
                return msg;
            }

            public static ProtoMessage ReCheckin()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.RECHECKIN;
                return msg;
            }

            #endregion

            #region invite

            public static ProtoMessage RequestMyInviteData()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_MY_INVITE_DATA;
                return msg;
            }

            public static ProtoMessage RequestBeenInvitedAward(string inviteCode)
            {
                var res = new RequestBeenInvitedAward();
                res.invite_code = inviteCode;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_BEEN_INVITED_AWARD;
                msg.Content = res;
                msg.Server = Server.HServer;

                return msg;
            }

            public static ProtoMessage RequestInviteAward(int inviteCount)
            {
                var res = new RequestInviteAward();
                res.invite_count = inviteCount;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_INVITE_AWARD;
                msg.Content = res;
                msg.Server = Server.HServer;

                return msg;
            }

            #endregion

            #region activity

            public static ProtoMessage RequestActivityAward(string activityCode, string conditionCode)
            {
                var res = new RequestActivityAward();
                res.activity_code = activityCode;
                res.condition_code = conditionCode;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_ACTIVITY_AWARD;
                msg.Content = res;
                msg.Model = typeof(RequestActivityAward);
                msg.Server = Server.HServer;
                return msg;
            }

            public static ProtoMessage RequestActivityStatus()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_ACTIVITY_STATUS;
                msg.Server = Server.HServer;
                return msg;
            }

            #endregion

            #region 元宝

            private static readonly Type RequestExchangeYuanBaoType = typeof(RequestExchangeYuanBao);

            public static ProtoMessage RequestExchangeYuanBao(string name)
            {
                var res = new RequestExchangeYuanBao();
                res.name = name;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.EXCHANGE_YUANBAO;
                msg.Content = res;
                msg.Model = RequestExchangeYuanBaoType;
                msg.Server = Server.HServer;
                return msg;
            }

            public static ProtoMessage RequestMyYuanBaoExchange()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_MY_YUANBAO_EXCHANGE;
                return msg;
            }

            public static ProtoMessage RequestYuanBaoConfig()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_YUANBAO_CONFIG;
                return msg;
            }

            #endregion

            #region 比赛房

            public static ProtoMessage RequestRaceConfigList()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_RACE_CONFIG_LIST;
                msg.Server = Server.HServer;
                return msg;
            }

            private static readonly Type ApplyRaceType = typeof(ApplyRace);

            public static ProtoMessage ApplyRace(long raceId)
            {
                var res = new ApplyRace();
                res.race_id = raceId;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.APPLY_RACE;
                msg.Content = res;
                msg.Model = ApplyRaceType;
                msg.Server = Server.HServer;
                return msg;
            }

            #endregion

            #region 每日任务

            public static ProtoMessage RequestUserTaskList()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.REQUEST_USER_TASK_LIST;
                msg.Server = Server.HServer;
                return msg;
            }

            public static ProtoMessage GetUserTaskAward(string stateId)
            {
                var res = new GetUserTaskAward();
                res.task_code = stateId;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.GET_USER_TASK_AWARD;
                msg.Content = res;
                msg.Server = Server.HServer;
                return msg;
            }

            public static ProtoMessage NotifyDoShare(string id, int type, string userTaskCode, string awardCode,
                string shareResult)
            {
                var res = new NotifyDoShare();
                res.id = id;
                res.type = type;
                res.award_code = awardCode;
                res.share_source = shareResult;
                res.user_task_code = userTaskCode;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.NOTIFY_DO_SHARE;
                msg.Content = res;
                msg.Server = Server.HServer;
                return msg;
            }

            #endregion
        }

        public class GU
        {
            private static readonly Type GLoginType = typeof(GLogin);

            public static ProtoMessage Login(string username, string token)
            {
                var res = new GLogin();
                res.username = username;
                res.token = token;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.LOGIN_V6;
                msg.Content = res;
                msg.Model = GLoginType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type GLoginResultType = typeof(GLoginResult);

            public static ProtoMessage LoginResult(int result, Room room, Table table)
            {
                var res = new GLoginResult();
                res.result = result;
                res.room = room;
                res.table_data = table;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.LOGIN_RESULT_V6;
                msg.Content = res;
                msg.Model = GLoginResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            public static ProtoMessage ShowRoom()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.SHOW_ROOM_V6;
                return msg;
            }

            private static readonly Type ShowRoomResultType = typeof(ShowRoomResult);

            public static ProtoMessage ShowRoomResult(int result, Room room)
            {
                var res = new ShowRoomResult();
                res.result = result;
                res.room = room;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.SHOW_ROOM_RESULT_V6;
                msg.Content = res;
                msg.Model = ShowRoomResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type ChooseTableType = typeof(ChooseTable);

            public static ProtoMessage ChooseTable(long tableId, int type)
            {
                var res = new ChooseTable();
                res.table_id = tableId;
                res.type = type;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.CHOOSE_TABLE_V6;
                msg.Content = res;
                msg.Model = ChooseTableType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type ChooseTableResultType = typeof(ChooseTableResult);

            public static ProtoMessage ChooseTableResult(int result, int type, Table table)
            {
                var res = new ChooseTableResult();
                res.result = result;
                res.type = type;
                res.table = table;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.CHOOSE_TABLE_RESULT_V6;
                msg.Content = res;
                msg.Model = ChooseTableResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type LeaveTableType = typeof(LeaveTable);

            public static ProtoMessage LeaveTable(bool confirmLeave)
            {
                var res = new LeaveTable();
                res.confirm_leave = confirmLeave;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.LEAVE_TABLE_V6;
                msg.Content = res;
                msg.Model = LeaveTableType;
                msg.Server = Server.GServer;

                return msg;
            }

            private static readonly Type LeaveTableResultType = typeof(LeaveTableResult);

            public static ProtoMessage LeaveTableResult(int result, List<Table> tables)
            {
                var res = new LeaveTableResult();
                res.result = result;
                if (tables != null)
                    res.table.AddRange(tables);

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.LEAVE_TABLE_RESULT_V6;
                msg.Content = res;
                msg.Model = LeaveTableResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            public static ProtoMessage Escape()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.ESCAPE_V6;
                return msg;
            }

            private static readonly Type EscapeResultType = typeof(EscapeResult);

            public static ProtoMessage EscapeResult(
                int result,
                List<Table> tables,
                int usedCount,
                int leftCount)
            {
                var res = new EscapeResult();
                res.result = result;
                if (tables != null)
                    res.table.AddRange(tables);

                res.used_count = usedCount;
                res.left_count = leftCount;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.ESCAPE_RESULT_V6;
                msg.Content = res;
                msg.Model = EscapeResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BEscapeType = typeof(BEscape);

            public static ProtoMessage BEscape(
                int seat,
                User player,
                int compensation,
                int cardUsed)
            {
                var res = new BEscape();
                res.seat = seat;
                res.player = player;
                res.compensation = compensation;
                res.card_used = cardUsed;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_ESCAPE_V6;
                msg.Content = res;
                msg.Model = BEscapeType;
                return msg;
            }

            private static readonly Type BTableInOutType = typeof(BTableInOut);

            public static ProtoMessage BTableInOut(
                int inOrOut,
                int seat,
                User player)
            {
                var res = new BTableInOut();
                res.in_or_out = inOrOut;
                res.seat = seat;
                res.player = player;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_TABLE_IN_OUT_V6;
                msg.Content = res;
                msg.Model = BTableInOutType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BTableChangedType = typeof(BTableChanged);

            public static ProtoMessage BTableChanged(Table table)
            {
                var res = new BTableChanged();
                res.table = table;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_TABLE_CHANGED_V6;
                msg.Content = res;
                msg.Model = BTableChangedType;
                msg.Server = Server.GServer;
                return msg;
            }

            public static ProtoMessage ShowTable()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.SHOW_TABLE_V6;
                return msg;
            }

            private static readonly Type ShowTableResultType = typeof(ShowTableResult);

            public static ProtoMessage ShowTableResult(Table table)
            {
                var res = new ShowTableResult();
                res.table = table;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.SHOW_TABLE_RESULT_V6;
                msg.Content = res;
                msg.Model = ShowTableResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type KickOutType = typeof(KickOut);

            public static ProtoMessage KickOut(int seat)
            {
                var res = new KickOut();
                res.seat = seat;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.KICK_OUT_V6;
                msg.Content = res;
                msg.Model = KickOutType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type KickOutResultType = typeof(KickOutResult);

            public static ProtoMessage KickOutResult(int result)
            {
                var res = new KickOutResult();
                res.result = result;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.KICK_OUT_RESULT_V6;
                msg.Content = res;
                msg.Model = KickOutResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BKickOutType = typeof(BKickOut);

            public static ProtoMessage BKickOut(
                int reason,
                string kickerNickname,
                string kickedNickname,
                User kickedUser)
            {
                var res = new BKickOut();
                res.reason = reason;
                res.kicker_nickname = kickerNickname;
                res.kicked_nickname = kickedNickname;
                res.kicked_user = kickedUser;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_KICK_OUT_V6;
                msg.Content = res;
                msg.Model = BKickOutType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BKickOutCounterType = typeof(BKickOutCounter);

            public static ProtoMessage BKickOutCounter(
                int startOrStop,
                int seat,
                int leftTime)
            {
                var res = new BKickOutCounter();
                res.start_or_stop = startOrStop;
                res.seat = seat;
                res.left_time = leftTime;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_KICK_OUT_COUNTER_V6;
                msg.Content = res;
                msg.Model = BKickOutCounterType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type ChooseSeatType = typeof(ChooseSeat);

            public static ProtoMessage ChooseSeat(int seat)
            {
                var res = new ChooseSeat();
                res.seat = seat;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.CHOOSE_SEAT_V6;
                msg.Content = res;
                msg.Model = ChooseSeatType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BPlayerChooseSeatType = typeof(BPlayerChooseSeat);

            public static ProtoMessage BPlayerChooseSeat(
                string username,
                int seat)
            {
                var res = new BPlayerChooseSeat();
                res.username = username;
                res.seat = seat;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_PLAYER_CHOOSE_SEAT_V6;
                msg.Content = res;
                msg.Model = BPlayerChooseSeatType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type ReadyType = typeof(Ready);

            public static ProtoMessage Ready(bool readyOrNot)
            {
                var res = new Ready();
                // 1：准备，2：取消准备
                res.ready_or_not = readyOrNot ? 1 : 2;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.READY_V6;
                msg.Content = res;
                msg.Model = ReadyType;
                msg.Server = Server.GServer;
                return msg;
            }

            public static ProtoMessage ReadyResult(int code, string errMsg)
            {
                var res = new ReadyResult();
                res.res = new MsgResult();
                res.res.code = code;
                res.res.msg = errMsg;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.READY_RESULT_V6;
                msg.Content = res;
                msg.Model = typeof(ReadyResult);
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BUserReadyType = typeof(BUserReady);

            public static ProtoMessage BUserReady(int seat, int readyOrNot)
            {
                var res = new BUserReady();
                res.seat = seat;
                res.ready_or_not = readyOrNot;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_USER_READY_V6;
                msg.Content = res;
                msg.Model = BUserReadyType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type StartRoundType = typeof(StartRound);

            public static ProtoMessage StartRound(
                PokerNumType team1Host,
                PokerNumType team2Host,
                int hostTeam,
                byte[] pokers,
                User user1,
                User user2,
                User user3,
                User user4,
                int roundCount,
                PlayingData playingData)
            {
                var res = new StartRound();
                res.team1_host = team1Host;
                res.team2_host = team2Host;
                res.host_team = hostTeam;
                res.pokers = pokers;
                res.user1 = user1;
                res.user2 = user2;
                res.user3 = user3;
                res.user4 = user4;
                res.round_count = roundCount;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.START_ROUND_V6;
                msg.Content = res;
                msg.Model = StartRoundType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BVipEffectType = typeof(BVipEffect);

            public static ProtoMessage BVipEffect(List<int> seats)
            {
                var res = new BVipEffect();
                if (seats != null)
                    res.seat.AddRange(seats);

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_VIP_EFFECT_V6;
                msg.Content = res;
                msg.Model = BVipEffectType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type TempLeaveType = typeof(TempLeave);

            public static ProtoMessage TempLeave(bool tempLeaveOrNot)
            {
                var res = new TempLeave();
                res.temp_leave_or_not = tempLeaveOrNot ? 1 : 2;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.TEMP_LEAVE_V6;
                msg.Content = res;
                msg.Model = TempLeaveType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BTempLeaveType = typeof(BTempLeave);

            public static ProtoMessage BTempLeave(string username, int tempLeaveOrNot)
            {
                var res = new BTempLeave();
                res.username = username;
                res.temp_leave_or_not = tempLeaveOrNot;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_TEMP_LEAVE_V6;
                msg.Content = res;
                msg.Model = BTempLeaveType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BJinGongRequestType = typeof(BJinGongRequest);

            public static ProtoMessage BJinGongRequest(int jinGong1, int jinGong2, int time, PlayingData playingData)
            {
                var res = new BJinGongRequest();
                res.jingong1 = jinGong1;
                res.jingong2 = jinGong2;
                res.time = time;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_JINGONG_REQUEST_V6;
                msg.Content = res;
                msg.Model = BJinGongRequestType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type JinGongType = typeof(JinGong);

            public static ProtoMessage JinGong(int pokerId)
            {
                var res = new JinGong();
                res.poker_id = pokerId;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.JINGONG_V6;
                msg.Content = res;
                msg.Model = JinGongType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type JinGongResultType = typeof(JinGongResult);

            public static ProtoMessage JinGongResult(int result, int pokerId, byte[] myPokers, PlayingData playingData)
            {
                var res = new JinGongResult();
                res.result = result;
                res.poker_id = pokerId;
                res.my_pokers = myPokers;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.JINGONG_RESULT_V6;
                msg.Content = res;
                msg.Model = JinGongResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            public static ProtoMessage BeenJinGong(int fromSeat, int pokerId, byte[] myPokers, PlayingData playingData)
            {
                var res = new BeenJinGong();
                res.from_seat = fromSeat;
                res.poker_id = pokerId;
                res.my_pokers = myPokers;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.BEEN_JINGONG_V6;
                msg.Content = res;
                msg.Model = typeof(BeenJinGong);
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BJinGongPokerType = typeof(BJinGongPoker);

            public static ProtoMessage BJinGongPoker(int fromSeat, int pokerId, PlayingData playingData)
            {
                var res = new BJinGongPoker();
                res.from_seat = fromSeat;
                res.poker_id = pokerId;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_JINGONG_POKER_V6;
                msg.Content = res;
                msg.Model = BJinGongPokerType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BJinGongResultType = typeof(BJinGongResult);

            public static ProtoMessage BJinGongResult(
                int fromSeat1,
                int toSeat1,
                int pokerId1,
                int fromSeat2,
                int toSeat2,
                int pokerId2,
                PlayingData playingData)
            {
                var res = new BJinGongResult();
                res.from_seat1 = fromSeat1;
                res.to_seat1 = toSeat1;
                res.poker_id1 = pokerId1;
                res.from_seat2 = fromSeat2;
                res.to_seat2 = toSeat2;
                res.poker_id2 = pokerId2;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_JINGONG_RESULT_V6;
                msg.Content = res;
                msg.Model = BJinGongResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BHuanGongRequestType = typeof(BHuanGongRequest);

            public static ProtoMessage BHuanGongRequest(
                int huanGong1,
                int huanGong2,
                int time,
                PlayingData playingData)
            {
                var res = new BHuanGongRequest();
                res.huangong1 = huanGong1;
                res.huangong2 = huanGong2;
                res.time = time;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_HUANGONG_REQUEST_V6;
                msg.Content = res;
                msg.Model = BHuanGongRequestType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type HuanGongType = typeof(HuanGong);

            public static ProtoMessage HuanGong(int pokerId)
            {
                var res = new HuanGong();
                res.poker_id = pokerId;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.HUANGONG_V6;
                msg.Content = res;
                msg.Model = HuanGongType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type HuanGongResultType = typeof(HuanGongResult);

            public static ProtoMessage HuanGongResult(int result, int pokerId, byte[] myPokers, PlayingData playingData)
            {
                var res = new HuanGongResult();
                res.result = result;
                res.poker_id = pokerId;
                res.my_pokers = myPokers;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.HUANGONG_RESULT_V6;
                msg.Content = res;
                msg.Model = HuanGongResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            public static ProtoMessage BeenHuanGong(int fromSeat, int pokerId, byte[] myPokers, PlayingData playingData)
            {
                var res = new BeenHuanGong();
                res.from_seat = fromSeat;
                res.poker_id = pokerId;
                res.my_pokers = myPokers;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.BEEN_HUANGONG_V6;
                msg.Content = res;
                msg.Model = typeof(BeenHuanGong);
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BHuanGongPokerType = typeof(BHuanGongPoker);

            public static ProtoMessage BHuanGongPoker(int fromSeat, int toSeat, int pokerId, PlayingData playingData)
            {
                var res = new BHuanGongPoker();
                res.from_seat = fromSeat;
                res.to_seat = toSeat;
                res.poker_id = pokerId;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_HUANGONG_POKER_V6;
                msg.Content = res;
                msg.Model = BHuanGongPokerType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BKangGongType = typeof(BKangGong);

            public static ProtoMessage BKangGong(int seat1, int seat2, PlayingData playingData)
            {
                var res = new BKangGong();
                res.seat1 = seat1;
                res.seat2 = seat2;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_KANGGONG_V6;
                msg.Content = res;
                msg.Model = BKangGongType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BJieFengType = typeof(BJieFeng);

            public static ProtoMessage BJieFeng(int seat, PlayingData playingData)
            {
                var res = new BJieFeng();
                res.seat = seat;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_JIEFENG_V6;
                msg.Content = res;
                msg.Model = BJieFengType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type ChuPaiKeyType = typeof(ChuPaiKey);

            public static ProtoMessage ChuPaiKey(
                string key,
                int leftTime,
                bool mustChuPai,
                PlayingData playingData)
            {
                var res = new ChuPaiKey();
                res.key = key;
                res.left_time = leftTime;
                res.must_chupai = mustChuPai ? 1 : 2;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.CHUPAI_KEY_V6;
                msg.Content = res;
                msg.Model = ChuPaiKeyType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BChuPaiKeyOwnerType = typeof(BChuPaiKeyOwner);

            public static ProtoMessage BChuPaiKeyOwner(int seat, int leftTime, PlayingData playingData)
            {
                var res = new BChuPaiKeyOwner();
                res.seat = seat;
                res.left_time = leftTime;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_CHUPAI_KEY_OWNER_V6;
                msg.Content = res;
                msg.Model = BChuPaiKeyOwnerType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type ChuPaiType = typeof(ChuPai);

            public static ProtoMessage ChuPai(string chuPaiKey, PokerPattern chuPai)
            {
                var res = new ChuPai();
                res.chupai_key = chuPaiKey;
                res.chupai = chuPai;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.CHUPAI_V6;
                msg.Content = res;
                msg.Model = ChuPaiType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type ChuPaiResultType = typeof(ChuPaiResult);

            public static ProtoMessage ChuPaiResult(
                int result,
                PokerPattern pattern,
                byte[] myPokers,
                PlayingData playingData)
            {
                var res = new ChuPaiResult();
                res.result = result;
                res.pattern = pattern;
                res.my_pokers = myPokers;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.CHUPAI_RESULT_V6;
                msg.Content = res;
                msg.Model = ChuPaiResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BChuPaiType = typeof(BChuPai);

            public static ProtoMessage BChuPai(int seat, PokerPattern chuPai, int leftPokerCount,
                PlayingData playingData)
            {
                var res = new BChuPai();
                res.seat = seat;
                res.chupai = chuPai;
                res.left_poker_count = leftPokerCount;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_CHUPAI_V6;
                msg.Content = res;
                msg.Model = BChuPaiType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BRoundEndType = typeof(BRoundEnd);

            public static ProtoMessage BRoundEnd(
                string username1,
                string username2,
                string username3,
                string username4,
                PokerNumType nextTeam1Host,
                PokerNumType nextTeam2Host,
                int hostTeam,
                int finalExp1,
                int finalExp2,
                int finalExp3,
                int finalExp4,
                int finalMoney1,
                int finalMoney2,
                int finalMoney3,
                int finalMoney4,
                int originExp1,
                int originExp2,
                int originExp3,
                int originExp4,
                int originMoney1,
                int originMoney2,
                int originMoney3,
                int originMoney4,
                int raceId,
                int totalScore1,
                int totalScore2,
                int totalScore3,
                int totalScore4,
                Table table,
                int totalFanbei,
                PlayingData playingData)
            {
                var res = new BRoundEnd();
                res.username1 = username1;
                res.username2 = username2;
                res.username3 = username3;
                res.username4 = username4;

                res.next_team1_host = nextTeam1Host;
                res.next_team2_host = nextTeam2Host;

                res.host_team = hostTeam;

                res.final_exp1 = finalExp1;
                res.final_exp2 = finalExp2;
                res.final_exp3 = finalExp3;
                res.final_exp4 = finalExp4;

                res.final_money1 = finalMoney1;
                res.final_money2 = finalMoney2;
                res.final_money3 = finalMoney3;
                res.final_money4 = finalMoney4;

                res.origin_exp1 = originExp1;
                res.origin_exp2 = originExp2;
                res.origin_exp3 = originExp3;
                res.origin_exp4 = originExp4;

                res.origin_money1 = originMoney1;
                res.origin_money2 = originMoney2;
                res.origin_money3 = originMoney3;
                res.origin_money4 = originMoney4;

                res.race_id = raceId;

                res.total_score1 = totalScore1;
                res.total_score2 = totalScore2;
                res.total_score3 = totalScore3;
                res.total_score4 = totalScore4;

                res.table = table;
                res.total_multiple = totalFanbei;

                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_ROUND_END_V6;
                msg.Content = res;
                msg.Model = BRoundEndType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type HostInfoResultType = typeof(HostInfoResult);

            public static ProtoMessage HostInfoResult(
                PokerNumType team1Host,
                PokerNumType team2Host,
                int hostTeam,
                PlayingData playingData)
            {
                var res = new HostInfoResult();
                res.team1_host = team1Host;
                res.team2_host = team2Host;
                res.host_team = hostTeam;
                res.playing_data = playingData;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.HOST_INFO_RESULT_V6;
                msg.Content = res;
                msg.Model = HostInfoResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type MatchType = typeof(Match);

            public static ProtoMessage Match(bool confirmLeave)
            {
                var res = new Match();
                res.confirm_leave = confirmLeave;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.MATCH_V6;
                msg.Content = res;
                msg.Model = MatchType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type MatchResultType = typeof(MatchResult);

            public static ProtoMessage MatchResult(int result, Table table)
            {
                var res = new MatchResult();
                res.result = result;
                res.table = table;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.MATCH_RESULT_V6;
                msg.Content = res;
                msg.Model = MatchResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            public static ProtoMessage CancelMatch()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.CANCEL_MATCH_V6;
                return msg;
            }

            private static readonly Type CancelMatchResultType = typeof(CancelMatchResult);

            public static ProtoMessage CancelMatchResult(int result)
            {
                var res = new CancelMatchResult();
                res.result = result;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.CANCEL_MATCH_RESULT_V6;
                msg.Content = res;
                msg.Model = CancelMatchResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type PokerPeeperType = typeof(PokerPeeper);

            public static ProtoMessage PokerPeeper(int seat)
            {
                var res = new PokerPeeper();
                res.seat = seat;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.POKER_PEEPER_V6;
                msg.Content = res;
                msg.Model = PokerPeeperType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type PokerPeeperResultType = typeof(PokerPeeperResult);

            public static ProtoMessage PokerPeeperResult(
                int result,
                int seat,
                byte[] pokers)
            {
                var res = new PokerPeeperResult();
                res.result = result;
                res.seat = seat;
                res.pokers = pokers;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.POKER_PEEPER_RESULT_V6;
                msg.Content = res;
                msg.Model = PokerPeeperResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type ClosePokerPeeperType = typeof(ClosePokerPeeper);

            public static ProtoMessage ClosePokerPeeper(int seat)
            {
                var res = new ClosePokerPeeper();
                res.seat = seat;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.CLOSE_POKER_PEEPER_V6;
                msg.Content = res;
                msg.Model = ClosePokerPeeperType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type ClosePokerPeeperResultType = typeof(ClosePokerPeeperResult);

            public static ProtoMessage ClosePokerPeeperResult(int result, int seat)
            {
                var res = new ClosePokerPeeperResult();
                res.result = result;
                res.seat = seat;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.CLOSE_POKER_PEEPER_RESULT_V6;
                msg.Content = res;
                msg.Model = ClosePokerPeeperResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            public static ProtoMessage PokerRecorder()
            {
                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.POKER_RECORDER_V6;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type PokerRecorderResultType = typeof(PokerRecorderResult);

            public static ProtoMessage PokerRecorderResult(int result, List<int> leftCounts)
            {
                var res = new PokerRecorderResult();
                res.result = result;
                if (leftCounts != null)
                    res.left_count.AddRange(leftCounts);

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.POKER_RECORDER_RESULT_V6;
                msg.Content = res;
                msg.Model = PokerRecorderResultType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type BFanbeiType = typeof(BFanbei);

            public static ProtoMessage BFanbei(int newFanbei, int totalFanbei)
            {
                var res = new BFanbei();
                res.new_multiple = newFanbei;
                res.total_multiple = totalFanbei;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.B_FANBEI_V6;
                msg.Content = res;
                msg.Model = BFanbeiType;
                msg.Server = Server.GServer;
                return msg;
            }

            private static readonly Type DoInteractionType = typeof(DoInteraction);

            public static ProtoMessage DoInteraction(int code, string toUsername)
            {
                var res = new DoInteraction();
                res.code = code;
                res.to_username = toUsername;

                var msg = new ProtoMessage();
                msg.Type = CmdType.GU.INTERACTION_V6;
                msg.Content = res;
                msg.Model = DoInteractionType;
                msg.Server = Server.GServer;
                return msg;
            }
        }

        public class CU
        {
            private static readonly Type BTextMsgType = typeof(BTextMsg);

            public static ProtoMessage BTextMsg(string content, string username, string nickname, long timestamp)
            {
                var msg = new BTextMsg();
                msg.chat_channel = ChatChannel.OnTable;
                msg.content = content;
                msg.from_username = username;
                msg.from_nickname = nickname;
                msg.timestamp = timestamp;

                var res = new ProtoMessage();
                res.Type = CmdType.CU.B_TEXT_MSG_V6;
                res.Content = msg;
                res.Model = BTextMsgType;
                res.Server = Server.GServer;
                return res;
            }

            private static readonly Type USendTextMsgType = typeof(USendTextMsg);

            // public static ProtoMessage USendTextMsg(string content, string username, List<string> toUsernames, long timestamp = 0, ChatChannel chatChannel = ChatChannel.OnTable)
            // 不知道为什么在最后的ChatChannel的默认参数存在的时候，混淆的时候就会报找不到guandan_new_proto的dll。
            public static ProtoMessage USendTextMsg(string content, string username, List<string> toUsernames,
                long timestamp = 0)
            {
                var msg = new USendTextMsg();
                msg.from_username = username;
                msg.chat_channel = ChatChannel.OnTable;
                msg.content = content;
                msg.timestamp = timestamp;
                if (toUsernames != null && toUsernames.Count > 0)
                    msg.to_username.AddRange(toUsernames);

                var res = new ProtoMessage();
                res.Type = CmdType.CU.SEND_TEXT_MSG_V6;
                res.Content = msg;
                res.Model = USendTextMsgType;
                res.Server = Server.GServer;
                return res;
            }

            private static readonly Type SendJianMengType = typeof(SendJianMeng);

            public static ProtoMessage SendJianMeng(string cmd, string username, List<string> toUsernames,
                long timestamp = 0)
            {
                var msg = new SendJianMeng();
                msg.from_username = username;
                msg.chat_channel = ChatChannel.OnTable;
                msg.cmd = cmd;
                msg.timestamp = timestamp;
                if (toUsernames != null && toUsernames.Count > 0)
                    msg.to_username.AddRange(toUsernames);

                var res = new ProtoMessage();
                res.Type = CmdType.CU.SEND_JIAN_MENG_V6;
                res.Content = msg;
                res.Model = SendJianMengType;
                res.Server = Server.GServer;
                return res;
            }

            private static readonly Type UFriendDetailType = typeof(UFriendDetail);

            public static ProtoMessage UFriendDetail(string username)
            {
                var msg = new UFriendDetail();
                msg.username = username;

                var res = new ProtoMessage();
                res.Type = CmdType.CU.FRIEND_DETAIL_V6;
                res.Content = msg;
                res.Model = UFriendDetailType;
                res.Server = Server.GServer;
                return res;
            }

            private static readonly Type URefreshFriendListType = typeof(URefreshFriendList);

            public static ProtoMessage URefreshFriendList()
            {
                var msg = new URefreshFriendList();

                var res = new ProtoMessage();
                res.Type = CmdType.CU.REFRESH_FRIEND_LIST_V6;
                res.Content = msg;
                res.Model = URefreshFriendListType;
                res.Server = Server.GServer;
                return res;
            }

            private static readonly Type URemoveFriendType = typeof(URemoveFriend);

            public static ProtoMessage URemoveFriend(string username)
            {
                var msg = new URemoveFriend();
                msg.friend_username = username;

                var res = new ProtoMessage();
                res.Type = CmdType.CU.REMOVE_FRIEND_V6;
                res.Content = msg;
                res.Model = URemoveFriendType;
                res.Server = Server.GServer;
                return res;
            }

            private static readonly Type UAddFriendType = typeof(UAddFriendRequestFromSender);

            public static ProtoMessage UAddFriend(string username)
            {
                var msg = new UAddFriendRequestFromSender();
                msg.friend_username = username;

                var res = new ProtoMessage();
                res.Type = CmdType.CU.ADD_FRIEND_REQUEST_FROM_SENDER_V6;
                res.Content = msg;
                res.Model = UAddFriendType;
                res.Server = Server.GServer;

                return res;
            }

            private static readonly Type UAddFriendResponseType = typeof(UAddFriendResponseFromReceiver);

            public static ProtoMessage UAddFriendResponse(bool accept, string senderUsername)
            {
                var msg = new UAddFriendResponseFromReceiver();
                msg.accept = accept;
                msg.sender_username = senderUsername;

                var res = new ProtoMessage();
                res.Type = CmdType.CU.ADD_FRIEND_RESPONSE_FROM_RECEIVER_V6;
                res.Content = msg;
                res.Model = UAddFriendResponseType;
                res.Server = Server.GServer;

                return res;
            }

            private static readonly Type UCSearchUserType = typeof(UCSearchUser);

            public static ProtoMessage SearchUser(string username)
            {
                var msg = new UCSearchUser();
                msg.target_username = username;

                var res = new ProtoMessage();
                res.Type = CmdType.CU.SEARCH_USER_V6;
                res.Content = msg;
                res.Model = UCSearchUserType;
                res.Server = Server.GServer;

                return res;
            }

            public static ProtoMessage SystemMsg(int type, string content, long timestamp)
            {
                var msg = new BSysTextMsg();
                msg.type = type;
                msg.msg = content;
                msg.timestamp = timestamp;

                var res = new ProtoMessage();
                res.Type = CmdType.CU.B_SYS_TEXT_MSG_V6;
                res.Content = msg;
                res.Model = typeof(BSysTextMsg);
                res.Server = Server.GServer;

                return res;
            }
        }

        public class Shared
        {
            private static readonly Type HBReqType = typeof(HBReq);

            public static ProtoMessage HBReq(long timestamp, Server server)
            {
                var res = new HBReq();
                res.timestamp = timestamp;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.HB_REQ;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.HB_REQ_V6;
                        break;

                    case Server.CServer:
                        msg.Type = CmdType.CU.HB_REQ_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = HBReqType;
                return msg;
            }

            private static readonly Type HBResType = typeof(HBRes);

            public static ProtoMessage HBRes(long timestamp, Server server)
            {
                var res = new HBRes();
                res.timestamp = timestamp;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.HB_RES;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.HB_RES_V6;
                        break;

                    case Server.CServer:
                        msg.Type = CmdType.CU.HB_RES_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = HBResType;
                return msg;
            }

            private static readonly Type ToastType = typeof(Toast);

            public static ProtoMessage Toast(int type, string content, Server server)
            {
                var res = new Toast();
                res.type = type;
                res.content = content;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.PServer:
                        msg.Type = CmdType.PU.TOAST;
                        msg.Server = Server.PServer;
                        break;

                    case Server.HServer:
                        msg.Type = CmdType.HU.TOAST;
                        msg.Server = Server.HServer;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.TOAST_V6;
                        msg.Server = Server.GServer;
                        break;

                    case Server.CServer:
                        msg.Type = CmdType.CU.TOAST_V6;
                        msg.Server = Server.HServer;
                        break;
                }

                msg.Content = res;
                msg.Model = ToastType;
                return msg;
            }

            private static readonly Type LevelUpType = typeof(LevelUp);

            public static ProtoMessage LevelUp(int currentLevel, Server server)
            {
                var res = new LevelUp();
                res.current_level = currentLevel;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.LEVEL_UP;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.LEVEL_UP_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = LevelUpType;
                return msg;
            }

            private static readonly Type AllowanceType = typeof(Allowance);

            public static ProtoMessage Allowance(string text, long amount, Server server)
            {
                var res = new Allowance();
                res.text = text;
                res.amount = amount;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.ALLOWANCE;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.ALLOWANCE_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = AllowanceType;
                return msg;
            }

            private static readonly Type RefillHintType = typeof(RefillHint);

            public static ProtoMessage RefillHint(
                string prepaymentName,
                string title,
                string text,
                bool enterShop,
                Server server)
            {
                var res = new RefillHint();
                res.prepayment_name = prepaymentName;
                res.title = title;
                res.text = text;
                res.can_enter_shop = enterShop;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.REFILL_HINT;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.REFILL_HINT_V6;
                        break;

                    case Server.CServer:
                        msg.Type = CmdType.CU.REFILL_HINT_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = RefillHintType;
                return msg;
            }

            private static readonly Type VipHintType = typeof(VipHint);

            public static ProtoMessage VipHint(string title, string text, Server server)
            {
                var res = new VipHint();
                res.title = title;
                res.text = text;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.VIP_HINT;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.VIP_HINT_V6;
                        break;

                    case Server.CServer:
                        msg.Type = CmdType.CU.VIP_HINT_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = VipHintType;
                return msg;
            }

            private static readonly Type PunishType = typeof(Punish);

            public static ProtoMessage Punish(
                string reason,
                int money,
                int exp,
                Server server)
            {
                var res = new Punish();
                res.reason = reason;
                res.money = money;
                res.exp = exp;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.PUNISH;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.PUNISH_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = PunishType;
                return msg;
            }

            private static readonly Type GiftResultType = typeof(GiftResult);

            public static ProtoMessage GiftResult(int result, Gift gift, Server server)
            {
                var res = new GiftResult();
                res.result = result;
                res.gift = gift;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.GIFT_RESULT;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.GIFT_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = GiftResultType;
                return msg;
            }

            public static ProtoMessage CommodityList(Server server)
            {
                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.COMMODITY_LIST;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.COMMODITY_LIST_V6;
                        break;
                }

                return msg;
            }

            private static readonly Type CommodityListResultType = typeof(CommodityListResult);

            public static ProtoMessage CommodityListResult(List<Commodity> commodities, Server server)
            {
                var res = new CommodityListResult();
                if (commodities != null)
                    res.commodity.AddRange(commodities);

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.COMMODITY_LIST_RESULT;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.COMMODITY_LIST_RESULT_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = CommodityListResultType;
                return msg;
            }

            public static ProtoMessage MyBag(Server server)
            {
                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.MYBAG;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.MYBAG_V6;
                        break;
                }

                return msg;
            }

            private static readonly Type MyBagResultType = typeof(MyBagResult);

            public static ProtoMessage MyBagResult(int result, Bag myBag, Server server)
            {
                var res = new MyBagResult();
                res.result = result;
                res.my_bag = myBag;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.MYBAG_RESULT;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.MYBAG_RESULT_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = MyBagResultType;
                return msg;
            }

            private static readonly Type BuyCommodityType = typeof(BuyCommodity);

            public static ProtoMessage BuyCommodity(
                string name,
                int count,
                string receiver,
                int currencyType,
                Server server)
            {
                var res = new BuyCommodity();
                res.name = name;
                res.count = count;
                res.receiver = receiver;
                res.currency_type = currencyType;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.BUY_COMMODITY;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.BUY_COMMODITY_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = BuyCommodityType;
                return msg;
            }

            private static readonly Type BuyCommodityResultType = typeof(BuyCommodityResult);

            public static ProtoMessage BuyCommodityResult(
                int result,
                string name,
                long currentMoney,
                int currentCount,
                Currency currentCurrency,
                long currentSecondMoney)
            {
                var res = new BuyCommodityResult();
                res.result = result;
                res.name = name;
                res.current_money = currentMoney;
                res.current_currency = currentCurrency;
                res.current_second_money = currentSecondMoney;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.BUY_COMMODITY_RESULT;
                msg.Content = res;
                msg.Model = BuyCommodityResultType;
                return msg;
            }

            private static readonly Type UseCommodityType = typeof(UseCommodity);

            public static ProtoMessage UseCommodity(
                string cname, int count, int useOrNot, Server server)
            {
                var res = new UseCommodity();
                res.cname = cname;
                res.count = count;
                res.use_or_not = useOrNot;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.USE_COMMODITY;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.USE_COMMODITY_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = UseCommodityType;
                return msg;
            }

            private static readonly Type UseCommodityResultType = typeof(UseCommodityResult);

            public static ProtoMessage UseCommodityResult(
                int result, string cname, int useOrNot, int leftCount, Server server)
            {
                var res = new UseCommodityResult();
                res.result = result;
                res.cname = cname;
                res.use_or_not = useOrNot;
                res.left_count = leftCount;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.USE_COMMODITY_RESULT;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.USE_COMMODITY_RESULT_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = UseCommodityResultType;
                return msg;
            }

            private static readonly Type BUseCommodityType = typeof(BUseCommodity);

            public static ProtoMessage BUseCommodity(
                string username, string cname, int useOrNot, int count, Server server)
            {
                var res = new BUseCommodity();
                res.username = username;
                res.cname = cname;
                res.use_or_not = useOrNot;
                res.count = count;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.B_USE_COMMODITY;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.B_USE_COMMODITY_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = BUseCommodityType;
                return msg;
            }

            private static readonly Type RequestActionPriceType = typeof(RequestActionPrice);

            public static ProtoMessage RequestActionPrice(int actionCode, Server server)
            {
                var res = new RequestActionPrice();
                res.action_code = actionCode;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.REQUEST_ACTION_PRICE;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.REQUEST_ACTION_PRICE_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = RequestActionPriceType;
                return msg;
            }

            private static readonly Type ActionPriceResultType = typeof(ActionPriceResult);

            public static ProtoMessage ActionPriceResult(
                int actionCode, Currency price, Server server)
            {
                var res = new ActionPriceResult();
                res.action_code = actionCode;
                res.price = price;

                var msg = new ProtoMessage();
                switch (server)
                {
                    case Server.HServer:
                        msg.Type = CmdType.HU.ACTION_PRICE_RESULT;
                        break;

                    case Server.GServer:
                        msg.Type = CmdType.GU.ACTION_PRICE_RESULT_V6;
                        break;
                }

                msg.Content = res;
                msg.Model = ActionPriceResultType;
                return msg;
            }

            private static readonly Type PushHintType = typeof(PushHint);

            public static ProtoMessage PushHint(
                string code, HintItem item, Server server)
            {
                var res = new PushHint();
                res.code = code;
                res.item = item;

                var msg = new ProtoMessage();
                msg.Type = CmdType.HU.PUSH_HINT;
                msg.Content = res;
                msg.Model = PushHintType;
                return msg;
            }

            private static readonly Type TreasureChestDataType = typeof(TreasureChestData);

            public static ProtoMessage TreasureChestData(
                int needRoundCount,
                int userRoundCount,
                bool hserver)
            {
                var res = new TreasureChestData();
                res.need_round_count = needRoundCount;
                res.user_round_count = userRoundCount;

                var msg = new ProtoMessage();
                msg.Type = hserver ? CmdType.HU.TREASURE_CHEST_DATA : CmdType.GU.TREASURE_CHEST_DATA_V6;
                msg.Content = res;
                msg.Model = TreasureChestDataType;
                return msg;
            }
        }

        public class Model
        {
            public static Currency Currency(int type, long count)
            {
                var res = new Currency();
                res.type = type;
                res.count = count;
                return res;
            }

            public static Level Level(int levelNum, string levelName, long minExp)
            {
                var res = new Level();
                res.level_num = levelNum;
                res.level_name = levelName;
                res.min_exp = minExp;
                return res;
            }

            public static User UserBrief(
                string username, string nickname, int sex)
            {
                var res = new User();
                res.username = username;
                res.nickname = nickname;
                res.sex = sex;
                return res;
            }

            public static User UserPublic(
                string username, string nickname, int sex,
                string description, string city,
                int level, string levelTitle,
                long money, int vip,
                long roundCount, long winCount, long escapeCount,
                string hair, string body, string deskItem,
                int ready, int tempLeave)
            {
                var res = new User();
                res.username = username;
                res.nickname = nickname;
                res.sex = sex;
                res.description = description;
                res.city = city;
                res.level = level;
                res.title = levelTitle;
                res.money = money;
                res.vip = vip;
                res.round_count = roundCount;
                res.win_count = winCount;
                res.escape_count = escapeCount;
                res.hair = hair;
                res.body = body;

                if (!string.IsNullOrEmpty(deskItem))
                    res.item_show.Add(deskItem);

                res.ready = ready;
                res.temp_leave = tempLeave;
                return res;
            }

            public static User User(
                string username,
                string nickname,
                int sex,
                int level,
                int type,
                long money,
                long exp,
                int vip,
                long roundCount,
                long winCount,
                long escapeCount,
                string hair,
                string body,
                string itemShow,
                int ready,
                int tempLeave,
                int vipLeftDays,
                string email,
                string description,
                long secondMoney,
                long vipGrowth,
                string title,
                long expCeil,
                long expFloor,
                string city)
            {
                var res = new User();
                res.username = username;
                res.nickname = nickname;
                res.sex = sex;
                res.level = level;
                res.type = type;
                res.money = money;
                res.exp = exp;
                res.vip = vip;
                res.round_count = roundCount;
                res.win_count = winCount;
                res.escape_count = escapeCount;
                res.hair = hair;
                res.body = body;
                if (!string.IsNullOrEmpty(itemShow))
                    res.item_show.Add(itemShow);
                res.ready = ready;
                res.temp_leave = tempLeave;
                res.vip_left_days = vipLeftDays;
                res.email = email;
                res.description = description;
                res.second_money = secondMoney;
                // deprecated res.vip_growth = vipGrowth;
                res.title = title;
                res.exp_ceil = expCeil;
                res.exp_floor = expFloor;
                res.city = city;
                return res;
            }

            public static UserPrivacy UserPrivacy(
                string realName,
                string idCard,
                string phoneNumber,
                string email,
                string city,
                string addr,
                string school,
                string job,
                string company,
                string device,
                string imei,
                int longitude,
                int latitude)
            {
                var res = new UserPrivacy();
                res.real_name = realName;
                res.id_card = idCard;
                res.phone_number = phoneNumber;
                res.email = email;
                res.city = city;
                res.addr = addr;
                res.school = school;
                res.job = job;
                res.company = company;
                res.device = device;
                res.imei = imei;
                res.longitude = longitude;
                res.latitude = latitude;
                return res;
            }

            public static Bag Bag(List<BagItem> items)
            {
                var res = new Bag();
                if (items != null)
                    res.item.AddRange(items);

                return res;
            }

            public static BagItem BagItem(
                string name,
                int count,
                int state,
                int level)
            {
                var res = new BagItem();
                res.name = name;
                res.count = count;
                res.state = state;
                res.level = level;
                return res;
            }

            public static Room Room(
                long roomId,
                int type,
                long maxPlayerNum,
                long currentPlayerNum,
                int vipThreshhold,
                int baseMoney,
                PokerNumType targetHost,
                List<Table> tables,
                string roomName,
                string description,
                long raceId,
                string tag,
                string tag1,
                int expMultiple,
                long moneyThreshhold,
                int currencyType,
                string extra,
                string roomPicV5,
                int playMode,
                bool containTreasureChest)
            {
                var res = new Room();
                res.room_id = roomId;
                res.type = type;
                res.max_player_num = maxPlayerNum;
                res.current_player_num = currentPlayerNum;
                res.vip_threshhold = vipThreshhold;
                res.base_money = baseMoney;
                res.target_host = targetHost;
                if (tables != null)
                    res.table.AddRange(tables);
                res.room_name = roomName;
                res.description = description;
                res.race_id = raceId;
                res.tag = tag;
                res.tag1 = tag1;
                res.exp_multiple = expMultiple;
                res.money_threshhold = moneyThreshhold;
                res.currency_type = currencyType;
                res.extra = extra;
                res.room_pic_v5 = roomPicV5;
                res.play_mode = playMode;
                res.contain_treasure_chest = containTreasureChest;
                return res;
            }

            public static Table Table(
                long roomId,
                long tableId,
                int type,
                int playerCount,
                PokerNumType targetHost,
                User user1,
                User user2,
                User user3,
                User user4,
                int state,
                int roundCount,
                PlayingData playingData,
                PokerNumType team1Host,
                PokerNumType team2Host,
                int hostTeam)
            {
                var res = new Table();
                res.room_id = roomId;
                res.table_id = tableId;
                res.type = type;
                res.player_count = playerCount;
                res.target_host = targetHost;
                res.user1 = user1;
                res.user2 = user2;
                res.user3 = user3;
                res.user4 = user4;
                res.state = state;
                res.round_count = roundCount;
                res.playing_data = playingData;
                res.team1_host = team1Host;
                res.team2_host = team2Host;
                res.host_team = hostTeam;
                return res;
            }

            public static PlayingData PlayingData(
                int chuPaiKeyOwnerSeat,
                string chuPaiKey,
                int leftTime,
                int mustChuPai,
                byte[] myPokers,
                int leftCount1,
                int leftCount2,
                int leftCount3,
                int leftCount4,
                PokerPattern lastChuPai1,
                PokerPattern lastChuPai2,
                PokerPattern lastChuPai3,
                PokerPattern lastChuPai4,
                int period,
                int winLevel,
                int jgSeat1,
                int jgPoker1,
                int jgDest1,
                int jgSeat2,
                int jgPoker2,
                int jgDest2,
                int hgSeat1,
                int hgPoker1,
                int hgDest1,
                int hgSeat2,
                int hgPoker2,
                int hgDest2,
                int fanBei)
            {
                var res = new PlayingData();
                res.chupai_key_owner_seat = chuPaiKeyOwnerSeat;
                res.chupai_key = chuPaiKey;
                res.left_time = leftTime;
                res.must_chupai = mustChuPai;
                res.my_pokers = myPokers;

                res.left_count1 = leftCount1;
                res.left_count2 = leftCount2;
                res.left_count3 = leftCount3;
                res.left_count4 = leftCount4;

                res.last_chupai1 = lastChuPai1;
                res.last_chupai2 = lastChuPai2;
                res.last_chupai3 = lastChuPai3;
                res.last_chupai4 = lastChuPai4;

                res.period = period;
                res.win_level = winLevel;

                res.jg_seat1 = jgSeat1;
                res.jg_poker1 = jgPoker1;
                res.jg_dest1 = jgDest1;

                res.jg_seat2 = jgSeat2;
                res.jg_poker2 = jgPoker2;
                res.jg_dest2 = jgDest2;

                res.hg_seat1 = hgSeat1;
                res.hg_poker1 = hgPoker1;
                res.hg_dest1 = hgDest1;

                res.hg_seat2 = hgSeat2;
                res.hg_poker2 = hgPoker2;
                res.hg_dest2 = hgDest2;

                res.fanbei = fanBei;
                return res;
            }

            public static PokerPattern PokerPattern(int type, byte[] pokers)
            {
                var res = new PokerPattern();
                res.type = type;
                res.pokers = pokers;
                return res;
            }

            public static Commodity Commodity(
                string name,
                int type,
                string displayName,
                int price,
                int buyMode,
                string description,
                List<int> permittedCurrencies,
                string iosProductId,
                int minAmount,
                int tagId,
                int prePrice,
                string discountDescription,
                long salesVolume,
                int vipLevel,
                int currencyType,
                string overridePic,
                List<int> levelPrices,
                List<int> levelExps,
                List<int> levelThreshholds,
                List<int> levelCurrencyTypes,
                bool hideSale)
            {
                var res = new Commodity();
                res.name = name;
                res.type = type;
                res.display_name = displayName;
                res.price = price;
                res.description = description;
                res.tag_id = tagId;
                res.vip_level = vipLevel;
                res.currency_type = currencyType;
                if (levelPrices != null)
                    res.level_price.AddRange(levelPrices);
                if (levelExps != null)
                    res.level_exp.AddRange(levelExps);
                if (levelThreshholds != null)
                    res.level_thresh_hold.AddRange(levelThreshholds);
                if (levelCurrencyTypes != null)
                    res.level_currency_type.AddRange(levelCurrencyTypes);
                res.hide_sale = hideSale;
                return res;
            }

            public static Gift Gift(int exp, List<BagItem> items, string giftName, List<Currency> currencies)
            {
                var res = new Gift();
                res.exp = exp;
                if (items != null)
                    res.item.AddRange(items);
                res.gift_name = giftName;
                if (currencies != null)
                    res.curreny.AddRange(currencies);

                return res;
            }

            /// <summary>
            /// 奖励。
            /// </summary>
            /// <param name="itemList"></param>
            /// <param name="description"></param>
            /// <returns></returns>
            public static Award Award(List<Currency> itemList, string description)
            {
                var res = new Award();
                if (itemList != null)
                    res.item.AddRange(itemList);

                res.desc = description;
                return res;
            }

            /// <summary>
            /// 支付包。
            /// </summary>
            /// <param name="name"></param>
            /// <param name="displayName"></param>
            /// <param name="description"></param>
            /// <param name="price"></param>
            /// <param name="currencyType"></param>
            /// <param name="currencyCount"></param>
            /// <param name="payChannelList"></param>
            /// <param name="pic"></param>
            /// <param name="tag"></param>
            /// <returns></returns>
            public static Prepayment Prepayment(
                string name,
                string displayName,
                string description,
                double price,
                int currencyType,
                long currencyCount,
                List<int> payChannelList,
                string pic,
                string tag)
            {
                var res = new Prepayment();
                res.name = name;
                res.display_name = displayName;
                res.description = description;
                res.price = price;
                res.currency_type = currencyType;
                res.currency_count = currencyCount;

                if (payChannelList != null)
                    res.pay_channel.AddRange(payChannelList);

                res.pic = pic;
                res.tag = tag;
                return res;
            }

            /// <summary>
            /// 交易对象。
            /// </summary>
            /// <param name="outTradeNo"></param>
            /// <param name="tradeNo"></param>
            /// <param name="prepaymentName"></param>
            /// <param name="totalFee"></param>
            /// <param name="payChannel"></param>
            /// <param name="buyer"></param>
            /// <param name="receiver"></param>
            /// <param name="payState"></param>
            /// <param name="postState"></param>
            /// <param name="createdTime"></param>
            /// <param name="payTime"></param>
            /// <param name="closedTime"></param>
            /// <param name="count"></param>
            /// <returns></returns>
            public static Trade Trade(
                string outTradeNo,
                string tradeNo,
                string prepaymentName,
                double totalFee,
                int payChannel,
                string buyer,
                string receiver,
                int payState,
                int postState,
                long createdTime,
                long payTime,
                long closedTime,
                int count)
            {
                var res = new Trade();
                res.out_trade_no = outTradeNo;
                res.trade_no = tradeNo;
                res.prepayment_name = prepaymentName;
                res.total_fee = totalFee;
                res.pay_channel = payChannel;
                res.buyer = buyer;
                res.receiver = receiver;
                res.pay_state = payState;
                res.post_state = postState;
                res.created_time = createdTime;
                res.pay_time = payTime;
                res.closed_time = closedTime;
                res.count = count;
                return res;
            }

            /// <summary>
            /// 支付渠道。
            /// </summary>
            /// <param name="payChannelId"></param>
            /// <param name="displayName"></param>
            /// <param name="channelUrl"></param>
            /// <param name="description"></param>
            /// <returns></returns>
            public static PayChannel PayChannel(
                int payChannelId,
                string displayName,
                // optional
                string channelUrl,
                string description)
            {
                var res = new PayChannel();
                res.pay_channel_id = payChannelId;
                res.display_name = displayName;
                res.channel_url = channelUrl;
                res.description = description;
                return res;
            }

            /// <summary>
            /// 兑换包。
            /// </summary>
            /// <param name="name"></param>
            /// <param name="displayName"></param>
            /// <param name="description"></param>
            /// <param name="pic"></param>
            /// <param name="sourceType"></param>
            /// <param name="sourceAmount"></param>
            /// <param name="targetType"></param>
            /// <param name="targetAmount"></param>
            /// <returns></returns>
            public static Exchange Exchange(
                string name,
                string displayName,
                string description,
                string pic,
                int sourceType,
                long sourceAmount,
                int targetType,
                long targetAmount)
            {
                var res = new Exchange();
                res.name = name;
                res.display_name = displayName;
                res.pic = pic;
                res.source_type = sourceType;
                res.source_amount = sourceAmount;
                res.target_type = targetType;
                res.target_amount = targetAmount;

                return res;
            }

            public static VipConfig VipConfig(
                int level,
                string displayName,
                int expAdd,
                int moneyAdd,
                int dayExp,
                int dayMoney,
                int daySpeaker,
                int bodyCount,
                int hairCount,
                int deskItemCount,
                int randomCycle,
                int validitySpeaker,
                int pokerPeeper,
                int pokerCounter,
                int pokerRecorder,
                string description,
                List<string> itemNames)
            {
                var res = new VipConfig();
                res.level = level;
                res.display_name = displayName;
                res.exp_add = expAdd;
                res.money_add = moneyAdd;
                res.day_exp = dayExp;
                res.day_money = dayMoney;
                res.day_speaker = daySpeaker;
                res.body_count = bodyCount;
                res.hair_count = hairCount;
                res.desk_item_count = deskItemCount;
                res.random_cycle = randomCycle;
                res.validity_speaker = validitySpeaker;
                res.poker_peeper = pokerPeeper;
                res.poker_counter = pokerCounter;
                res.poker_recorder = pokerRecorder;
                res.description = description;
                if (itemNames != null)
                    res.item_name.AddRange(itemNames);

                return res;
            }

            public static Field Field(
                string name,
                FieldType type,
                string displayName,
                string stringValue,
                int intValue,
                long longValue,
                float floatValue,
                double doubleValue,
                bool boolValue)
            {
                var res = new Field();
                res.name = name;
                res.type = type;
                res.display_name = displayName;
                res.string_value = stringValue;
                res.int_value = intValue;
                res.long_value = longValue;
                res.float_value = floatValue;
                res.double_value = doubleValue;
                res.bool_value = boolValue;
                return res;
            }

            public static DeviceInfo DeviceInfo(
                string key,
                int attrInteger,
                long attrLong,
                string attrString,
                bool attrBool,
                float attrFloat,
                double attrDouble,
                byte[] attrBytes)
            {
                var res = new DeviceInfo();
                res.key = key;
                res.attr_integer = attrInteger;
                res.attr_long = attrLong;
                res.attr_string = attrString;
                res.attr_bool = attrBool;
                res.attr_float = attrFloat;
                res.attr_double = attrDouble;
                res.attr_bytes = attrBytes;
                return res;
            }

            public static com.morln.game.gd.command.Mail Mail(
                string mailId,
                string sender,
                string receiver,
                string title,
                string content,
                int readOrNot,
                long time,
                bool withGift,
                Gift gift)
            {
                var res = new com.morln.game.gd.command.Mail();
                res.mail_id = mailId;
                res.sender = sender;
                res.receiver = receiver;
                res.title = title;
                res.content = content;
                res.time = time;
                // FIXME 修改邮件命令的实现。
//                res.with_gift = withGift;
//                res.gift = gift;
                return res;
            }

            public static MsgResult MsgResult(int code, string msg)
            {
                var res = new MsgResult();
                res.code = code;
                res.msg = msg;
                return res;
            }

            public static FriendInfo FriendInfo(
                string username,
                string nickname,
                OnlineState onlineState,
                int sex,
                int level)
            {
                var res = new FriendInfo();
                res.username = username;
                res.nickname = nickname;
                res.online_state = onlineState;
                res.sex = sex;
                res.level = level;
                return res;
            }

            public static HintItem HintItem(
                int pos,
                string outerPic,
                string contentPic,
                int size,
                int type,
                string url,
                Prepayment prepayment,
                Commodity commodity)
            {
                var res = new HintItem();
                res.pos = pos;
                res.outer_pic = outerPic;
                res.content_pic = contentPic;
                res.size = size;
                res.type = type;
                res.url = url;
                res.prepayment = prepayment;
                res.commodity = commodity;
                return res;
            }
        }
    }
}