using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class InviteConfigToHallDataRelation : ChildRelationAdapter<InviteConfig>
    {
        private readonly IDataContainer<HallData> _parent;

        public InviteConfigToHallDataRelation(IDataContainer<HallData> parent)
        {
            _parent = parent;
        }

        public override InviteConfig Data
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

                var childData = data.invite_config;
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

                data.invite_config = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}