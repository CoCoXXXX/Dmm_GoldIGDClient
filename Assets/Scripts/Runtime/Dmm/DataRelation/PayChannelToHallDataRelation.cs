using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class PayChannelToHallDataRelation : ChildRelationAdapter<List<PayChannel>>
    {
        private readonly IDataContainer<HallData> _parent;

        public PayChannelToHallDataRelation(IDataContainer<HallData> parent)
        {
            _parent = parent;
        }

        public override List<PayChannel> Data
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

                var childData = data.pay_channel;
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