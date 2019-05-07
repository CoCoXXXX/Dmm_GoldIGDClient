namespace Dmm.PokerLogic
{
    public class MatchResult
    {
        public int PatternType;

        public int MajorNumType;

        public MatchResult(int patternType, int majorNumType)
        {
            PatternType = patternType;
            MajorNumType = majorNumType;
        }

        public bool IsPatternNull
        {
            get { return PatternType == PokerLogic.PatternType.NULL; }
        }

        public bool IsMajorNumTypeSet
        {
            get { return MajorNumType != PokerNumType.NULL; }
        }
    }
}