using System;
using System.Collections;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Help;
using Dmm.Log;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class HelpDialog : MyDialog
    {
        #region 内容

        public const string ServiceContentKey = "ServiceContent";

        public const string AboutContentKey = "AboutContentKey";

        private string _helpContent =
            "官方客服QQ：800026732\n" +
            "官方粉丝QQ群：98818761\n" +
            "官方公众号：爱掼蛋官方";

        private string _aboutContent =
            "著作权人：\n南京呆萌猫网络科技有限公司\n" +
            "出版服务单位：\n江苏凤凰电子音像出版社有限公司\n" +
            "批准文号：\n新广出审 [2016] 674号\n" +
            "出版物号：\n978-7-89400-652-8";

        private string _serviceQQ = "800026732";

        private string _serviceQQGroup = "98818761";

        private string _serviceContent = "爱掼蛋官方";

        private void InitContent()
        {
            HelpText.text = _helpContent;
            AboutUsText.text = _aboutContent;

            PayProblemToggle.isOn = false;
            GameProblemToggle.isOn = false;
            OtherProblemToggle.isOn = false;

            CommitProblemType = ProblemType.None;
            OnAboutUsBtnClick();
        }

        #endregion

        public enum ProblemType
        {
            None = 0,
            Pay = 100,
            Game = 200,
            Other = 300
        }

        public int PhoneNumLenght = 11;

        public ProblemType CommitProblemType;

        public Text HelpText;

        public Text AboutUsText;

        public GameObject AboutUsSelectedObj;

        public GameObject FeedbackSelectedObj;

        public GameObject AboutUsContent;

        public GameObject FeedbackContent;

        public GameObject HistoryRecordContent;

        public Toggle PayProblemToggle;

        public Toggle GameProblemToggle;

        public Toggle OtherProblemToggle;

        public InputField ContactInfoTxt;

        public InputField DescriptionTxt;

        public Text NoRecordText;

        public HistoryRecordList HistoryRecordList;

        private IDataContainer<VersionResult> _versionContainer;

        private IDataContainer<string> _serviceQQContainer;

        private IDataContainer<string> _serviceQQGroupContainer;

        private IDataContainer<string> _serviceContentContainer;

        private IDataContainer<string> _aboutContentContainer;

        private IDataContainer<HistoryRecordResult> _historyRecordResult;

        private IDataContainer<CommitIssueResult> _commitIssueResult;

        private IDataContainer<User> _user;

        public override void BeforeShow()
        {
            InitContent();
        }

        private float _refreshTime;

        private void OnEnable()
        {
            PayProblemToggle.onValueChanged.AddListener(OnPayProblemToggleValueChanged);
            GameProblemToggle.onValueChanged.AddListener(OnGameProblemToggleValueChanged);
            OtherProblemToggle.onValueChanged.AddListener(OnOtherProblemToggleValueChanged);

            _versionContainer = GetContainer<VersionResult>(DataKey.VersionResult);
            _serviceQQContainer = GetContainer<string>(DataKey.ServiceQQ);
            _serviceQQGroupContainer = GetContainer<string>(DataKey.ServiceQQGroup);
            _serviceContentContainer = GetContainer<string>(DataKey.ServiceContent);
            _aboutContentContainer = GetContainer<string>(DataKey.AboutContent);
            _historyRecordResult = GetContainer<HistoryRecordResult>(DataKey.HistoryRecordResult);
            _commitIssueResult = GetContainer<CommitIssueResult>(DataKey.CommitIssueResult);
            _user = GetContainer<User>(DataKey.MyUser);
        }

        public void Update()
        {
            if (_refreshTime >= _versionContainer.Timestamp)
            {
                return;
            }

            _refreshTime = _versionContainer.Timestamp;

            var serviceQQ = _serviceQQContainer.Read();
            var helpContent = "";
            if (!string.IsNullOrEmpty(serviceQQ))
            {
                helpContent = "官方客服QQ：" + serviceQQ;
                _serviceQQ = serviceQQ;
            }

            var serviceQQGroup = _serviceQQGroupContainer.Read();
            if (!string.IsNullOrEmpty(serviceQQGroup))
            {
                helpContent = helpContent + "\n官方粉丝QQ群：" + serviceQQGroup;
                _serviceQQGroup = serviceQQGroup;
            }

            var serviceContent = _serviceContentContainer.Read();
            if (!string.IsNullOrEmpty(serviceContent))
            {
                helpContent = helpContent + "\n官方公众号：" + serviceContent;
                _serviceContent = serviceContent;
            }

            if (!string.IsNullOrEmpty(helpContent))
            {
                _helpContent = helpContent;
            }

            var aboutContentContent = _aboutContentContainer.Read();
            if (!string.IsNullOrEmpty(aboutContentContent))
            {
                _aboutContent = aboutContentContent;
            }

            HelpText.text = _helpContent;
            AboutUsText.text = _aboutContent;
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }

        public void OnAboutUsBtnClick()
        {
            if (!AboutUsContent.activeSelf)
            {
                AboutUsContent.SetActive(true);
            }

            if (FeedbackContent.activeSelf)
            {
                FeedbackContent.SetActive(false);
            }

            if (HistoryRecordContent.activeSelf)
            {
                HistoryRecordContent.SetActive(false);
            }

            if (!AboutUsSelectedObj.activeSelf)
            {
                AboutUsSelectedObj.SetActive(true);
            }

            if (FeedbackSelectedObj.activeSelf)
            {
                FeedbackSelectedObj.SetActive(false);
            }
        }

        public void OnFeedbackBtnClick()
        {
            if (!FeedbackContent.activeSelf)
            {
                FeedbackContent.SetActive(true);
            }

            if (AboutUsContent.activeSelf)
            {
                AboutUsContent.SetActive(false);
            }

            if (HistoryRecordContent.activeSelf)
            {
                HistoryRecordContent.SetActive(false);
            }

            if (!FeedbackSelectedObj.activeSelf)
            {
                FeedbackSelectedObj.SetActive(true);
            }

            if (AboutUsSelectedObj.activeSelf)
            {
                AboutUsSelectedObj.SetActive(false);
            }
        }

        public void OnHistoryRecordBtnClick()
        {
            var taskManager = GetTaskManager();
            var dialogManager = GetDialogManager();
            dialogManager.ShowWaitingDialog(true);
            _historyRecordResult.ClearNotInvalidate();
            taskManager.ExecuteTask(CheckHistoryRecordResult, () => dialogManager.ShowWaitingDialog(false));
            GetHistoryRecordResult();
        }

        private bool CheckHistoryRecordResult()
        {
            var dialogManager = GetDialogManager();
            var historyRecordResult = _historyRecordResult.Read();
            if (historyRecordResult == null)
            {
                return false;
            }

            dialogManager.ShowWaitingDialog(false);

            var data = historyRecordResult;
            if (data.result == CommitIssueResult.Ok)
            {
                var dataCount = GetHistoryRecordResultCount();

                if (!HistoryRecordContent.activeSelf)
                {
                    HistoryRecordContent.SetActive(true);
                }

                if (FeedbackContent.activeSelf)
                {
                    FeedbackContent.SetActive(false);
                }

                if (AboutUsContent.activeSelf)
                {
                    AboutUsContent.SetActive(false);
                }

                NoRecordText.gameObject.SetActive(dataCount < 0);
                if (dataCount > 0)
                {
                    HistoryRecordList.Init();
                }
            }
            else
            {
                var msg = data.error;
                if (!string.IsNullOrEmpty(msg))
                {
                    dialogManager.ShowConfirmBox(msg);
                }
                else
                {
                    dialogManager.ShowConfirmBox("获取历史记录失败，如有疑问请联系客服");
                }
            }

            return true;
        }

        private int GetHistoryRecordResultCount()
        {
            var historyRecordResult = _historyRecordResult.Read();
            if (historyRecordResult == null)
            {
                return 0;
            }
            var data = historyRecordResult.data;
            if (data == null)
            {
                return 0;
            }

            return data.Length;
        }

        public void OnBackBtnClicked()
        {
            if (HistoryRecordContent.activeSelf)
            {
                HistoryRecordContent.SetActive(false);
            }

            if (!FeedbackContent.activeSelf)
            {
                FeedbackContent.SetActive(true);
            }
        }

        public void OnCommitProblemBtnClick()
        {
            var dialogManager = GetDialogManager();
            var contactInfo = ContactInfoTxt.text;
            if (CommitProblemType == ProblemType.None)
            {
                dialogManager.ShowConfirmBox("请选择反馈问题的类型");
                return;
            }
            if (string.IsNullOrEmpty(contactInfo))
            {
                dialogManager.ShowConfirmBox("请输入您的联系方式");
                return;
            }
            if (contactInfo.Length != PhoneNumLenght)
            {
                dialogManager.ShowConfirmBox("请输入正确的联系方式");
                return;
            }
            try
            {
                var phoneNum = long.Parse(contactInfo);
            }
            catch (Exception e)
            {
                dialogManager.ShowConfirmBox("请输入正确的联系方式");
                return;
            }

            var description = DescriptionTxt.text;

            if (string.IsNullOrEmpty(description))
            {
                dialogManager.ShowConfirmBox("请输入您的问题描述");
                return;
            }

            var taskManager = GetTaskManager();
            dialogManager.ShowWaitingDialog(true);
            _commitIssueResult.ClearNotInvalidate();
            taskManager.ExecuteTask(CheckCommitIssueResult, () => dialogManager.ShowWaitingDialog(false));
            GetIssueCommitResult(contactInfo, ((int) CommitProblemType), description);
        }

        private bool CheckCommitIssueResult()
        {
            var dialogManager = GetDialogManager();
            var commitIssueResult = _commitIssueResult.Read();
            if (commitIssueResult == null)
            {
                return false;
            }

            dialogManager.ShowWaitingDialog(false);

            var data = commitIssueResult;
            if (data.result == CommitIssueResult.Ok)
            {
                dialogManager.ShowConfirmBox("问题已提交，请等待客服回复！\n可在历史记录中查看反馈结果");
                DescriptionTxt.text = "";
            }
            else
            {
                var msg = data.error;
                if (!string.IsNullOrEmpty(msg))
                {
                    dialogManager.ShowConfirmBox(msg);
                }
                else
                {
                    dialogManager.ShowConfirmBox("问题提交失败，如有疑问请联系客服");
                }
            }

            return true;
        }

        public void OnPayProblemToggleValueChanged(bool value)
        {
            if (value)
            {
                CommitProblemType = ProblemType.Pay;
            }
        }

        public void OnGameProblemToggleValueChanged(bool value)
        {
            if (value)
            {
                CommitProblemType = ProblemType.Game;
            }
        }

        public void OnOtherProblemToggleValueChanged(bool value)
        {
            if (value)
            {
                CommitProblemType = ProblemType.Other;
            }
        }

        public void CopyQQ()
        {
            var clipboardManager = GetClipboardManager();
            var dialogManager = GetDialogManager();
            if (string.IsNullOrEmpty(_serviceQQ))
            {
                return;
            }
            try
            {
                clipboardManager.CopyToClipboard(_serviceQQ);
            }
            catch (Exception e)
            {
                dialogManager.ShowToast("复制官方客服QQ失败", 2, true);
                return;
            }

            dialogManager.ShowToast("已复制官方客服QQ到粘贴板", 2);
        }

        public void CopyQQGroup()
        {
            var clipboardManager = GetClipboardManager();
            var dialogManager = GetDialogManager();
            if (string.IsNullOrEmpty(_serviceQQGroup))
            {
                return;
            }
            try
            {
                clipboardManager.CopyToClipboard(_serviceQQGroup);
            }
            catch (Exception e)
            {
                dialogManager.ShowToast("复制官方粉丝QQ群号失败", 2, true);
                return;
            }

            dialogManager.ShowToast("已复制官方粉丝QQ群号到粘贴板", 2);
        }

        public void CopyServiceContent()
        {
            var clipboardManager = GetClipboardManager();
            var dialogManager = GetDialogManager();
            if (string.IsNullOrEmpty(_serviceContent))
            {
                return;
            }
            try
            {
                clipboardManager.CopyToClipboard(_serviceContent);
            }
            catch (Exception e)
            {
                dialogManager.ShowToast("复制官方公众号失败", 2, true);
                return;
            }

            dialogManager.ShowToast("已复制官方公众号到粘贴板", 2);
        }

        #region 访问Url

        //格式 http://localhost/submit_issue?username=huang&contact=18021401774&type=100&content=反馈内容
        public void GetIssueCommitResult(string contact, int type, string content)
        {
            var configHolder = GetConfigHolder();
            var user = _user.Read();
            var userName = "";
            if (user == null)
            {
                userName = "";
            }
            else
            {
                userName = user.username;
            }

            var address = configHolder.IssueSubmitUrl;
            var data = string.Format("username={0}&contact={1}&type={2}&content={3}",
                userName, contact, type.ToString(), content);
            var url = address + data;

            StartCoroutine(GetIssueCommitResult(url));
        }

        private IEnumerator GetIssueCommitResult(string url)
        {
            var dialogManager = GetDialogManager();

            CommitIssueResult res = null;
            var www = new WWW(url);
            yield return www;

            var errorMsg = "提交反馈失败";
            if (www.error != null)
            {
                var errLog = www.error;
                MyLog.ErrorWithFrame("huData", "GetIssueCommitResult fail :" + errLog);

                dialogManager.ShowConfirmBox(errorMsg);
                res = new CommitIssueResult(CommitIssueResult.Error, errorMsg);
                _commitIssueResult.Write(res, Time.time);

                www.Dispose();
                www = null;
                yield break;
            }

            var data = www.text.ToString();
            MyLog.InfoWithFrame("huData", "IssueCommitResult data is :" + data);

            try
            {
                res = JsonUtility.FromJson<CommitIssueResult>(data);
            }
            catch (Exception e)
            {
                res = new CommitIssueResult(CommitIssueResult.Error, errorMsg);
            }

            www.Dispose();
            www = null;
            _commitIssueResult.Write(res, Time.time);
        }

        //格式 http://localhost/my_issues?username=黄其帆
        public void GetHistoryRecordResult()
        {
            var user = _user.Read();
            var configHolder = GetConfigHolder();
            var userName = "";
            if (user == null)
            {
                userName = "";
            }
            else
            {
                userName = user.username;
            }

            var address = configHolder.IssueHistoryUrl;
            var data = string.Format("username={0}", userName);
            var url = address + data;

            StartCoroutine(GetHistoryRecordResult(url));
        }

        private IEnumerator GetHistoryRecordResult(string url)
        {
            var dialogManager = GetDialogManager();

            HistoryRecordResult res = null;
            var www = new WWW(url);
            yield return www;

            var errorMsg = "获取历史记录失败";
            if (www.error != null)
            {
                var errLog = www.error;

                MyLog.ErrorWithFrame("huData", "GetHistoryRecordResult fail :" + errLog);

                dialogManager.ShowConfirmBox(errorMsg);
                res = new HistoryRecordResult(HistoryRecordResult.Error, errorMsg, null);
                _historyRecordResult.Write(res, Time.time);

                www.Dispose();
                www = null;
                yield break;
            }

            var data = www.text.ToString();
            MyLog.InfoWithFrame("huData", "IssueCommitResult data is :" + data);
            try
            {
                res = JsonUtility.FromJson<HistoryRecordResult>(data);
            }
            catch (Exception e)
            {
                res = new HistoryRecordResult(HistoryRecordResult.Error, errorMsg, null);
            }

            www.Dispose();
            www = null;
            _historyRecordResult.Write(res, Time.time);
        }

        #endregion
    }
}