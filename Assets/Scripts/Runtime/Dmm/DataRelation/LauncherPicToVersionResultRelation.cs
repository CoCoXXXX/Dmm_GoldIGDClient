using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class LauncherPicToVersionResultRelation : ChildRelationAdapter<string>
    {
        private readonly IDataContainer<VersionResult> _parent;

        public LauncherPicToVersionResultRelation(IDataContainer<VersionResult> parent)
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
                if (data == null)
                {
                    return null;
                }

                if (data.result != ResultCode.OK)
                    return null;

                var items = data.hint_item;
                if (items == null || items.Count <= 0)
                    return null;

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item.pos == 1)
                        return item.outer_pic;
                }

                return null;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}