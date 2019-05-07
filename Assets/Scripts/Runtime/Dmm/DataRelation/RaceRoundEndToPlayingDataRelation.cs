using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class RaceRoundEndToPlayingDataRelation : ChildRelationAdapter<com.morln.game.gd.command.RoundEnd>
    {
        private readonly IDataContainer<PlayingData> _playingDataContainer;

        private com.morln.game.gd.command.RoundEnd _raceRoundEnd;

        public RaceRoundEndToPlayingDataRelation(IDataContainer<PlayingData> playingDataContainer)
        {
            _playingDataContainer = playingDataContainer;
        }

        public override com.morln.game.gd.command.RoundEnd Data
        {
            get
            {
                var playingData = _playingDataContainer.Read();

                if (playingData == null)
                {
                    _raceRoundEnd = null;
                    return _raceRoundEnd;
                }

                var tablePeriod = playingData.period;

                if (tablePeriod != TablePeriod.BetweenRound && tablePeriod != TablePeriod.EndRound)
                {
                    _raceRoundEnd = null;
                    return _raceRoundEnd;
                }

                return _raceRoundEnd;
            }
            set { _raceRoundEnd = value; }
        }

        protected override float ParentTimestamp
        {
            get { return 0; }
        }
    }
}