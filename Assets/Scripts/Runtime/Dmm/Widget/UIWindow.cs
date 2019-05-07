using UnityEngine;

namespace Dmm.Widget
{
    /// <summary>
    /// 所有窗口的基类，主要提供开启和关闭窗口的功能。
    /// </summary>
    public abstract class UIWindow : MonoBehaviour
    {
        #region 显示隐藏

        /// <summary>
        /// 显示界面。
        /// </summary>
        public abstract void Show();

        /// <summary>
        /// 隐藏界面。 /// </summary>
        public abstract void Hide();

        #endregion
    }
}