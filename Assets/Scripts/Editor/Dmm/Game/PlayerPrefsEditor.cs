using Dmm.Base;
using Dmm.Login;
using Dmm.Util;
using UnityEditor;
using UnityEngine;

namespace Dmm.Game
{
    public class PlayerPrefsEditor : EditorWindow
    {

        public void OnGUI()
        {
            EditorGUILayout.LabelField("公告栏时间戳", "" + PrefsUtil.GetLong(BillboardPanel.BillboardTimestampKey, 0));
            // EditorGUILayout.LabelField("公告栏已读", PrefsUtil.GetBool(BillboardPanel.BillboardReadFlagKey, false) ? "是" : "否");
            EditorGUILayout.LabelField("公告栏内容：");
            var content = PrefsUtil.GetString(BillboardPanel.BillboardContentKey, null);
            if (!string.IsNullOrEmpty(content))
                EditorGUILayout.LabelField(content);

            if (GUILayout.Button("清空公告板记录"))
            {
                PrefsUtil.DeleteKey(BillboardPanel.BillboardContentKey);
                PrefsUtil.DeleteKey(BillboardPanel.BillboardTimestampKey);
                // PrefsUtil.DeleteKey(BillboardPanel.BillboardReadFlagKey);
                PrefsUtil.Flush();
            }

            if (GUILayout.Button("清空配置"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
        }

    }
}
