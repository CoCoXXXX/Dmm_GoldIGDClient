namespace Dmm.Constant
{
    public class TablePeriod
    {
        public const int Null = -1;

        public const int Waiting = 0;

        public const int StartRound = 10;

        public const int JinGong = 1;

        public const int HuanGong = 2;

        public const int BeforeChuPai = 3;

        public const int ChuPai = 5;

        public const int EndRound = 6;

        public const int BetweenRound = 7;

        public const int Matching = 50;

        public static string LabelOf(int state)
        {
            switch (state)
            {
                case Waiting:
                    return "Waiting";

                case StartRound:
                    return "StartRound";

                case JinGong:
                    return "JinGong";

                case HuanGong:
                    return "HuanGong";

                case ChuPai:
                    return "ChuPai";

                case EndRound:
                    return "EndRound";

                case BetweenRound:
                    return "BetweenRound";

                case Matching:
                    return "Matching";

                default:
                    return "Null";
            }
        }
    }
}