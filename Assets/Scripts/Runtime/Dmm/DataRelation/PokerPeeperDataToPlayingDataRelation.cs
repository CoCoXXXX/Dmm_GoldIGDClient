using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Hall;

namespace Dmm.DataRelation
{
    public class PokerPeeperDataToPlayingDataRelation : ChildRelationAdapter<PokerPeeperData>
    {
        private readonly IDataContainer<PlayingData> _playingDataContainer;
        private readonly IDataContainer<TableUserData> _tableUserContainer;

        public PokerPeeperDataToPlayingDataRelation(
            IDataContainer<PlayingData> playingDataContainer,
            IDataContainer<TableUserData> tableUserDataContainer)
        {
            _playingDataContainer = playingDataContainer;
            _tableUserContainer = tableUserDataContainer;
        }

        public override PokerPeeperData Data
        {
            get
            {
                var playingData = _playingDataContainer.Read();
                if (playingData == null)
                {
                    return null;
                }

                if (playingData.period != TablePeriod.ChuPai && playingData.period != TablePeriod.BetweenRound &&
                    playingData.period != TablePeriod.EndRound)
                {
                    return null;
                }

                var tableUser = _tableUserContainer.Read();
                if (tableUser == null)
                {
                    return null;
                }

                var bottom = playingData.PokerPeeperOfSeat(tableUser.SeatOfPosition(SeatPosition.Bottom));
                var top = playingData.PokerPeeperOfSeat(tableUser.SeatOfPosition(SeatPosition.Top));
                var left = playingData.PokerPeeperOfSeat(tableUser.SeatOfPosition(SeatPosition.Left));
                var right = playingData.PokerPeeperOfSeat(tableUser.SeatOfPosition(SeatPosition.Right));

                var res = new PokerPeeperData
                {
                    BottomData = bottom,
                    TopData = top,
                    LeftData = left,
                    RightData = right
                };

                return res;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _playingDataContainer.Timestamp; }
        }
    }
}