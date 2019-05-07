namespace Dmm.Constant
{
    public class ToastType
    {
        /// <summary>
        /// 普通Toast。
        /// </summary>
        public const int Normal = 0;

        /// <summary>
        /// 普通Toast，但是字体是红色。
        /// </summary>
        public const int Error = 1;

        /// <summary>
        /// 需要玩家关闭的对话框。
        /// 有背景，点击背景也会关闭。
        /// </summary>
        public const int MessageBox = 2;

        /// <summary>
        /// 需要玩家点击确认的对话框。
        /// 有背景，只有点击确认按钮，才会关闭。
        /// </summary>
        public const int ConfirmBox = 3;
    }
}