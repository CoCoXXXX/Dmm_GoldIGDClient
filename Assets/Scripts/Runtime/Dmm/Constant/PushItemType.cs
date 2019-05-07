namespace Dmm.Constant
{
    public class PushItemType
    {
        public const int Prepayment = 1;

        public const int Vip = 2;

        public const int RecheckinCard = 3;

        public const int UserTask = 4;

        public const int CardRecorder = 5;

        public static string IdOf(int type)
        {
            switch (type)
            {
                case Prepayment:
                    return "prepayment";

                case Vip:
                    return "vip";

                case RecheckinCard:
                    return "recheckin_card";

                case UserTask:
                    return "activity";

                case CardRecorder:
                    return "CardRecorder";

                default:
                    return "default";
            }
        }
    }
}