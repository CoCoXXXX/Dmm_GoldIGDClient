using Dmm.Log;
using UnityEngine;

namespace Dmm.Base
{
    public class MyScreen
    {
        public const string Tag = "MyScreen";

        private static bool _initiated = false;

        /// <summary>
        /// 是否已经初始化过屏幕的参数了。
        /// </summary>
        /// <returns></returns>
        public static bool Initiated()
        {
            return _initiated;
        }

        #region 视口参数

        private static float _width;
        private static float _height;

        /// <summary>
        /// 初始化屏幕参数。
        /// </summary>
        /// <param name="screenWidth">屏幕宽度（Unity单位）</param>
        /// <param name="screenHeight">屏幕高度（Unity单位）</param>
        public static void Initiate(float screenWidth, float screenHeight)
        {
            _width = screenWidth;
            _height = screenHeight;

            _initiated = true;
            MyLog.InfoWithFrame(Tag, "Set viewport width: " + screenWidth + " height: " + screenHeight);
        }

        /// <summary>
        /// 视口的宽度。
        /// </summary>
        /// <returns></returns>
        public static float ViewportWidth()
        {
            return _width;
        }

        /// <summary>
        /// 视口的高度。
        /// </summary>
        /// <returns></returns>
        public static float ViewportHeight()
        {
            return _height;
        }

        #endregion

        #region 预定义的游戏参数

        /// <summary>
        /// 屏幕内容区域的宽度。
        /// 按照4/3的比例由高度计算得出。
        /// </summary>
        public const int ContentWidth = 855;

        /// <summary>
        /// 屏幕内容区域的高度。
        /// </summary>
        public const int ContentHeight = 640;

        /// <summary>
        /// 游戏的屏幕宽高比。
        /// </summary>
        public const float GameCameraAspect = 4f / 3f;

        #endregion

        /// <summary>
        /// 是否屏幕内可见。
        /// </summary>
        /// <param name="position">当前位置</param>
        /// <returns></returns>
        public static bool Visible(Vector3 position)
        {
            if (position.x < -_width / 2 ||
                position.x > _width / 2)
            {
                return false;
            }

            if (position.y < -_height / 2 ||
                position.y > _height / 2)
            {
                return false;
            }

            return true;
        }
    }
}