using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.StateLogic;

namespace Dmm.Game.State
{
    public class HuanGongState : StateAdapter<GameWindow>
    {
        private const string Tag = "HuanGongState";

        private readonly IDataContainer<PlayingData> _playingData;

        public HuanGongState(IAppContext context)
        {
            var dataRepository = context.GetDataRepository();
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        public override int GetStateCode()
        {
            return TablePeriod.HuanGong;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        //LastChuPaiGroup打开之后自动刷新 JinGongInfoPanel打开之后会自动刷新
        public override void Initialize(GameWindow gameWindow, float time)
        {
            gameWindow.ShowCardLayout(true);
            gameWindow.ShowChuPaiButtonGroup(false);
            gameWindow.HideCardRecorder();
            gameWindow.HideRoundEndPanel();
            gameWindow.ShowTopFuncBar(true);
            gameWindow.ShowMatchingTxt(false);
            gameWindow.ShowLeftPokerCount(false);
            gameWindow.ShowTableClock(true);
            gameWindow.ShowPokerPeeperGroup(false);
            gameWindow.ShowTempLeave(false);
            gameWindow.ClearLastChuPaiGroup();
            gameWindow.ShowLastChuPaiGroup(true);
            gameWindow.ShowJinGongButtonGroup(true);
            gameWindow.ShowJinGongInfoPanel(true);
            gameWindow.ShowReadyGroup(false);
            gameWindow.HideTouYouImage();
        }

        public override bool Process(GameWindow gameWindow, float time)
        {
            gameWindow.JinGongButtonGroup.RefreshContent();

            gameWindow.CardLayout.RefreshTotal();
            gameWindow.CardLayout.RefreshJinHuanGongPoker();

            gameWindow.TableClock.RefreshTotal();

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
            gameWindow.ShowJinGongInfoPanel(false);
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