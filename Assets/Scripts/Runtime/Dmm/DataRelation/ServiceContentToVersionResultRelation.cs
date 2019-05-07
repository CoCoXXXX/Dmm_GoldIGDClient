using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class ServiceContentToVersionResultRelation : ChildRelationAdapter<string>
    {
        private readonly IDataContainer<VersionResult> _parent;

        public ServiceContentToVersionResultRelation(IDataContainer<VersionResult> parent)
        {
            _parent = parent;
        }

        public override string Data
        {
            get
            {
                if (_parent == null)
                {
                    return null;
                }

                var data = _parent.Read();
                if (data == null ||
                    data.result != ResultCode.OK)
                    return null;

                var config = data.in_game_config;
                if (config == null)
                    return null;

                return config.service_content;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}