using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.StateLogic;

namespace Dmm.Game.State
{
    public class ChuPaiState : StateAdapter<GameWindow>
    {
        private const string Tag = "ChuPaiState";

        private readonly IDataContainer<PlayingData> _playingData;

        public ChuPaiState(IAppContext context)
        {
            var dataRepository = context.GetDataRepository();
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        public override int GetStateCode()
        {
            return TablePeriod.ChuPai;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        private int _gameType;

        public override void Initialize(GameWindow gameWindow, float time)
        {
            gameWindow.ShowCardLayout(true);
            gameWindow.ShowChuPaiButtonGroup(true);
            gameWindow.AutoShowCardRecorderInChuPaiState();
            gameWindow.HideRoundEndPanel();
            gameWindow.ShowTopFuncBar(true);
            gameWindow.ShowMatchingTxt(false);
            gameWindow.ShowLeftPokerCount(false);
            gameWindow.ShowTableClock(true);
            gameWindow.ShowPokerPeeperGroup(true);
            gameWindow.ShowLastChuPaiGroup(true);
            gameWindow.ClearLastChuPaiGroup();
            gameWindow.ShowJinGongButtonGroup(false);
            gameWindow.ShowJinGongInfoPanel(false);
            gameWindow.ShowReadyGroup(false);
            gameWindow.HideTouYouImage();
            gameWindow.CardRecorder.Reset();
            gameWindow.CardLayout.HideCardJinHuanGongColor(false);
        }

        public override bool Process(GameWindow gameWindow, float time)
        {
            gameWindow.ChuPaiButtonGroup.RefreshContent();

            gameWindow.LastChuPaiGroup.RefreshTotal();

            gameWindow.CardLayout.RefreshTotal();
            gameWindow.CardLayout.RefreshTiShiPattern();

            gameWindow.PokerPeeperGroup.RefreshContent();

            gameWindow.TableClock.RefreshTotal();

            gameWindow.RefreshLeftPokerCount();

            if (gameWindow.IsCardRecorderOpen())
            {
                gameWindow.CardRecorder.RefreshContent();
            }

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
            gameWindow.CardRecorder.Reset();

            var stateResult = new StateResult();
            var playingData = _playingData.Read();
            if (playingData == null)
            {
                stateResult.NextStateCode = Constant.TablePeriod.Waiting;
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