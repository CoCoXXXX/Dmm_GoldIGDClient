namespace Dmm.PokerLogic
{
    public interface IPileRelation
    {
        PRVerifyResult VerifyRoot(PokerPile pile, int pokerCount, int heartHostCount);

        PRVerifyResult Verify(PokerPile previous, PokerPile current, int pokerCount, int heartHostCount);
    }
}