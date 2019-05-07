using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class PokerRecorderToPlayingDataRelation : ChildRelationAdapter<List<int>>
    {
        private readonly IDataContainer<PlayingData> _parent;

        public PokerRecorderToPlayingDataRelation(IDataContainer<PlayingData> parent)
        {
            _parent = parent;
        }

        public override List<int> Data
        {
            get
            {
                var playingData = _parent.Read();
                if (playingData == null)
                {
                    return null;
                }

                return playingData.poker_recorder;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}