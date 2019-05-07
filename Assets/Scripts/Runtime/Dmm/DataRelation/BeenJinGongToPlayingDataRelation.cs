using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class BeenJinGongToPlayingDataRelation : ChildRelationAdapter<BeenJinGong>
    {
        private readonly IDataContainer<PlayingData> _playingData;

        public BeenJinGongToPlayingDataRelation(IDataContainer<PlayingData> playingData)
        {
            _playingData = playingData;
        }

        private BeenJinGong _data;

        public override BeenJinGong Data
        {
            get
            {
                var playingData = _playingData.Read();
                if (playingData == null)
                {
                    _data = null;
                    return null;
                }

                var period = playingData.period;
                if (period != TablePeriod.JinGong && period != TablePeriod.HuanGong)
                {
                    _data = null;
                    return null;
                }

                return _data;
            }
            set { _data = value; }
        }

        protected override float ParentTimestamp
        {
            get { return 0; }
        }
    }
}