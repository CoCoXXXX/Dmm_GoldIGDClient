namespace Dmm.PokerLogic.Relation
{
    public class PRMatchNull : IPileRelation
    {
        public PRVerifyResult VerifyRoot(PokerPile pile, int pokerCount, int heartHostCount)
        {
            if (pile == null)
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            if (pokerCount < pile.Count)
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            int needHeartHost = pokerCount - pile.Count;

            if (needHeartHost > 0 &&
                (pile.NumType == PokerNumType.PX || pile.NumType == PokerNumType.PD))
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            if (needHeartHost <= heartHostCount)
            {
                pile.AddPoker(needHeartHost);
                return new PRVerifyResult(true, 0, needHeartHost, null);
            }
            else
            {
                return new PRVerifyResult(false, 0, 0, null);
            }
        }

        public PRVerifyResult Verify(PokerPile previous, PokerPile current, int pokerCount, int heartHostCount)
        {
            if (pokerCount < current.Count)
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            int needHeartHost = pokerCount - current.Count;
            if (needHeartHost > 0 &&
                (current.NumType == PokerNumType.PX || current.NumType == PokerNumType.PD))
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            if (needHeartHost <= heartHostCount)
            {
                current.AddPoker(needHeartHost);
                return new PRVerifyResult(true, 0, needHeartHost, null);
            }
            else
            {
                return new PRVerifyResult(false, 0, 0, null);
            }
        }
    }
}