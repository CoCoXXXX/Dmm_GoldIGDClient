namespace Dmm.Constant
{
    /// <summary>
    /// 如果是比赛客户端的话，则需要区别玩家的类型。
    /// </summary>
    public class PlayerType
    {
        /// <summary>
        /// 正常玩家。
        /// </summary>
        public const int Player = 1;

        /// <summary>
        /// 观众。
        /// </summary>
        public const int Audience = 2;

        /// <summary>
        /// 裁判。
        /// </summary>
        public const int Judge = 3;
    }
}