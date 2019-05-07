using System.Collections.Generic;
using DG.Tweening;
using Dmm.Log;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Common
{
    public class SystemMsgController : MonoBehaviour, ISystemMsgController
    {
        #region Inject

        private IGameCanvas _gameCanvas;

        [Inject]
        public void Initialize(IGameCanvas gameCanvas)
        {
            _gameCanvas = gameCanvas;

            var msgParent = _gameCanvas.GetSystemMsgParent();
            if (msgParent)
                msgParent.anchoredPosition = new Vector2(0, HidePosition);
        }

        #endregion

        public float HidePosition = 39;

        public float MoveSpeed = 100;

        public Text TextItemPrefab;

        public Color NormalColor = Color.white;

        public Color ErrorColor = Color.red;

        public float ShowTime = 0.2f;

        public float HideTime = 0.2f;

        private Tweener _contentTweener;

        private bool _show;

        private readonly List<Text> _textList = new List<Text>();

        public void Show(string content, bool error = false)
        {
            if (string.IsNullOrEmpty(content))
                return;

            var msgParent = _gameCanvas.GetSystemMsgParent();
            var msgContainer = _gameCanvas.GetSystemMsgContainer();
            if (!msgParent || !msgContainer)
            {
                MyLog.ErrorWithFrame(name, "Missing SystemMsgParent!");
                return;
            }

            if (!TextItemPrefab)
            {
                MyLog.ErrorWithFrame(name, "Missing TextItemPrefab!");
                return;
            }

            var text = Instantiate(TextItemPrefab) as Text;
            if (!text) return;

            text.transform.SetParent(msgContainer, false);
            var y = text.rectTransform.anchoredPosition.y;
            var x = 0f;
            for (int i = 0; i < _textList.Count; i++)
            {
                var t = _textList[i];
                var tx = t.rectTransform.anchoredPosition.x + t.rectTransform.rect.width;
                if (tx + t.rectTransform.rect.width > x)
                    x = tx;
            }

            text.rectTransform.anchoredPosition = new Vector2(x + 1, y);

            text.text = content;
            text.color = error ? ErrorColor : NormalColor;

            _textList.Add(text);

            if (!_show)
            {
                if (_contentTweener != null)
                {
                    _contentTweener.Kill();
                    _contentTweener = null;
                }

                if (msgParent)
                {
                    if (!msgParent.gameObject.activeSelf)
                        msgParent.gameObject.SetActive(true);

                    msgParent.anchoredPosition = new Vector2(0, HidePosition);
                    _contentTweener = msgParent.DOAnchorPos(new Vector2(0, 0), ShowTime);
                }

                _show = true;
            }
        }

        public void Hide()
        {
            _show = false;

            if (_contentTweener != null)
            {
                _contentTweener.Kill();
                _contentTweener = null;
            }

            var msgParent = _gameCanvas.GetSystemMsgParent();
            if (msgParent && msgParent.gameObject.activeSelf)
            {
                _contentTweener = msgParent
                    .DOAnchorPosY(HidePosition, HideTime)
                    .OnComplete(() => msgParent.gameObject.SetActive(false));
            }
        }

        public void Update()
        {
            if (!_show) return;

            if (_textList.Count <= 0)
            {
                Hide();
            }
            else
            {
                for (int i = 0; i < _textList.Count; i++)
                    UpdateTextPosition(_textList[i]);
            }
        }

        public void LateUpdate()
        {
            for (int i = 0; i < _textList.Count; i++)
            {
                var t = _textList[i];
                if (t.rectTransform.anchoredPosition.x <
                    (-_gameCanvas.GetCanvasWidth() - t.rectTransform.rect.width))
                {
                    _textList.RemoveAt(i);

                    Destroy(t.gameObject);

                    i--;
                }
            }
        }

        private void UpdateTextPosition(Text item)
        {
            if (!item) return;

            var pos = item.rectTransform.anchoredPosition;
            item.rectTransform.anchoredPosition = new Vector2(pos.x - (MoveSpeed * Time.deltaTime), pos.y);
        }

        private float TextWidth(Text item)
        {
            if (!item) return 0;

            return item.rectTransform.rect.width;
        }
    }
}