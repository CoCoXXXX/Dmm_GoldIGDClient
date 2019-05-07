using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.StateLogic;

namespace Dmm.Game.State
{
    public class RoundEndState : StateAdapter<GameWindow>
    {
        private const string Tag = "RoundEndState";

        private readonly IAppContext _context;

        private readonly IDataContainer<PlayingData> _playingData;

        public RoundEndState(IAppContext context)
        {
            _context = context;
            var dataRepository = context.GetDataRepository();
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        public override int GetStateCode()
        {
            return TablePeriod.EndRound;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        private float _startRoundTime;

        /// <summary>
        /// 回合中间状态不需要强制开启或关闭结算面板
        /// </summary>
        /// <param name="gameWindow"></param>
        /// <param name="time"></param>
        public override void Initialize(GameWindow gameWindow, float time)
        {
            var app = _context.GetAppController();
            gameWindow.ShowLeftPokerCount(true);
            gameWindow.ShowCardLayout(false);
            gameWindow.ShowChuPaiButtonGroup(false);
            gameWindow.HideCardRecorder();
            gameWindow.ShowTopFuncBar(true);
            gameWindow.ShowMatchingTxt(false);
            gameWindow.ShowTableClock(false);
            gameWindow.ShowPokerPeeperGroup(true);
            gameWindow.ShowTempLeave(false);
            gameWindow.ShowLastChuPaiGroup(true);
            gameWindow.ShowJinGongButtonGroup(false);
            gameWindow.ShowJinGongInfoPanel(false);
            gameWindow.ShowReadyGroup(true);
            gameWindow.CardLayout.HideCardJinHuanGongColor(true);
        }

        public override bool Process(GameWindow gameWindow, float time)
        {
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