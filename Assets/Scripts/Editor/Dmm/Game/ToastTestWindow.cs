using Dmm.Common;
using UnityEditor;
using UnityEngine;

namespace Dmm.Game
{
    public class ToastTestWindow : EditorWindow
    {

        public void OnEnable()
        {
            title = "游滚Toast测试";
        }

        private string _systemText;

        private bool _systemTextError;

        private SystemMsgController _systemMsgController;

        public void OnGUI()
        {
            _systemText = EditorGUILayout.TextField("游滚", _systemText);
            _systemTextError = EditorGUILayout.Toggle("错误", _systemTextError);
            if (GUILayout.Button("发送游滚"))
            {
                _systemMsgController.Show(_systemText, _systemTextError);
            }
        }

    }
}
