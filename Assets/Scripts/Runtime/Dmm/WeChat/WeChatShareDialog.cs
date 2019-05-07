using Dmm.Dialog;
using UnityEngine.UI;
using Zenject;

namespace Dmm.WeChat
{
    public class WeChatShareDialog : MyDialog
    {
        #region Inject

        private IWeChatManager _weChatManager;

        [Inject]
        public void Inject(IWeChatManager weChatManager)
        {
            _weChatManager = weChatManager;
        }

        #endregion

        private string _url;
        private string _imgUrl;
        private string _imgPath;
        private string _title;
        private string _content;
        private string _thumbUrl;
        private string _awardCode;

        public Text TitleTxt;

        public void ApplyData(
            string dialogTitle,
            string url,
            string imgUrl,
            string imgPath,
            string title,
            string content,
            string thumbUrl,
            string awardCode = null)
        {
            TitleTxt.text = dialogTitle;
            _url = url;
            _imgUrl = imgUrl;
            _imgPath = imgPath;
            _title = title;
            _content = content;
            _thumbUrl = thumbUrl;
            _awardCode = awardCode;
        }

        public void DoShare(bool circle)
        {
            if (circle)
            {
                _weChatManager.WxCircle(_url, _imgUrl, _imgPath, _title, _content, _thumbUrl, _awardCode);
            }
            else
            {
                _weChatManager.WxShare(_url, _imgUrl, _imgPath, _title, _content, _thumbUrl, _awardCode);
            }

            Hide();
        }
    }
}