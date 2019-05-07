using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class HostInfoToTableRelation : ChildRelationAdapter<HostInfoResult>
    {
        private readonly IDataContainer<Table> _tableContainer;

        public HostInfoToTableRelation(IDataContainer<Table> tableContainer)
        {
            _tableContainer = tableContainer;
        }

        public override HostInfoResult Data
        {
            get
            {
                var table = _tableContainer.Read();
                if (table == null)
                {
                    return null;
                }

                var hostInfoResult = new HostInfoResult();
                hostInfoResult.host_team = table.host_team;
                hostInfoResult.team1_host = table.team1_host;
                hostInfoResult.team2_host = table.team2_host;

                return hostInfoResult;
            }
            set
            {
                var table = _tableContainer.Read();
                if (table == null)
                {
                    return;
                }

                table.host_team = value.host_team;
                table.team1_host = value.team1_host;
                table.team2_host = value.team2_host;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _tableContainer.Timestamp; }
        }
    }
}