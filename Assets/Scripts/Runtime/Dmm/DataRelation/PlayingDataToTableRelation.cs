using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class PlayingDataToTableRelation : ChildRelationAdapter<PlayingData>
    {
        private readonly IDataContainer<Table> _parent;

        public PlayingDataToTableRelation(IDataContainer<Table> parent)
        {
            _parent = parent;
        }

        public override PlayingData Data
        {
            get
            {
                if (_parent == null)
                {
                    return null;
                }

                var data = _parent.Read();
                if (data == null)
                {
                    return null;
                }

                return data.playing_data;
            }
            set
            {
                if (_parent == null)
                {
                    return;
                }

                var data = _parent.Read();
                if (data == null)
                {
                    return;
                }

                data.playing_data = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}