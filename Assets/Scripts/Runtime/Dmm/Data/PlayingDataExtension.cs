using com.morln.game.gd.command;
using Dmm.Hall;
using PokerPattern = com.morln.game.gd.command.PokerPattern;

namespace Dmm.Data
{
    public static class PlayingDataExtension
    {
        public static void ResetAll(this PlayingData playingData)
        {
            if (playingData == null) return;

            // 清空当前的出牌座位。
            playingData.chupai_key_owner_seat = -1;
            playingData.chupai_key = null;

            playingData.left_time = 0;
            playingData.must_chupai = 2;

            playingData.my_pokers = null;

            // 清空剩余牌数。
            playingData.left_count1 = 32;
            playingData.left_count2 = 32;
            playingData.left_count3 = 32;
            playingData.left_count4 = 32;

            // 开局的时候清空出牌记录。
            playingData.last_chupai1 = null;
            playingData.last_chupai2 = null;
            playingData.last_chupai3 = null;
            playingData.last_chupai4 = null;

            playingData.win_level = 0;

            // 清空进贡还贡信息。
            playingData.jg_seat1 = -1;
            playingData.jg_poker1 = -1;
            playingData.jg_dest1 = -1;
            playingData.jg_seat2 = -1;
            playingData.jg_poker2 = -1;
            playingData.jg_dest2 = -1;
            playingData.hg_seat1 = -1;
            playingData.hg_poker1 = -1;
            playingData.hg_dest1 = -1;
            playingData.hg_seat2 = -1;
            playingData.hg_poker2 = -1;
            playingData.hg_dest2 = -1;

            // 重置翻倍数据。
            playingData.fanbei = 1;
        }

        public static int LeftPokerCountOfSeat(this PlayingData playingData, int seat)
        {
            if (playingData == null)
            {
                return 32;
            }

            switch (seat)
            {
                case 0:
                    return playingData.left_count1;
                case 1:
                    return playingData.left_count2;
                case 2:
                    return playingData.left_count3;
                case 3:
                    return playingData.left_count4;
                default:
                    return 32;
            }
        }

        public static PokerPattern LastChuPaiOfSeat(this PlayingData playingData, int seat)
        {
            if (playingData == null)
            {
                return null;
            }

            switch (seat)
            {
                case 0:
                    return playingData.last_chupai1;
                case 1:
                    return playingData.last_chupai2;
                case 2:
                    return playingData.last_chupai3;
                case 3:
                    return playingData.last_chupai4;

                default:
                    return null;
            }
        }

        public static byte[] PokerPeeperOfSeat(this PlayingData playingData, int seat)
        {
            if (playingData == null)
            {
                return null;
            }

            switch (seat)
            {
                case 0:
                    return playingData.poker_peeper1;

                case 1:
                    return playingData.poker_peeper2;

                case 2:
                    return playingData.poker_peeper3;

                case 3:
                    return playingData.poker_peeper4;

                default:
                    return null;
            }
        }
    }
}