using System;
using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Log;
using Dmm.Sdk;
using Dmm.Util;
using UnityEngine;
using Zenject;

namespace Dmm.Pay
{
    /// <summary>
    /// 支付管理器。
    /// </summary>
    public class PayManager : MonoBehaviour, IPayManager
    {
        #region Inject

        private AndroidSDK _android;

        private IosSDK _ios;

        private ConfigHolder _config;

        private IDataContainer<User> _myUser;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            AndroidSDK android,
            IosSDK ios,
            ConfigHolder config)
        {
            _android = android;
            _ios = ios;
            _config = config;
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        #endregion

        /// <summary>
        /// 开始支付逻辑。
        /// </summary>
        /// <param name="trade"></param>
        /// <param name="payChannel"></param>
        public void Pay(TradeNoResult trade, int payChannel)
        {
            switch (payChannel)
            {
                case PayChannelType.ALIPAY_IOS:
                    AlipayIOS(trade.extra);
                    break;

                case PayChannelType.ALIPAY_CLIENT:
                    AlipayAndroid(trade.extra);
                    break;

                case PayChannelType.IOS_IAP:
                    IapPay(trade);
                    break;

                case PayChannelType.WEI_XIN:
                    WxPay(trade.extra);
                    break;

                case PayChannelType.XIAOMI:
                    XiaoMiPay(trade);
                    break;
            }
        }

        public float GetPayResultTime(int payChannel)
        {
            switch (payChannel)
            {
                case PayChannelType.ALIPAY_IOS:
                case PayChannelType.ALIPAY_CLIENT:
                    return AlipayResultTime;

                case PayChannelType.WEI_XIN:
                    return WxPayResultTime;

                case PayChannelType.IOS_IAP:
                    return IapPayResultTime;

                case PayChannelType.XIAOMI:
                    return MiPayResultTime;

                default:
                    return 0;
            }
        }

        #region 支付宝

        /// <summary>
        /// 执行IOS支付宝逻辑。
        /// </summary>
        /// <param name="orderStr"></param>
        public void AlipayIOS(string orderStr)
        {
            AlipayResultTime = 0;
            AlipayResultData = null;

            if (string.IsNullOrEmpty(orderStr))
            {
                AlipayResultTime = Time.time;
                AlipayResultData = new AlipayResult(Dmm.Pay.AlipayResult.StatusFail, null, "订单数据发生错误，无法支付");
                return;
            }

#if UNITY_IOS // _ios.Alipay(orderStr, _config.AlipaySchemes);
#endif
        }

        /// <summary>
        /// 执行Android支付逻辑。
        /// </summary>
        /// <param name="orderStr"></param>
        public void AlipayAndroid(string orderStr)
        {
            AlipayResultTime = 0;
            AlipayResultData = null;

            if (string.IsNullOrEmpty(orderStr))
            {
                AlipayResultTime = Time.time;
                AlipayResultData = new AlipayResult(Dmm.Pay.AlipayResult.StatusFail, null, "订单数据发生错误，无法支付");
                return;
            }

#if UNITY_ANDROID
            _android.AlipayAndroid(orderStr);
#endif
        }

        public float AlipayResultTime { get; private set; }

        public AlipayResult AlipayResultData { get; private set; }

        /// <summary>
        /// 接收支付宝支付的结果。
        /// </summary>
        /// <param name="result"></param>
        public void AlipayResult(string result)
        {
            MyLog.InfoWithFrame(name, string.Format("alipay result: {0}", result));
            AlipayResultTime = Time.time;
            AlipayResultData = DataUtil.ParseAlipayResult(result);
        }

        public AlipayResult GetAlipayResultData()
        {
            return AlipayResultData;
        }

        #endregion

        #region 微信支付

        public void WxPay(string order)
        {
            WxPayResultTime = 0;
            WxPayResultData = null;

            if (string.IsNullOrEmpty(order))
            {
                WxPayResultTime = Time.time;
                WxPayResultData = new WxPayResult(Dmm.Pay.WxPayResult.Error, "订单数据发生错误，无法支付");
                return;
            }

#if UNITY_ANDROID
            _android.WxPayAndroid(order);
#endif
#if UNITY_IOS
			_ios.WxPay(order);
#endif
        }

        public float WxPayResultTime { get; private set; }

        public WxPayResult WxPayResultData { get; private set; }

        public WxPayResult GetWxPayResultData()
        {
            return WxPayResultData;
        }

        public void WxPayResult(string result)
        {
            MyLog.InfoWithFrame(name, string.Format("wxpay result: {0}", result));
            WxPayResultTime = Time.time;

            if (string.IsNullOrEmpty(result))
            {
                return;
            }

            WxPayResultData = JsonUtility.FromJson<WxPayResult>(result);
        }

        #endregion

        #region IAP支付

        public void IapPay(TradeNoResult res)
        {
            IapPayResultTime = 0;
            IapPayResultData = null;

            if (res == null) return;

            string productId = null;
            string outTradeNo = null;

            IapProductID iapProductId = null;
            try
            {
                iapProductId = JsonUtility.FromJson<IapProductID>(res.extra);
            }
            catch (Exception e)
            {
            }

            if (iapProductId != null)
            {
                productId = iapProductId.productID;
            }

            var trade = res.trade;
            if (trade != null)
                outTradeNo = trade.out_trade_no;

#if UNITY_IOS
			_ios.IapPay(productId, outTradeNo);
#endif
        }

        public float IapPayResultTime { get; private set; }

        public IapResult IapPayResultData { get; private set; }

        public IapResult GetIapPayResultData()
        {
            return IapPayResultData;
        }

        public void IapPayResult(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            MyLog.InfoWithFrame(name, string.Format("iappay result: {0}", content));
            IapPayResultTime = Time.time;
            IapPayResultData = JsonUtility.FromJson<IapResult>(content);
        }

        #endregion

        #region 小米支付

        public void XiaoMiPay(TradeNoResult res)
        {
            MiPayResultTime = 0;
            MiPayResultData = null;

            if (res == null)
                return;

            var trade = res.trade;
            if (trade == null)
                return;

#if UNITY_ANDROID
            var myUser = _myUser.Read();
            var userName = myUser == null ? null : myUser.username;
            _android.MiPay(trade.out_trade_no, userName, (int) trade.total_fee);
#endif
        }

        public float MiPayResultTime { get; private set; }

        public MiPayResult MiPayResultData { get; private set; }

        public MiPayResult GetMiPayResultData()
        {
            return MiPayResultData;
        }

        public void MiPayResult(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            MyLog.InfoWithFrame(name, string.Format("mipay result: {0}", content));
            MiPayResultTime = Time.time;
            try
            {
                MiPayResultData = JsonUtility.FromJson<MiPayResult>(content);
            }
            catch (Exception e)
            {
            }
        }

        #endregion
    }
}