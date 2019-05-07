using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Util;
using Test.Scripts.Runtime.Dmm.TestLogin.Record;
using UnityEngine;
using UnityEngine.UI;

namespace Test.Scripts.Runtime.Dmm.TestLogin.Dialog
{
    public class SetClientVersionDialog : MyDialog {
        
        public InputField ClientVersion;

        public InputField SaleChannel;

        public InputField Product;

        public InputField Platform;

        public InputField LastVisitorUsername;

        public InputField LasVisitorId;
        
        private IDataContainer<bool> _isSetTestClientVersion;
        
        private void OnEnable()
        {
            var dataRepository = GetDataRepository();
            
            _isSetTestClientVersion = dataRepository.GetContainer<bool>(DataKey.IsSetTestClientVersion);
            
            var clientVersion = PrefsUtil.GetInt(TestLoginRecord.TestClientVersion, 0);
            var saleChannel = PrefsUtil.GetString(TestLoginRecord.TestSaleChannel, null);
            var product = PrefsUtil.GetString(TestLoginRecord.TestProduct, null);
            var platform = PrefsUtil.GetInt(TestLoginRecord.TestPlatform, 0);
            var lastUserName = LoginRecord.LastVisitorUsername;
            var lastVisitorId = LoginRecord.LastVisitorId;
            
            if (!string.IsNullOrEmpty(saleChannel))
            {
                SaleChannel.text = saleChannel;
            }
            
            if (!string.IsNullOrEmpty(product))
            {
                Product.text = product;
            }

            if (!string.IsNullOrEmpty(lastUserName))
            {
                LastVisitorUsername.text = lastUserName;
            }
            
            if (!string.IsNullOrEmpty(lastVisitorId))
            {
                LasVisitorId.text = lastVisitorId;
            }
            
            if (clientVersion != 0)
            {
                ClientVersion.text = clientVersion + "";
            }
            
            if (platform != 0)
            {
                Platform.text = platform + "";
            }
        }
        
        public void Confirm()
        {
            var saleChannel = SaleChannel.text;
            
            PrefsUtil.SetString(TestLoginRecord.TestSaleChannel,saleChannel);
            
            var product = Product.text;
            
            PrefsUtil.SetString(TestLoginRecord.TestProduct,product);
            
            int clientVersion = 0;
            
            if ( int.TryParse(ClientVersion.text, out clientVersion))
            {
                PrefsUtil.SetInt(TestLoginRecord.TestClientVersion,clientVersion);
            }
            else
            {
                GetDialogManager().ShowToast("ClientVersion不对",2);
                return;
            }
            int platform = 0;
            if ( int.TryParse(Platform.text, out platform))
            {
                PrefsUtil.SetInt(TestLoginRecord.TestPlatform,platform);
            }
            else
            {
                GetDialogManager().ShowToast("Platform 不对",2);
                return;
            }

            var lastUsername = LastVisitorUsername.text;
            LoginRecord.LastVisitorUsername = lastUsername;
            
            var lastVisitorId = LasVisitorId.text;
            LoginRecord.LastVisitorId = lastVisitorId;
            
            PrefsUtil.Flush();
            
            _isSetTestClientVersion.Write(true,Time.time);
            Hide();
        }
    }
}
