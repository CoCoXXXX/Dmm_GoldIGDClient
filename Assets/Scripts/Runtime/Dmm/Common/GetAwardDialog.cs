using System.Collections;
using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Dialog;
using Dmm.Util;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Common
{
    public class GetAwardDialog : MyDialog
    {
        #region Inject

        private CurrencyValue.Factory _currencyValueFactory;

        private AwardMoneyObject.Factory _awardMoneyFactory;

        [Inject]
        public void Initialize(
            CurrencyValue.Factory currencyValueFactory,
            AwardMoneyObject.Factory awardMoneyFactory)
        {
            _currencyValueFactory = currencyValueFactory;
            _awardMoneyFactory = awardMoneyFactory;
        }

        #endregion

        public Text RewardTitle;

        public RectTransform CurrencyContainer;

        public Image Image;

        public Sprite ChargeLogo;

        public Sprite TreasureChestLogo;

        public Sprite GirlLogo;

        #region 弹出奖励物体

        public int GoldEggCount = 10;

        public GameObject GoldEggPrefab;

        public int YinPiaoCount = 10;

        public GameObject YinPiaoPrefab;

        public int YuanBaoCount = 1;

        public GameObject YuanBaoPrefab;

        public int VipCount = 1;

        public GameObject VipPrefab;

        public int CardRecorderCount = 1;

        public GameObject CardRecorderPrefab;

        public int ReCheckinCardCount = 1;

        public GameObject ReCheckinCardPrefab;

        public int GetMaxBornCount(int currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.GOLDEN_EGG:
                    return GoldEggCount;

                case CurrencyType.YIN_PIAO:
                    return YinPiaoCount;

                case CurrencyType.YUAN_BAO:
                    return YuanBaoCount;

                case CurrencyType.VIP:
                    return VipCount;

                case CurrencyType.CARD_RECORDER:
                    return CardRecorderCount;

                case CurrencyType.RECHECKIN_CARD:
                    return ReCheckinCardCount;

                default:
                    return 0;
            }
        }

        public GameObject GetMoneyPrefab(int currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.GOLDEN_EGG:
                    return GoldEggPrefab;

                case CurrencyType.YIN_PIAO:
                    return YinPiaoPrefab;

                case CurrencyType.YUAN_BAO:
                    return YuanBaoPrefab;

                case CurrencyType.VIP:
                    return VipPrefab;

                case CurrencyType.CARD_RECORDER:
                    return CardRecorderPrefab;

                case CurrencyType.RECHECKIN_CARD:
                    return ReCheckinCardPrefab;

                default:
                    return null;
            }
        }

        public Vector2 BornPosition;

        /// <summary>
        /// 播放金蛋和银票效果的时候，执行的物理时间间隔。
        /// </summary>
        public float FixedDeltaTime = 0.01f;

        /// <summary>
        /// 初始化金蛋银票的时候，给的最大速度。
        /// </summary>
        public float InitialMaxSpeed = 10;

        private readonly List<Currency> _data = new List<Currency>();

        private readonly List<CurrencyValue> _currencyList = new List<CurrencyValue>();

        #endregion

        public void OnDisable()
        {
            if (_currencyList.Count > 0)
            {
                for (int i = 0; i < _currencyList.Count; i++)
                {
                    Destroy(_currencyList[i].gameObject);
                }

                _currencyList.Clear();
            }

            // 恢复配置的物理计算间隔。
            Time.fixedDeltaTime = 0.5f;
        }

        public override void AfterShow()
        {
            var remoteAPI = GetRemoteAPI();
            remoteAPI.RequestUserInfo();

            var list = new List<Currency>();
            for (int i = 0; i < _data.Count; i++)
            {
                var c = _data[i];
                var maxBornCount = GetMaxBornCount(c.type);
                if (c.count > 0)
                {
                    var born = new Currency();
                    born.type = c.type;

                    if (maxBornCount > 0)
                        born.count = c.count > maxBornCount ? maxBornCount : c.count;
                    else
                        born.count = c.count;

                    list.Add(born);
                }
            }

            if (list.Count > 0)
            {
                StartCoroutine(GenerateMoneyCoroutine(list));
            }
        }

        private IEnumerator GenerateMoneyCoroutine(List<Currency> currencies)
        {
            if (currencies == null || currencies.Count <= 0)
                yield break;

            var list = new List<Rigidbody2D>();
            for (int i = 0; i < currencies.Count; i++)
            {
                var cur = currencies[i];
                for (int c = 0; c < cur.count; c++)
                {
                    var prefab = GetMoneyPrefab(cur.type);
                    if (prefab != null)
                    {
                        var go = _awardMoneyFactory.Create(prefab);
                        if (go)
                        {
                            go.transform.SetParent(transform, false);
                            go.transform.localPosition = BornPosition;
                            go.transform.SetAsLastSibling();

                            var r = go.GetComponent<Rigidbody2D>();
                            if (r) list.Add(r);
                        }
                    }
                }
            }

            Time.fixedDeltaTime = FixedDeltaTime;

            yield return null;

            var soundController = GetSoundController();
            soundController.PlayGiftPackExplodeSound();

            for (int i = 0; i < list.Count; i++)
            {
                var r = list[i];
                if (!r.gameObject.activeSelf)
                    r.gameObject.SetActive(true);

                r.velocity = Random.insideUnitCircle * InitialMaxSpeed;
            }
        }

        public void ApplyData(Award award)
        {
            ApplyData(award.desc, award.award_type, award.item, award.pic);
        }

        public void ApplyData(string rewardTitle, int type, List<Currency> currencies)
        {
            ApplyData(rewardTitle, type, currencies, null);
        }

        public void ApplyData(string rewardTitle, int type, List<Currency> currencies, string pic)
        {
            // 记录当前的数据。
            _data.Clear();

            if (currencies != null)
                _data.AddRange(currencies);

            if (RewardTitle)
                RewardTitle.text = rewardTitle;

            if (_currencyList.Count > 0)
            {
                for (int i = 0; i < _currencyList.Count; i++)
                    Destroy(_currencyList[i].gameObject);

                _currencyList.Clear();
            }

            if (currencies != null && currencies.Count > 0)
            {
                for (int i = 0; i < currencies.Count; i++)
                {
                    var cur = _currencyValueFactory.Create();
                    if (cur)
                    {
                        cur.transform.SetParent(CurrencyContainer, false);
                        cur.SetCurrency(currencies[i].count, currencies[i].type);
                        _currencyList.Add(cur);
                    }
                }
            }

            var analyticManager = GetAnalyticManager();
            var count = DataUtil.CalculateGeValue(currencies);
            Sprite sprite;
            switch (type)
            {
                case AwardType.Charge:
                    sprite = ChargeLogo;
                    break;

                case AwardType.Exchange:
                    sprite = ChargeLogo;
                    break;

                case AwardType.LoginReward:
                    sprite = ChargeLogo;
                    analyticManager.Bonus(count, AwardType.LoginReward);
                    break;

                case AwardType.TreasureChest:
                    sprite = TreasureChestLogo;
                    analyticManager.Bonus(count, AwardType.TreasureChest);
                    break;

                default:
                    sprite = GirlLogo;
                    analyticManager.Bonus(count, AwardType.Default);
                    break;
            }

            if (Image)
            {
                if (sprite)
                {
                    if (!Image.gameObject.activeSelf)
                        Image.gameObject.SetActive(true);

                    Image.sprite = sprite;
                    Image.SetNativeSize();
                }
                else
                {
                    if (Image.gameObject.activeSelf)
                        Image.gameObject.SetActive(false);
                }
            }
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }

    public class AwardType
    {
        public const int Default = 1;
        public const int LoginReward = 2;
        public const int TreasureChest = 3;
        public const int RoundEndWin = 4;
        public const int Compensation = 5;
        public const int Exchange = 6;
        public const int Charge = 8;
        public const int UserTask = 10;
    }
}