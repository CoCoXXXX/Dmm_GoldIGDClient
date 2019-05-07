using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class GameTipListToVersionResultRelation : ChildRelationAdapter<List<string>>
    {
        private readonly IDataContainer<VersionResult> _parent;

        public GameTipListToVersionResultRelation(IDataContainer<VersionResult> parent)
        {
            _parent = parent;
        }

        public override List<string> Data
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
                {
                    return null;
                }

                var config = data.game_tip;
                if (config == null)
                {
                    return null;
                }

                return config.tip;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}