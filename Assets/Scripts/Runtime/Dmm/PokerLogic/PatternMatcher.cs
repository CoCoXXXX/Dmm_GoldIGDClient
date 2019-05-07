using System;
using System.Collections.Generic;
using Dmm.PokerLogic.Relation;

namespace Dmm.PokerLogic
{
    public class PatternMatcher : IPatternMatcher
    {
        private static readonly MatchTree MatchTree;

        static PatternMatcher()
        {
            MatchTree = new MatchTree();

            MatchTree.AddRoot(new MatchNode(0, PatternType.BUCHU, true, null));

            MatchTree.AddRoot(new MatchNode(1, PatternType.A, true, new PRMatchStraight()))
                .AddNext(new MatchNode(1, PatternType.NULL, false, new PRMatchStraight()))
                .AddNext(new MatchNode(1, PatternType.NULL, false, new PRMatchStraight()))
                .AddNext(new MatchNode(1, PatternType.NULL, false, new PRMatchStraight()))
                .AddNext(new MatchNode(1, PatternType.ABCDE, true, new PRMatchStraight()));

            MatchTree.AddRoot(new MatchNode(2, PatternType.AA, true, new PRMatchStraight()))
                .AddNext(new XXDDMatchNode())
                .AddNext(new MatchNode(2, PatternType.AABBCC, true, new PRMatchStraight()));

            MatchTree.AddRoot(new MatchNode(3, PatternType.AAA, true, new PRMatchStraight()))
                .AddNext(new MatchNode(2, PatternType.AAAXX, false, new PRMatchNull()));

            MatchTree.GetRoot(3)
                .AddNext(new MatchNode(3, PatternType.AAABBB, true, new PRMatchStraight()));

            MatchTree.AddRoot(new MatchNode(4, PatternType.XXXX, true, new PRMatchStraight()));
            MatchTree.AddRoot(new MatchNode(5, PatternType.XXXX, true, new PRMatchStraight()));
            MatchTree.AddRoot(new MatchNode(6, PatternType.XXXX, true, new PRMatchStraight()));
            MatchTree.AddRoot(new MatchNode(7, PatternType.XXXX, true, new PRMatchStraight()));
            MatchTree.AddRoot(new MatchNode(8, PatternType.XXXX, true, new PRMatchStraight()));
            MatchTree.AddRoot(new MatchNode(9, PatternType.XXXX, true, new PRMatchStraight()));
            MatchTree.AddRoot(new MatchNode(10, PatternType.XXXX, true, new PRMatchStraight()));
        }

        private readonly PatternValue _value;

        public PatternMatcher(PatternValue value)
        {
            _value = value;

            _fullStraightComparer = new FullStraightComparer(value);
            _fullComparer = new FullComparer(value);
            _halfStraightComparer = new HalfStraightComparer(value);
            _halfComparer = new HalfComparer(value);
        }

