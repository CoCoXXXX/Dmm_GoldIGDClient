using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class ActivityConfigListToHallDataRelation : ChildRelationAdapter<List<ActivityConfig>>
    {
        private readonly IDataContainer<HallData> _parentData;

        public ActivityConfigListToHallDataRelation(IDataContainer<HallData> parentData)
        {
            _parentData = parentData;
        }

        public override List<ActivityConfig> Data
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

                return data.activity_config;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _parentData.Timestamp; }
        }
    }
}