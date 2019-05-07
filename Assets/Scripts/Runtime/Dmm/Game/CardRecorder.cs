using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Base;
using Dmm.Data;
using Dmm.DataContainer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Game
{
    public class CardRecorder : MonoBehaviour, IDragHandler
    {
        #region Inject

        private IAppController _appController;

        private IDataContainer<List<int>> _pokerRecorder;

        private IDataContainer<User> _myUser;

        [Inject]
        public void Initialize(IAppController appController, IDataRepository dataRepository)
        {
            _appController = appController;
            _pokerRecorder = dataRepository.GetContainer<List<int>>(DataKey.PokerRecorder);
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        #endregion

        public RectTransform Parent;

        public RectTransform RectTransform;

        #region 数量组件

        public Text WD;

        public Text WX;

        public Text PA;

        public Text PK;

        public Text PQ;

        public Text PJ;

        public Text P10;

        public Text P9;

        public Text P8;

        public Text P7;

        public Text P6;

        public Text P5;

        public Text P4;

        public Text P3;

        public Text P2;

        public Text Days;

        private readonly Text[] _textList = new Text[15];

        #endregion

        public void Awake()
        {
            Reset();

            _textList[0] = P2;
            _textList[1] = P3;
            _textList[2] = P4;
            _textList[3] = P5;
            _textList[4] = P6;
            _textList[5] = P7;
            _textList[6] = P8;
            _textList[7] = P9;
            _textList[8] = P10;
            _textList[9] = PJ;
            _textList[10] = PQ;
            _textList[11] = PK;
            _textList[12] = PA;
            _textList[13] = WX;
            _textList[14] = WD;
        }

        private float RefreshTime { get; set; }

        public void RefreshContent()
        {
            if (RefreshTime >= _pokerRecorder.Timestamp)
            {
                return;
            }

            RefreshTime = _pokerRecorder.Timestamp;

            var data = _pokerRecorder.Read();
            if (data == null)
            {
                Reset();
                return;
            }

            if (Days)
            {
                var user = _myUser.Read();
                if (user == null)
                {
                    return;
                }

                if (_appController.IsSingleGameMode())
                {
                    Days.text = "单机";
                }
                else if (user.vip > 0)
                {
                    Days.text = "VIP";
                }
                else if (user.CardRecorderLeftDays() > 0)
                {
                    Days.text = string.Format("{0}天", user.CardRecorderLeftDays());
                }
            }

            SetPokerCount(data);
        }

        private float _startRoundRefreshTime;

        public void Reset()
        {
            WD.text = "";
            WX.text = "";
            PA.text = "";
            PK.text = "";
            PQ.text = "";
            PJ.text = "";
            P10.text = "";
            P9.text = "";
            P8.text = "";
            P7.text = "";
            P6.text = "";
            P5.text = "";
            P4.text = "";
            P3.text = "";
            P2.text = "";
            Days.text = "";
        }

        private void SetPokerCount(List<int> leftCounts)
        {
            if (leftCounts == null)
                return;

            for (int i = 0; i < _textList.Length; i++)
            {
                var text = _textList[i];
                if (!text)
                {
                    continue;
                }

                if (i < leftCounts.Count)
                {
                    if (leftCounts[i] >= 0)
                    {
                        text.text = "" + leftCounts[i];
                    }
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransform.anchoredPosition = ToLocalPosition(eventData.position);
            // TODO 限制记牌器，不能出屏幕。
        }

        private Vector2 ToLocalPosition(Vector2 touchPos)
        {
            Vector2 localPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Parent, touchPos, MainCamera.Current,
                out localPos))
                return localPos;

            return touchPos;
        }
    }
}