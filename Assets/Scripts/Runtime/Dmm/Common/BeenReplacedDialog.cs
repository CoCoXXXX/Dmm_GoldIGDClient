using Dmm.Dialog;

namespace Dmm.Common
{
    public class BeenReplacedDialog : MyDialog
    {
        public void ReLogin()
        {
            var network = GetNetworkManager();
            network.InitLogin();
            Hide();
        }

        public void SwitchAccount()
        {
#if UNITY_ANDROID
            var configHolder = GetConfigHolder();
            var xiaoMiManager = GetXiaoMiManager();

            if (configHolder.XiaoMiMode)
            {
                // 小米模式下，调用小米的登录。
                xiaoMiManager.Login();
                Hide();
                return;
            }
#endif

            var network = GetNetworkManager();
            network.Logout();
            Hide();
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}