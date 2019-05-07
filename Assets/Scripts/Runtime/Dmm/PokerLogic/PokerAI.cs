using System;
using System.Collections.Generic;
using Dmm.PokerLogic.Relation;

namespace Dmm.PokerLogic
{
    public class PokerAI
    {
        public static readonly PokerPattern NULL_ABCDE;

        public static readonly PokerPattern NULL_SUPER_ABCDE;

        public static readonly PokerPattern NULL_XXXX;

        public static readonly PokerPattern NULL_AAA;

        public static readonly PokerPattern NULL_AA;

        public static readonly PokerPattern NULL_A;

        public static readonly PokerPattern NULL_AAAXX;

        public static readonly PokerPattern NULL_AABBCC;

        public static readonly PokerPattern NULL_AAABBB;

        static PokerAI()
        {
            NULL_ABCDE = new PatternNull(PatternType.ABCDE, PileChain(
                    Pile(PokerNumType.NULL, 1),
                    Pile(PokerNumType.NULL, 1),
                    Pile(PokerNumType.NULL, 1),
                    Pile(PokerNumType.NULL, 1),
                    Pile(PokerNumType.NULL, 1)),
                5);

            NULL_SUPER_ABCDE = new PatternNull(PatternType.SuperABCDE, PileChain(
                    Pile(PokerNumType.NULL, 1),
                    Pile(PokerNumType.NULL, 1),
                    Pile(PokerNumType.NULL, 1),
                    Pile(PokerNumType.NULL, 1),
                    Pile(PokerNumType.NULL, 1)),
                5);

            NULL_XXXX = new PatternNull(PatternType.XXXX, Pile(PokerNumType.NULL, 4), 4);
            NULL_A = new PatternNull(PatternType.A, Pile(PokerNumType.NULL, 1), 1);
            NULL_AA = new PatternNull(PatternType.AA, Pile(PokerNumType.NULL, 2), 2);
            NULL_AAA = new PatternNull(PatternType.AAA, Pile(PokerNumType.NULL, 3), 3);
            NULL_AAAXX = new PatternNull(PatternType.AAAXX,
                PileChain(Pile(PokerNumType.NULL, 3), Pile(PokerNumType.NULL, 2)), 5);
            NULL_AABBCC = new PatternNull(PatternType.AABBCC,
                PileChain(Pile(PokerNumType.NULL, 2), Pile(PokerNumType.NULL, 2), Pile(PokerNumType.NULL, 2)), 6);
            NULL_AAABBB = new PatternNull(PatternType.AAABBB,
                PileChain(Pile(PokerNumType.NULL, 3), Pile(PokerNumType.NULL, 3)), 6);
        }

        public static PokerPile Pile(int numType, int count)
        {
            var pile = new PokerPile(numType);
            pile.AddPoker(count);
            return pile;
        }

        public static PokerPile PileChain(params PokerPile[] piles)
        {
            if (piles == null || piles.Length <= 0)
                return null;

            for (int i = 0; i < piles.Length - 1; i++)
            {
                piles[i].Next = piles[i + 1];
            }

            return piles[0];
        }

        private readonly PatternMatcher _matcher;

        private readonly PatternValue _value;

        private readonly List<Poker> _myPokers = new List<Poker>();

        private readonly List<Poker> _heartHosts = new List<Poker>();

        private readonly Dictionary<int, List<Poker>> _pokerPool = new Dictionary<int, List<Poker>>();

        private readonly Dictionary<int, List<PokerPile>> _pilePool = new Dictionary<int, List<PokerPile>>();

        private PokerPile _xxdd;

        public PokerAI(PatternMatcher matcher, PatternValue value)
        {
            _matcher = matcher;
            _value = value;
        }

