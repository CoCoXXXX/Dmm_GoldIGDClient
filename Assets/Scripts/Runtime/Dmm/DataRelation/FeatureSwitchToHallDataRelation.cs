using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class FeatureSwitchToHallDataRelation : ChildRelationAdapter<FeatureSwitch>
    {
        private readonly IDataContainer<HallData> _parent;

        public FeatureSwitchToHallDataRelation(IDataContainer<HallData> parent)
        {
            _parent = parent;
        }

        public override FeatureSwitch Data
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

                var childData = data.feature_switch;
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

                data.feature_switch = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}