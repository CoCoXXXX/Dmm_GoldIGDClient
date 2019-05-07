namespace Dmm.PokerLogic
{
    public class PatternNull : PokerPattern
    {
        private int _count;

        public PatternNull(int type, PokerPile headPile, int pokerCount)
            : base(type, headPile, PokerNumType.NULL, null)
        {
            _count = pokerCount;
        }

        public override int PokerCount
        {
            get { return _count; }
        }
    }
}