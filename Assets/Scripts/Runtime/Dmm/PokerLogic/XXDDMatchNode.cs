using Dmm.PokerLogic.Relation;

namespace Dmm.PokerLogic
{
    public class XXDDMatchNode : MatchNode
    {
        public XXDDMatchNode() : base(2, PokerLogic.PatternType.NULL, true, new PRMatchStraight())
        {
        }

        public override MatchResult Match(PokerPile previous, PokerPile current, int heartHostCount)
        {
            if (previous == null || current == null)
            {
                return base.Match(previous, current, heartHostCount);
            }

            int numType1 = previous.NumType;
            int numType2 = current.NumType;

            if ((numType1 == PokerNumType.PX || numType1 == PokerNumType.PD) && previous.Count == 2)
            {
                if ((numType2 == PokerNumType.PX || numType2 == PokerNumType.PD) && current.Count == 2)
                {
                    return new MatchResult(PokerLogic.PatternType.XXDD, PokerNumType.PD);
                }
            }

            return base.Match(previous, current, heartHostCount);
        }
    }
}