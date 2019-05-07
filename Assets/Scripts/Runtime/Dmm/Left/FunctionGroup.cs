using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.MoreFunction;
using Dmm.QuickTools;
using Dmm.Shop;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Left
{
    /// <summary>
    /// 左面板的快捷功能面板。
    /// </summary>
    public class FunctionGroup : MonoBehaviour
    {
        #region Inject

        private IDialogManager _dialogManager;

        private IDataContainer<FeatureSwitch> _featureSwitch;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager)
        {
            _dialogManager = dialogManager;
            _featureSwitch = dataRepository.GetContainer<FeatureSwitch>(DataKey.FeatureSwitch);
        }

        #endregion

        public Button ChangeSexBtn;

        public void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            var featureSwitch = _featureSwitch.Read();
            var isEnablePersonalInfo = featureSwitch != null && featureSwitch.personal_info;
            if (ChangeSexBtn.gameObject.activeSelf != isEnablePersonalInfo)
            {
                ChangeSexBtn.gameObject.SetActive(isEnablePersonalInfo);
            }
        }

        public void ShowExchange()
        {
            _dialogManager.ShowDialog<ShopPanel>(DialogName.ShopPanel, false, false,
                (shop) => { shop.Show(ShopPanel.ShopType.Exchange); });
        }

        public void ChangeNickname()
        {
            _dialogManager.ShowDialog<ChangeNicknameDialog>(DialogName.ChangeNicknameDialog);
        }

        public void ChangeSex()
        {
            _dialogManager.ShowDialog<ChangeSexDialog>(DialogName.ChangeSexDialog);
        }

        public void ResetWinRate()
        {
            _dialogManager.ShowDialog<ResetWinRateDialog>(DialogName.ResetWinRateDialog);
        }

        public void ShowMoreFunctionPanel()
        {
            _dialogManager.ShowDialog<MoreFunctionDialog>(DialogName.MoreFunctionPanel);
        }
    }
}