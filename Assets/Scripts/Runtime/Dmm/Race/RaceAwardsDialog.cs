using System.Collections;
using System.Collections.Generic;
using System.IO;
using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.Common;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Res;
using Dmm.WeChat;
using Dmm.Widget;
using Dmm.ZXing;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Race
{
    public class RaceAwardsDialog : MyDialog
    {
        #region Inject

        private CurrencyValue.Factory _currencyValueFactory;

        private IWeChatManager _wechatManager;

        private IAnalyticManager _analyticManager;

        private IDataContainer<Queue<RaceAward>> _raceAwardQueue;

        [Inject]
        public void Initialize(
            CurrencyValue.Factory currencyValueFactory,
            IWeChatManager weChatManager,
            IDataRepository dataRepository,
            IAnalyticManager analyticManager)
        {
            _currencyValueFactory = currencyValueFactory;
            _wechatManager = weChatManager;
            _analyticManager = analyticManager;
            _raceAwardQueue = dataRepository.GetContainer<Queue<RaceAward>>(DataKey.RaceAwardQueue);
        }

        #endregion

        #region 组件

        public GameObject AwardContent;

        public GameObject ShareContent;

        public Text RankTxt;

        public Text AwardHongbaoTxt;

        public GameObject AwardCurrency;

        public CurrencyValue AwardCurrencyValue;

        public Text RaceNameTxt;

        public Text HelpDescriptionTxt;

        public Text ShareRaceNameTxt;

        public Text ShareRankTxt;

        public Text ShareAwardHongbaoTxt;

        public GameObject ShareAwardCurrency;

        public CurrencyValue ShareAwardCurrencyValue;

        public Text ShareNickname;

        public Image ShareQrCodeImage;

        public GameObject QrCodeImageBg;

        public AsyncImage HeadIcon;

        #endregion

        public override void BeforeShow()
        {
            if (!AwardContent.activeSelf)
            {
                AwardContent.SetActive(true);
            }
            if (ShareContent.activeSelf)
            {
                ShareContent.SetActive(false);
            }
        }

        private readonly List<CurrencyValue> _currencyList = new List<CurrencyValue>();

        private readonly List<Currency> _currency = new List<Currency>();

        public void Apply(RaceAward msg)
        {
            var raceAward = msg;
            if (raceAward == null)
            {
                Reset();
                return;
            }
            RankTxt.text = "" + raceAward.race_rank;
            ShareRankTxt.text = "" + raceAward.race_rank;
            RaceNameTxt.text = raceAward.race_name;
            ShareRaceNameTxt.text = raceAward.race_name;
            HelpDescriptionTxt.text = raceAward.help;

            if (string.IsNullOrEmpty(raceAward.wx_share_url))
            {
                QrCodeImageBg.SetActive(false);
            }
            else
            {
                var sprite = GenerateQRCode.GenerateQRCodeSpriteFromUrl(raceAward.wx_share_url);
                if (sprite == null)
                {
                    QrCodeImageBg.SetActive(false);
                }
                else
                {
                    ShareQrCodeImage.sprite = sprite;
                    QrCodeImageBg.SetActive(true);
                }
            }

            if (string.IsNullOrEmpty(raceAward.description))
            {
                AwardHongbaoTxt.gameObject.SetActive(false);
                ShareAwardHongbaoTxt.gameObject.SetActive(false);
            }
            else
            {
                AwardHongbaoTxt.text = raceAward.description;
                ShareAwardHongbaoTxt.text = raceAward.description;
                AwardHongbaoTxt.gameObject.SetActive(true);
                ShareAwardHongbaoTxt.gameObject.SetActive(true);
            }

            if (_currencyList.Count > 0)
            {
                for (var i = 0; i < _currencyList.Count; i++)
                    Destroy(_currencyList[i].gameObject);

                _currencyList.Clear();
            }

            var currencys = raceAward.currency;
            _currency.Clear();

            if (currencys == null || currencys.Count == 0)
            {
                AwardCurrency.SetActive(false);
                ShareAwardCurrency.SetActive(false);
            }
            else
            {
                _currency.AddRange(currencys);
                for (var i = 0; i < currencys.Count; i++)
                {
                    var cur = _currencyValueFactory.Create();
                    var shareCur = _currencyValueFactory.Create();
                    if (cur)
                    {
                        cur.transform.SetParent(AwardCurrency.transform, false);
                        cur.SetCurrency(currencys[i].count, currencys[i].type);
                        cur.AmountTxt.color = Color.yellow;
                        _currencyList.Add(cur);
                    }
                    if (shareCur)
                    {
                        shareCur.transform.SetParent(ShareAwardCurrency.transform, false);
                        shareCur.SetCurrency(currencys[i].count, currencys[i].type);
                        shareCur.AmountTxt.color = Color.yellow;
                        _currencyList.Add(shareCur);
                    }
                }
                AwardCurrency.SetActive(true);
                ShareAwardCurrency.SetActive(true);
            }

            var user = raceAward.user;
            if (user == null)
            {
                HeadIcon.Reset();
                ShareNickname.text = "";
            }
            else
            {
                ShareNickname.text = user.nickname;
                if (string.IsNullOrEmpty(user.headicon_url))
                {
                    HeadIcon.gameObject.SetActive(false);
                }
                else
                {
                    HeadIcon.SetTargetPic(HeadIconPic(user), null, user.headicon_url);
                }
            }
        }

        public string HeadIconPic(User user)
        {
            if (user == null || string.IsNullOrEmpty(user.headicon_url))
            {
                return "default";
            }

            return string.Format("headicon-{0}", WWW.EscapeURL(user.headicon_url));
        }

        public void Confirm()
        {
            Hide();
            ShowGetAwardDialog();
        }

        public void Share()
        {
            if (AwardContent.activeSelf)
            {
                AwardContent.SetActive(false);
            }
            if (!ShareContent.activeSelf)
            {
                ShareContent.SetActive(true);
            }
            WxShare();
        }

        private bool _sharing = false;

        public void WxShare()
        {
            if (_sharing) return;

            _sharing = true;
            StartCoroutine(ShareCoroutine());
        }

        private IEnumerator ShareCoroutine()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            var path = "ScreenShot.png";
#else
			var path = Application.persistentDataPath + "/ScreenShot.png";
#endif

            if (File.Exists(path)) File.Delete(path);

            ScreenCapture.CaptureScreenshot("ScreenShot.png");

            while (!File.Exists(path))
            {
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.5f);

            _wechatManager.WxCircle(null, null, path, null, null, null,
                null);
            _analyticManager.Event("raceAward_wx_share");

            _sharing = false;
            Hide();
            ShowGetAwardDialog();
        }

        public override void AfterHide()
        {
            if (_currencyList.Count > 0)
            {
                for (int i = 0; i < _currencyList.Count; i++)
                    Destroy(_currencyList[i].gameObject);

                _currencyList.Clear();
            }

            var raceAwardQueue = _raceAwardQueue.Read();

            var awardCount = raceAwardQueue == null ? 0 : raceAwardQueue.Count;
            if (awardCount <= 0)
            {
                Destroy(gameObject);
                return;
            }

            var raceAward = raceAwardQueue == null ? null : raceAwardQueue.Dequeue();
            if (raceAward == null)
            {
                Destroy(gameObject);
                return;
            }
            Apply(raceAward);
            Show();
        }

        public void Reset()
        {
            RankTxt.text = "";
            AwardHongbaoTxt.text = "";
            RaceNameTxt.text = "";
            HelpDescriptionTxt.text = "";
            ShareRaceNameTxt.text = "";
            ShareRankTxt.text = "";
            ShareAwardHongbaoTxt.text = "";
            ShareNickname.text = "";
            HeadIcon.Reset();
            AwardCurrencyValue.Clear();
        }

        private void ShowGetAwardDialog()
        {
            if (_currency.Count <= 0)
            {
                return;
            }
            var dialogManager = GetDialogManager();
            dialogManager.ShowDialog<GetAwardDialog>(DialogName.GetAwardDialog, false, false,
                (dialog) =>
                {
                    dialog.ApplyData(
                        "恭喜您获得比赛奖励！",
                        AwardType.TreasureChest,
                        _currency);
                    dialog.Show();
                });
        }
    }
}