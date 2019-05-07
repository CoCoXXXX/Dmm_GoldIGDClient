using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.StateLogic;

namespace Dmm.Game.State
{
    public class JinGongState : StateAdapter<GameWindow>
    {
        private const string Tag = "JinGongState";

        private readonly IDataContainer<int> _tablePeriod;

        private readonly IDataContainer<PlayingData> _playingData;

        public JinGongState(IAppContext context)
        {
            var dataRepository = context.GetDataRepository();
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        public override int GetStateCode()
        {
            return TablePeriod.JinGong;
        }

        public override string GetStateName()
        {
            return Tag;
        }

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

            gameWindow.LastChuPaiGroup.RefreshTotal();
            gameWindow.LastChuPaiGroup.RefreshKangGong();

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