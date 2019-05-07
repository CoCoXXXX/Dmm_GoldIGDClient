using Dmm.Log;
using UnityEngine;

namespace Dmm.Base
{
    /// <summary>
    /// 爱掼蛋不像飞机游戏，需要限制
    /// </summary>
    public class MyCamera : MonoBehaviour
    {
        public static MyCamera Instance { get; private set; }

        private Animator _animator;

        public void Awake()
        {
            Instance = this;

            _animator = GetComponent<Animator>();
            if (!_animator)
            {
                MyLog.ErrorWithFrame(name, "No animator attached to camera!");
            }

            InitCamera();
        }

        /// <summary>
        /// 初始化摄像机。
        /// </summary>
        private void InitCamera()
        {
            // 在Awake中调整视口的大小主要是为了早于所有物体的Start调用。

            float windowAspect = (float) Screen.width / (float) Screen.height;
            float scaleHeight = windowAspect / MyScreen.GameCameraAspect;

            var cam = GetComponent<Camera>();
            MyLog.DebugWithFrame(name, "Camera origin aspect " + cam.aspect);

            if (scaleHeight < 1f)
            {
                // 在屏幕的上下两端留黑边。
                Rect rect = cam.rect;

                rect.width = 1f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1f - scaleHeight) / 2;

                cam.rect = rect;
            }
            else
            {
                // 在屏幕的左右两边留黑边。
                var scaleWidth = 1f / scaleHeight;

                var rect = cam.rect;

                rect.width = scaleWidth;
                rect.height = 1f;
                rect.x = (1f - scaleWidth) / 2f;
                rect.y = 0;

                cam.rect = rect;
            }

            MyLog.InfoWithFrame(name, "Set camera viewport to " + cam.rect);
            MyLog.DebugWithFrame(name, "Camera aspect change to " + cam.aspect);

            // 设置MyScreen的参数。
            float screenHeight = 2f * cam.orthographicSize * cam.rect.height;
            // camera的aspect已经根据新的视口参数调整过了，因此，不需要再*camera.rect.width
            float screenWidth = 2f * cam.orthographicSize * cam.aspect;
            MyScreen.Initiate(screenWidth, screenHeight);
        }

        /// <summary>
        /// 屏幕震动。
        /// </summary>
        public void ShakeScreen()
        {
            if (_animator)
            {
                _animator.SetTrigger("Shake");
            }
        }
    }
}