using System.Collections.Generic;

namespace Dmm.PokerLogic
{
    public class MatchTree
    {
        private readonly Dictionary<int, MatchNode> _nodes = new Dictionary<int, MatchNode>();

        public MatchNode GetRoot(int pokerCount)
        {
            return _nodes[pokerCount];
        }

        public MatchNode AddRoot(MatchNode node)
        {
            if (_nodes.ContainsKey(node.PokerCount))
                _nodes[node.PokerCount] = node;
            else
                _nodes.Add(node.PokerCount, node);

            return node;
        }

        public MatchResult Match(PokerPileChain ppc)
        {
            if (ppc.IsEmpty)
            {
                return new MatchResult(PatternType.BUCHU, PokerNumType.NULL);
            }

            if (ppc.PileCount == 0)
            {
                if (!ppc.ContainHeartHost)
                {
                    return new MatchResult(PatternType.NULL, PokerNumType.NULL);
                }

                MatchNode root = GetRoot(ppc.HeartHostCount);
                if (root == null)
                {
                    return new MatchResult(PatternType.NULL, PokerNumType.NULL);
                }

                return root.MatchHeartHost(ppc.Head, ppc.HeartHostCount);
            }

            PokerPile firstPile = ppc.FirstPile;

            if (!ppc.ContainHeartHost)
            {
                MatchNode root = GetRoot(firstPile.Count);
                if (root != null)
                {
                    return root.Match(null, firstPile, 0);
                }
                else
                {
                    return new MatchResult(PatternType.NULL, PokerNumType.NULL);
                }
            }
            else
            {
                MatchResult result = new MatchResult(PatternType.NULL, PokerNumType.NULL);
                for (int i = ppc.HeartHostCount; i >= 0; i--)
                {
                    int pokerCount = firstPile != null ? firstPile.Count : 0;
                    MatchNode root = GetRoot(pokerCount + i);
                    if (root != null)
                    {
                        result = root.Match(null, firstPile, ppc.HeartHostCount);
                        if (!result.IsPatternNull)
                        {
                            break;
                        }
                    }
                }

                return result;
            }
        }
    }
}