using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class AboutContentToVersionResultRelation : ChildRelationAdapter<string>
    {
        private readonly IDataContainer<VersionResult> _parentData;

        public AboutContentToVersionResultRelation(IDataContainer<VersionResult> parentData)
        {
            _parentData = parentData;
        }

        public override string Data
        {
            get
            {
                if (_parentData == null)
                {
                    return null;
                }

                var data = _parentData.Read();
                if (data == null ||
                    data.result != ResultCode.OK)
                {
                    return null;
                }

                var config = data.in_game_config;
                if (config == null)
                {
                    return null;
                }

                return config.about_content;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _parentData.Timestamp; }
        }
    }
}