namespace Dmm.WeChat
{
    public interface IWeChatManager
    {
        void Init();

        void WxShare(string url, string wxImgUrl, string wxImgPath, string wxTitle, string wxContent,
            string thumbUrl, string content = null);

        void WxCircle(string url, string wxImgUrl, string wxImgPath, string wxTitle, string wxContent,
            string thumbUrl, string content = null);

        void WxIgdInvite();

        bool IsWechatInstalled();

        void Auth(string deviceId);

        WxAuthResult GetWxAuthResult();

        void WxShareResult(string content);

        void WechatShareScreenShot(int offset);
    }
}