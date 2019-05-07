using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class InviteDataToInviteDataResultRelation : ChildRelationAdapter<InviteData>
    {
        private readonly IDataContainer<InviteDataResult> _parent;

        public InviteDataToInviteDataResultRelation(IDataContainer<InviteDataResult> parent)
        {
            _parent = parent;
        }

        public override InviteData Data
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

                var childData = data.invite_data;
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

                data.invite_data = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}