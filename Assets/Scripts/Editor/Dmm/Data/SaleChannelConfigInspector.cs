using System.Collections.Generic;
using Dmm.Base;
using UnityEditor;
using UnityEngine;

namespace Dmm.Data
{
    [CustomEditor(typeof (SaleChannelConfig))]
    public class SaleChannelConfigInspector : Editor
    {

        private SaleChannelConfig _data;

        public void OnEnable()
        {
            _data = target as SaleChannelConfig;
        }

        private string _newSaleChannel;

        private string _errMsg;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("渠道列表：");

            MyEditorStyles.Separator(Color.white);

            if (_data.SaleChannelList == null)
                _data.SaleChannelList = new List<string>();

            for (int i = 0; i < _data.SaleChannelList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i + ": " + _data.SaleChannelList[i]);
                if (GUILayout.Button("删除", EditorStyles.miniButton, MyEditorStyles.MidBtnWidth))
                {
                    _data.SaleChannelList.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }

            MyEditorStyles.Separator(Color.gray);

            EditorGUILayout.BeginHorizontal();
            _newSaleChannel = EditorGUILayout.TextField("新渠道", _newSaleChannel);
            if (GUILayout.Button("添加", EditorStyles.miniButton, MyEditorStyles.MidBtnWidth))
            {
                if (!string.IsNullOrEmpty(_newSaleChannel))
                {
                    _data.SaleChannelList.Add(_newSaleChannel);
                    _newSaleChannel = null;
                    _errMsg = null;
                }
                else
                {
                    _errMsg = "渠道不能为空";
                }
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(_errMsg))
                EditorGUILayout.HelpBox(_errMsg, MessageType.Error);

            MyEditorStyles.Separator(Color.white);

            if (GUILayout.Button("更新渠道数据"))
                EditorUtility.SetDirty(_data);
        }
    }
}
