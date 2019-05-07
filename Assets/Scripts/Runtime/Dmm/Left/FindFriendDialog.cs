using Dmm.Dialog;
using UnityEngine.UI;

namespace Dmm.Left
{
    public class FindFriendDialog : MyDialog
    {
        public InputField UsernameEdit;

        public void DoFindFriend()
        {
            if (!UsernameEdit)
            {
                return;
            }

            var username = UsernameEdit.text;
            if (string.IsNullOrEmpty(username))
            {
                GetDialogManager().ShowToast("请输入玩家的用户名", 2);
                return;
            }

            username = username.Trim();
            GetRemoteAPI().FindFriend(username);
        }
    }
}