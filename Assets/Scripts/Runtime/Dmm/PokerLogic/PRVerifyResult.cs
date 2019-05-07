namespace Dmm.PokerLogic
{
    public class PRVerifyResult
    {
        public bool Success { get; private set; }

        public int UsedHeartHostCount { get; private set; }

        public int StepCount { get; private set; }

        public PokerPile InsertedHeadPile { get; private set; }

        public PRVerifyResult(bool success, int stepCount, int usedHeartHostCount, PokerPile insertedHeadPile)
        {
            Success = success;
            StepCount = stepCount;
            UsedHeartHostCount = usedHeartHostCount;
            InsertedHeadPile = insertedHeadPile;
        }
    }
}