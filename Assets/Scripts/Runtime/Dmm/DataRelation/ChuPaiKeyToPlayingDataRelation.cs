using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class ChuPaiKeyToPlayingDataRelation : ChildRelationAdapter<ChuPaiKey>
    {
        private readonly IDataContainer<PlayingData> _parent;

        private ChuPaiKey _data;

        public ChuPaiKeyToPlayingDataRelation(IDataContainer<PlayingData> parent)
        {
            _parent = parent;
        }

        public override ChuPaiKey Data
        {
            get
            {
                var playingDdata = _parent.Read();
                if (playingDdata == null || playingDdata.period != TablePeriod.ChuPai)
                {
                    _data = null;
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