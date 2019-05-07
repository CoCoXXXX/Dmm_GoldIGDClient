using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class PrepaymentListToHallDataRelation : ChildRelationAdapter<List<Prepayment>>
    {
        private readonly IDataContainer<HallData> _parent;

        public PrepaymentListToHallDataRelation(IDataContainer<HallData> parent)
        {
            _parent = parent;
        }

        public override List<Prepayment> Data
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

                var childData = data.prepayment;
                return childData;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}