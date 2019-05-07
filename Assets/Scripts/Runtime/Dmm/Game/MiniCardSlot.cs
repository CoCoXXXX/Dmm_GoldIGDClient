using Dmm.PokerLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Game
{
    [RequireComponent(typeof(RectTransform))]
    public class MiniCardSlot : MonoBehaviour
    {
        public CardHelper CardHelper;

        public Image CardImage;

        public RectTransform RectTransform { get; private set; }

        public void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public Poker Poker
        {
            get { return _poker; }
            set
            {
                _poker = value;
                if (value != null)
                {
                    NumType = value.NumType;
                    SuitType = value.SuitType;

                    if (CardImage)
                    {
                        CardImage.sprite = CardHelper.GetEndRoundCard(NumType, SuitType);
                    }
                }
                else
                {
                    Clear();
                }
            }
        }

        private Poker _poker;

        public int NumType { get; private set; }

        public int SuitType { get; private set; }

        public void Clear()
        {
            _poker = null;
            NumType = PokerNumType.NULL;
            SuitType = PokerNumType.NULL;

            if (CardImage) CardImage.sprite = null;
        }
    }
}