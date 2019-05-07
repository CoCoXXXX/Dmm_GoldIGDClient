using System.Collections.Generic;
using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.Res;
using Dmm.Sound;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Chat
{
    public class ChatBubble : MonoBehaviour
    {
        #region Inject

        private ISoundController _soundController;

        [Inject]
        public void Initialize(ISoundController soundController)
        {
            _soundController = soundController;
        }

        public class Factory : Factory<ChatBubble>
        {
        }

        #endregion

        public float BubbleShowTime = 0.2f;

        public float BubbleStayTime = 3;

        public float BubbleHideTime = 1f;

        public List<Sprite> EmojiList;

        public Text Text;

        public AsyncImage Image;

        public CanvasGroup CanvasGroup;

        public void SetText(string content)
        {
            if (!Text.gameObject.activeSelf)
            {
                Text.gameObject.SetActive(true);
            }

            Text.text = content;

            Image.Reset();
        }

        public void SetEmoji(string emoji)
        {
            Sprite sprite = null;
            if (EmojiList != null)
            {
                for (int i = 0; i < EmojiList.Count; i++)
                {
                    var e = EmojiList[i];
                    if (e.name == emoji)
                    {
                        sprite = e;
                        break;
                    }
                }
            }

            if (sprite)
            {
                if (Text.gameObject.activeSelf)
                {
                    Text.gameObject.SetActive(false);
                }

                Image.SetSprite(sprite, true);
            }
            else
            {
                if (!Text.gameObject.activeSelf)
                {
                    Text.gameObject.SetActive(true);
                }

                Text.text = emoji;

                if (Image.gameObject.activeSelf)
                {
                    Image.gameObject.SetActive(false);
                }
            }
        }

        public void SetJianMeng(JianMengItem jianMeng)
        {
            if (jianMeng == null)
            {
                Image.Reset();
                if (!Text.gameObject.activeSelf)
                {
                    Text.gameObject.SetActive(true);
                }

                Text.text = "#%?~";
                return;
            }

            if (!Text.gameObject.activeSelf)
            {
                Text.gameObject.SetActive(true);
            }

            Text.text = jianMeng.display_msg;

            Image.SetTargetPic(
                jianMeng.pic,
                ResourcePath.JianMeng,
                jianMeng.pic_url,
                true,
                null,
                () =>
                {
                    if (Text.gameObject.activeSelf)
                        Text.gameObject.SetActive(false);
                });
        }

        private Sequence _tweener;

        public void Show()
        {
            _soundController.PlayChatBubbleSound();

            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }

            transform.localScale = new Vector3(0, 0, 1);
            CanvasGroup.alpha = 1;
            _tweener = DOTween.Sequence();
            _tweener.Append(transform.DOScale(new Vector3(1, 1, 1), BubbleShowTime))
                .AppendInterval(BubbleStayTime)
                .Append(CanvasGroup.DOFade(0, BubbleHideTime))
                .OnComplete(() => Destroy(gameObject));
            _tweener.Play();
        }
    }
}