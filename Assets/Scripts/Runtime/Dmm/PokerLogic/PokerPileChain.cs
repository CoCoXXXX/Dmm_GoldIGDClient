namespace Dmm.PokerLogic
{
    public class PokerPileChain
    {
        public readonly PokerPile Head = new PokerPile(PokerNumType.NULL, 0, null);

        public PokerPileChain(int heartHostCount)
        {
            HeartHostCount = heartHostCount;
        }

        public PokerPileChain(PokerPile firstPile, int heartHostCount)
        {
            if (firstPile != null)
                this.Head.Next = firstPile;

            HeartHostCount = heartHostCount;
        }

        public int HeartHostCount { get; private set; }

        public bool IsEmpty
        {
            get { return PokerCount == 0; }
        }

        public PokerPile FirstPile
        {
            get { return Head.Next; }
            set { Head.Next = value; }
        }

        public int PokerCount
        {
            get
            {
                int ppcCount = 0;
                PokerPile next = Head.Next;
                while (next != null)
                {
                    ppcCount += next.Count;
                    next = next.Next;
                }

                return ppcCount + HeartHostCount;
            }
        }

        public int PileCount
        {
            get
            {
                int count = 0;
                PokerPile next = Head.Next;
                while (next != null)
                {
                    count++;
                    next = next.Next;
                }
                return count;
            }
        }

        public bool ContainHeartHost
        {
            get { return HeartHostCount != 0; }
        }
    }
}