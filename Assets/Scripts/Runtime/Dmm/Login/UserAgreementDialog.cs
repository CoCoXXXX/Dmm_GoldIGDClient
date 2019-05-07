using Dmm.Data;
using Dmm.Dialog;
using Dmm.PIP;

namespace Dmm.Login
{
    public class UserAgreementDialog : MyDialog
    {
        private IPIPLogic _pip;

        private ConfigHolder _config;

        private IDialogManager _dialogManager;

        public void ShowMythCode()
        {
            _pip = GetPIPLogic();
            _config = GetConfigHolder();
            _dialogManager = GetDialogManager();
            var pipData = _pip.GetPIPData();
            var pipStr = "NULL";
            if (pipData != null)
            {
                pipStr = string.Format("{0}::{1}::{2}::{3}",
                    _pip.EnableIpV6(),
                    _pip.GetNewVersion(),
                    _pip.GetForceUpdate(),
                    _pip.ReplaceWS());
            }

            var code = string.Format("{0}::{1}::{2}::{3}::{4}::test-{5}\n{6}",
                _config.Platform,
                _config.Product,
                _config.SaleChannel,
                _config.VersionTxt,
                _config.ClientVersion,
                _pip.IsTest(),
                pipStr
            );
            _dialogManager.ShowMessageBox(code);
        }
    }
}