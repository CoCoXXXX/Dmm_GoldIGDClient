using com.morln.game.gd.command;

namespace Dmm.Game
{
    public static class TableExtension
    {
        public static void ResetTable(this Table table)
        {
            if (table == null)
            {
                return;
            }

            table.host_team = 1;
            table.team1_host = PokerNumType.p2;
            table.team2_host = PokerNumType.p2;
            table.round_count = 0;
        }
    }
}