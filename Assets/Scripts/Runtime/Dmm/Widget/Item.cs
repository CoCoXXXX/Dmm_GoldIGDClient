using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Widget
{
    public abstract class Item<T> : MonoBehaviour
    {
        public delegate void OnClick(Item<T> item);

        /// <summary>
        /// 返回当前绑定的数据。
        /// </summary>
        /// <returns></returns>
        public abstract T GetData();

        /// <summary>
        /// 绑定数据。
        /// </summary>
        /// <param name="currentIndex"></param>
        /// <param name="data"></param>
        public abstract void BindData(int currentIndex, T data);

        /// <summary>
        /// 重置。
        /// </summary>
        public abstract void Reset(int currentIndex);

        /// <summary>
        /// 选中状态改变。
        /// </summary>
        /// <param name="selected"></param>
        public abstract void Select(bool selected);

        /// <summary>
        /// 返回Item中被Click的Button组件。
        /// </summary>
        /// <returns></returns>
        public abstract Button GetClickButton();
    }
}