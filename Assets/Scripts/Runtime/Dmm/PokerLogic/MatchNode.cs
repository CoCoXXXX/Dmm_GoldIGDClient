using System.Collections.Generic;

namespace Dmm.PokerLogic
{
    public class MatchNode
    {
        public int PatternType { get; private set; }

        public int PokerCount { get; private set; }

        public IPileRelation Validator { get; private set; }

        public bool IsMajor { get; private set; }

        public readonly Dictionary<int, MatchNode> NextNodes = new Dictionary<int, MatchNode>();

        public readonly List<MatchNode> OrderedNodes = new List<MatchNode>();

        public MatchNode(int pokerCount, int patternType, bool isMajor, IPileRelation validator)
        {
            PokerCount = pokerCount;
            PatternType = patternType;
            IsMajor = isMajor;
            Validator = validator;
        }

        public MatchNode AddNext(MatchNode node)
        {
            NextNodes[node.PokerCount] = node;
            for (int i = 0; i < OrderedNodes.Count; i++)
            {
                if (node.PokerCount < OrderedNodes[i].PokerCount)
                {
                    OrderedNodes.Insert(i, node);
                    break;
                }
            }

            return node;
        }

        public virtual MatchResult Match(PokerPile previous, PokerPile current, int heartHostCount)
        {
            if (PokerCount < current.Count)
            {
                // 匹配节点的数量小于牌堆牌的数量，是无效节点。
                return new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);
            }

            if (PokerCount - current.Count > heartHostCount)
            {
                // 主牌数量不够，匹配失败。
                return new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);
            }

            PRVerifyResult verifyResult = ValidateRelation(previous, current, PokerCount, heartHostCount);

