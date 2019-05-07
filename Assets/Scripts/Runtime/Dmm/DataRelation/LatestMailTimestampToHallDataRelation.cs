using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class LatestMailTimestampToHallDataRelation : ChildRelationAdapter<long>
    {
        private readonly IDataContainer<HallData> _parent;

        public LatestMailTimestampToHallDataRelation(IDataContainer<HallData> parent)
        {
            _parent = parent;
        }

        public override long Data
        {
            get
            {
                if (_parent == null)
                {
                    return 0;
                }

                var data = _parent.Read();
                if (data == null)
                {
                    return 0;
                }

                var childData = data.latest_mail_timestamp;
                return childData;
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

                data.latest_mail_timestamp = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}