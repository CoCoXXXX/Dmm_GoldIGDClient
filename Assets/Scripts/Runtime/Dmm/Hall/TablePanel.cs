using Dmm.Analytic;
using Dmm.App;
using Dmm.Common;
using Dmm.Dialog;
using Dmm.Login;
using Dmm.MoreFunction;
using Dmm.Network;
using Dmm.Shop;
using UnityEngine;
using Zenject;

namespace Dmm.Hall
{
    public class TablePanel : MonoBehaviour
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IAppContext _context;

        private IDialogManager _dialogManager;

        private IAnalyticManager _analyticManager;

        [Inject]
        public void Initialize(
            IAppContext context,
            IDialogManager dialogManager,
            IAnalyticManager analyticManager,
            RemoteAPI remoteAPI)
        {
            _context = context;
            _remoteAPI = remoteAPI;
            _dialogManager = dialogManager;
            _analyticManager = analyticManager;
        }

        #endregion

        public void OnEnable()
        {
            _analyticManager.PageStart("TablePanel");
        }

        public void OnDisable()
        {
            _analyticManager.PageEnd("TablePanel");
        }

        public void Back()
        {
            _remoteAPI.LeaveRoom(false);
        }

        public void ShowShop()
        {
            _dialogManager.ShowDialog<ShopPanel>(DialogName.ShopPanel, false, false,
                (shop) => { shop.Show(ShopPanel.ShopType.Commodity); });
        }

        public void ShowHelp()
        {
            _dialogManager.ShowDialog<HelpDialog>(DialogName.HelpDialog);
        }

        public void ShowCheckin()
        {
            _dialogManager.RequestShowCheckinDialog(_context);
        }

        public void ShowBillboard()
        {
            _dialogManager.ShowDialog<BillboardPanel>(DialogName.BillboardDialog);
        }

        public void ShowMoreFunction()
        {
            _dialogManager.ShowDialog<MoreFunctionDialog>(DialogName.MoreFunctionPanel);
        }

        public void Update()
        {
            CheckBackKey();
        }

        private void CheckBackKey()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Back();
            }
        }
    }
}