            if (!verifyResult.Success)
            {
                return new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);
            }

            // 关系verify成功，此时需要用剩下的主牌进行匹配。
            int leftHeartHost = heartHostCount - verifyResult.UsedHeartHostCount;
            // 计算在本次verify之间，跳过了多少个匹配节点。
            int stepCount = verifyResult.StepCount;

            // 如果下一个节点存在，则尝试继续匹配。
            PokerPile nextPile = current.Next;

            if (nextPile != null)
            {
                // 用下一个匹配节点检查一下牌堆。
                MatchResult result = new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);

                // 在下一个节点，以当前的pokerCount为起始，尝试选择匹配节点进行匹配。
                for (int nextPokerCount = PokerCount; nextPokerCount > 0; nextPokerCount--)
                {
                    MatchNode nextMatchNode = null;
                    if (NextNodes.ContainsKey(nextPokerCount))
                        nextMatchNode = NextNodes[nextPokerCount];

                    for (int i = 0; i < stepCount; i++)
                    {
                        if (nextMatchNode != null)
                            nextMatchNode = nextMatchNode.Next(nextPokerCount);
                        else
                            break;
                    }

                    if (nextMatchNode != null)
                    {
                        result = nextMatchNode.Match(current, nextPile, leftHeartHost);
                        if (IsMajor &&
                            !result.IsPatternNull &&
                            !result.IsMajorNumTypeSet)
                        {
                            result.MajorNumType = current.NumType;
                        }
                    }
                    else
                    {
                        result = new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);
                    }

                    if (!result.IsPatternNull)
                    {
                        break;
                    }
                }

                if (result.IsPatternNull)
                {
                    if (previous != null) previous.Next = current;
                    current.RemovePoker(verifyResult.UsedHeartHostCount - (verifyResult.StepCount * PokerCount));
                }

                return result;
            }
            else
            {
                if (leftHeartHost == 0)
                {
                    MatchNode curMatchNode = this;
                    for (int i = 0; i < stepCount; i++)
                    {
                        if (curMatchNode != null)
                            curMatchNode = curMatchNode.Next(PokerCount);
                        else
                            break;
                    }

                    if (curMatchNode != null)
                    {
                        return new MatchResult(curMatchNode.PatternType,
                            curMatchNode.IsMajor ? current.NumType : PokerNumType.NULL);
                    }
                    else
                    {
                        return new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);
                    }
                }
                else
                {
                    MatchResult result = new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);
                    for (int nextPokerCount = leftHeartHost; nextPokerCount > 0; nextPokerCount--)
                    {
                        MatchNode nextMatchNode = null;
                        if (NextNodes.ContainsKey(nextPokerCount))
                            nextMatchNode = NextNodes[nextPokerCount];

                        for (int i = 0; i < stepCount; i++)
                        {
                            if (nextMatchNode != null)
                            {
                                nextMatchNode = nextMatchNode.Next(nextPokerCount);
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (nextMatchNode != null)
                        {
                            result = nextMatchNode.MatchHeartHost(current, leftHeartHost);
                            if (nextMatchNode.IsMajor &&
                                !result.IsPatternNull &&
                                !result.IsMajorNumTypeSet)
                            {
                                result.MajorNumType = current.NumType;
                            }
                        }
                        else
                        {
                            result = new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);
                        }

                        if (!result.IsPatternNull)
                        {
                            break;
                        }
                    }

                    return result;
                }
            }
        }

        public MatchResult MatchHeartHost(PokerPile current, int heartHostCount)
        {
            if (current == null)
            {
                return new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);
            }

            if (PokerCount > heartHostCount)
            {
                return new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);
            }

            if (PokerCount <= heartHostCount)
            {
                if (heartHostCount % PokerCount != 0)
                {
                    return new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);
                }

                int pileCount = heartHostCount / PokerCount;
                PokerPile cur = current;
                for (int i = 0; i < pileCount; i++)
                {
                    PokerPile newPile = new PokerPile(PokerNumType.PHost);
                    newPile.AddPoker(PokerCount);
                    cur.Next = newPile;
                    cur = newPile;
                }

                MatchNode nextMatchNode = this;
                for (int i = 0; i < pileCount - 1; i++)
                {
                    if (nextMatchNode != null)
                    {
                        nextMatchNode = nextMatchNode.Next(PokerCount);
                    }
                    else
                    {
                        break;
                    }
                }

                if (nextMatchNode != null)
                {
                    int majorType = nextMatchNode.IsMajor
                        ? (current.NumType == PokerNumType.NULL ? PokerNumType.PHost : current.NumType)
                        : PokerNumType.NULL;

                    if (majorType != PokerNumType.NULL &&
                        majorType != PokerNumType.PHost &&
                        majorType != PokerNumType.PA &&
                        PokerLogic.PatternType.IsStraight(nextMatchNode.PatternType))
                    {
                        int maxPokerNumber =
                            majorType + PokerLogic.PatternType.GetMajorPileCount(nextMatchNode.PatternType);
                        if (maxPokerNumber > PokerNumType.PA)
                        {
                            majorType = majorType - (maxPokerNumber - PokerNumType.PA - 1);
                        }
                    }
                    return new MatchResult(nextMatchNode.PatternType, majorType);
                }
                else
                {
                    return new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);
                }
            }

            if (OrderedNodes.Count <= 0)
            {
                return new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);
            }

            for (int i = 0; i < OrderedNodes.Count; i++)
            {
                MatchResult result = OrderedNodes[i].MatchHeartHost(current, heartHostCount);
                if (!result.IsPatternNull) return result;
            }

            return new MatchResult(PokerLogic.PatternType.NULL, PokerNumType.NULL);
        }

        public PRVerifyResult ValidateRelation(PokerPile previous, PokerPile current, int pokerCount,
            int heartHostCount)
        {
            if (Validator == null)
            {
                return new PRVerifyResult(true, 0, 0, null);
            }
            else
            {
                if (previous == null)
                    return Validator.VerifyRoot(current, pokerCount, heartHostCount);
                else
                    return Validator.Verify(previous, current, pokerCount, heartHostCount);
            }
        }

        public MatchNode Next(int count)
        {
            if (NextNodes.ContainsKey(count))
                return NextNodes[count];
            else
                return null;
        }
    }
}