        public void BuildPokerPool()
        {
            _pokerPool.Clear();
            _pilePool.Clear();
            _xxdd = null;
            _heartHosts.Clear();

            if (_myPokers.Count <= 0) return;

            var piles = new Dictionary<int, PokerPile>();
            foreach (var p in _myPokers)
            {
                if (_value.IsHeartHost(p))
                {
                    _heartHosts.Add(p);
                }
                else
                {
                    List<Poker> list = null;
                    if (_pokerPool.ContainsKey(p.NumType))
                        list = _pokerPool[p.NumType];

                    if (list == null)
                    {
                        list = new List<Poker>();
                        _pokerPool[p.NumType] = list;
                    }

                    list.Add(p);

                    PokerPile pile = null;
                    if (piles.ContainsKey(p.NumType))
                        pile = piles[p.NumType];

                    if (pile == null)
                    {
                        pile = new PokerPile(p.NumType);
                        piles[p.NumType] = pile;
                    }

                    pile.AddPoker(1);
                }
            }

            foreach (var pp in piles.Values)
            {
                List<PokerPile> list = null;
                if (_pilePool.ContainsKey(pp.Count))
                {
                    list = _pilePool[pp.Count];
                }

                if (list == null)
                {
                    list = new List<PokerPile>();
                    _pilePool[pp.Count] = list;
                }

                var added = false;
                for (int i = 0; i < list.Count; i++)
                {
                    if (_value.ValueOf(pp.NumType) < _value.ValueOf(list[i].NumType))
                    {
                        list.Insert(i, pp);
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    list.Add(pp);
                }
            }

            List<PokerPile> piles2 = null;
            if (_pilePool.ContainsKey(2))
                piles2 = _pilePool[2];

            if (piles2 != null && piles2.Count >= 2)
            {
                var pd = piles2[piles2.Count - 1];
                var px = piles2[piles2.Count - 2];

                if (px.NumType == PokerNumType.PX &&
                    pd.NumType == PokerNumType.PD)
                {
                    var pw = new List<PokerPile>();
                    pw.Add(px);
                    pw.Add(pd);
                    px.Next = pd;
                    _xxdd = px;
                    piles2.Remove(px);
                    piles2.Remove(pd);

                    if (piles2.Count <= 0)
                        _pilePool.Remove(2);
                }
            }
        }

        public void SetMyPokers(byte[] myPokers)
        {
            _myPokers.Clear();

            if (myPokers == null)
                return;

            for (int i = 0; i < myPokers.Length; i++)
            {
                var number = myPokers[i];
                if (number != Poker.NullPoker)
                    _myPokers.Add(new Poker(number));
            }
        }

        public void SetMyPokers(List<Poker> myPokers)
        {
            _myPokers.Clear();

            if (myPokers != null && myPokers.Count > 0)
            {
                _myPokers.AddRange(myPokers);
            }
        }

        /// <summary>
        /// 平常不把炸弹包含在内的选牌方式。
        /// </summary>
        /// <param name="lastChuPai"></param>
        /// <param name="chaiPai"></param>
        /// <param name="needBomb"></param>
        /// <returns></returns>
        public PokerPattern GetSmallestPatternBiggerThan(PokerPattern lastChuPai, bool chaiPai, bool needBomb)
        {
            return GetSmallestPatternBiggerThan(lastChuPai, chaiPai, needBomb, 3);
        }

        public PokerPattern GetSmallestPatternBiggerThan(PokerPattern lastChuPai, bool chaiPai, bool needBomb,
            int endSelectSize)
        {
            BuildPokerPool();

            if (lastChuPai == null ||
                lastChuPai.IsNull ||
                lastChuPai.Type == PatternType.BUCHU)
            {
                return SelectChuPai();
            }

            if (lastChuPai.Type == PatternType.XXDD)
            {
                // 如果是王炸就没必要再找下去了。
                return PokerPattern.NULL;
            }

            if (PatternType.IsBomb(lastChuPai.Type))
            {
                if (needBomb)
                    return SelectBiggerXXXXOrSuperABCDE(lastChuPai, _heartHosts.Count, chaiPai);
                else
                    return PokerPattern.NULL;
            }

            var selectChain = BuildSelectChain(lastChuPai);
            if (selectChain == null)
                return PokerPattern.NULL;

            PokerPile result = SelectNormal(PatternType.IsStraight(lastChuPai.Type), selectChain, 0, null,
                _heartHosts.Count, chaiPai, endSelectSize);
            if (result != null)
            {
                int majorNumType = result.NumType == PokerNumType.PHost ? _value.CurrentHost : result.NumType;
                result = AdjustPileOrder(result, lastChuPai.Type);
                return new PokerPattern(
                    lastChuPai.Type,
                    result,
                    majorNumType,
                    GetAllPokers(result, false)
                );
            }
            else
            {
                if (needBomb)
                {
                    return SelectBiggerXXXXOrSuperABCDE(null, _heartHosts.Count, chaiPai);
                }
                else
                {
                    return PokerPattern.NULL;
                }
            }
        }

        public List<Poker> GetAllPokers(PokerPile pile, bool needFlush)
        {
            var pokers = new List<Poker>();
            var next = pile;

            int flushSuit = PokerSuitType.NULL;

            if (needFlush)
            {
                var suitCount = new int[] {0, 0, 0, 0, 0};
                var suitFound = new Boolean[suitCount.Length];

                while (next != null)
                {
                    for (int i = 0; i < suitFound.Length; i++)
                    {
                        suitFound[i] = false;
                    }

                    List<Poker> list = null;
                    if (_pokerPool.ContainsKey(next.NumType))
                        list = _pokerPool[next.NumType];

                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            suitFound[list[i].SuitType] = true;
                        }
                    }

                    for (int i = 0; i < suitCount.Length; i++)
                    {
                        if (suitFound[i])
                            suitCount[i]++;
                    }

                    next = next.Next;
                }

                for (int i = 0; i < suitCount.Length; i++)
                {
                    if (suitCount[i] + _heartHosts.Count >= 5)
                    {
                        flushSuit = PokerSuitType.ValueOf(i);
                    }
                }

                if (flushSuit == PokerSuitType.NULL)
                {
                    return null;
                }
            }

