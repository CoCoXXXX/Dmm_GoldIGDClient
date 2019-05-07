using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Common;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.HintItemLogic;
using Dmm.Login;
using Dmm.MoreFunction;
using Dmm.Network;
using Dmm.Res;
using Dmm.Shop;
using Dmm.WeChat;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Hall
{
    public class PortalWindow : MonoBehaviour
    {
        #region Inject

        private IAppContext _appContext;

        private IAppController _app;

        private IDialogManager _dialogManager;

        private INetworkManager _networkManager;

        private IDataContainer<int> _currentGameMode;

        private IDataContainer<List<HintItem>> _hintItemList;

        private IDataContainer<InGameConfig> _inGameConfig;

        [Inject]
        public void Inject(
            IAppContext context,
            RemoteAPI remoteApi,
            IAppController appController,
            IDialogManager dialogManager,
            IDataRepository dataRepository,
            INetworkManager networkManager)
        {
            _appContext = context;
            _app = appController;
            _dialogManager = dialogManager;
            _networkManager = networkManager;
            _currentGameMode = dataRepository.GetContainer<int>(DataKey.CurrentGameMode);
            _hintItemList = dataRepository.GetContainer<List<HintItem>>(DataKey.HintItemList);
            _inGameConfig = dataRepository.GetContainer<InGameConfig>(DataKey.InGameConfig);
        }

        public class Factory : PrefabFactory<PortalWindow>
        {
        }

        #endregion

        public AsyncImage AdImg;

        private HintItem _hintItem;

        private void Start()
        {
            if (AdImg)
            {
                _hintItem = GetHintItemByPos(HintItemPos.HALL_BOTTOM);
                if (_hintItem == null || string.IsNullOrEmpty(_hintItem.outer_pic))
                {
                    AdImg.Reset();
                }
                else
                {
                    AdImg.SetTargetPic(ResourcePicName.ActivityOuterPic, ResourcePath.PortalWindow, null, true);
                }
            }
        }

        private void Update()
        {
            CheckAndroidBackKey();
            //更新需要自动打开的对话框
            _dialogManager.UpdateAutoShowDialog();
        }

        public void GoToClassicMode()
        {
            _currentGameMode.Write(GameMode.Classic, Time.time);
        }

        public void GoToFanbeiMode()
        {
            _currentGameMode.Write(GameMode.Fanbei, Time.time);
        }

        public void GoToFriendMode()
        {
            _currentGameMode.Write(GameMode.Friend, Time.time);
        }

        public void GoToSingleMode()
        {
            _app.GoToSingleGame();
        }

        public void GoToRaceRoom()
        {
            _currentGameMode.Write(GameMode.Race, Time.time);
        }

        public void GoToTtzRoom()
        {
            _currentGameMode.Write(GameMode.Ttz, Time.time);
        }

        public void ShowShop()
        {
            _dialogManager.ShowDialog<ShopPanel>(DialogName.ShopPanel, false, false,
                (shop) => { shop.Show(ShopPanel.ShopType.Charge); });
        }

        public void ShowUserTaskDialog()
        {
            _dialogManager.ShowUserTaskDialog(_appContext);
        }

        public void ShowHelp()
        {
            _dialogManager.ShowDialog<HelpDialog>(DialogName.HelpDialog, true, true);
        }

        public void ShowCheckin()
        {
            _dialogManager.RequestShowCheckinDialog(_appContext);
        }

        public void ShowBillboard()
        {
            _dialogManager.ShowDialog<BillboardPanel>(DialogName.BillboardDialog, true, true);
        }

        public void ShowMoreFunction()
        {
            _dialogManager.ShowDialog<MoreFunctionDialog>(DialogName.MoreFunctionPanel);
        }

        public void Logout()
        {
            _dialogManager.ShowConfirmBox("是否退出当前登陆的账号？", true, "确定", () => { _networkManager.Logout(); },
                true, "取消",
                null, true, false, false);
        }

        public void ClickAd()
        {
            if (_hintItem == null)
            {
                return;
            }

            _dialogManager.ShowDialog<HintItemDialog>(DialogName.HintItemDialog, false, true,
                (dialog) =>
                {
                    dialog.ApplyData(_hintItem);
                    dialog.Show();
                });
        }

        private AlertBox _quitSingleDialog;

        private void CheckAndroidBackKey()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_quitSingleDialog == null)
                {
                    _quitSingleDialog = _dialogManager.ShowConfirmBox("是否退出游戏？",
                        true, "退出", () => { Application.Quit(); },
                        true, "继续", null, true, true, true);
                }
            }
        }

        public HintItem GetHintItemByPos(int pos)
        {
            var hintItems = _hintItemList.Read();
            if (hintItems == null || hintItems.Count <= 0)
            {
                return null;
            }

            foreach (var item in hintItems)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.pos == pos)
                {
                    return item;
                }
            }

            return null;
        }
    }
}