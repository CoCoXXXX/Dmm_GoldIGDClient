using com.morln.game.gd.command;
using Dmm.Msg;

namespace Dmm.MsgLogic
{
    /// <summary>
    /// 过滤系统消息中的文字。
    /// </summary>
    public class TextFilter : IMessageFilter
    {
        #region Filter接口

        public bool Filter(ProtoMessage msg)
        {
            switch (msg.Type)
            {
                case CmdType.PU.TOAST:
                case CmdType.HU.TOAST:
                case CmdType.GU.TOAST_V6:
                case CmdType.CU.TOAST_V6:
                    ProcessToast(msg);
                    break;

                case CmdType.CU.B_SYS_TEXT_MSG_V6:
                    ProcessSysTextMsg(msg);
                    break;

                case CmdType.PU.CLIENT_VERSION_RESULT:
                    ProcessClientVersionResult(msg);
                    break;

                case CmdType.HU.MAIL_BRIEF_LIST_RESULT:
                    ProcessMailBriefList(msg);
                    break;

                case CmdType.HU.MAIL_CONTENT_RESULT:
                    ProcessMailContent(msg);
                    break;
            }

            // 并不会过滤掉任何消息。
            return false;
        }

        #endregion

        #region 处理不同消息

        /// <summary>
        /// 处理Toast消息。
        /// </summary>
        /// <param name="msg"></param>
        private void ProcessToast(ProtoMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            var toast = msg.Content as Toast;
            if (toast == null)
            {
                return;
            }

            toast.content = ReplaceContent(toast.content);
        }

        /// <summary>
        /// 处理系统消息。
        /// </summary>
        /// <param name="msg"></param>
        private void ProcessSysTextMsg(ProtoMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            var sysMsg = msg.Content as BSysTextMsg;
            if (sysMsg == null)
            {
                return;
            }

            sysMsg.msg = ReplaceContent(sysMsg.msg);
        }

        /// <summary>
        /// 处理ClientVersionResult。
        /// </summary>
        /// <param name="msg"></param>
        private void ProcessClientVersionResult(ProtoMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            var versionResult = msg.Content as VersionResult;
            if (versionResult == null)
            {
                return;
            }

            var billboard = versionResult.billboard_6_2;
            if (billboard == null)
            {
                return;
            }

            billboard.content = ReplaceContent(billboard.content);
        }

        private void ProcessMailBriefList(ProtoMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            var res = msg.Content as MailBriefListResult;
            if (res == null)
            {
                return;
            }

            var list = res.mail_brief;
            if (list.Count <= 0)
            {
                return;
            }

            foreach (var m in list)
            {
                if (m == null)
                {
                    continue;
                }

                m.title = ReplaceContent(m.title);
                m.content = ReplaceContent(m.content);
            }
        }

        private void ProcessMailContent(ProtoMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            var res = msg.Content as MailContentResult;
            if (res == null)
            {
                return;
            }

            var mail = res.content;

            if (mail == null)
            {
                return;
            }

            mail.title = ReplaceContent(mail.title);
            mail.content = ReplaceContent(mail.content);
        }

        #endregion

        #region 替换文本

        public string SearchStr;

        public string ReplaceStr;

        private string ReplaceContent(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }

            if (string.IsNullOrEmpty(SearchStr) || string.IsNullOrEmpty(ReplaceStr))
            {
                return content;
            }

            return content.Replace(SearchStr, ReplaceStr);
        }

        #endregion
    }
}