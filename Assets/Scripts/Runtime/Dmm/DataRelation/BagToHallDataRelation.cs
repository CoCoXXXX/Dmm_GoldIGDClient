using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class BagToHallDataRelation : ChildRelationAdapter<Bag>
    {
        private readonly IDataContainer<HallData> _parentData;

        public BagToHallDataRelation(IDataContainer<HallData> parentData)
        {
            _parentData = parentData;
        }

        public override Bag Data
        {
            get
            {
                if (_parentData == null)
                {
                    return null;
                }

                var data = _parentData.Read();
                if (data == null)
                {
                    return null;
                }

                var childData = data.bag;
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

                data.bag = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parentData.Timestamp; }
        }
    }
}