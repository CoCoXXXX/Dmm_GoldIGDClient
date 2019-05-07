using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Dialog;
using Dmm.Util;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class PunishTipDialog : MyDialog
    {
        public Text TipText;

        public Text ExitBtnTxt;

        public delegate void ExitLogic();

        private ExitLogic _exitLogic;

        private Room _data;

        public void ApplyData(Room room, string exitBtnContent, ExitLogic exitLogic)
        {
            _data = room;

            if (room == null)
                return;

            if (exitLogic == null)
                return;

            if (ExitBtnTxt)
                ExitBtnTxt.text = exitBtnContent;

            _exitLogic = exitLogic;

            if (TipText)
                TipText.text = string.Format(
                    "本房间需<color=red>打到{0}</color>\n如果现在退出\n会被扣<color=green>{1}{2}</color>的哦",
                    PokerLogicUtil.LabelOfSessionNumType(room.target_host),
                    room.leave_punish_money,
                    CurrencyType.LabelOf(room.currency_type)
                );

            var attrs = new Dictionary<string, string>();
            attrs.Add("room_id", "" + room.room_id);
            var analyticManager = GetAnalyticManager();
            analyticManager.Event("leave_room_punish_tip", attrs);
        }

        public void Exit()
        {
            if (_exitLogic != null)
                _exitLogic();

            var attrs = new Dictionary<string, string>();
            if (_data != null) attrs.Add("room_id", "" + _data.room_id);
            var analyticManager = GetAnalyticManager();
            analyticManager.Event("leave_room_punish_tip_exit", attrs);

            Hide();
        }

        public void Continue()
        {
            var attrs = new Dictionary<string, string>();
            if (_data != null) attrs.Add("room_id", "" + _data.room_id);
            var analyticManager = GetAnalyticManager();
            analyticManager.Event("leave_room_punish_tip_continue", attrs);

            Hide();
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}