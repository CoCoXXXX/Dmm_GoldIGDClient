namespace Dmm.Msg
{
    public class ResultCode
    {
        #region 通用结果码

        /// <summary>
        /// 请求成功
        /// </summary>
        public const int OK = 0;

        /// <summary>
        /// 失败。
        /// </summary>
        public const int FAILED = 10;

        /// <summary>
        /// 数据库操作失败。
        /// </summary>
        public const int DATA_FAILED = 20;

        /// <summary>
        /// 参数错误
        /// </summary>
        public const int INVALID_PARAM = 30;

        /// <summary>
        /// 用户在数据库中不存在。
        /// </summary>
        public const int COMMODITY_USER_NOT_IN_DB = 51;

        /// <summary>
        /// 用户金蛋不够。
        /// </summary>
        public const int COMMODITY_BUY_MONEY_LIMIT = 52;

        /// <summary>
        /// 用户银票不够。
        /// </summary>
        public const int COMMODITY_BUY_YINPIAO_LIMIT = 54;

        /// <summary>
        /// 商品不存在。
        /// </summary>
        public const int COMMODITY_NOT_FOUND = 53;

        /// <summary>
        /// 玩家等级不够，无法购买商品。
        /// </summary>
        public const int COMMODITY_BUY_LEVEL_LIMIT = 55;

        /// <summary>
        /// 数据库中没有用户的背包数据。
        /// </summary>
        public const int COMMODITY_BAG_NOT_IN_DB = 61;

        /// <summary>
        /// 用户没有指定的商品。
        /// </summary>
        public const int COMMODITY_USER_NOT_HAVE = 62;

        /// <summary>
        /// 用户商品数量不够。
        /// </summary>
        public const int COMMODITY_COUNT_NOT_ENOUGH = 63;

        /// <summary>
        /// 商品已经过期。
        /// </summary>
        public const int COMMODITY_VALIDITY_EXPIRED = 64;

        /// <summary>
        /// 商品已经在使用中。
        /// </summary>
        public const int COMMODITY_ALREADY_IN_USE = 65;

        /// <summary>
        /// 商品已经不再使用中。
        /// </summary>
        public const int COMMODITY_ALREADY_NOT_USE = 66;

        /// <summary>
        /// 玩家没有必要使用该商品
        /// </summary>
        public const int COMMODITY_USE_NOT_NECESSARY = 67;

        /// <summary>
        /// 在错误的时间使用商品
        /// </summary>
        public const int COMMODITY_USE_WRONG_TIME = 68;

        /// <summary>
        /// VIP等级不够，无法购买该VIP商品
        /// </summary>
        public const int COMMODITY_BUY_VIP_LIMIT = 69;

        /// <summary>
        /// 不满足房间的底注条件
        /// </summary>
        public const int LESS_THAN_THRESHHOLD = 71;

        /// <summary>
        /// 玩家无权进入比赛房。
        /// </summary>
        public const int RACE_ROOM_NOT_AUTHORIZED = 72;

        #endregion

        #region PServer结果码

        /// <summary>
        /// 大厅服务器在玩家服务器中已经注册过了。
        /// </summary>
        public const int P_HSERVER_ALREADY_REGISTERED = 100;

        /// <summary>
        /// 注册时用户名不合法
        /// </summary>
        public const int P_REGISTER_USERNAME_ILLEGAL = 111;

        /// <summary>
        /// 注册时昵称名不合法
        /// </summary>
        public const int P_REGISTER_NICKNAME_ILLEGAL = 112;

        /// <summary>
        /// 注册时密码输入不合法
        /// </summary>
        public const int P_REGISTER_PASSWORD_ILLEGAL = 113;

        /// <summary>
        /// 用户名已存在
        /// </summary>
        public const int P_REGISTER_USER_EXIST = 121;

        /// <summary>
        /// 用户密码错误
        /// </summary>
        public const int P_LOGIN_PASSWORD_WRONG = 122;

        /// <summary>
        /// 用户不存在
        /// </summary>
        public const int P_LOGIN_USER_NOT_FOUND = 123;

        /// <summary>
        /// 昵称已存在
        /// </summary>
        public const int P_REGISTER_NICKNAME_EXIST = 124;

        /// <summary>
        /// 没有可用的大厅服务器。
        /// </summary>
        public const int P_LOGIN_NO_HALL_SERVER = 131;

        /// <summary>
        /// 无效的客户端版本号。
        /// </summary>
        public const int P_LOGIN_INVALID_CLIENT_VERSION = 132;

        /// <summary>
        /// 需要转正的游客不存在
        /// </summary>
        public const int P_REGULARIZE_NO_VISITOR = 141;

        /// <summary>
        /// 客户端的visitorUsername与deviceId不匹配。
        /// 客户端收到这个code之后，应当清空保存的visitorUsername，直接使用deviceId登陆。
        /// </summary>
        public const int P_USER_WRONG_DEVICE = 150;

        #endregion

        #region HServer结果码

        /// <summary>
        /// 游戏服务器已经在大厅服务器中注册过了。
        /// </summary>
        public const int H_GSERVER_ALREADY_REGISTERED = 201;

        /// <summary>
        /// 聊天服务器已经在大厅服务器中注册过了
        /// </summary>
        public const int H_CSERVER_ALREADY_REGISTERED = 202;

        /// <summary>
        /// 大厅已满，无法接受新玩家
        /// </summary>
        public const int H_PSERVER_HALL_FULL = 203;

        /// <summary>
        /// 没有发现用户。
        /// </summary>
        public const int H_VALIDATE_PLAYER_NOT_FOUND = 211;

        /// <summary>
        /// 无效的Token值。
        /// </summary>
        public const int H_VALIDATE_INVALID_TOKEN = 212;

        /// <summary>
        /// 无效的客户端版本。
        /// </summary>
        public const int INVALID_CLIENT_VERSION = 213;

        /// <summary>
        /// 没有礼物可以领取
        /// </summary>
        public const int H_NO_GIFT = 221;

        /// <summary>
        /// 今天已经领过礼物了
        /// </summary>
        public const int H_GIFT_HAVE_BEEN_RECEIVED = 222;

        /// <summary>
        /// 房间里面已经存在用户了，无法进入。
        /// </summary>
        public const int H_ROOM_PLAYER_ALREADY_EXIST = 224;

        /// <summary>
        /// 房间已经满了，无法进入。
        /// </summary>
        public const int H_ROOM_FULL = 225;

        /// <summary>
        /// 参加比赛，结果报名费不够。
        /// </summary>
        public const int H_CHOOSE_RACE_ROOM_NO_ENOUGH_FEE = 226;

        /// <summary>
        /// 交易信息不存在
        /// </summary>
        public const int H_TRADE_NOT_EXIST = 231;

        /// <summary>
        /// 交易未完成
        /// </summary>
        public const int H_TRADE_NOT_PAID = 232;

        /// <summary>
        /// 交易结束（可能是手动关闭也可能是验证不通过，对应Trade的状态：验证不通过 4，手动关闭 5）
        /// </summary>
        public const int H_TRADE_CLOSED = 233;

        /// <summary>
        /// 订单已付款，正等待发货
        /// </summary>
        public const int H_TRADE_PAID_WAIT_POST = 234;

        /// <summary>
        /// 创建订单时累计支付额度超过渠道设定的上限
        /// </summary>
        public const int H_TRADE_OUTOF_LIMIT = 235;

        /// <summary>
        /// 短信支付的订单是过期的（处理过了），无法再领取金蛋
        /// </summary>
        public const int H_TRADE_OUTOF_DATE = 236;

        /// <summary>
        /// 为他人充值的时候，充值的账户不存在
        /// </summary>
        public const int H_TRADE_RECEIVER_NOT_EXIST = 237;

        /// <summary>
        /// 未找到支付包
        /// </summary>
        public const int H_TRADE_PREPAYMENT_NOT_FOUND = 238;

        /// <summary>
        /// 未找到支付渠道
        /// </summary>
        public const int H_TRADE_PAYCHANNEL_NOT_FOUND = 239;

        /// <summary>
        /// 苹果IAP的收据无效
        /// </summary>
        public const int H_TRADE_RECEIPT_INVALID = 240;

        /// <summary>
        /// 玩家未完善过个人信息
        /// </summary>
        public const int H_PRIVACY_NOT_EXIST = 241;

        /// <summary>
        /// 玩家更新个人信息失败
        /// </summary>
        public const int H_PRIVACY_CHANGE_FAILED = 242;

        /// <summary>
        /// 未发现房间
        /// </summary>
        public const int H_ROOM_NOT_FOUND = 243;

        /// <summary>
        /// 选择游戏服务器的时候发生错误
        /// </summary>
        public const int H_CHOOSE_GAME_SERVER_ERROR = 244;

        /// <summary>
        /// HServer向GServer发送玩家失败。
        /// </summary>
        public const int H_SEND_PLAYER_TO_GSERVER_ERROR = 245;

        /// <summary>
        /// 没有找到CSERVER
        /// </summary>
        public const int H_CSERVER_NOT_FOUND = 246;

        /// <summary>
        /// 修改个人信息时昵称输入不合法
        /// </summary>
        public const int H_EDIT_NICKNAME_ILLEGAL = 251;

        /// <summary>
        /// 修改个人信息时昵称已被使用
        /// </summary>
        public const int H_EDIT_NICKNAME_USED = 252;

        /// <summary>
        /// 修改密码时密码输入不合法
        /// </summary>
        public const int H_EDIT_PASSWORD_ILLEGAL = 253;

        /// <summary>
        /// 修改密码时旧密码错误
        /// </summary>
        public const int H_EDIT_PASSWORD_NOT_AUTH = 254;

        /// <summary>
        /// 修改个人信息时性别不合法
        /// </summary>
        public const int H_EDIT_SEX_ILLEGAL = 255;

        /// <summary>
        /// 今天领取的经验和金蛋都已达上限，不能再领取了
        /// </summary>
        public const int H_NO_MORE_REWARD = 261;

        /// <summary>
        /// 今天领取的经验奖励已达上限，不能再领取了
        /// </summary>
        public const int H_NO_MORE_EXP_REWARD = 262;

        /// <summary>
        /// 今天领取的经验奖励已达上限，不能再领取了
        /// </summary>
        public const int H_NO_MORE_MONEY_REWARD = 263;

        /// <summary>
        /// 货币兑换的接收者不存在
        /// </summary>
        public const int H_EXCHANGE_RECEIVER_NOT_EXIST = 271;

        /// <summary>
        /// 货币兑换的时候源货币不足
        /// </summary>
        public const int H_EXCHANGE_SOURCE_NOT_ENOUGH = 272;

        /// <summary>
        /// 兑换包不存在。
        /// </summary>
        public const int H_EXCHANGE_NOT_FOUND = 273;

        /// <summary>
        /// 兑换失败。
        /// </summary>
        public const int H_EXCHANGE_FAIL = 274;

        /// <summary>
        /// 升级商品的时候等级不足
        /// </summary>
        public const int H_UPGRADE_COMMODITY_INSUFFICIENT_LEVEL = 281;

        /// <summary>
        /// 升级商品的时候钱不足
        /// </summary>
        public const int H_UPGRADE_COMMODITY_INSUFFICIENT_MONEY = 282;

        /// <summary>
        /// 货币类型不支持
        /// </summary>
        public const int CURRENCY_NOT_SUPPORTED = 283;

        #endregion

        #region GServer结果码

        /// <summary>
        /// 玩家不在线，无法跟踪。
        /// </summary>
        public const int PLAYER_NOT_PLAYING = 290;

        /// <summary>
        /// 玩家所在的桌子满了，无法跟踪。
        /// </summary>
        public const int DEST_TABLE_FULL = 291;

        /// <summary>
        /// 没有发现用户
        /// </summary>
        public const int G_USER_NOT_FOUND = 311;

        /// <summary>
        /// 无效的Token值
        /// </summary>
        public const int G_INVALID_TOKEN = 312;

        /// <summary>
        /// GServer 通知 HServer 玩家登陆失败
        /// </summary>
        public const int GH_PLAYER_CON_FAIL = 313;

        /// <summary>
        /// 没有发现房间
        /// </summary>
        public const int G_ROOM_NOT_FOUND = 321;

        /// <summary>
        /// 新建玩家失败
        /// </summary>
        public const int G_NEW_PLAYER_ERROR = 322;

        /// <summary>
        /// 房间已经满了
        /// </summary>
        public const int G_ROOM_FULL = 323;

        /// <summary>
        /// 选桌时未找到该桌子
        /// </summary>
        public const int G_CHOOSE_TABLE_NOT_FOUND = 331;

        /// <summary>
        /// 裁判不能够进入正在打的桌子
        /// </summary>
        public const int G_CHOOSE_TABLE_PLAYING_JUDGE_REJECTED = 332;

        /// <summary>
        /// 向已有桌子添加玩家时，桌子已满
        /// </summary>
        public const int G_TABLE_FULL = 341;

        /// <summary>
        /// 向已有桌子添加玩家时，玩家已在
        /// </summary>
        public const int G_PLAYER_ALREADY_IN = 342;

        /// <summary>
        /// 出的牌不对
        /// </summary>
        public const int G_INVALID_POKER = 351;

        /// <summary>
        /// 没有出牌权
        /// </summary>
        public const int G_NOT_CHUPAI_RIGHT = 352;

        /// <summary>
        /// 服务器端强制客户端出牌
        /// </summary>
        public const int G_FORCE_CHUPAI = 353;

        /// <summary>
        /// 玩家在第一次出牌的时候，发送了不出命令，此时出牌是无效的。
        /// </summary>
        public const int G_FIRST_BUCHU = 354;

        /// <summary>
        /// 玩家在强制出牌的时候，发送了不出命令，是无效的。
        /// </summary>
        public const int G_FORCE_CHUPAI_BUCHU = 355;

        /// <summary>
        /// 玩家在接风的时候，发送了不出命令，是无效的。
        /// </summary>
        public const int G_JIEFENG_BUCHU = 356;

        /// <summary>
        /// 玩家出的牌没有匹配出牌型。
        /// </summary>
        public const int G_CHUPAI_NO_MATCHED_PATTERN = 361;

        /// <summary>
        /// 玩家出的牌不够大。
        /// </summary>
        public const int G_CHUPAI_NO_GREATER = 362;

        /// <summary>
        /// 玩家的进贡无效。
        /// </summary>
        public const int G_INVALID_JINGONG = 363;

        /// <summary>
        /// 玩家的还贡无效。
        /// </summary>
        public const int G_INVALID_HUANGONG = 364;

        /// <summary>
        /// 进入桌子失败。
        /// </summary>
        public const int G_CHOOSE_TABLE_ERROR = 365;

        /// <summary>
        /// 牌局正在进行，无法离开桌子
        /// </summary>
        public const int G_TABLE_PLAYING_CANNOT_LEAVE = 366;

        /// <summary>
        /// 玩家出的牌不符合牌型
        /// </summary>
        public const int G_PATTERN_NOT_MATCH = 367;

        /// <summary>
        /// 没打到目标，将惩罚玩家。
        /// 需要弹出惩罚警告对话框。
        /// </summary>
        public const int LEAVE_WILL_PUNISH = 368;

        /// <summary>
        /// 桌子不允许观众进入
        /// </summary>
        public const int G_TABLE_AUDIENCE_REJECTED = 371;

        /// <summary>
        /// 桌子不允许裁判进入
        /// </summary>
        public const int G_TABLE_JUDGE_REJECTED = 372;

        /// <summary>
        /// 桌子已有裁判
        /// </summary>
        public const int G_TABLE_JUDGE_ALREADY_HAVE = 373;

        /// <summary>
        /// VIP等级不够无法踢人
        /// </summary>
        public const int G_KICK_VIP_LEVEL_NOT_ENOUGH = 381;

        #endregion

        #region CServer结果码

        /// <summary>
        /// 没有权限
        /// </summary>
        public const int USER_NOT_FOUND = 411;

        /// <summary>
        /// 无效的Token值
        /// </summary>
        public const int C_INVALID_TOKEN = 412;

        /// <summary>
        /// 没有语音的接收者
        /// </summary>
        public const int C_NO_TO_USERS = 421;

        /// <summary>
        /// 喇叭个数不够
        /// </summary>
        public const int C_SPEAKER_NOT_ENOUGH = 422;

        /// <summary>
        /// 找不到好友的详细信息
        /// </summary>
        public const int C_FRIEND_NO_DETAIL = 431;

        /// <summary>
        /// 好友个数超过上限
        /// </summary>
        public const int C_FRIEND_COUNT_OUT_OF_LIMIT = 441;

        /// <summary>
        /// 好友不在线
        /// </summary>
        public const int C_FRIEND_OFFLINE = 442;

        /// <summary>
        /// 好友关系已存在
        /// </summary>
        public const int C_FRIEND_ALREADY_ADDED = 443;

        /// <summary>
        /// 好友关系不存在
        /// </summary>
        public const int C_FRIEND_RELATION_NOT_EXIST = 451;

        #endregion
    }
}