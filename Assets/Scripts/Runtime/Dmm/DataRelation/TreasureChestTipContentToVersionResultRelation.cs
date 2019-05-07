using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class TreasureChestTipContentToVersionResultRelation : ChildRelationAdapter<string>
    {
        private readonly IDataContainer<VersionResult> _parent;

        public TreasureChestTipContentToVersionResultRelation(IDataContainer<VersionResult> parent)
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

                var info = data.treasure_chest_info;
                if (info == null)
                    return null;

                return info.content;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}