            next = pile;
            while (next != null)
            {
                List<Poker> list = null;
                if (_pokerPool.ContainsKey(next.NumType))
                    list = _pokerPool[next.NumType];

                if (list != null)
                {
                    int needHeartHost = next.Count - list.Count;
                    if (needHeartHost < 0)
                        needHeartHost = 0;

                    if (needHeartHost > _heartHosts.Count)
                        return null;

                    if (flushSuit != PokerSuitType.NULL)
                    {
                        int needCount = next.Count - needHeartHost;
                        var sameSuit = new List<Poker>();

                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].SuitType == flushSuit)
                                sameSuit.Add(list[i]);
                        }

                        if (sameSuit.Count < needCount)
                        {
                            int n = needCount - sameSuit.Count;
                            if (n > _heartHosts.Count)
                            {
                                return null;
                            }
                            else
                            {
                                for (int c = 0; c < n; c++)
                                {
                                    var p = _heartHosts[0];
                                    sameSuit.Add(p);

                                    _heartHosts.RemoveAt(0);
                                }
                            }
                        }

                        for (int i = 0; i < needCount; i++)
                        {
                            var p = sameSuit[i];
                            sameSuit.RemoveAt(i);
                            list.Remove(p);
                            pokers.Add(p);
                        }
                    }
                    else
                    {
                        for (int i = 0, n = next.Count - needHeartHost; i < n; i++)
                        {
                            var p = list[0];
                            pokers.Add(p);

                            list.RemoveAt(0);
                        }
                    }

                    if (needHeartHost > _heartHosts.Count)
                        return null;

