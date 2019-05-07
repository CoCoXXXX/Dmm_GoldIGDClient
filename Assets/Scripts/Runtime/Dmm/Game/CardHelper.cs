using System.Collections.Generic;
using Dmm.PokerLogic;
using UnityEngine;

namespace Dmm.Game
{
    public class CardHelper : MonoBehaviour
    {
        #region Initialize

        private bool _initialized = false;

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// 初始化所有Cache
        /// </summary>
        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            Initialize(_cardCache, CardSpriteList);
            Initialize(_lastChuPaiCardCache, LastChuPaiCardSpriteList);
            Initialize(_endRoundCardCache, EndRoundCardSpriteList);

            _initialized = true;
        }

        #endregion

        #region Card

        public List<Sprite> CardSpriteList;

        private readonly Dictionary<string, Sprite> _cardCache = new Dictionary<string, Sprite>();

        public Sprite GetCard(int numType, int suitType)
        {
            var key = KeyOf(numType, suitType);
            if (_cardCache.ContainsKey(key))
            {
                return _cardCache[key];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region LastChuPaiCard

        public List<Sprite> LastChuPaiCardSpriteList;

        private readonly Dictionary<string, Sprite> _lastChuPaiCardCache = new Dictionary<string, Sprite>();

        public Sprite GetLastChuPaiCard(int numType, int suitType)
        {
            var key = KeyOf(numType, suitType);
            if (_lastChuPaiCardCache.ContainsKey(key))
            {
                return _lastChuPaiCardCache[key];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region EndRoundCard

        public List<Sprite> EndRoundCardSpriteList;

        private readonly Dictionary<string, Sprite> _endRoundCardCache = new Dictionary<string, Sprite>();

        public Sprite GetEndRoundCard(int numType, int suitType)
        {
            var key = KeyOf(numType, suitType);
            if (_endRoundCardCache.ContainsKey(key))
            {
                return _endRoundCardCache[key];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Utility Method

        /// <summary>
        /// 初始载入54张牌的sprite。
        /// </summary>
        private static void Initialize(Dictionary<string, Sprite> cache, List<Sprite> sprites)
        {
            cache.Clear();

            if (sprites == null || sprites.Count <= 0) return;

            for (int i = 2; i <= 14; i++)
            {
                for (int n = 0; n < sprites.Count; n++)
                {
                    var sprite = sprites[n];
                    if (sprite.name == "heart_" + i)
                    {
                        AddSprite(i, PokerSuitType.HEART, cache, sprite);
                    }
                }
            }

            for (int i = 2; i <= 14; i++)
            {
                for (int n = 0; n < sprites.Count; n++)
                {
                    var sprite = sprites[n];
                    if (sprite.name == "diamond_" + i)
                    {
                        AddSprite(i, PokerSuitType.DIAMOND, cache, sprite);
                    }
                }
            }

            for (int i = 2; i <= 14; i++)
            {
                for (int n = 0; n < sprites.Count; n++)
                {
                    var sprite = sprites[n];
                    if (sprite.name == "spade_" + i)
                    {
                        AddSprite(i, PokerSuitType.SPADE, cache, sprite);
                    }
                }
            }

            for (int i = 2; i <= 14; i++)
            {
                for (int n = 0; n < sprites.Count; n++)
                {
                    var sprite = sprites[n];
                    if (sprite.name == "club_" + i)
                    {
                        AddSprite(i, PokerSuitType.CLUB, cache, sprite);
                    }
                }
            }

            for (int n = 0; n < sprites.Count; n++)
            {
                var sprite = sprites[n];
                if (sprite.name == "joker_red")
                {
                    AddSprite(PokerNumType.PD, PokerSuitType.NULL, cache, sprite);
                }
            }

            for (int n = 0; n < sprites.Count; n++)
            {
                var sprite = sprites[n];
                if (sprite.name == "joker_black")
                {
                    AddSprite(PokerNumType.PX, PokerSuitType.NULL, cache, sprite);
                }
            }
        }

        private static void AddSprite(int numType, int suitType, Dictionary<string, Sprite> cache, Sprite sprite)
        {
            if (cache == null)
            {
                return;
            }

            var key = KeyOf(numType, suitType);
            cache[key] = sprite;
        }

        private static string KeyOf(int numType, int suitType)
        {
            return numType + "-" + suitType;
        }

        #endregion
    }
}