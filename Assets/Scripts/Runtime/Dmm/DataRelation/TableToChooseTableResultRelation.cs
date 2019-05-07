using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class TableToChooseTableResultRelation : ChildRelationAdapter<Table>
    {
        private readonly IDataContainer<ChooseTableResult> _parent;

        public TableToChooseTableResultRelation(IDataContainer<ChooseTableResult> parent)
        {
            _parent = parent;
        }

        public override Table Data
        {
            get
            {
                var data = _parent.Read();
                if (data == null || data.result != ResultCode.OK)
                {
                    return null;
                }

                return data.table;
            }
            set
            {
                var data = _parent.Read();
                if (data == null)
                {
                    return;
                }

                data.table = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}