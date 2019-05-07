using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class InteractionPriceToHallDataRelation : ChildRelationAdapter<Currency>
    {
        private readonly IDataContainer<HallData> _parentData;

        public InteractionPriceToHallDataRelation(IDataContainer<HallData> parentData)
        {
            _parentData = parentData;
        }

        public override Currency Data
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

                var childData = data.interaction_price;
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

                data.interaction_price = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parentData.Timestamp; }
        }
    }
}