namespace Dmm.App
{
    public interface IAppController
    {
        bool IsSingleGameMode();
        void GoToSingleGame();
        void ExitSingleGame();

        void ClearAppStateData();
    }

    public enum AppState
    {
        Null,

        // 尚未登录。
        NoLogin,

        // 登陆成功。
        LoginOk,

        // 选房界面。
        ChooseRoom,

        // 在房间内。
        InRoom,

        // 在桌子内。
        InTable,

        // 游戏中。
        Playing,

        // 两局之间。
        BetweenRound
    }
}