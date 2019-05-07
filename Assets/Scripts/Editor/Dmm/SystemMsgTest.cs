using Dmm.Constant;
using Dmm.Msg;
using UnityEditor;
using UnityEngine;

namespace Dmm
{
    public class SystemMsgTest : EditorWindow
    {
        private string _content;
        private int _newFanBei;
        private int _totalFanBei;
        public void OnGUI()
        {
            _content = EditorGUILayout.TextField("内容", _content);
            if (GUILayout.Button("发送游滚"))
            {
                var msg = CmdUtil.CU.SystemMsg(ToastType.Normal, _content, 0);
                var msgRouter = Object.FindObjectOfType<MessageRouter>();
                var msgRepo = msgRouter.GetMsgRepo();
                msgRepo.ReceiveMsg(msg);
            }
            
            _newFanBei = EditorGUILayout.IntField("当前倍数", _newFanBei);
            _totalFanBei = EditorGUILayout.IntField("总倍数", _totalFanBei);

            if (GUILayout.Button("翻倍"))
            {
                var msg = CmdUtil.GU.BFanbei(_newFanBei,_totalFanBei);
                var msgRouter = Object.FindObjectOfType<MessageRouter>();
                var msgRepo = msgRouter.GetMsgRepo();
                msgRepo.ReceiveMsg(msg);
            }
        }
    }
}