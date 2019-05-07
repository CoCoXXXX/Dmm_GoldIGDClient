using System.Runtime.InteropServices;
using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Pay;
using Dmm.Shop;
using Dmm.WeChat;
using Dmm.Widget;
using UnityEngine;

namespace Dmm.HintItemLogic
{
    public class HintItemDialog : MyDialog
    {
        public AsyncImage ContentImage;

        private HintItem _data;

        private IDataContainer<User> _myUser;

        private void OnEnable()
        {
            var dataRepository = GetDataRepository();
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        /// <summary>
        /// 返回true，应用数据成功，返回false，应用数据失败。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ApplyData(HintItem data)
        {
            _data = data;

            if (_data == null)
            {
                return false;
            }

            if (!ContentImage)
            {
                return false;
            }

            var resourcePath = GetAssetBundleByPicNameMap.GetAssetBundleName(data.content_pic);
            ContentImage.SetTargetPic(data.content_pic, resourcePath, data.content_pic_url, true);

            var eventId = string.Format(
                "hintitem_{0}_{1}_show",
                HintItemPos.IdOfPos(data.pos),
                HintItemType.IdOf(data.type));

            GetAnalyticManager().Event(eventId);

            return true;
        }

        public void OnClickContent()
        {
            if (_data == null)
            {
                Hide();
                return;
            }

            var eventId = string.Format(
                "hintitem_{0}_{1}_click_content",
                HintItemPos.IdOfPos(_data.pos),
                HintItemType.IdOf(_data.type));

            GetAnalyticManager().Event(eventId);

            var dialogManager = GetDialogManager();
            var wechatManager = GetWeChatManager();
            switch (_data.type)
            {
                case HintItemType.CHARGE:
                    dialogManager.ShowDialog<PayChannelPanel>(DialogName.PayChannelPanel, false, false,
                        (dialog) =>
                        {
                            dialog.ApplyData(string.Format("hintitem_{0}", HintItemPos.LabelOfPos(_data.pos)),
                                _data.prepayment);
                            dialog.Show();
                        });

                    break;

                case HintItemType.URL:
                    var myUser = _myUser.Read();
                    Application.OpenURL(string.Format("{0}?username={1}", _data.url, myUser.Username()));
                    break;

                case HintItemType.VIP:
                    dialogManager.ShowDialog<ShopPanel>(DialogName.ShopPanel, false, false,
                        (shop) => { shop.Show(ShopPanel.ShopType.Vip); });
                    break;

                case HintItemType.WX_SHARE:
                {
                    var shareContent = new ShareContent(ShareResultType.AwardCode, _data.award_code);
                    var content = JsonUtility.ToJson(shareContent);
                    wechatManager.WxShare(_data.url, _data.wx_img_url, null, _data.wx_title, _data.wx_content,
                        _data.wx_thumb_url, content);
                    break;
                }


                case HintItemType.WX_CIRCLE:
                {
                    var shareContent = new ShareContent(ShareResultType.AwardCode, _data.award_code);
                    var content = JsonUtility.ToJson(shareContent);
                    wechatManager.WxCircle(_data.url, _data.wx_img_url, null, _data.wx_title, _data.wx_content,
                        _data.wx_thumb_url, content);
                    break;
                }

                case HintItemType.APP_PROMOTE:
#if UNITY_ANDROID
                    dialogManager.ShowDialog<ApkDownloadDialog>(DialogName.ApkDownloadDialog, false, false,
                        (apkDownloadDialog) =>
                        {
                            apkDownloadDialog.ApplyData(_data.app_android_url, _data.app_name, _data.award_code);
                            apkDownloadDialog.Show();
                        });
#endif

#if UNITY_IOS
                    if (!string.IsNullOrEmpty(_data.ios_app_id))
                    {
                        MyLog.InfoWithFrame(name, string.Format("jump to appstore, id:{0}", _data.ios_app_id));
                        // _ios.OpenProductPage(_data.ios_app_id, _data.award_code);
                    }
                    else if (!string.IsNullOrEmpty(_data.app_ios_url))
                    {
                        MyLog.InfoWithFrame(name, string.Format("open url: {0}", _data.app_ios_url));
                        GetIosSDK().OpenUrl(_data.app_ios_url);

                        if (!string.IsNullOrEmpty(_data.award_code))
                        {
                            GetRemoteAPI().RequestAward(_data.award_code);
                        }
                    }
#endif
                    break;
            }

            Hide();
        }

        public void OnClickBg()
        {
            Hide();
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}