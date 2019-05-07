using com.morln.game.gd.command;
using Dmm.Util;
using PokerNumType = Dmm.PokerLogic.PokerNumType;

namespace Dmm.Game
{
    public static class HostInfoResultExtension
    {
        public static int GetCurrentHost(this HostInfoResult host)
        {
            if (host == null)
            {
                return PokerNumType.P2;
            }

            var currentHost = host.host_team == 1 ? host.team1_host : host.team2_host;
            return PokerLogicUtil.PokerNumTypeOf(currentHost);
        }

        public static string GetCurrentHostLabel(this HostInfoResult host)
        {
            if (host == null)
            {
                return PokerLogicUtil.LabelOfSessionNumType(com.morln.game.gd.command.PokerNumType.p2);
            }

            var currentHost = host.host_team == 1 ? host.team1_host : host.team2_host;
            return PokerLogicUtil.LabelOfSessionNumType(currentHost);
        }
    }
}