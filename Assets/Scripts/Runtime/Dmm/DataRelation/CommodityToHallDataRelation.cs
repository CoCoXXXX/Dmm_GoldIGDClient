using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class CommodityToHallDataRelation : ChildRelationAdapter<List<Commodity>>
    {
        private readonly IDataContainer<HallData> _parentData;

        public CommodityToHallDataRelation(IDataContainer<HallData> parentData)
        {
            _parentData = parentData;
        }

        public override List<Commodity> Data
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

                return data.commodity;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _parentData.Timestamp; }
        }
    }
}