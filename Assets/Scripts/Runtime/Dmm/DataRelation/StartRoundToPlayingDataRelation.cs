using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class StartRoundToPlayingDataRelation : ChildRelationAdapter<StartRound>
    {
        private readonly IDataContainer<PlayingData> _playingDataContainer;

        private StartRound _startRound;

        public StartRoundToPlayingDataRelation(IDataContainer<PlayingData> playingDataContainer)
        {
            _playingDataContainer = playingDataContainer;
        }

        public override StartRound Data
        {
            get
            {
                var palyingData = _playingDataContainer.Read();

                if (palyingData == null)
                {
                    _startRound = null;
                    return _startRound;
                }

                var tablePeriod = palyingData.period;

                if (tablePeriod != TablePeriod.StartRound)
                {
                    _startRound = null;
                    return _startRound;
                }

                return _startRound;
            }
            set { _startRound = value; }
        }

        protected override float ParentTimestamp
        {
            get { return 0; }
        }
    }
}