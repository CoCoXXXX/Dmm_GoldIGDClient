using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class BRoundEndToPlayingDataRelation : ChildRelationAdapter<BRoundEnd>
    {
        private readonly IDataContainer<PlayingData> _playingDataContainer;

        private BRoundEnd _bRoundEnd;

        public BRoundEndToPlayingDataRelation(IDataContainer<PlayingData> playingDataContainer)
        {
            _playingDataContainer = playingDataContainer;
        }

        public override BRoundEnd Data
        {
            get
            {
                var playingData = _playingDataContainer.Read();

                if (playingData == null)
                {
                    _bRoundEnd = null;
                    return _bRoundEnd;
                }

                var tablePeriod = playingData.period;

                if (tablePeriod != TablePeriod.BetweenRound && tablePeriod != TablePeriod.EndRound)
                {
                    _bRoundEnd = null;
                    return _bRoundEnd;
                }

                return _bRoundEnd;
            }
            set { _bRoundEnd = value; }
        }

        protected override float ParentTimestamp
        {
            get { return 0; }
        }
    }
}