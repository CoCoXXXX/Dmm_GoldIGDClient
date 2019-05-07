using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class HallDataToHLoginResultRelation : ChildRelationAdapter<HallData>
    {
        private readonly IDataContainer<HLoginResult> _parent;

        public HallDataToHLoginResultRelation(IDataContainer<HLoginResult> parent)
        {
            _parent = parent;
        }

        public override HallData Data
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

                var childData = data.hall_data;
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

                data.hall_data = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}