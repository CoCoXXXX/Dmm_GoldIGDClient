namespace Dmm.PokerLogic.Relation
{
    public class PRSelectRoot : IPileRelation
    {
        public int NumType;

        public bool IsStraight { get; private set; }

        public int StraightCount { get; private set; }

        private IPatternValue _value;

        public PRSelectRoot(int numType, bool straight, int staightCount, IPatternValue value)
        {
            NumType = numType;
            IsStraight = straight;
            StraightCount = staightCount;
            _value = value;
        }

        public PRVerifyResult VerifyRoot(PokerPile pile, int pokerCount, int heartHostCount)
        {
            if (pile == null)
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            bool isGoodValue;
            if (IsStraight)
            {
                if (pile.NumType == PokerNumType.PHost)
                {
                    return new PRVerifyResult(false, 0, 0, null);
                }

                int v1 = pile.NumType == PokerNumType.PA ? 1 : _value.ValueInStraight(pile.NumType);

                if (_value.ValueInStraight(PokerNumType.PA) - v1 < (StraightCount - 1))
                {
                    isGoodValue = false;
                }
                else
                {
                    int v2 = NumType == PokerNumType.PA ? 1 : _value.ValueInStraight(NumType);
                    isGoodValue = v1 > v2;
                }
            }
            else
            {
                isGoodValue = _value.ValueOf(pile.NumType) > _value.ValueOf(NumType);
            }

            if (pile.Count >= pokerCount)
            {
                return new PRVerifyResult(isGoodValue, 0, 0, null);
            }

            int needHeartHost = pokerCount - pile.Count;

            if (needHeartHost > 0 &&
                (pile.NumType == PokerNumType.PX || pile.NumType == PokerNumType.PD))
            {
                return new PRVerifyResult(false, 0, 0, null);
            }

            if (needHeartHost <= heartHostCount)
            {
                if (isGoodValue)
                {
                    pile.AddPoker(needHeartHost);
                }
                return new PRVerifyResult(isGoodValue, 0, needHeartHost, null);
            }
            else
            {
                return new PRVerifyResult(false, 0, 0, null);
            }
        }

        public PRVerifyResult Verify(PokerPile previous, PokerPile current, int pokerCount, int heartHostCount)
        {
            return null;
        }
    }
}