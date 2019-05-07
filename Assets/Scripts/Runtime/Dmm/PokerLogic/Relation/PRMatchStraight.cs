namespace Dmm.PokerLogic.Relation
{
    public class PRMatchStraight : IPileRelation
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

            if (needHeartHost > heartHostCount)
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            if (needHeartHost > 0 &&
                (pile.NumType == PokerNumType.PX || pile.NumType == PokerNumType.PD))
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            pile.AddPoker(needHeartHost);
            return new PRVerifyResult(true, 0, needHeartHost, null);
        }

        public PRVerifyResult Verify(PokerPile previous, PokerPile current, int pokerCount, int heartHostCount)
        {
            if (previous == null || current == null)
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            if (pokerCount < current.Count)
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            if (current.NumType == PokerNumType.PA && current.Next != null)
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            int v1 = previous.NumType;
            int v2 = current.NumType == PokerNumType.PA ? 1 : current.NumType;
            int v = v1 - v2;
            int stepCount = v - 1;

            if (v < 1)
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            int needHeartHost = stepCount * pokerCount + (pokerCount - current.Count);

            if (needHeartHost > 0 &&
                (current.NumType == PokerNumType.PX || current.NumType == PokerNumType.PD))
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            if (needHeartHost <= heartHostCount)
            {
                PokerPile cur = previous;
                PokerPile insertedHead = null;
                for (int i = 1; i <= stepCount; i++)
                {
                    PokerPile newPile = new PokerPile(previous.NumType - i);
                    if (insertedHead == null)
                    {
                        insertedHead = newPile;
                    }

                    newPile.AddPoker(pokerCount);
                    cur.Next = newPile;
                    newPile.Next = current;
                    cur = newPile;
                }

                current.AddPoker(pokerCount - current.Count);
                return new PRVerifyResult(true, stepCount, needHeartHost, insertedHead);
            }
            else
            {
                return new PRVerifyResult(false, 0, 0, null);
            }
        }
    }
}