                    for (int i = 0; i < needHeartHost; i++)
                    {
                        var p = _heartHosts[0];
                        pokers.Add(p);

                        _heartHosts.RemoveAt(0);
                    }
                }
                else
                {
                    int needHeartHost = next.Count;
                    if (needHeartHost > _heartHosts.Count)
                    {
                        return null;
                    }

                    for (int i = 0; i < needHeartHost; i++)
                    {
                        var p = _heartHosts[0];
                        pokers.Add(p);

                        _heartHosts.RemoveAt(0);
                    }
                }

                next = next.Next;
            }

            return pokers;
        }

        public List<SelectNode> BuildSelectChain(PokerPattern pattern)
        {
            var list = new List<SelectNode>();
            var pileList = new List<PokerPile>();
            var headPile = pattern.HeadPile;
            if (headPile == null)
                return null;

            var nextPile = headPile;
            while (nextPile != null)
            {
                pileList.Add(nextPile);
                nextPile = nextPile.Next;
            }

            var majorCount = PatternType.GetMajorPileCount(pattern.Type);
            if (majorCount > pileList.Count)
                return null;

            var majorPile = pileList[majorCount - 1];
            var majorNode = _matcher.GetRoot(majorPile.Count);
            if (majorNode == null)
                return null;

            IPileRelation rootRelation = null;
            if (pattern.Type == PatternType.XXXX)
            {
                rootRelation = new PRSelectXXXX(majorPile.NumType, majorPile.Count, _value);
            }
            else
            {
                rootRelation = new PRSelectRoot(majorPile.NumType, PatternType.IsStraight(pattern.Type),
                    PatternType.GetMajorPileCount(pattern.Type), _value);
            }

            var rootNode = new SelectNode(majorNode.PokerCount, rootRelation);
            list.Add(rootNode);

            var curNode = majorNode;
            for (int i = majorCount - 2; i >= 0; i--)
            {
                var pile = pileList[i];
                curNode = curNode.Next(pile.Count);
                if (curNode == null)
                    return null;

                list.Add(new SelectNode(curNode));
            }

            for (int i = pileList.Count - 1, n = majorCount - 1; i > n; i--)
            {
                var pile = pileList[i];
                curNode = curNode.Next(pile.Count);
                if (curNode == null)
                    return null;

                list.Add(new SelectNode(curNode));
            }

            return list;
        }

        public PokerPile SelectNormal(bool isStraight, List<SelectNode> selectChain, int curSelectIndex,
            PokerPile prePile, int heartHostCount, bool chaiPai, int endSelectSize)
        {
            if (curSelectIndex >= selectChain.Count)
                return null;

            var selectNode = selectChain[curSelectIndex];
            for (int availableHeartHost = 0; availableHeartHost <= heartHostCount; availableHeartHost++)
            {
                int start = selectNode.PokerCount - availableHeartHost;
                if (start > endSelectSize)
                    continue;

                int[] selectSeq = new int[endSelectSize + 1 - start];
                selectSeq[0] = selectNode.PokerCount;

                if (selectSeq.Length > 1)
                {
                    int seqIdx = 1;
                    for (int pokerCount = start; pokerCount <= endSelectSize; pokerCount++)
                    {
                        if (pokerCount == selectNode.PokerCount)
                            continue;

                        selectSeq[seqIdx] = pokerCount;
                        seqIdx++;
                    }
                }

                for (int idx = 0; idx < selectSeq.Length; idx++)
                {
                    int pokerCount = selectSeq[idx];
                    if (!chaiPai && selectNode.PokerCount < pokerCount)
                        continue;

                    List<PokerPile> piles = null;
                    if (_pilePool.ContainsKey(pokerCount))
                        piles = _pilePool[pokerCount];

                    if (piles == null || piles.Count <= 0)
                        continue;

                    for (int i = 0; i < piles.Count; i++)
                    {
                        PokerPile pile = piles[i];
                        PRVerifyResult valid = Verify(selectNode, prePile, pile, availableHeartHost);

                        PokerPile selected = null;
                        if (valid.Success)
                            selected = pile;

                        if (selected != null)
                        {
                            bool isChaiPai = selectNode.PokerCount < pokerCount;
                            if (curSelectIndex < selectChain.Count - 1)
                            {
                                piles.Remove(selected);
                                if (isChaiPai)
                                    selected = AdjustChaiPaiPile(selected, piles, pokerCount, selectNode.PokerCount);

                                PokerPile result = SelectNormal(
                                    isStraight,
                                    selectChain,
                                    curSelectIndex + 1,
                                    selected,
                                    availableHeartHost - valid.UsedHeartHostCount,
                                    chaiPai,
                                    endSelectSize
                                );

                                if (result != null)
                                {
                                    selected.Next = result;
                                    return selected;
                                }
                                else
                                {
                                    if (isChaiPai)
                                        RestoreChaiPaiPile(selected.NumType, pokerCount, selectNode.PokerCount);
                                    else
                                        InsertPile(selected, piles);

                                    selected.RemovePoker(valid.UsedHeartHostCount);
                                }
                            }
                            else
                            {
                                piles.Remove(selected);
                                if (isChaiPai)
                                {
                                    selected = AdjustChaiPaiPile(selected, piles, pokerCount, selectNode.PokerCount);
                                }

                                return selected;
                            }
                        }
                    }
                }

                if (selectNode.PokerCount <= availableHeartHost)
                {
                    int currentNumType;
                    if (!isStraight)
                    {
                        currentNumType = PokerNumType.PHost;
                    }
                    else
                    {
                        currentNumType = GetNextNumType(prePile, true);
                        if (currentNumType == PokerNumType.NULL)
                            currentNumType = PokerNumType.PHost;
                    }

                    PokerPile selected = new PokerPile(currentNumType, selectNode.PokerCount);
                    PRVerifyResult valid = Verify(selectNode, prePile, selected, availableHeartHost);

                    if (!valid.Success)
                        continue;

                    if (curSelectIndex < selectChain.Count - 1)
                    {
                        PokerPile result = SelectNormal(
                            isStraight,
                            selectChain,
                            curSelectIndex + 1,
                            selected,
                            availableHeartHost - selectNode.PokerCount,
                            chaiPai,
                            endSelectSize
                        );

                        if (result != null)
                        {
                            selected.Next = result;
                            return selected;
                        }
                    }
                    else
                    {
                        return selected;
                    }
                }
            }

            return null;
        }

        public PRVerifyResult Verify(SelectNode node, PokerPile previous, PokerPile current, int heartHostCount)
        {
            IPileRelation validator = node.Validator;
            PRVerifyResult result;
            if (validator != null)
            {
                if (previous == null)
                {
                    result = validator.VerifyRoot(current, node.PokerCount, heartHostCount);
                }
                else
                {
                    result = validator.Verify(previous, current, node.PokerCount, heartHostCount);
                }
            }
            else
            {
                result = new PRVerifyResult(true, 0, 0, null);
            }

            return result;
        }

        public PokerPile AdjustChaiPaiPile(PokerPile selected, List<PokerPile> from, int pokerCount, int needCount)
        {
            int newCount = pokerCount - needCount;
            from.Remove(selected);
            selected.RemovePoker(needCount);

            List<PokerPile> list = null;
            if (_pilePool.ContainsKey(newCount))
            {
                list = _pilePool[newCount];
            }

            if (list == null)
            {
                list = new List<PokerPile>();
                _pilePool[newCount] = list;
            }

            InsertPile(selected, list);

            return new PokerPile(selected.NumType, needCount);
        }

        public void RestoreChaiPaiPile(int numType, int fromPool, int count)
        {
            List<PokerPile> origin = null;
            if (_pilePool.ContainsKey(fromPool))
                origin = _pilePool[fromPool];

            List<PokerPile> to = null;
            if (_pilePool.ContainsKey(fromPool - count))
                to = _pilePool[fromPool - count];

            int leftCount = 0;
            for (int i = 0; i < to.Count; i++)
            {
                PokerPile cur = to[i];
                if (cur.NumType == numType)
                {
                    leftCount = cur.Count;
                    to.RemoveAt(i);
                    break;
                }
            }

            var found = false;
            for (int i = 0; i < origin.Count; i++)
            {
                PokerPile cur = origin[i];
                if (cur.NumType == numType)
                {
                    cur.AddPoker(count + leftCount);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                InsertPile(new PokerPile(numType, count + leftCount), origin);
            }
        }

        public void InsertPile(PokerPile pile, List<PokerPile> list)
        {
            bool added = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (_value.Compare(pile.NumType, list[i].NumType) == CompareResult.SMALLER)
                {
                    list.Insert(i, pile);
                    added = true;
                    break;
                }
            }

            if (!added) list.Add(pile);
        }

        public PokerPile AdjustPileOrder(PokerPile head, int patternType)
        {
            var piles = new List<PokerPile>();
            PokerPile next = head;
            while (next != null)
            {
                piles.Add(next);
                next = next.Next;
            }

            int majorCount = PatternType.GetMajorPileCount(patternType);
            if (piles.Count < majorCount)
            {
                return null;
            }

            for (int i = majorCount - 1; i > 0; i--)
            {
                piles[i].Next = piles[i - 1];
            }

            piles[0].Next = null;

            if (piles.Count > majorCount)
            {
                for (int i = piles.Count - 1; i > majorCount; i--)
                {
                    piles[i].Next = piles[i - 1];
                }

                piles[majorCount].Next = null;
                piles[0].Next = piles[piles.Count - 1];
            }

            return piles[majorCount - 1];
        }

        public int GetNextNumType(PokerPile prePile, bool isStraight)
        {
            if (prePile == null)
                return PokerNumType.NULL;

            int number;
            if (isStraight)
            {
                if (prePile.NumType == PokerNumType.PA)
                {
                    number = 1;
                }
                else
                {
                    number = prePile.NumType;
                }
            }
            else
            {
                if (prePile.NumType == PokerNumType.PA)
                {
                    return PokerNumType.NULL;
                }
                else
                {
                    number = prePile.NumType;
                }
            }

            return number + 1;
        }

        public PokerPattern SelectBiggerXXXXOrSuperABCDE(PokerPattern lastChuPai, int heartHostCount, bool chaiPai)
        {
            if (lastChuPai == null || lastChuPai.IsNull)
            {
                lastChuPai = NULL_XXXX;
            }

            PokerPattern selected;

            if (lastChuPai.Type == PatternType.SuperABCDE)
            {
                for (int availableHeartHost = 0; availableHeartHost <= heartHostCount; availableHeartHost++)
                {
                    selected = SelectSuperABCDE(lastChuPai, availableHeartHost, chaiPai);
                    if (!selected.IsNull)
                    {
                        return selected;
                    }
                }

                selected = SelectBiggerXXXX(lastChuPai, 6, 10, heartHostCount);

                if (!selected.IsNull)
                {
                    return selected;
                }
            }
            else
            {
                selected = SelectBiggerXXXX(lastChuPai, 4, 5, heartHostCount);

                if (!selected.IsNull)
                {
                    return selected;
                }

                if (lastChuPai.PokerCount < 6)
                {
                    selected = SelectSuperABCDE(null, heartHostCount, chaiPai);
                    if (!selected.IsNull)
                    {
                        return selected;
                    }
                }

                selected = SelectBiggerXXXX(lastChuPai, 6, 10, heartHostCount);
                if (!selected.IsNull)
                {
                    return selected;
                }
            }

            if (_xxdd != null)
            {
                return new PokerPattern(PatternType.XXDD, _xxdd, PokerNumType.PD, GetAllPokers(_xxdd, false));
            }

            return PokerPattern.NULL;
        }

        private PokerPattern SelectBiggerXXXX(PokerPattern lastChuPai, int fromPokerCount, int toPokerCount,
            int heartHostCount)
        {
            for (int availableHeartHost = 0; availableHeartHost <= heartHostCount; availableHeartHost++)
            {
                for (int pokerCount = fromPokerCount - availableHeartHost; pokerCount <= toPokerCount; pokerCount++)
                {
                    List<PokerPile> piles = null;
                    if (_pilePool.ContainsKey(pokerCount))
                        piles = _pilePool[pokerCount];

                    if (piles != null && piles.Count > 0)
                    {
                        if (pokerCount + availableHeartHost == lastChuPai.PokerCount)
                        {
                            for (int n = 0; n < piles.Count; n++)
                            {
                                PokerPile pile = piles[n];
                                if (_value.Compare(pile.NumType, lastChuPai.MajorNumType) == CompareResult.BIGGER)
                                {
                                    pile.AddPoker(availableHeartHost);
                                    return new PokerPattern(
                                        PatternType.XXXX,
                                        pile,
                                        pile.NumType,
                                        GetAllPokers(pile, false));
                                }
                            }
                        }
                        else if (pokerCount + availableHeartHost > lastChuPai.PokerCount)
                        {
                            PokerPile pile = piles[0];
                            pile.AddPoker(availableHeartHost);
                            return new PokerPattern(
                                PatternType.XXXX,
                                pile,
                                pile.NumType,
                                GetAllPokers(pile, false));
                        }
                    }
                }
            }

            return PokerPattern.NULL;
        }

        public PokerPattern SelectSuperABCDE(PokerPattern lastABCDE, int heartHostCount, bool chaiPai)
        {
            if (lastABCDE == null || lastABCDE.IsNull)
            {
                lastABCDE = NULL_ABCDE;
            }

            List<SelectNode> selectChain = BuildSelectChain(lastABCDE);

            PokerPile selected =
                SelectNormal(PatternType.IsStraight(lastABCDE.Type), selectChain, 0, null, heartHostCount, chaiPai, 5);

            while (selected != null)
            {
                List<Poker> pokers = GetAllPokers(selected, true);
                if (pokers != null)
                {
                    return _matcher.Match(pokers);
                }
                else
                {
                    var validator = (PRSelectRoot) selectChain[0].Validator;
                    validator.NumType = selected.NumType;
                    BuildPokerPool();
                    selected = SelectNormal(PatternType.IsStraight(lastABCDE.Type), selectChain, 0, null,
                        heartHostCount,
                        true, 5);
                }
            }

            return PokerPattern.NULL;
        }

        /// <summary>
        /// 查询当前牌池中所有的同花顺。
        /// </summary>
        public List<PokerPattern> FindAllSuperABCDE()
        {
            var result = new List<PokerPattern>();

            var suits = new[] {PokerSuitType.SPADE, PokerSuitType.CLUB, PokerSuitType.DIAMOND, PokerSuitType.HEART};
            // Map<suitType, pokerList>
            var suitCount = new Dictionary<int, List<Poker>>();
            suitCount.Add(PokerSuitType.SPADE, new List<Poker>());
            suitCount.Add(PokerSuitType.CLUB, new List<Poker>());
            suitCount.Add(PokerSuitType.DIAMOND, new List<Poker>());
            suitCount.Add(PokerSuitType.HEART, new List<Poker>());

            for (int numType = 1; numType <= 10; numType++)
            {
                foreach (var s in suits)
                    suitCount[s].Clear();

                for (int cur = 0; cur < 5; cur++)
                {
                    int currentType = numType + cur;
                    currentType = currentType == 1 ? PokerNumType.PA : currentType;
                    List<Poker> pokers = null;
                    if (_pokerPool.ContainsKey(currentType))
                        pokers = _pokerPool[currentType];

                    if (pokers != null && pokers.Count > 0)
                    {
                        foreach (var poker in pokers)
                        {
                            var s = suitCount[poker.SuitType];
                            var foundSameNumType = false;
                            foreach (var sp in s)
                            {
                                if (sp.NumType == poker.NumType)
                                {
                                    foundSameNumType = true;
                                    break;
                                }
                            }

                            if (!foundSameNumType && !s.Contains(poker))
                            {
                                s.Add(poker);
                            }
                        }
                    }
                }

                foreach (var suitType in suitCount.Keys)
                {
                    var found = suitCount[suitType];
                    if (found.Count >= 5)
                    {
                        var pattern = _matcher.Match(found);
                        if (!pattern.IsNull)
                        {
                            result.Add(pattern);
                        }
                    }
                    else if (found.Count + _heartHosts.Count >= 5)
                    {
                        var list = new List<Poker>();
                        list.AddRange(found);
                        list.AddRange(_heartHosts);
                        var pattern = _matcher.Match(list);
                        if (!pattern.IsNull)
                        {
                            result.Add(pattern);
                        }
                    }
                }
            }

            return result;
        }

        public PokerPattern SelectChuPai()
        {
            var selSeq = AsList(
                PatternType.A,
                PatternType.AA,
                PatternType.AAA,
                PatternType.AAABBB,
                PatternType.AAAXX,
                PatternType.AABBCC,
                PatternType.ABCDE);

            Shuffle(selSeq);

            var selected = PokerPattern.NULL;
            for (int i = 0; i < selSeq.Count; i++)
            {
                selected = SelectChuPai(selSeq[i]);
                if (!selected.IsNull)
                    break;
            }

            if (selected.IsNull)
            {
                BuildPokerPool();
                for (int i = 0; i < 4; i++)
                {
                    if (!_pilePool.ContainsKey(i))
                        continue;

                    var list = _pilePool[i];
                    if (list != null && list.Count > 0)
                    {
                        var pile = list[0];
                        list.RemoveAt(0);
                        selected = new PokerPattern(i, Pile(pile.NumType, i), pile.NumType, GetAllPokers(pile, false));
                    }
                }

                if (selected.IsNull)
                    selected = SelectBiggerXXXXOrSuperABCDE(null, _heartHosts.Count, true);
            }

            return selected;
        }

        public static List<T> AsList<T>(params T[] values)
        {
            if (values == null || values.Length <= 0)
                return null;

            var result = new List<T>();
            for (int i = 0; i < values.Length; i++)
                result.Add(values[i]);

            return result;
        }

        public static void Shuffle<T>(List<T> list)
        {
            var r = new Random();
            for (int i = 0; i < list.Count; i++)
            {
                var idx = r.Next(list.Count);
                var tmp = list[i];
                list[i] = list[idx];
                list[idx] = tmp;
            }
        }

        public PokerPattern SelectChuPai(int patternType)
        {
            PokerPattern lastChupai;

            switch (patternType)
            {
                case PatternType.A:
                    lastChupai = NULL_A;
                    break;
                case PatternType.AA:
                    lastChupai = NULL_AA;
                    break;
                case PatternType.AAA:
                    lastChupai = NULL_AAA;
                    break;
                case PatternType.AAAXX:
                    lastChupai = NULL_AAAXX;
                    break;
                case PatternType.AABBCC:
                    lastChupai = NULL_AABBCC;
                    break;
                case PatternType.AAABBB:
                    lastChupai = NULL_AAABBB;
                    break;
                case PatternType.ABCDE:
                    lastChupai = NULL_ABCDE;
                    break;
                default:
                    lastChupai = NULL_AA;
                    break;
            }

            return GetSmallestPatternBiggerThan(lastChupai, true, false, 8);
        }

        ///
        /// 根据上一次出牌的人是不是对家，上次出牌的人还剩多少张牌来判断是否需要炸弹，是否拆牌。
        ///
        public PokerPattern SelectChuPai(PokerPattern lastChupai, bool isEnemy, int otherLeftCount)
        {
            var rnd = new Random(37);

            bool needBomb = false;
            bool chaipai = false;

            if (isEnemy)
            {
                if (otherLeftCount != 0)
                {
                    if (otherLeftCount <= 8)
                    {
                        needBomb = true;
                        chaipai = false;
                    }
                    else if (otherLeftCount <= 5)
                    {
                        needBomb = true;
                        chaipai = true;
                    }
                    else
                    {
                        needBomb = rnd.Next(10) <= 5;
                        chaipai = rnd.Next(10) <= 2;
                    }
                }
            }
            else
            {
                chaipai = rnd.Next(10) <= 3;
            }

            return GetSmallestPatternBiggerThan(lastChupai, chaipai, needBomb);
        }

        public int GetPokerCount(int numType)
        {
            if (_pokerPool.ContainsKey(numType))
            {
                var list = _pokerPool[numType];
                return list.Count;
            }
            else
            {
                return 0;
            }
        }

        public int AllPokerCount
        {
            get { return _myPokers.Count; }
        }

        public int GetHeartHostCount()
        {
            return _heartHosts.Count;
        }

        public int Compare(Poker p1, Poker p2)
        {
            return _value.Compare(p1.NumType, p2.NumType);
        }
    }
}