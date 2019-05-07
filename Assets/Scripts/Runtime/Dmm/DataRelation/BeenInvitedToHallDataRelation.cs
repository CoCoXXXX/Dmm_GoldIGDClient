using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class BeenInvitedToHallDataRelation : ChildRelationAdapter<bool>
    {
        private readonly IDataContainer<HallData> _parentData;

        public BeenInvitedToHallDataRelation(IDataContainer<HallData> parentData)
        {
            _parentData = parentData;
        }

        public override bool Data
        {
            get
            {
                if (_parentData == null)
                {
                    return false;
                }

                var data = _parentData.Read();
                if (data == null)
                {
                    return false;
                }

                var childData = data.been_invited;
                return childData;
            }
            set
            {
                if (_parentData == null)
                {
                    return;
                }

                var data = _parentData.Read();
                if (data == null)
                {
                    return;
                }

                data.been_invited = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parentData.Timestamp; }
        }
    }
}