using Dmm.PokerLogic.Relation;

namespace Dmm.PokerLogic
{
    public class SelectNode
    {
        public int PokerCount;

        public IPileRelation Validator;

        public SelectNode(int pokerCount, IPileRelation validator)
        {
            PokerCount = pokerCount;
            Validator = validator;
        }

        public SelectNode(MatchNode matchNode)
        {
            PokerCount = matchNode.PokerCount;
            var curValidator = matchNode.Validator;
            if (curValidator is PRMatchNull)
            {
                Validator = new PRSelectNull();
            }
            else if (curValidator is PRMatchStraight)
            {
                Validator = new PRSelectStraight();
            }
        }
    }
}