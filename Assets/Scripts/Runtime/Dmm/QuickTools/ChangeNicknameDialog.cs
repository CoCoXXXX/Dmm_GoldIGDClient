using System;
using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Dmm.QuickTools
{
    public class ChangeNicknameDialog : MyDialog
    {
        public Text CostTip;

        public InputField Nickname;

        public Button ConfirmBtn;

        public TextAsset Noun;

        public TextAsset Adjective;

        private readonly List<string> _nounList = new List<string>();

        private readonly List<string> _adjectiveList = new List<string>();

        private IDataContainer<ActionPriceResult> _actionPriceResult;

        private IDataContainer<EditNicknameResult> _editNicknameResult;

        private void OnEnable()
        {
            var dataRepository = GetDataRepository();
            _actionPriceResult = dataRepository.GetContainer<ActionPriceResult>(DataKey.ActionPriceResult);
            _editNicknameResult = dataRepository.GetContainer<EditNicknameResult>(DataKey.EditNicknameResult);
        }

        public override void BeforeShow()
        {
            _nounList.Clear();
            if (!string.IsNullOrEmpty(Noun.text))
            {
                var nouns = Noun.text.Split('\n');
                if (nouns.Length > 0)
                {
                    for (int i = 0; i < nouns.Length; i++)
                    {
                        var noun = nouns[i];
                        _nounList.Add(noun.Trim());
                    }
                }
            }

            _adjectiveList.Clear();
            if (!string.IsNullOrEmpty(Adjective.text))
            {
                var adjs = Adjective.text.Split('\n');
                if (adjs.Length > 0)
                {
                    for (int i = 0; i < adjs.Length; i++)
                    {
                        var adj = adjs[i];
                        _adjectiveList.Add(adj.Trim());
                    }
                }
            }

            RandomOtherNickname();

            CostTip.text = "正在与服务器通信中...";
            ConfirmBtn.interactable = false;

            _newNickname = null;
            _price = null;

            _actionPriceResult.ClearNotInvalidate();
            GetRemoteAPI().RequestActionPrice(ActionCode.EditNickname);

            GetTaskManager().ExecuteTask(CheckActionPriceResult, null);
        }

        public void RandomOtherNickname()
        {
            string noun = null;
            string adj = null;

            Random.InitState((int) DateTime.Now.ToFileTime());

            if (_nounList.Count > 0)
            {
                var nounIdx = Random.Range(0, _nounList.Count);
                noun = _nounList[nounIdx];
            }

            if (_adjectiveList.Count > 0)
            {
                var adjIdx = Random.Range(0, _adjectiveList.Count);
                adj = _adjectiveList[adjIdx];
            }

            Nickname.text = string.Format("{0}的{1}", adj, noun);
        }

        private ActionPriceResult _price;

        private bool CheckActionPriceResult()
        {
            _price = _actionPriceResult.Read(true);
            if (_price == null)
            {
                return false;
            }

            // 检查是否收到了ActionPriceResult。
            var price = _price.price;
            if (price != null)
            {
                CostTip.text = string.Format(
                    "只需{0}{1}，即可修改昵称",
                    price.count,
                    CurrencyType.LabelOf(price.type));

                ConfirmBtn.interactable = true;
            }

            _actionPriceResult.ClearNotInvalidate();

            return true;
        }

        public override void AfterHide()
        {
            _nounList.Clear();
            _adjectiveList.Clear();

            Destroy(gameObject);
        }

        private string _newNickname;

        public void DoChangeNickname()
        {
            _newNickname = Nickname.text;

            var dialogManager = GetDialogManager();
            if (string.IsNullOrEmpty(_newNickname))
            {
                dialogManager.ShowToast("昵称不能为空！", 2);
                return;
            }

            if (!DataUtil.ValidateNickname(_newNickname))
            {
                dialogManager.ShowMessageBox("昵称不能超过16个字，不能包含换行");
                return;
            }

            dialogManager.ShowWaitingDialog(true);

            _editNicknameResult.ClearNotInvalidate();
            GetRemoteAPI().EditNickname(_newNickname);

            GetTaskManager().ExecuteTask(
                CheckEditNicknameResult,
                () => dialogManager.ShowWaitingDialog(false));

            GetAnalyticManager().Event("change_nickname_dialog_apply");
        }

        private bool CheckEditNicknameResult()
        {
            var res = _editNicknameResult.Read(true);
            if (res == null)
            {
                return false;
            }

            var dialogManager = GetDialogManager();
            dialogManager.ShowWaitingDialog(false);

            if (res.result == ResultCode.OK)
            {
                dialogManager.ShowConfirmBox("恭喜您，修改昵称成功！");
                GetRemoteAPI().RequestUserInfo();

                var count = DataUtil.CalculateGeValue(res.cost);
                GetAnalyticManager().Buy("edit_nickname", 1, count);

                Hide();
            }
            else
            {
                if (!string.IsNullOrEmpty(res.msg))
                    dialogManager.ShowToast(res.msg, 3, true);
            }

            return true;
        }
    }
}