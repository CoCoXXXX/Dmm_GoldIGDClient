using System;
using Dmm.Dialog;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class DisconnectedDialog : MyDialog
    {
        public Text ErrMsg;

        public void ApplyData(string msg, Action onReconnect = null)
        {
            ErrMsg.text = msg;
        }

        public void ReLogin()
        {
            var network = GetNetworkManager();
            network.InitLogin();

            Hide();
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}