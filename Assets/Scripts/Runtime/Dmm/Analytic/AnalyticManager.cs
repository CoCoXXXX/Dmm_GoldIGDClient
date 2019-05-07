using System.Collections.Generic;
using Dmm.Constant;
using Dmm.Data;
using Dmm.Log;
using Dmm.Sdk;
using Umeng;
using Zenject;

namespace Dmm.Analytic
{
    public class AnalyticManager : IAnalyticManager
    {
        #region Inject

        private ConfigHolder _configHolder;

        [Inject]
        public void Initialize(
            IosSDK ios,
            ConfigHolder configHolder,
            AndroidSDK android)
        {
            _configHolder = configHolder;
        }

        #endregion

        /// <summary>
        /// 需要在应用程序开始的时候调用。
        /// 也就是在AppController的Awake当中调用。
        /// </summary>
        public void Init()
        {
            var umAppKey = _configHolder.UmAppKey;
            var saleChannel = _configHolder.SaleChannel;
            GA.StartWithAppKeyAndChannelId(umAppKey, saleChannel);
            //调试时开启日志 发布时设置为false
            GA.SetLogEnabled(false);
            MyLog.InfoWithFrame("AnalyticManager", "Init  umAppKey ==" + umAppKey + "  saleChannel == " + saleChannel);
        }

        public void StartLevel(string level)
        {
            GA.StartLevel(level);
        }

        public void FailLevel(string level)
        {
            GA.FailLevel(level);
        }

        public void FinishLevel(string level)
        {
            GA.FinishLevel(level);
        }

        public void Pay(float money, float coin, int payChannel)
        {
            GA.Pay(money, GetSourceOfPayChannel(payChannel), coin);
        }

        private static int GetSourceOfPayChannel(int payChannel)
        {
            switch (payChannel)
            {
                case PayChannelType.ALIPAY_CLIENT:
                    return 22;

                case PayChannelType.ALIPAY_IOS:
                    return 26;

                case PayChannelType.IOS_IAP:
                    return 23;

                case PayChannelType.WEI_XIN:
                    return 88;

                default:
                    return 99;
            }
        }

        public void Buy(string item, int number, float price)
        {
            GA.Buy(item, number, price);
        }

        public void Use(string item, int number, float price)
        {
            GA.Use(item, number, price);
        }

        public void Bonus(float price, int type)
        {
            GA.Bonus(price, (GA.BonusSource) type);
        }

        public void Bonus(string item, int number, float price, int trigger)
        {
            GA.Bonus(item, number, price, (GA.BonusSource) trigger);
        }

        public void SignIn(string provider, string id)
        {
            GA.ProfileSignIn(provider, id);
        }

        public void SignIn(string id)
        {
            GA.ProfileSignIn(id);
        }

        public void SignOff()
        {
            GA.ProfileSignOff();
        }

        public void Event(string eventId)
        {
            GA.Event(eventId);
        }

        public void Event(string eventId, Dictionary<string, string> attrs)
        {
            GA.Event(eventId, attrs);
        }

        public void EventValue(string eventId, Dictionary<string, string> attrs, int value)
        {
            GA.Event(eventId, attrs, value);
        }

        public void PageStart(string page)
        {
            GA.PageBegin(page);
        }

        public void PageEnd(string page)
        {
            GA.PageEnd(page);
        }
    }
}