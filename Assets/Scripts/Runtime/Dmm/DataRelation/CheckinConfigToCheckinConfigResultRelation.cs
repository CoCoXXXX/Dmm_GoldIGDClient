using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class CheckinConfigToCheckinConfigResultRelation : ChildRelationAdapter<CheckinConfig>
    {
        private readonly IDataContainer<CheckinConfigResult> _parentData;

        public CheckinConfigToCheckinConfigResultRelation(IDataContainer<CheckinConfigResult> parentData)
        {
            _parentData = parentData;
        }

        public override CheckinConfig Data
        {
            get
            {
                if (_parentData == null)
                {
                    return null;
                }

                var data = _parentData.Read();
                if (data == null || data.res.code != ResultCode.OK)
                {
                    return null;
                }

                var childData = data.checkin_config;
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

                data.checkin_config = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parentData.Timestamp; }
        }
    }
}