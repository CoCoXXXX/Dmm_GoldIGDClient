namespace Dmm.PokerLogic.Relation
{
    public class PRSelectXXXX : IPileRelation
    {
        public int NumType { get; private set; }

        public int Count { get; private set; }

        private IPatternValue _value;

        public PRSelectXXXX(int numType, int count, IPatternValue value)
        {
            NumType = numType;
            Count = count;
            _value = value;
        }

        public PRVerifyResult VerifyRoot(PokerPile pile, int pokerCount, int heartHostCount)
        {
            if (pile.Count > Count)
            {
                return new PRVerifyResult(true, 0, 0, null);
            }

            if (pile.Count == Count)
            {
                int v1 = _value.ValueOf(pile.NumType);
                int v2 = _value.ValueOf(NumType);
                return new PRVerifyResult(v1 > v2, 0, 0, null);
            }

            int needHeartHost = Count - pile.Count;
            if (needHeartHost <= heartHostCount)
            {
                return new PRVerifyResult(true, 0, needHeartHost, null);
            }

            return new PRVerifyResult(false, 0, 0, null);
        }

        public PRVerifyResult Verify(PokerPile previous, PokerPile current, int pokerCount, int heartHostCount)
        {
            return new PRVerifyResult(false, 0, 0, null);
        }
    }
}