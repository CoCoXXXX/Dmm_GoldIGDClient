using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.Common;
using Dmm.DataContainer;
using Dmm.Msg;
using Dmm.Network;
using Dmm.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Shop
{
    public class YuanBaoTab : MonoBehaviour
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IUIController _uiController;

        private IAnalyticManager _analyticManager;

        private bool _initialized = false;

        private IDataContainer<YuanBaoConfigResult> _yuanBaoConfigResult;


        [Inject]
        public void Initialize(
            IDataRepository dtaDataRepository,
            IUIController uiController,
            IAnalyticManager analyticManager,
            RemoteAPI remoteAPI)
        {
            _remoteAPI = remoteAPI;
            _uiController = uiController;
            _analyticManager = analyticManager;

            _initialized = true;

            _yuanBaoConfigResult =
                dtaDataRepository.GetContainer<YuanBaoConfigResult>(DataKey.YuanBaoConfigResult);

            InitYuanBaoConfigData();
        }

        #endregion

        #region 请求数据

        private void InitYuanBaoConfigData()
        {
            _yuanBaoConfigResult.ClearAndInvalidate(Time.time);
            _remoteAPI.RequestYuanBaoConfigData();

            _analyticManager.Event("yuanbao_show");
        }

        #endregion

        public Text TipText;

        public float RotateSpeed = 360;

        public Image WaitingImg;

        #region Unity 方法

        public void OnEnable()
        {
            if (_initialized)
            {
                InitYuanBaoConfigData();
            }
        }

        public void OnDisable()
        {
            _uiController.NeedUnloadAsset();
        }

        public void Update()
        {
            if (_yuanBaoConfigResult.Read() != null)
            {
                if (WaitingImg.gameObject.activeSelf)
                    WaitingImg.gameObject.SetActive(false);

                var data = _yuanBaoConfigResult.Read();
                if (data.res.code == ResultCode.OK)
                {
                    if (TipText.gameObject.activeSelf)
                        TipText.gameObject.SetActive(false);
                }
                else
                {
                    if (!TipText.gameObject.activeSelf)
                        TipText.gameObject.SetActive(true);

                    if (!string.IsNullOrEmpty(data.res.msg))
                    {
                        TipText.text = data.res.msg;
                    }
                    else
                    {
                        TipText.text = "找不到数据";
                    }
                }
            }
            else
            {
                if (!WaitingImg.gameObject.activeSelf)
                    WaitingImg.gameObject.SetActive(true);

                var r = WaitingImg.rectTransform.rotation.eulerAngles;
                WaitingImg.rectTransform.rotation = Quaternion.Euler(0, 0, r.z - RotateSpeed * Time.deltaTime);
            }
        }

        #endregion
    }
}