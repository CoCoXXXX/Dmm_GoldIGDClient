namespace Dmm.PokerLogic
{
    public class PokerPile
    {
        public int NumType { get; private set; }

        public int Count { get; private set; }

        public PokerPile Next;

        public PokerPile(int numType)
        {
            NumType = numType;
        }

        public PokerPile(int numType, int count)
        {
            NumType = numType;
            Count = count;
        }

        public PokerPile(int numType, int count, PokerPile next)
        {
            NumType = numType;
            Count = count;
            Next = next;
        }

        public void AddPoker(int count)
        {
            Count += count;
        }

        public void RemovePoker(int count)
        {
            Count -= count;
        }

        public override string ToString()
        {
            return "(" + PokerNumType.LabelOf(NumType) + ":" + Count + ")";
        }
    }
}