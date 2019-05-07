using System.Collections.Generic;

namespace Dmm.PokerLogic
{
    public interface IPatternMatcher
    {
        PokerPattern Match(List<Poker> pokers);

        PokerPattern Match(PokerPile pile, int heartHostCount, List<Poker> pokers);
    }
}