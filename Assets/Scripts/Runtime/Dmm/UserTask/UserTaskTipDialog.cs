using System.Collections;
using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.Dialog;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.UserTask
{
    public class UserTaskTipDialog : MyDialog
    {
        #region Inject

        private IDialogManager _dialogManager;

        [Inject]
        private void Initialize(IDialogManager dialogManager)
        {
            _dialogManager = dialogManager;
        }

        #endregion

        public Text Title;

        public Text Description;

        public float ShowTime = 0.5f;

        public float StayTime = 4f;

        public float HideTime = 0.5f;

        private Sequence _tweener;

        private UserTaskTip _data;

        public RectTransform DialogContent;

        public void ApplyData(UserTaskTip data)
        {
            _data = data;
        }

        public override void Show()
        {
            StartCoroutine(Showcoroutine(_data));
        }

        private IEnumerator Showcoroutine(UserTaskTip data)
        {
            _data = data;

            if (data == null)
            {
                Destroy(gameObject);
                yield break;
            }

            var title = data.title;
            var description = data.condition_description;

            Title.text = title;
            Description.text = description;

            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }

            Canvas.ForceUpdateCanvases();

            yield return null;

            var height = DialogContent.sizeDelta.y;
            DialogContent.anchoredPosition = new Vector2(0, height);
            _tweener = DOTween.Sequence();
            _tweener.Append(DialogContent.DOAnchorPos(Vector2.zero, ShowTime))
                .AppendInterval(StayTime)
                .Append(DialogContent.DOAnchorPos(new Vector2(0, height), HideTime))
                .OnComplete(() => Destroy(gameObject));

            _tweener.Play();
        }

        public void ShowUserTaskDialog()
        {
            _dialogManager.ShowUserTaskDialog(GetContext());
        }
    }
}