namespace Dmm.PokerLogic
{
    public interface IPatternValue
    {
        int Compare(PokerPattern p1, PokerPattern p2);

        int Compare(int numType1, int numType2);

        int ValueOf(PokerPattern pattern);

        int ValueOf(int numType);

        int ValueInStraight(int numType);
    }
}