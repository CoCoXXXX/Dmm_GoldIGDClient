namespace Dmm.HintItemLogic
{
    public class HintItemPos
    {
        public const int LAUNCHER = 1;

        public const int QUIT_PANEL = 2;

        public const int LOGIN_REWARD_PANEL = 3;

        public const int LEFT_BAR_BOTTOM = 4;

        public const int HALL_BOTTOM = 5;

        public const int IN_GAME = 6;

        public const int PUSH = 100;

        public static string LabelOfPos(int pos)
        {
            switch (pos)
            {
                case LAUNCHER:
                    return "launcher";

                case QUIT_PANEL:
                    return "quit_panel";

                case LOGIN_REWARD_PANEL:
                    return "login_reward";

                case LEFT_BAR_BOTTOM:
                    return "left_bar";

                case HALL_BOTTOM:
                    return "hall_bottom";

                case PUSH:
                    return "push";

                default:
                    return "default";
            }
        }

        public static string IdOfPos(int pos)
        {
            return LabelOfPos(pos);
        }
    }
}