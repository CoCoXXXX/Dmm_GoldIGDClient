using System;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class InGameConfigToVersionResultRelation : ChildRelationAdapter<InGameConfig>
    {
        private readonly IDataContainer<VersionResult> _versionResult;

        public InGameConfigToVersionResultRelation(IDataContainer<VersionResult> versionResult)
        {
            _versionResult = versionResult;
        }

        public override InGameConfig Data
        {
            get
            {
                var versionResult = _versionResult.Read();
                if (versionResult == null || versionResult.result != ResultCode.OK)
                {
                    return null;
                }

                return versionResult.in_game_config;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _versionResult.Timestamp; }
        }
    }
}