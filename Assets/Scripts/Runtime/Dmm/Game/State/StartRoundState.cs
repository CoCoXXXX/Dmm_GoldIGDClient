using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Log;
using Dmm.StateLogic;

namespace Dmm.Game.State
{
    public class StartRoundState : StateAdapter<GameWindow>
    {
        private const string Tag = "StartRoundState";

        private readonly IAppContext _context;

        private readonly IDataContainer<PlayingData> _playingData;

        private readonly IDataContainer<Table> _table;

        public StartRoundState(IAppContext context)
        {
            _context = context;

            var dataRepository = _context.GetDataRepository();
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
            _table = dataRepository.GetContainer<Table>(DataKey.CurrentTable);
        }

        public override int GetStateCode()
        {
            return TablePeriod.StartRound;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        private int _gameType;

        public override void Initialize(GameWindow gameWindow, float time)
        {
            var table = _table.Read();
            _gameType = table == null ? TableGameType.NULL : table.game_type;

            gameWindow.ShowCardLayout(true);
            gameWindow.HideRoundEndPanel();
            gameWindow.ShowTopFuncBar(true);
            gameWindow.ShowMatchingTxt(false);
            gameWindow.ShowLeftPokerCount(false);
            gameWindow.ShowTableClock(false);
            gameWindow.ShowPokerPeeperGroup(false);
            gameWindow.ShowTempLeave(false);
            gameWindow.ClearLastChuPaiGroup();
            gameWindow.ShowLastChuPaiGroup(false);
            gameWindow.ShowJinGongButtonGroup(false);
            gameWindow.ShowJinGongInfoPanel(false);
            gameWindow.ShowReadyGroup(false);
            gameWindow.HideTouYouImage();
            gameWindow.CardLayout.HideCardJinHuanGongColor(true);
        }

        public override bool Process(GameWindow gameWindow, float time)
        {
            if (_gameType == TableGameType.TTZ || _gameType == TableGameType.RACE_TTZ)
            {
                gameWindow.RefreshTTZStart();
            }

            gameWindow.CardLayout.RefreshFaPai();

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