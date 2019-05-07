using System.Collections;
using Dmm.PokerLogic;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace PokerLogic
{
    public class PokerAITest
    {
        private PokerAI _pokerAI;

        private PatternMatcher _matcher;

        private PatternValue _value;

        private int _currentHost;

        public int GetCurrentHost()
        {
            return _currentHost;
        }
        
        [Test]
        public void TestSelectSuperABCDE()
        {
            _currentHost = PokerNumType.PA;
            
            _value = new PatternValue(GetCurrentHost);
            _matcher = new PatternMatcher(_value);
            _pokerAI = new PokerAI(_matcher, _value);
            
            _pokerAI.SetMyPokers(PokerUtil.ParsePokerList("SQ, SQ, CQ, D2, HA"));
            
            var lastChuPai = _matcher.Match(PokerUtil.ParsePokerList("D7, HA, D9, D0, DJ"));
            var selected = _pokerAI.SelectBiggerXXXXOrSuperABCDE(lastChuPai, 0, true);
            Assert.AreEqual(PokerPattern.NULL, selected);
        }
    }
}