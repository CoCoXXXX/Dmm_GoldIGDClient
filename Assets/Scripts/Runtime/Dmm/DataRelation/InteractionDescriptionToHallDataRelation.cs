using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class InteractionDescriptionToHallDataRelation : ChildRelationAdapter<string>
    {
        private readonly IDataContainer<HallData> _parent;

        public InteractionDescriptionToHallDataRelation(IDataContainer<HallData> parent)
        {
            _parent = parent;
        }

        public override string Data
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

                var childData = data.interaction_description;
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

                data.interaction_description = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}