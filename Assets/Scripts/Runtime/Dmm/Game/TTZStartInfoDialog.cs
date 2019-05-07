using System.Collections;
using com.morln.game.gd.command;
using Dmm.Dialog;
using Dmm.PokerLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Game
{
    public class TTZStartInfoDialog : MyDialog
    {
        public CardHelper CardHelper;

        public Image MingPai;

        public Text Description;

        public float ShowTime = 5;

        public void ShowAndAutoHide()
        {
            Show();

            StartCoroutine(AutoHideCoroutine());
        }

        private IEnumerator AutoHideCoroutine()
        {
            yield return new WaitForSeconds(ShowTime);

            Hide();
        }

        public void ApplyData(TTZStartBroadcast msg)
        {
            if (msg == null)
            {
                ResetContent();
                return;
            }

            if (!MingPai.gameObject.activeSelf)
            {
                MingPai.gameObject.SetActive(true);
            }
            var poker = new Poker(msg.mingpai_id);
            MingPai.sprite = CardHelper.GetEndRoundCard(poker.NumType, poker.SuitType);

            if (!Description.gameObject.activeSelf)
            {
                Description.gameObject.SetActive(true);
            }
            Description.text = msg.description;
        }

        public void ResetContent()
        {
            MingPai.gameObject.SetActive(false);
            Description.text = "";
            Description.gameObject.SetActive(false);
        }
    }
}