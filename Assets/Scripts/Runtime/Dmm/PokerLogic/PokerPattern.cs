using System.Collections.Generic;

namespace Dmm.PokerLogic
{
    public class PokerPattern
    {
        public static readonly PokerPattern NULL = new PokerPattern(PatternType.NULL, null, PokerNumType.NULL, null);

        public static readonly PokerPattern BUCHU = new PokerPattern(PatternType.BUCHU, null, PokerNumType.NULL, null);

        public int Type { get; private set; }

        public readonly List<Poker> Pokers = new List<Poker>();

        public PokerPile HeadPile { get; private set; }

        public int MajorNumType { get; private set; }

        public bool IsNull
        {
            get { return Type == PatternType.NULL; }
        }

        public int PileCount
        {
            get
            {
                int count = 0;
                PokerPile next = HeadPile;
                while (next != null)
                {
                    count++;
                    next = next.Next;
                }
                return count;
            }
        }

        public virtual int PokerCount
        {
            get { return Pokers.Count; }
        }

        public PokerPattern(int type, PokerPile headPile, int majorNumType, List<Poker> pokers)
        {
            Type = type;
            HeadPile = headPile;
            MajorNumType = majorNumType;

            Pokers.Clear();
            if (pokers != null) Pokers.AddRange(pokers);
        }

        public override string ToString()
        {
            return PatternType.LabelOf(Type);
        }
    }
}