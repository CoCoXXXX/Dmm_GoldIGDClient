using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class VipConfigListToHallDataRelation : ChildRelationAdapter<List<VipConfig>>
    {
        private readonly IDataContainer<HallData> _parent;

        public VipConfigListToHallDataRelation(IDataContainer<HallData> parent)
        {
            _parent = parent;
        }

        public override List<VipConfig> Data
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

                var childData = data.vip_config;
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