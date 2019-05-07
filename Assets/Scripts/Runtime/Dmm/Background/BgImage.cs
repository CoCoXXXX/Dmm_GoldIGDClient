using Dmm.Common;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Background
{
    public class BgImage : MonoBehaviour
    {
        #region Inject

        private IGameCanvas _gameCanvas;

        [Inject]
        public void Initialize(IGameCanvas gameCanvas)
        {
            _gameCanvas = gameCanvas;
        }

        #endregion

        public AsyncImage Target;

        public void Update()
        {
            // 每帧刷新背景的高度。
            var canvasHeight = _gameCanvas.GetCanvasHeight();
            var canvasWidth = _gameCanvas.GetCanvasWidth();

            var bg = Target;
            var bgHeight = bg.ContentHeight;
            var bgWidth = bg.ContentWidth;
            var scaleHeight = canvasHeight / bgHeight;
            var scaleWidth = canvasWidth / bgWidth;
            var scale = Mathf.Max(scaleHeight, scaleWidth);
            bg.transform.localScale = new Vector3(scale, scale, 1);
        }
    }
}