using DG.Tweening;
using Dmm.PokerLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Game
{
    public class LastCardSlot : MonoBehaviour
    {
        public void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public LastChuPaiGroup Parent;

        public RectTransform RectTransform { get; private set; }

        public Image CardImage;

        public Poker Poker
        {
            get { return _poker; }
            set
            {
                _poker = value;
                if (value != null)
                {
                    if (CardImage)
                    {
                        CardImage.sprite = Parent.GetLastChuPaiSprite(_poker.NumType, _poker.SuitType);
                    }
                }
                else
                {
                    CardImage.sprite = null;
                }
            }
        }

        private Poker _poker;

        public Tweener CurTweener;
    }
}