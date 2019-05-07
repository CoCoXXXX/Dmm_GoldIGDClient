using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Game;
using Dmm.Shop;
using UnityEngine;
using Zenject;

namespace Dmm.Left
{
    public class InfoPanel : MonoBehaviour
    {
        #region Inject

        private IDialogManager _dialogManager;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager)
        {
            _dialogManager = dialogManager;
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        #endregion

        #region DataContainer

        private IDataContainer<User> _user;

        #endregion

        public PlayerFigure PlayerFigure;

        public void OnEnable()
        {
            RefreshContent();
        }

        public void Update()
        {
            RefreshContent();
        }

        private float _lastRefreshTime;

        public void RefreshContent()
        {
            if (_lastRefreshTime >= _user.Timestamp)
            {
                return;
            }

            _lastRefreshTime = _user.Timestamp;

            PlayerFigure.ApplyData(_user.Read());
        }

        public void ShowShopPanel()
        {
            _dialogManager.ShowDialog<ShopPanel>(DialogName.ShopPanel, false, false,
                (shop) => { shop.Show(ShopPanel.ShopType.Commodity); });
        }
    }
}