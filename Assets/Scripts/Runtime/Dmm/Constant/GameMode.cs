namespace Dmm.Constant
{
    public class GameMode
    {
        /// <summary>
        /// 尚未选择游戏模式。
        /// </summary>
        public const int Null = 0;

        /// <summary>
        /// 经典掼蛋。
        /// </summary>
        public const int Classic = 1;

        /// <summary>
        /// 翻倍模式。
        /// </summary>
        public const int Fanbei = 2;

        /// <summary>
        /// 好友模式。
        /// </summary>
        public const int Friend = 3;

        /// <summary>
        /// 单机模式。
        /// </summary>
        public const int Single = 4;

        /// <summary>
        /// 团团转
        /// </summary>
        public const int Ttz = 50;

        /// <summary>
        /// 比赛模式。
        /// </summary>
        public const int Race = 100;

        public static string LabelOf(int mode)
        {
            switch (mode)
            {
                case Classic:
                    return "经典掼蛋";

                case Fanbei:
                    return "翻倍玩法";

                case Friend:
                    return "好友场";

                case Race:
                    return "比赛场";

                case Ttz:
                    return "团团转";

                default:
                    return "掼蛋";
            }
        }
    }
}