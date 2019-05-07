using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Login;
using Dmm.Util;
using Test.Scripts.Runtime.Dmm.TestLogin.Record;
using UnityEngine;
using UnityEngine.UI;

namespace Test.Scripts.Runtime.Dmm.TestLogin.Dialog
{
    public class SetPServerDialog : MyDialog
    {
        public InputField Ip;

        public InputField Port;
        
        private IDataContainer<bool> _isSetTestLogin;
        
        private void OnEnable()
        {
            var dataRepository = GetDataRepository();
            
            _isSetTestLogin = dataRepository.GetContainer<bool>(DataKey.IsSetTestPServer);
            
            var ip = PrefsUtil.GetString(TestLoginRecord.PServerTestLoginIp, null);
            var port = PrefsUtil.GetInt(TestLoginRecord.PServerTestLoginPort, 0);

            if (!string.IsNullOrEmpty(ip))
            {
                Ip.text = ip;
            }
            
            if (port != 0)
            {
                Port.text = port + "";
            }
        }

        public void Confirm()
        {
            var ip = Ip.text;
            int port = 0;
            
            if ( int.TryParse(Port.text, out port))
            {
                PrefsUtil.SetString(TestLoginRecord.PServerTestLoginIp,ip);
                PrefsUtil.SetInt(TestLoginRecord.PServerTestLoginPort,port);
                PrefsUtil.Flush();
            
                _isSetTestLogin.Write(true,Time.time);
                Hide();
            }
            else
            {
                GetDialogManager().ShowToast("端口不对",2);
            }
        }
    }
}
