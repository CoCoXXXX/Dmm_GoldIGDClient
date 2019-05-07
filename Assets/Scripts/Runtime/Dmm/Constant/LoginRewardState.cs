namespace Dmm.Constant
{
    public class LoginRewardState
    {
        /// <summary>
        /// 奖励未领取，失效。
        /// </summary>
        public const int Discard = -2;

        /// <summary>
        /// 没有奖励可领。
        /// </summary>
        public const int NoHave = -1;

        /// <summary>
        /// 可以领奖励。
        /// </summary>
        public const int Ok = 1;

        /// <summary>
        /// 已经领取过了。
        /// </summary>
        public const int Already = 0;
    }
}