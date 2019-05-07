using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Race;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class RaceAwardHandler : MessageHandlerAdapter<RaceAward>
    {
        private readonly IDialogManager _dialogManager;

        private readonly IDataContainer<Queue<RaceAward>> _raceAward;

        public RaceAwardHandler(
            IDataRepository dataRepository,
            IDialogManager dialogManager) :
            base(Server.HServer, Msg.CmdType.HU.RACE_AWARD)
        {
            _raceAward = dataRepository.GetContainer<Queue<RaceAward>>(DataKey.RaceAwardQueue);
            _dialogManager = dialogManager;
        }

        protected override void DoHandle(RaceAward content)
        {
            var raceAwardQueue = _raceAward.Read();
            if (raceAwardQueue == null)
            {
                raceAwardQueue = new Queue<RaceAward>();
                _raceAward.Write(raceAwardQueue, Time.time);
            }

            if (!raceAwardQueue.Contains(content))
            {
                raceAwardQueue.Enqueue(content);
            }

            var cacheDialog = _dialogManager.GetCachedDialog(DialogName.RaceAwardsDialog);
            if (cacheDialog != null)
            {
                return;
            }

            if (raceAwardQueue.Count <= 0)
            {
                return;
            }

            var raceAward = raceAwardQueue.Dequeue();
            if (raceAward == null)
            {
                return;
            }

            _dialogManager.ShowDialog<RaceAwardsDialog>(DialogName.RaceAwardsDialog, false, true,
                (dialog) =>
                {
                    dialog.Apply(raceAward);
                    dialog.Show();
                });
        }
    }
}