using com.morln.game.gd.command;
using Dmm.DataContainer;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Game
{
    public class BCounterTips : MonoBehaviour
    {
        #region Inject

        private IDataContainer<BCounter> _bCounter;

        [Inject]
        public void Initialize(IDataRepository dataRepository)
        {
            _bCounter = dataRepository.GetContainer<BCounter>(DataKey.BCounter);
        }

        #endregion

        public GameObject Content;

        public Text TipText;

        public Text LeftTime;

        private void Update()
        {
            RefreshBCounter();
            UpdateBCounterTime();
        }

        private float _bcounterRefreshTime;

        private void RefreshBCounter()
        {
            if (_bcounterRefreshTime >= _bCounter.Timestamp)
            {
                return;
            }

            _bcounterRefreshTime = _bCounter.Timestamp;

            var msg = _bCounter.Read();
            if (msg == null)
            {
                return;
            }

            if (msg.start_or_stop)
            {
                _showbcounter = true;

                if (!Content.gameObject.activeSelf)
                {
                    Content.gameObject.SetActive(true);
                }

                _bcounterEndTime = _bCounter.Timestamp + msg.left_time;
                TipText.text = msg.msg;
            }
            else
            {
                _showbcounter = false;

                if (Content.gameObject.activeSelf)
                {
                    Content.gameObject.SetActive(false);
                }
            }
        }

        private bool _showbcounter;

        private float _bcounterEndTime;

        private void UpdateBCounterTime()
        {
            if (!_showbcounter || !Content.gameObject.activeSelf)
            {
                return;
            }

            if (_bcounterEndTime < Time.time)
            {
                if (Content.gameObject.activeSelf)
                {
                    Content.gameObject.SetActive(false);
                }

                _showbcounter = false;
                return;
            }

            var leftTime = Mathf.RoundToInt(_bcounterEndTime - Time.time);
            LeftTime.text = "还剩" + leftTime + "秒";
        }
    }
}