        public PokerPileChain BuildPPC(List<Poker> pokers)
        {
            List<PokerPile> pileList = new List<PokerPile>();

            List<Poker> pokerList = new List<Poker>();
            if (pokers != null && pokers.Count > 0)
            {
                pokerList.AddRange(pokers);
            }

            List<Poker> heartHosts = RemoveHeartHost(pokerList);

            if (pokerList.Count > 0)
            {
                List<PokerPile> xd = RemoveXD(pokerList);

                Dictionary<int, PokerPile> piles = new Dictionary<int, PokerPile>();
                foreach (var p in pokerList)
                {
                    PokerPile pi = null;

                    if (piles.ContainsKey(p.NumType))
                        pi = piles[p.NumType];

                    if (pi == null)
                    {
                        pi = new PokerPile(p.NumType);
                        if (piles.ContainsKey(p.NumType))
                            piles[p.NumType] = pi;
                        else
                            piles.Add(p.NumType, pi);
                    }

                    pi.AddPoker(1);
                }

                pileList.AddRange(piles.Values);

                if (heartHosts.Count <= 0)
                {
                    ListUtil.QuickSort(pileList, _fullStraightComparer);
                }
                else
                {
                    if (pileList.Count == 2)
                    {
                        if (pokerList.Count + heartHosts.Count == 5)
                        {
                            if (Math.Abs(pileList[0].Count - pileList[1].Count) <= heartHosts.Count)
                            {
                                ListUtil.QuickSort(pileList, _halfComparer);
                            }
                            else
                            {
                                ListUtil.QuickSort(pileList, _fullComparer);
                            }
                        }
                        else if (pokerList.Count + heartHosts.Count == 6)
                        {
                            ListUtil.QuickSort(pileList, _halfStraightComparer);
                        }
                    }
                    else if (pileList.Count >= 3)
                    {
                        ListUtil.QuickSort(pileList, _halfStraightComparer);
                    }
                    else
                    {
                    }
                }

                pileList.AddRange(xd);

                for (int i = 0; i < pileList.Count - 1; i++)
                {
                    pileList[i].Next = pileList[i + 1];
                }
            }

            PokerPile pile = pileList.Count > 0 ? pileList[0] : null;
            PokerPileChain ppc = new PokerPileChain(heartHosts.Count);
            ppc.FirstPile = pile;
            return ppc;
        }

        private readonly FullStraightComparer _fullStraightComparer;

        private class FullStraightComparer : IComparer<PokerPile>
        {
            private readonly IPatternValue _value;

            public FullStraightComparer(IPatternValue value)
            {
                _value = value;
            }

            public int Compare(PokerPile p1, PokerPile p2)
            {
                int v = p2.Count - p1.Count;

                if (v == 0)
                {
                    int v1 = _value.ValueInStraight(p1.NumType);
                    int v2 = _value.ValueInStraight(p2.NumType);

                    if (p1.NumType == PokerNumType.PA &&
                        (p2.NumType == PokerNumType.P2 ||
                         p2.NumType == PokerNumType.P3 ||
                         p2.NumType == PokerNumType.P4 ||
                         p2.NumType == PokerNumType.P5))
                    {
                        v1 = 1;
                    }

                    if (p2.NumType == PokerNumType.PA &&
                        (p1.NumType == PokerNumType.P2 ||
                         p1.NumType == PokerNumType.P3 ||
                         p1.NumType == PokerNumType.P4 ||
                         p1.NumType == PokerNumType.P5))
                    {
                        v2 = 1;
                    }

                    return v2 - v1;
                }
                else
                {
                    return v;
                }
            }
        }

        private readonly FullComparer _fullComparer;

        private class FullComparer : IComparer<PokerPile>
        {
            private readonly IPatternValue _value;

            public FullComparer(IPatternValue value)
            {
                _value = value;
            }

            public int Compare(PokerPile p1, PokerPile p2)
            {
                int v = p2.Count - p1.Count;

                if (v == 0)
                {
                    return _value.ValueOf(p2.NumType) - _value.ValueOf(p1.NumType);
                }
                else
                {
                    return v;
                }
            }
        }

        private readonly HalfStraightComparer _halfStraightComparer;

        private class HalfStraightComparer : IComparer<PokerPile>
        {
            private readonly IPatternValue _value;

            public HalfStraightComparer(IPatternValue value)
            {
                _value = value;
            }

            public int Compare(PokerPile p1, PokerPile p2)
            {
                int v1 = _value.ValueInStraight(p1.NumType);
                int v2 = _value.ValueInStraight(p2.NumType);

                if (p1.NumType == PokerNumType.PA &&
                    (p2.NumType == PokerNumType.P2 ||
                     p2.NumType == PokerNumType.P3 ||
                     p2.NumType == PokerNumType.P4 ||
                     p2.NumType == PokerNumType.P5))
                {
                    v1 = 1;
                }

                if (p2.NumType == PokerNumType.PA &&
                    (p1.NumType == PokerNumType.P2 ||
                     p1.NumType == PokerNumType.P3 ||
                     p1.NumType == PokerNumType.P4 ||
                     p1.NumType == PokerNumType.P5
                    ))
                {
                    v2 = 1;
                }

                return v2 - v1;
            }
        }

