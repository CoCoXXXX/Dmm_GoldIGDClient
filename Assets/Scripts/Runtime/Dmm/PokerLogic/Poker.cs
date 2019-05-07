namespace Dmm.PokerLogic
{
    /// <summary>
    /// 牌对象。
    /// </summary>
    public class Poker
    {
        public const byte NullPoker = 250;

        public int Number { get; private set; }

        public int NumType { get; private set; }

        public int SuitType { get; private set; }

        public Poker(int number)
        {
            Number = number;
            NumType = PokerUtil.NumTypeOf(number);
            SuitType = PokerUtil.SuitTypeOf(number);
        }

        public Poker(int numType, int suitType)
        {
            NumType = numType;
            SuitType = suitType;
            Number = PokerUtil.CalculateNumber(numType, suitType, 1);
        }

        #region Equals和HashCode方法

        protected bool Equals(Poker other)
        {
            return Number == other.Number;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Poker) obj);
        }

        public override int GetHashCode()
        {
            return Number;
        }

        #endregion

        public override string ToString()
        {
            if (NumType == PokerNumType.PX)
                return "小王";

            if (NumType == PokerNumType.PD)
                return "大王";

            return PokerSuitType.LabelOf(SuitType) + PokerNumType.LabelOf(NumType);
        }
    }
}