using Dmm.App;
using Dmm.Common;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Login;
using Dmm.MoreFunction;
using Dmm.Shop;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Hall
{
    public class RoomWindow : MonoBehaviour
    {
        #region Inject

        private IAppContext _context;

        private IDialogManager _dialogManager;

        private IDataContainer<int> _currentGameMode;

        [Inject]
        public void Inject(
            IDataRepository dataRepository,
            IAppContext context,
            IDialogManager dialogManager)
        {
            _context = context;
            _dialogManager = dialogManager;
            _currentGameMode = dataRepository.GetContainer<int>(DataKey.CurrentGameMode);
        }

        public class Factory : PrefabFactory<RoomWindow>
        {
        }

        #endregion

        public GameObject GameModeObj;

        public Text GameModeTxt;

        public GameObject RoomTable;

        public GameObject RaceTable;

        private void OnEnable()
        {
            GameModeTxt.text = GameMode.LabelOf(_currentGameMode.Read());

            if (_currentGameMode.Read() == GameMode.Race)
            {
                RaceTable.SetActive(true);
                RoomTable.SetActive(false);
            }
            else
            {
                RaceTable.SetActive(false);
                RoomTable.SetActive(true);
            }
        }

        private void Update()
        {
            CheckAndroidBackKey();
        }

        public void BackToPortal()
        {
            _currentGameMode.Write(GameMode.Null, Time.time);
        }

        public void ShowShop()
        {
            _dialogManager.ShowDialog<ShopPanel>(DialogName.ShopPanel, false, false,
                (shop) => { shop.Show(ShopPanel.ShopType.Commodity); });
        }

        public void ShowHelp()
        {
            _dialogManager.ShowDialog<HelpDialog>(DialogName.HelpDialog, true, true);
        }

        public void ShowCheckin()
        {
            _dialogManager.RequestShowCheckinDialog(_context);
        }

        public void ShowBillboard()
        {
            _dialogManager.ShowDialog<BillboardPanel>(DialogName.BillboardDialog, true, true);
        }

        public void ShowMoreFunction()
        {
            _dialogManager.ShowDialog<MoreFunctionDialog>(DialogName.MoreFunctionPanel);
        }

        private void CheckAndroidBackKey()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackToPortal();
            }
        }
    }
}