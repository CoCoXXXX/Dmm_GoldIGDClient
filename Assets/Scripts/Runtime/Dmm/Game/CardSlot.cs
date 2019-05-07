using DG.Tweening;
using Dmm.PokerLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Game
{
    /// <summary>
    /// 卡槽。
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CardSlot : MonoBehaviour
    {
        public CardLayout Parent;

        /// <summary>
        /// 当前使用的Tweener。
        /// </summary>
        public Tweener CurTweener;

        /// <summary>
        /// 显示牌的图片。
        /// </summary>
        public Image CardImage;

        /// <summary>
        /// 是否是逢人配。
        /// </summary>
        public Image FrpTag;

        /// <summary>
        /// 牌被选中之后的颜色。
        /// </summary>
        public Color SelectedColor;

        public const int NoJinHuanGong = 0;

        public const int StateJinGong = 1;

        public const int StateHuanGong = 2;

        /// <summary>
        /// 进贡和还贡的时候，牌的颜色。
        /// </summary>
        public Color JinHuanGongColor;

        /// <summary>
        /// 牌正常的颜色。
        /// </summary>
        public Color NormalColor;

        /// <summary>
        /// 卡槽的transform。
        /// </summary>
        public RectTransform RectTransform { get; private set; }

        public void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public bool JinHuanGong
        {
            get { return _jinHuanGong; }
            set
            {
                _jinHuanGong = value;
                Color color;
                if (value)
                {
                    color = Selected ? SelectedColor : JinHuanGongColor;
                }
                else
                {
                    color = Selected ? SelectedColor : NormalColor;
                }

                CardImage.color = color;
                FrpTag.color = color;
            }
        }

        private bool _jinHuanGong;

        /// <summary>
        /// 是否被选中。
        /// </summary>
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;

                Color color;
                if (value)
                {
                    color = SelectedColor;
                }
                else
                {
                    color = JinHuanGong ? JinHuanGongColor : NormalColor;
                }

                CardImage.color = color;
                FrpTag.color = color;
            }
        }

        private bool _selected;

        public bool HeartHost
        {
            get { return _heartHost; }
            set
            {
                _heartHost = value;
                if (FrpTag.gameObject.activeSelf != value)
                {
                    FrpTag.gameObject.SetActive(value);
                }
            }
        }

        private bool _heartHost;

        /// <summary>
        /// 当前牌槽中的牌。
        /// </summary>
        public Poker Poker
        {
            get { return _poker; }
            set
            {
                _poker = value;

                if (value != null)
                {
                    _numType = value.NumType;
                    _suitType = value.SuitType;

                    if (CardImage)
                    {
                        CardImage.sprite = Parent.GetCardSprite(value.NumType, value.SuitType);
                    }
                }
                else
                {
                    Clear();
                }
            }
        }

        private Poker _poker;

        /// <summary>
        /// 当前卡槽中牌的NumType。
        /// </summary>
        public int NumType
        {
            get { return _numType; }
        }

        private int _numType = PokerNumType.NULL;

        /// <summary>
        /// 当前卡槽中的牌的SuitType。
        /// </summary>
        public int SuitType
        {
            get { return _suitType; }
        }

        private int _suitType = PokerSuitType.NULL;

        public int PokerNumber
        {
            get
            {
                if (_poker == null)
                {
                    return -1;
                }

                return _poker.Number;
            }
        }

        /// <summary>
        /// 清空卡槽中的数据。
        /// </summary>
        public void Clear()
        {
            _poker = null;
            _numType = PokerNumType.NULL;
            _suitType = PokerSuitType.NULL;
            _selected = false;
            _jinHuanGong = false;

            CardImage.sprite = null;
            CardImage.color = NormalColor;
        }

        /// <summary>
        /// 卡槽是否是空的。
        /// </summary>
        /// <returns></returns>
        public bool Empty
        {
            get { return _poker == null; }
        }

        public override string ToString()
        {
            if (_poker == null)
            {
                return "Empty";
            }

            return _poker.ToString();
        }
    }
}