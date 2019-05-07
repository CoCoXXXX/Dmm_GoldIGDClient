using System;
using System.Collections;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Util;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Dmm.MoreFunction
{
    public class RealNameDialog : MyDialog
    {
        public InputField RealName;

        public InputField IdentityCardNum;

        private IDataContainer<RealNameResult> _realNameResult;

        private IDataContainer<User> _myUser;

        private void OnEnable()
        {
            _realNameResult = GetDataRepository().GetContainer<RealNameResult>(DataKey.RealNameResult);
            _myUser = GetDataRepository().GetContainer<User>(DataKey.MyUser);
        }

        public void Commit()
        {
            var dialog = GetDialogManager();
            var realName = RealName.text;
            var num = IdentityCardNum.text;

            if (!StringUtil.CheckIsChineseName(realName))
            {
                dialog.ShowToast("您输入的姓名格式非法，请输入您的真实姓名。", 2, true);
                return;
            }

            if (!StringUtil.CheckIDCard(num))
            {
                dialog.ShowToast("您输入的身份证号格式非法，请输入您真实的身份证号。", 2, true);
                return;
            }

            var taskManager = GetTaskManager();
            dialog.ShowWaitingDialog(true);
            _realNameResult.ClearNotInvalidate();
            taskManager.ExecuteTask(CheckRealNameResult, () => dialog.ShowWaitingDialog(false));

            GetRealNameResult(realName, num);
        }

        //格式 http://114.55.30.148:18080/realinfo-service/submitInfo?username=xxx&realname=黄其帆&idNumber=xxxx
        public void GetRealNameResult(string realName, string idNum)
        {
            var configHolder = GetConfigHolder();
            var user = _myUser.Read();
            var userName = "";
            if (user == null)
            {
                userName = "";
            }
            else
            {
                userName = user.username;
            }

            var address = configHolder.RealNameUrl;
            var data = string.Format("username={0}&realname={1}&idNumber={2}",
                userName, realName, idNum);
            var url = address + data;

            StartCoroutine(GetRealNameResult(url));
        }

        private IEnumerator GetRealNameResult(string url)
        {
            var dialogManager = GetDialogManager();

            RealNameResult res = null;
            var www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            var errorMsg = "提交实名认证失败了T_T";
            if (www.error != null)
            {
                var errLog = www.error;
                MyLog.ErrorWithFrame("huData", "RealNameResult fail :" + errLog);

                dialogManager.ShowConfirmBox(errorMsg);
                res = new RealNameResult(RealNameResult.Error, errorMsg);
                _realNameResult.Write(res, Time.time);

                www.Dispose();
                www = null;
                yield break;
            }

            var data = www.downloadHandler.text.ToString();
            MyLog.InfoWithFrame("huData", "RealNameResult data is :" + data);

            try
            {
                res = JsonUtility.FromJson<RealNameResult>(data);
            }
            catch (Exception e)
            {
                res = new RealNameResult(RealNameResult.Error, errorMsg);
            }

            www.Dispose();
            www = null;
            _realNameResult.Write(res, Time.time);
        }

        private bool CheckRealNameResult()
        {
            var dialogManager = GetDialogManager();
            var realNameResult = _realNameResult.Read();
            if (realNameResult == null)
            {
                return false;
            }

            dialogManager.ShowWaitingDialog(false);

            var data = realNameResult;
            if (data.result == RealNameResult.Ok)
            {
                dialogManager.ShowConfirmBox("提交成功");
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
                    dialogManager.ShowConfirmBox("实名信息提交失败，如有疑问请联系客服");
                }
            }

            return true;
        }
    }
}