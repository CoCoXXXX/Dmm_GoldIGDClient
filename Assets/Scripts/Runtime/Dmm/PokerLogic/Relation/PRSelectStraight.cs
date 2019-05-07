namespace Dmm.PokerLogic.Relation
{
    public class PRSelectStraight : IPileRelation
    {
        public PRVerifyResult VerifyRoot(PokerPile pile, int pokerCount, int heartHostCount)
        {
            return new PRVerifyResult(false, 0, 0, null);
        }

        public PRVerifyResult Verify(PokerPile previous, PokerPile current, int pokerCount, int heartHostCount)
        {
            if (previous == null || current == null)
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            int v1 = previous.NumType == PokerNumType.PA ? 1 : previous.NumType;
            int v2 = current.NumType;

            int stepCount = v2 - v1 - 1;

            if (stepCount != 0)
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            int needHeartHost = pokerCount - current.Count;

            if (needHeartHost < 0)
            {
                return new PRVerifyResult(true, 0, 0, null);
            }

            if (needHeartHost > 0 &&
                (current.NumType == PokerNumType.PX || current.NumType == PokerNumType.PD))
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            if (needHeartHost <= heartHostCount)
            {
                current.AddPoker(needHeartHost);
                return new PRVerifyResult(true, stepCount, needHeartHost, null);
            }
            else
            {
                return new PRVerifyResult(false, 0, 0, null);
            }
        }
    }
}