        private readonly HalfComparer _halfComparer;

        private class HalfComparer : IComparer<PokerPile>
        {
            private readonly IPatternValue _value;

            public HalfComparer(IPatternValue value)
            {
                _value = value;
            }

            public int Compare(PokerPile p1, PokerPile p2)
            {
                return _value.ValueOf(p2.NumType) - _value.ValueOf(p1.NumType);
            }
        }

        public PokerPattern Match(List<Poker> pokers)
        {
            PokerPileChain ppc = BuildPPC(pokers);
            MatchResult matched = MatchTree.Match(ppc);
            PostProcess(matched, pokers);

            return new PokerPattern(matched.PatternType, ppc.FirstPile, matched.MajorNumType, pokers);
        }

        private void PostProcess(MatchResult result, List<Poker> pokers)
        {
            if (result.PatternType == PatternType.ABCDE)
            {
                if (IsAllSameSuitType(pokers))
                {
                    result.PatternType = PatternType.SuperABCDE;
                }
            }

            if (result.MajorNumType == PokerNumType.PHost)
            {
                result.MajorNumType = _value.CurrentHost;
            }
        }

        public bool IsAllSameSuitType(List<Poker> pokers)
        {
            int suitType = PokerSuitType.NULL;
            foreach (var p in pokers)
            {
                if (_value.IsHeartHost(p))
                {
                    continue;
                }

                if (suitType != PokerSuitType.NULL)
                {
                    if (p.SuitType != suitType)
                    {
                        return false;
                    }
                }
                else
                {
                    suitType = p.SuitType;
                }
            }

            return true;
        }

        public PokerPattern Match(PokerPile pile, int heartHostCount, List<Poker> pokers)
        {
            PokerPileChain ppc = new PokerPileChain(pile, heartHostCount);
            MatchResult matched = MatchTree.Match(ppc);
            PostProcess(matched, pokers);
            return new PokerPattern(matched.PatternType, ppc.FirstPile, matched.MajorNumType, pokers);
        }

        public MatchNode GetRoot(int count)
        {
            return MatchTree.GetRoot(count);
        }

        public List<Poker> RemoveHeartHost(List<Poker> pokers)
        {
            List<Poker> heartHosts = new List<Poker>();
            for (int i = 0; i < pokers.Count; i++)
            {
                if (_value.IsHeartHost(pokers[i]))
                {
                    heartHosts.Add(pokers[i]);
                }
            }

            if (heartHosts.Count > 0)
            {
                foreach (var p in heartHosts)
                {
                    pokers.Remove(p);
                }
            }

            return heartHosts;
        }

        public List<PokerPile> RemoveXD(List<Poker> pokers)
        {
            List<Poker> xd = new List<Poker>();
            PokerPile x = new PokerPile(PokerNumType.PX, 0);
            PokerPile d = new PokerPile(PokerNumType.PD, 0);

            for (int i = 0; i < pokers.Count; i++)
            {
                Poker p = pokers[i];
                if (p.NumType == PokerNumType.PX || p.NumType == PokerNumType.PD)
                {
                    xd.Add(p);
                    if (p.NumType == PokerNumType.PX)
                    {
                        x.AddPoker(1);
                    }
                    else
                    {
                        d.AddPoker(1);
                    }
                }
            }

            if (xd.Count > 0)
            {
                foreach (var p in xd)
                {
                    pokers.Remove(p);
                }
            }

            List<PokerPile> xdPile = new List<PokerPile>();

            if (x.Count > 0)
            {
                xdPile.Add(x);
            }
            if (d.Count > 0)
            {
                xdPile.Add(d);
            }

            return xdPile;
        }
    }
}