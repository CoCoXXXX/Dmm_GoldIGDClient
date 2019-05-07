using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class ExchangeListToHallDataRelation : ChildRelationAdapter<List<Exchange>>
    {
        private readonly IDataContainer<HallData> _rootData;

        public ExchangeListToHallDataRelation(IDataContainer<HallData> rootData)
        {
            _rootData = rootData;
        }

        public override List<Exchange> Data
        {
            get
            {
                if (_rootData == null)
                {
                    return null;
                }

                var data = _rootData.Read();
                if (data == null)
                {
                    return null;
                }

                var childData = data.exchange;
                return childData;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _rootData.Timestamp; }
        }
    }
}