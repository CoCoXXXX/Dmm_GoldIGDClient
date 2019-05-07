using System;
using System.Collections;
using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Help;
using Dmm.Log;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Report
{
    public enum ReportType
    {
        Cheat = 1,
        Abuse,
        AD
    }

    public class ReportDialog : MyDialog
    {
        private User _data;

        public int PhoneNumLenght = 11;

        public ReportType CommitReportType;

        public Toggle ChatShieldToggle;

        public GameObject SelectReportType;

        public GameObject CommitReport;

        public InputField ContactInfoTxt;

        public InputField DescriptionTxt;

        private IDataContainer<bool> _shieldChat;

        private IDataContainer<ReportResult> _reportResult;

        private IDataContainer<User> _myUser;

        private void OnEnable()
        {
            _shieldChat = GetContainer<bool>(DataKey.ShieldChat);
            _reportResult = GetContainer<ReportResult>(DataKey.ReportResult);
            _myUser = GetContainer<User>(DataKey.MyUser);
        }

        public override void BeforeShow()
        {
            InitContent();
        }

        public void ApplyData(User data)
        {
            _data = data;
        }

        private void InitContent()
        {
            SelectReportType.SetActive(true);
            CommitReport.SetActive(false);
            ChatShieldToggle.isOn = _shieldChat.Read();
        }

        public void Cheat()
        {
            CommitReportType = ReportType.Cheat;

            if (SelectReportType.activeSelf)
            {
                SelectReportType.SetActive(false);
            }

            if (!CommitReport.activeSelf)
            {
                CommitReport.SetActive(true);
            }
        }

        public void Abuse()
        {
            if (SelectReportType.activeSelf)
            {
                SelectReportType.SetActive(false);
            }

            if (!CommitReport.activeSelf)
            {
                CommitReport.SetActive(true);
            }

            CommitReportType = ReportType.Abuse;
        }

        public void AD()
        {
            if (SelectReportType.activeSelf)
            {
                SelectReportType.SetActive(false);
            }

            if (!CommitReport.activeSelf)
            {
                CommitReport.SetActive(true);
            }

            CommitReportType = ReportType.AD;
        }

        public void ChatShieldValueChanged()
        {
            _shieldChat.Write(ChatShieldToggle.isOn, Time.time);
        }

        public void OnCommitReportBtnClick()
        {
            var dialogManager = GetDialogManager();
            var contactInfo = ContactInfoTxt.text;
            var badPlayer = _data;
            if (badPlayer == null)
            {
                dialogManager.ShowToast("举报玩家失败", 2, true);
                return;
            }

            var description = DescriptionTxt.text;
            var taskManager = GetTaskManager();
            dialogManager.ShowWaitingDialog(true);
            _reportResult.ClearNotInvalidate();
            taskManager.ExecuteTask(CheckCommitReportResult, () => dialogManager.ShowWaitingDialog(false));
            GetReportCommitResult(badPlayer, contactInfo, ((int) CommitReportType), description);
        }

        private bool CheckCommitReportResult()
        {
            var dialogManager = GetDialogManager();
            if (_reportResult.Read() == null)
                return false;

            dialogManager.ShowWaitingDialog(false);

            var data = _reportResult.Read();
            if (data.result == ReportResult.Ok)
            {
                dialogManager.ShowConfirmBox("举报成功，我们已收到您的举报。\n我们会立即安排工作人员处理。\n感谢您对我们游戏的支持。");
                DescriptionTxt.text = "";
                Hide();
            }
            else
            {
                var msg = data.error;
                if (!string.IsNullOrEmpty(msg))
                {
                    dialogManager.ShowToast(msg, 2, true);
                }
                else
                {
                    dialogManager.ShowToast("举报失败，请重新举报，如有疑问请联系客服", 2, true);
                }

                Hide();
            }

            return true;
        }

        //格式 http://api.innertest.com:18080/bad-player/report?type=1&badPlayerNickname=qwe&badPlayerUsername=12313&reporterNickname=reporter&reporterUsername=reportername&description=举报内容
        public void GetReportCommitResult(User badPlayer, string contact, int type, string content)
        {
            if (badPlayer == null)
            {
                return;
            }

            var myUser = _myUser.Read();
            var dialogManager = GetDialogManager();
            var configHolder = GetConfigHolder();
            var myUserName = myUser.Username();
            var myNickName = myUser.Nickname();
            var badPlayerUserName = badPlayer.username;
            var badPlayerNickName = badPlayer.nickname;
            var address = configHolder.ReportUrl;

            if (string.IsNullOrEmpty(address))
            {
                MyLog.ErrorWithFrame(name, "address is null");
                dialogManager.ShowToast("举报失败，请重新举报，如有疑问请联系客服", 2, true);
                Hide();
                return;
            }

            if (badPlayerUserName == myUserName)
            {
                dialogManager.ShowToast("举报失败，请重新举报，如有疑问请联系客服", 2, true);
                dialogManager.ShowWaitingDialog(false);
                Hide();
                return;
            }

            var data = string.Format(
                "type={0}&badPlayerNickname={1}&badPlayerUsername={2}&reporterNickname={3}&reporterUsername={4}&description={5}&contact={6}",
                type, badPlayerNickName, badPlayerUserName, myNickName, myUserName, content, contact);
            var url = address + data;

            StartCoroutine(GetReportCommitResult(url));
        }

        private IEnumerator GetReportCommitResult(string url)
        {
            var dialogManager = GetDialogManager();

            ReportResult res = null;
            var www = new WWW(url);
            yield return www;

            var errorMsg = "举报失败";
            if (www.error != null)
            {
                var errLog = www.error;
                MyLog.ErrorWithFrame("huData", "ReportResult fail :" + errLog);

                dialogManager.ShowConfirmBox(errorMsg);
                res = new ReportResult(CommitIssueResult.Error, errorMsg);
                _reportResult.Write(res, Time.time);

                www.Dispose();
                www = null;
                yield break;
            }

            var data = www.text.ToString();
            MyLog.InfoWithFrame("huData", "ReportResult data is :" + data);

            try
            {
                res = JsonUtility.FromJson<ReportResult>(data);
            }
            catch (Exception e)
            {
                res = new ReportResult(ReportResult.Error, errorMsg);
            }

            www.Dispose();
            www = null;
            _reportResult.Write(res, Time.time);
        }
    }
}