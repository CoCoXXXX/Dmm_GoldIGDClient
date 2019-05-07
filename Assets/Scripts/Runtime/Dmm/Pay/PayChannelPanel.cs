using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Msg;
using Dmm.Task;
using Dmm.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Pay
{
    public class PayChannelPanel : MyDialog
    {
        public Text PayContent;

        public Text PayPrice;

        public Text Description;

        public PayChannelList PayChannelList;

        public GameObject StateGroup;

        public Text StateTxt;

        public Image WaitingImg;

        public Text ServiceQQ;

        /// <summary>
        /// 向服务器发出请求后，多长时间认为是Timeout。
        /// </summary>
        public float ServerRequestTimeout = 10;

        private IDataContainer<string> _serviceQQ;

        private IDataContainer<TradeNoResult> _tradeNoResult;

        private IDataContainer<CheckTradeResult> _checkTradeResult;

        private IDataContainer<FeatureSwitch> _featureSwitch;

        public string Reason
        {
            get { return string.IsNullOrEmpty(_reason) ? "def" : _reason; }
        }

        private string _reason;

        private Prepayment _data;

        private int _selectedPayChannel;

        public void OnEnable()
        {
            _serviceQQ = GetContainer<string>(DataKey.ServiceQQ);
            _tradeNoResult = GetContainer<TradeNoResult>(DataKey.TradeNoResult);
            _checkTradeResult = GetContainer<CheckTradeResult>(DataKey.CheckTradeResult);
            _featureSwitch = GetContainer<FeatureSwitch>(DataKey.FeatureSwitch);

            SetServiceQQ();
        }

        private void SetServiceQQ()
        {
            var serviceQQ = _serviceQQ.Read();
            ServiceQQ.text = "客服QQ：" + serviceQQ;
        }

        public void ApplyData(string reason, Prepayment prepayment)
        {
            _reason = reason;
            _data = prepayment;

            if (prepayment == null)
                return;

            if (PayContent)
            {
                if (!PayContent.gameObject.activeSelf)
                    PayContent.gameObject.SetActive(true);

                var content = !string.IsNullOrEmpty(prepayment.display_name)
                    ? "购买：" + prepayment.display_name
                    : "";

                PayContent.text = content;
            }

            if (PayPrice)
            {
                if (!PayPrice.gameObject.activeSelf)
                    PayPrice.gameObject.SetActive(true);

                PayPrice.text = "价格：¥ " + prepayment.price;
            }

            if (Description)
            {
                if (!string.IsNullOrEmpty(prepayment.description))
                {
                    if (!Description.gameObject.activeSelf)
                        Description.gameObject.SetActive(true);

                    Description.text = prepayment.description;
                }
                else
                {
                    if (Description.gameObject.activeSelf)
                        Description.gameObject.SetActive(false);
                }
            }

            AnalyticPanelShow();

            // 不再在客户端对支付渠道进行过滤。
            var channelList = prepayment.pay_channel;

            // 因为releaseConfig本身就是针对各个不同渠道特殊性而设计的。
            // 所以也应该是根据特殊的情况来进行过滤。

            var featureSwitch = _featureSwitch.Read();
            var extraCharge = featureSwitch != null && featureSwitch.extra_charge;
#if UNITY_IOS
            if (featureSwitch == null || !extraCharge)
            {
                if (channelList != null && channelList.Count > 0)
                {
                    for (int i = 0; i < channelList.Count; i++)
                    {
                        if (channelList[i] != PayChannelType.IOS_IAP)
                        {
                            channelList.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
#endif

#if UNITY_ANDROID
            var configHolder = GetConfigHolder();
            if (configHolder.XiaoMiMode && !extraCharge)
            {
                if (channelList != null && channelList.Count > 0)
                {
                    for (int i = 0; i < channelList.Count; i++)
                    {
                        if (channelList[i] != PayChannelType.XIAOMI)
                        {
                            channelList.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
#endif

            if (channelList == null || channelList.Count <= 0)
            {
                // 数据错误，关闭支付列表，显示一个错误的提示。
                EnableComponent(false, false, true);
                SetStateTxt("数据发生错误，无法支付！\n请联系下方客服QQ解决");
            }
            else if (channelList.Count <= 1)
            {
                // 如果只有一种支付方式则直接开始支付。
                StartPay(channelList[0]);
            }
            else
            {
                // 多于一种支付方式，则显示支付列表。
                EnableComponent(true, false, false);
                if (PayChannelList)
                    PayChannelList.ApplyData(channelList);
            }
        }

        private void EnableComponent(bool payChannelList, bool waiting, bool stateTxt)
        {
            if (PayChannelList && PayChannelList.gameObject.activeSelf != payChannelList)
                PayChannelList.gameObject.SetActive(payChannelList);

            var state = waiting || stateTxt;
            if (StateGroup && StateGroup.activeSelf != state)
                StateGroup.SetActive(state);

            if (WaitingImg && WaitingImg.gameObject.activeSelf != waiting)
                WaitingImg.gameObject.SetActive(waiting);

            if (StateTxt && StateTxt.gameObject.activeSelf != stateTxt)
                StateTxt.gameObject.SetActive(stateTxt);
        }

        private void SetStateTxt(string content)
        {
            if (!StateTxt) return;

            if (string.IsNullOrEmpty(content))
            {
                if (StateTxt.gameObject.activeSelf)
                    StateTxt.gameObject.SetActive(false);
                return;
            }

            StateTxt.text = content;
        }

        public void Update()
        {
            UpdateWaitingImg();
            UpdatePayLogic();
        }

        private void UpdateWaitingImg()
        {
            if (!StateGroup.activeSelf)
                return;

            if (!WaitingImg.gameObject.activeSelf)
                return;

            // 让菊花旋转起来。
            var r = WaitingImg.rectTransform.rotation.eulerAngles;
            WaitingImg.rectTransform.rotation = Quaternion.Euler(0, 0, r.z - 360 * Time.deltaTime);
        }

        private void UpdatePayLogic()
        {
            if (_paySeq != null && _paySeq.Running)
                _paySeq.Process();
        }

        private ActionSequence _paySeq;

        private float _payStartTime;

        public void StartPay(int payChannel)
        {
            // 执行实际的支付逻辑。
            if (_paySeq != null && _paySeq.Running)
            {
                // 正在执行支付逻辑，就不需要做任何动作，继续执行之前的任务。
                var dialogManager = GetDialogManager();
                dialogManager.ShowToast("请等待当前支付完成", 2);
                return;
            }

            _selectedPayChannel = payChannel;

            _paySeq = new ActionSequence();
            _paySeq.Append(RequestTradeNo, CheckTradoNoResult, TradeNoFail);
            _paySeq.Append(StartClientPay, CheckClientPayResult, ClientPayTimeout, 600);
            // 3秒钟后，开始第一次检测。
            _paySeq.AppendInterval(3);
            _paySeq.Append(StartCheckTrade, CheckPostState, CheckTradeTimeout, 600);

            _paySeq.Start();
            _payStartTime = Time.realtimeSinceStartup;

            AnalyticStartPay();
        }

        public void StopPay()
        {
            if (_paySeq != null)
            {
                _paySeq.Cancel();
                _paySeq = null;
            }
        }

        private void RequestTradeNo()
        {
            EnableComponent(false, true, true);
            SetStateTxt("向服务器申请订单");

            var remoteAPI = GetRemoteAPI();
            remoteAPI.RequestTradeNo(_data.name, _selectedPayChannel);
        }

        private TradeNoResult _trade;

        private string OutTradeNo
        {
            get
            {
                if (_trade == null || _trade.trade == null)
                    return null;

                return _trade.trade.out_trade_no;
            }
        }

        private bool CheckTradoNoResult()
        {
            _trade = _tradeNoResult.Read();
            if (_trade == null)
            {
                return false;
            }

            if (_trade.result != ResultCode.OK)
            {
                // 申请订单失败。
                TradeNoFail();

                AnalyticPayFail("trade_no_fail", "" + _trade.result);
            }

            return true;
        }

        private void TradeNoFail()
        {
            EnableComponent(false, false, true);

            if (_trade == null)
            {
                SetStateTxt("<color=red>申请订单失败！</color>");
            }
            else
            {
                switch (_trade.result)
                {
                    case ResultCode.H_TRADE_CLOSED:
                        SetStateTxt("<color=red>申请订单失败！</color>");
                        break;

                    case ResultCode.H_TRADE_OUTOF_LIMIT:
                        SetStateTxt("<color=red>申请订单失败，订单超额了！</color>");
                        break;

                    default:
                        SetStateTxt("<color=red>申请订单失败！</color>");
                        break;
                }
            }

            StopPay();
        }

        private float _clientPayResultTime;

        private void StartClientPay()
        {
            EnableComponent(false, true, true);
            SetStateTxt("等待玩家完成支付");
            _clientPayResultTime = 0;

            var payManager = GetPayManager();
            payManager.Pay(_trade, _selectedPayChannel);
            MyLog.InfoWithFrame(name, "_selectedPayChannel is " + _selectedPayChannel + "  _trade is " + _trade);
        }

        private bool CheckClientPayResult()
        {
            if (_selectedPayChannel == PayChannelType.TEST_PAY)
            {
                return true;
            }

            var payManager = GetPayManager();
            var time = payManager.GetPayResultTime(_selectedPayChannel);
            if (_clientPayResultTime >= time)
            {
                return false;
            }

            switch (_selectedPayChannel)
            {
                case PayChannelType.ALIPAY_CLIENT:
                case PayChannelType.ALIPAY_IOS:
                    ProcessAlipayResult(payManager.GetAlipayResultData());
                    break;

                case PayChannelType.IOS_IAP:
                    ProcessIapPayResult(payManager.GetIapPayResultData());
                    break;

                case PayChannelType.WEI_XIN:
                    ProcessWxPayResult(payManager.GetWxPayResultData());
                    break;

                case PayChannelType.XIAOMI:
                    ProcessMiPayResult(payManager.GetMiPayResultData());
                    break;
            }

            return true;
        }

        private void ClientPayTimeout()
        {
            EnableComponent(false, false, true);
            SetStateTxt("<color=red>玩家超过十分钟未完成支付，自动取消订单</color>");
            StopPay();

            AnalyticPayFail("client_pay_timeout");
        }

        private void ProcessAlipayResult(AlipayResult res)
        {
            if (res == null)
            {
                // 支付失败了，支付宝结果数据解析失败。
                EnableComponent(false, false, true);
                SetStateTxt(string.Format("<color=red>支付失败！</color>\n如有疑问请联系下方客服QQ{0}",
                    !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));
                StopPay();

                AnalyticPayFail("alipay_res_parse_error");
                return;
            }

            if (StringUtil.AreEqual(res.status, AlipayResult.StatusOk))
            {
                var param = DataUtil.ParseParameter(res.result);
                if (param.ContainsKey("success") && StringUtil.AreEqual("true", param["success"]))
                {
                    // 支付成功。
                    EnableComponent(false, true, true);
                    SetStateTxt("恭喜您，充值成功^_^\n马上请求发货");

                    AnalyticPayOk();
                }
                else
                {
                    EnableComponent(false, false, true);
                    if (StateTxt)
                        SetStateTxt(string.Format("<color=red>支付失败！</color>\n请联系下方客服QQ{0}",
                            !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));
                    StopPay();

                    AnalyticPayFail("alipay_status_ok_not_success");
                }
            }
            else
            {
                // 支付失败。
                if (StringUtil.AreEqual(res.status, AlipayResult.StatusProcessing))
                {
                    // 订单正在处理当中。
                    EnableComponent(false, false, true);
                    SetStateTxt(string.Format("<color=red>支付宝未能处理订单！</color>\n请联系下方客服QQ{0}",
                        !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));

                    AnalyticPayFail("alipay_processing");
                }
                else if (StringUtil.AreEqual(res.status, AlipayResult.StatusFail))
                {
                    // 支付失败。
                    EnableComponent(false, false, true);
                    SetStateTxt(string.Format("<color=red>支付失败！{0}</color>\n请联系下方客服QQ{1}",
                        !string.IsNullOrEmpty(res.memo) ? "\n" + res.memo : "",
                        !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));

                    AnalyticPayFail("alipay_fail");
                }
                else if (StringUtil.AreEqual(res.status, AlipayResult.StatusCanceled))
                {
                    // 玩家取消支付。
                    EnableComponent(false, false, true);
                    SetStateTxt("<color=red>玩家取消订单</color>");

                    AnalyticPayFail("alipay_cancel");
                }
                else if (StringUtil.AreEqual(res.status, AlipayResult.StatusNetworkError))
                {
                    // 网络出错。
                    EnableComponent(false, false, true);
                    SetStateTxt("<color=red>网络出错，无法连接支付宝</color>");

                    AnalyticPayFail("alipay_network_broken");
                }
                else
                {
                    // 未知原因失败。
                    EnableComponent(false, false, true);
                    SetStateTxt(string.Format("<color=red>支付失败！</color>\n请联系下方客服QQ{0}",
                        !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));

                    AnalyticPayFail("alipay_unknown_error");
                }

                StopPay();
            }
        }

        private void ProcessWxPayResult(WxPayResult res)
        {
            if (res == null)
            {
                // 支付失败了，支付宝结果数据解析失败。
                EnableComponent(false, false, true);
                SetStateTxt(string.Format("<color=red>支付失败！</color>\n如有疑问请联系下方客服QQ{0}",
                    !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));
                StopPay();

                AnalyticPayFail("wxpay_res_parse_error");
                return;
            }

            if (res.Result == WxPayResult.Ok)
            {
                // 支付成功。
                EnableComponent(false, true, true);
                SetStateTxt("恭喜您，充值成功^_^\n马上请求发货");

                AnalyticPayOk();
            }
            else
            {
                // 支付失败。

                if (res.Result == WxPayResult.Cancel)
                {
                    // 玩家取消支付。
                    EnableComponent(false, false, true);
                    SetStateTxt("<color=red>玩家取消订单</color>");

                    AnalyticPayFail("wxpay_cancel");
                }
                else
                {
                    EnableComponent(false, false, true);
                    SetStateTxt(string.Format("<color=red>支付失败！</color>\n请联系下方客服QQ{0}",
                        !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));

                    AnalyticPayFail("wxpay_fail");
                }

                StopPay();
            }
        }

        private void ProcessIapPayResult(IapResult res)
        {
            if (res == null)
            {
                EnableComponent(false, false, true);
                SetStateTxt(string.Format("<color=red>支付失败！</color>\n请联系下方客服QQ{0}",
                    !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));

                StopPay();
                AnalyticPayFail("iappay_res_parse_error");
                return;
            }

            if (res.Result == IapResult.IapSuccess)
            {
                _extra = "{\"receipt\":\"" + res.Receipt + "\"}";

                EnableComponent(false, true, true);
                SetStateTxt("恭喜您，充值成功^_^\n马上请求发货");

                AnalyticPayOk();
            }
            else
            {
                // 支付失败。

                if (res.Result == IapResult.IapFailProductInfo)
                {
                    EnableComponent(false, false, true);
                    SetStateTxt(string.Format("<color=red>向苹果请求订单失败！</color>\n请联系下方客服QQ{0}",
                        !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));

                    AnalyticPayFail("iap_product_fail");
                }
                else if (res.Result == IapResult.IapFailCannotPay)
                {
                    EnableComponent(false, false, true);
                    SetStateTxt(string.Format("<color=red>您的设备不支持支付功能！</color>\n请联系下方客服QQ{0}",
                        !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));

                    AnalyticPayFail("iap_cannot_pay");
                }
                else if (res.Result == IapResult.IapFailTradeInvalid)
                {
                    EnableComponent(false, false, true);
                    SetStateTxt(string.Format("<color=red>订单数据错误！</color>\n请联系下方客服QQ{0}",
                        !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));

                    AnalyticPayFail("iap_trade_invalid");
                }
                else if (res.Result == IapResult.IapFailPayment)
                {
                    EnableComponent(false, false, true);
                    SetStateTxt(string.Format("<color=red>支付失败！</color>\n请联系下方客服QQ{0}",
                        !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));

                    AnalyticPayFail("iap_fail_payment");
                }

                StopPay();
            }
        }

        private void ProcessMiPayResult(MiPayResult res)
        {
            if (res == null)
            {
                // 支付失败了，支付宝结果数据解析失败。
                EnableComponent(false, false, true);
                SetStateTxt(string.Format("<color=red>支付失败！</color>\n如有疑问请联系下方客服QQ{0}",
                    !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));
                StopPay();

                AnalyticPayFail("mipay_res_parse_error");
                return;
            }

            if (res.result == MiPayResult.OK)
            {
                // 支付成功。
                EnableComponent(false, true, true);
                SetStateTxt("恭喜您，充值成功^_^\n马上请求发货");

                AnalyticPayOk();
            }
            else
            {
                // 支付失败。

                if (res.result == MiPayResult.CANCEL)
                {
                    // 玩家取消支付。
                    EnableComponent(false, false, true);
                    SetStateTxt("<color=red>玩家取消订单</color>");

                    AnalyticPayFail("mipay_cancel");
                }
                else
                {
                    EnableComponent(false, false, true);
                    SetStateTxt(string.Format("<color=red>支付失败！</color>\n请联系下方客服QQ{0}",
                        !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));

                    AnalyticPayFail("mipay_fail");
                }

                StopPay();
            }
        }

        // 如果需要，从客户端支付结果中查找
        private string _tradeNo;

        private string _extra;

        private int _checkCount;

        private float _checkStartTime;

        private float _checkFinishTime;

        private float _waitInterval;

        private bool _waiting;

        private readonly int[] _checkIntervals =
        {
            10, 30, 60
        };

        private void StartCheckTrade()
        {
            // 开始检查订单。
            EnableComponent(false, true, true);
            SetStateTxt("正在请求发货");

            _checkCount = 0;
            _checkFinishTime = 0;
            _waiting = false;
            _checkStartTime = Time.time;

            var remoteAPI = GetRemoteAPI();
            remoteAPI.CheckTrade(_tradeNo, OutTradeNo, _extra);
        }

        private bool CheckPostState()
        {
            // 如果当前在等待状态，则不检查结果。
            if (_waiting)
            {
                // 尚未超时，直接返回。
                if (Time.time - _checkFinishTime < _waitInterval)
                    return false;

                // 超时，开始新一次检查。
                _waiting = false;
                _checkStartTime = Time.time;
                var remoteAPI = GetRemoteAPI();
                remoteAPI.CheckTrade(_tradeNo, OutTradeNo, _extra);
                return false;
            }

            // TODO 不在等待状态的情况下，检查结果。
            // 几次超时的过程中，是否需要提示玩家进度。
            var res = _checkTradeResult.Read();
            if (res == null)
            {
                if (Time.time - _checkStartTime > ServerRequestTimeout)
                {
                    // 请求超时了。
                    EnableComponent(false, false, true);
                    SetStateTxt("<color=red>请求超时</color>\n请稍后重试\n如有疑问请联系下方客服QQ");
                    StopPay();
                    AnalyticPostFail("post_timeout");
                    return true;
                }

                return false;
            }

            // 完成一次检查。
            _checkCount++;
            _checkFinishTime = Time.time;

            // 服务器端尚未通知成功，则开始一次等待。
            if (res.result == ResultCode.H_TRADE_NOT_PAID)
            {
                // 如果超过检查次数了，通知玩家发货失败。
                if (_checkCount > _checkIntervals.Length)
                {
                    EnableComponent(false, false, true);
                    SetStateTxt(string.Format("<color=red>发货失败！</color>\n请联系下方客服QQ{0}",
                        !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));
                    return true;
                }

                _waitInterval = _checkIntervals[_checkCount - 1];
                _waiting = true;

                return false;
            }

            // 成功充值。
            if (res.result == ResultCode.OK)
            {
                EnableComponent(false, false, true);
                SetStateTxt("恭喜您，成功充值！");

                AnalyticPostOk();
                return true;
            }

            // 服务器端已经通知过了，并且发货失败。
            EnableComponent(false, false, true);
            SetStateTxt(string.Format("<color=red>发货失败！</color>\n请联系客服QQ{0}",
                !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));

            AnalyticPostFail("post_fail");
            return true;
        }

        private void CheckTradeTimeout()
        {
            EnableComponent(false, false, true);
            SetStateTxt(string.Format("<color=red>发货失败，超时了！</color>\n请联系客服QQ：{0}",
                !string.IsNullOrEmpty(OutTradeNo) ? "\n订单号：" + OutTradeNo : ""));
            StopPay();

            AnalyticPostFail("post_timeout");
        }

        public void Close()
        {
            if (_paySeq != null && _paySeq.Running)
            {
                // 正在支付过程中，询问玩家是否停止支付。
                var dialogManager = GetDialogManager();
                dialogManager.ShowConfirmBox(
                    "正在支付过程中，是否取消支付？",
                    true, "仍然取消", () =>
                    {
                        StopPay();
                        Hide();
                    },
                    true, "继续支付", null,
                    true, false, false);
            }
            else
            {
                // 不在支付的过程中，直接关闭。
                Hide();
            }
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }

        private void AnalyticPanelShow()
        {
            var eventId = string.Format("charge_show_{0}", Reason);
            var attrs = new Dictionary<string, string>();
            attrs.Add("prepayment", _data != null ? _data.name : "NULL");
            var analyticManager = GetAnalyticManager();
            analyticManager.Event(eventId, attrs);
        }

        private void AnalyticStartPay()
        {
            var eventId = string.Format("charge_pay_{0}", Reason);
            var attrs = new Dictionary<string, string>();
            attrs.Add("prepayment", _data != null ? _data.name : "NULL");
            attrs.Add("channel", PayChannelType.GetPayChannelNameId(_selectedPayChannel));
            var analyticManager = GetAnalyticManager();
            analyticManager.EventValue(eventId, attrs, (int) Time.realtimeSinceStartup);
        }

        private void AnalyticPayOk()
        {
            var analyticManager = GetAnalyticManager();
            if (_data != null)
            {
                analyticManager.Pay((float) _data.price, DataUtil.CalculateGeValue(_data), _selectedPayChannel);
            }

            var eventId = string.Format("charge_pay_{0}_ok", Reason);
            var attrs = new Dictionary<string, string>();
            attrs.Add("prepayment", _data != null ? _data.name : "NULL");
            attrs.Add("channel", PayChannelType.GetPayChannelNameId(_selectedPayChannel));
            attrs.Add("total_fee", _data != null ? "" + _data.price : "0");
            analyticManager.EventValue(eventId, attrs, (int) (Time.realtimeSinceStartup - _payStartTime));
        }

        private void AnalyticPayFail(string error, string errorCode = null)
        {
            var eventId = string.Format("charge_pay_{0}_fail", Reason);
            var attrs = new Dictionary<string, string>();
            attrs.Add("prepayment", _data != null ? _data.name : "NULL");
            attrs.Add("channel", PayChannelType.GetPayChannelNameId(_selectedPayChannel));
            attrs.Add("error", error);
            if (!string.IsNullOrEmpty(errorCode)) attrs.Add("error_code", errorCode);

            var analyticManager = GetAnalyticManager();
            analyticManager.EventValue(eventId, attrs, (int) (Time.realtimeSinceStartup - _payStartTime));
        }

        private void AnalyticPostOk()
        {
            var eventId = string.Format("charge_post_{0}_ok", Reason);
            var attrs = new Dictionary<string, string>();
            attrs.Add("prepayment", _data != null ? _data.name : "NULL");
            var analyticManager = GetAnalyticManager();
            analyticManager.EventValue(eventId, attrs, (int) (Time.realtimeSinceStartup - _payStartTime));
        }

        private void AnalyticPostFail(string error, string errorCode = null)
        {
            var eventId = string.Format("charge_post_{0}_fail", Reason);
            var attrs = new Dictionary<string, string>();
            attrs.Add("prepayment", _data != null ? _data.name : "NULL");
            attrs.Add("error", error);
            if (!string.IsNullOrEmpty(errorCode)) attrs.Add("error_code", errorCode);
            var analyticManager = GetAnalyticManager();
            analyticManager.EventValue(eventId, attrs, (int) (Time.realtimeSinceStartup - _payStartTime));
        }
    }
}