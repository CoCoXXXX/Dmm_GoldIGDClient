using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Checkin;
using Dmm.Common;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class CheckinAwardResultHandler : MessageHandlerAdapter<CheckinAwardResult>
    {
        private readonly IDataContainer<CheckinAwardResult> _checkinAwardResult;

        private readonly IDataContainer<CheckinResult> _checkinResult;

        private readonly IDataContainer<CheckinConfig> _checkinConfig;

        private readonly IDialogManager _dialogManager;

        public CheckinAwardResultHandler(
            IDataRepository dataRepository,
            IDialogManager dialogManager) :
            base(Server.HServer, Msg.CmdType.HU.CHECKIN_AWARD_RESULT)
        {
            _checkinAwardResult = dataRepository.GetContainer<CheckinAwardResult>(DataKey.CheckinAwardResult);
            _checkinResult = dataRepository.GetContainer<CheckinResult>(DataKey.CheckinResult);
            _checkinConfig = dataRepository.GetContainer<CheckinConfig>(DataKey.CheckinConfig);
            _dialogManager = dialogManager;
        }

        protected override void DoHandle(CheckinAwardResult msg)
        {
            _checkinAwardResult.Write(msg, Time.time);
            var res = msg.res;
            if (res.code == ResultCode.OK)
            {
                var list = msg.award;
                if (list != null && list.Count > 0)
                {
                    var invalidate = false;

                    foreach (var a in list)
                    {
                        if (a.award != null)
                        {
                            _dialogManager.ShowDialog<GetAwardDialog>(DialogName.GetAwardDialog, false, false,
                                (dialog) =>
                                {
                                    dialog.ApplyData(a.award);
                                    dialog.Show();
                                });
                        }

                        if (a.type == CheckinAwardType.Period)
                        {
                            var checkinConfig = _checkinConfig.Read();
                            List<CheckinCondition> condList = null;
                            if (checkinConfig != null)
                            {
                                condList = checkinConfig.condition;
                            }

                            CheckinCondition checkinCond = null;
                            foreach (var cond in condList)
                            {
                                if (cond.day_count == a.checkin_days)
                                {
                                    checkinCond = cond;
                                    break;
                                }
                            }

                            if (checkinCond != null)
                            {
                                checkinCond.awarded = true;
                                invalidate = true;
                            }
                        }
                    }

                    if (invalidate)
                    {
                        _checkinResult.Invalidate(Time.time);
                    }
                }
            }
        }
    }
}