using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.StateLogic;

namespace Dmm.Game.State
{
    public class MatchingState : StateAdapter<GameWindow>
    {
        private const string Tag = "MatchingState";

        private readonly IDataContainer<PlayingData> _playingData;

        public MatchingState(IAppContext context)
        {
            var dataRepository = context.GetDataRepository();
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        public override int GetStateCode()
        {
            return TablePeriod.Matching;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        private float _startTime;

        public override void Initialize(GameWindow gameWindow, float time)
        {
            _startTime = time;

            gameWindow.HideRoundEndPanel();
            gameWindow.ShowTopFuncBar(false);
            gameWindow.ShowLeftPokerCount(false);
            gameWindow.ShowMatchingTxt(true);
            gameWindow.ShowTableClock(false);
            gameWindow.ShowReadyGroup(false);
            gameWindow.ShowPokerPeeperGroup(false);
            gameWindow.ClearLastChuPaiGroup();
            gameWindow.ShowLastChuPaiGroup(false);
            gameWindow.ShowJinGongButtonGroup(false);
            gameWindow.ShowJinGongInfoPanel(false);
            gameWindow.HideTouYouImage();
            gameWindow.CardLayout.HideCardJinHuanGongColor(true);
        }

        public override bool Process(GameWindow gameWindow, float time)
        {
            gameWindow.RefreshMatchingSeconds(_startTime, time);

            var playingData = _playingData.Read();
            if (playingData == null)
            {
                return true;
            }

            var state = playingData.period;
            return state != GetStateCode();
        }

        public override StateResult Finish(GameWindow gameWindow, float time)
        {
            gameWindow.ShowMatchingTxt(false);
            gameWindow.ShowTopFuncBar(true);

            var stateResult = new StateResult();
            var playingData = _playingData.Read();
            if (playingData == null)
            {
                stateResult.NextStateCode = TablePeriod.Waiting;
                stateResult.Result = StateResult.Error;
                return stateResult;
            }

            var state = playingData.period;
            stateResult.NextStateCode = state;
            stateResult.Result = StateResult.Ok;
            return stateResult;
        }
    }
}