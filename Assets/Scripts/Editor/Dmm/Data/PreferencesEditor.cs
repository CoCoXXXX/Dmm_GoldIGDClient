using System.IO;
using Dmm.Constant;
using Dmm.Login;
using Dmm.Util;
using UnityEditor;
using UnityEngine;

namespace Dmm.Data
{
    public class PreferencesEditor : EditorWindow
    {
        private string _content;

        private void OnEnable()
        {
            _content = PrefsUtil.GetString( "InitAssetKey", "");
        }

        public void OnGUI()
        {
            if (GUILayout.Button("清空登陆类型"))
            {
                PrefsUtil.SetInt(LoginRecord.LoginTypeKey, LoginRecord.NoLogin);
                PrefsUtil.Flush();
            }

            if (GUILayout.Button("清空DeviceId和Username"))
            {
                PrefsUtil.SetString(LoginRecord.VisitorIdKey, "");
                PrefsUtil.SetString(LoginRecord.VisitorUsernameKey, "");
                PrefsUtil.Flush();
            }

            if (GUILayout.Button("清空公告"))
            {
                PrefsUtil.SetString(BillboardPanel.BillboardContentKey, "");
                PrefsUtil.SetLong(BillboardPanel.BillboardTimestampKey, 0);
                PrefsUtil.Flush();
            }

            if (GUILayout.Button("清空选牌类型引导"))
            {
                PrefsUtil.SetBool(PrefsKeys.HasGuideSelectPoker, false);
                PrefsUtil.Flush();
            }

            if (GUILayout.Button(("清空记录选牌的方式")))
            {
                PrefsUtil.DeleteKey(PrefsKeys.XuanDanZhangKey);
                PrefsUtil.DeleteKey(PrefsKeys.HasGuideSelectPoker);
                PrefsUtil.Flush();
            }

            if (GUILayout.Button("清空更新记录"))
            {
                PrefsUtil.DeleteKey(LoginRecord.DontUpdateDateKey);
                PrefsUtil.Flush();
            }

            _content = EditorGUILayout.TextField("版本号", _content);            
            
            if (GUILayout.Button("清空初始化Cache记录"))
            {
                PrefsUtil.SetString( "InitAssetKey", _content);;
                var key = PrefsKeys.AssetBundleInitializedKey + _content;
                PrefsUtil.DeleteKey(key);
                PrefsUtil.Flush();
            }

            if (GUILayout.Button("清空所有记录的更新的AssetBundle记录"))
            {
                //路径  
                var fullPath = Application.streamingAssetsPath;

                //获取指定路径下面的所有资源文件  
                if (Directory.Exists(fullPath))
                {
                    var direction = new DirectoryInfo(fullPath);
                    var files = direction.GetFiles("*", SearchOption.AllDirectories);

                    Debug.Log(files.Length);

                    for (var i = 0; i < files.Length; i++)
                    {
                        if (files[i].Name.EndsWith(".meta") || files[i].Name.EndsWith(".manifest"))
                        {
                            continue;
                        }

                        PrefsUtil.DeleteKey(files[i].Name);
                        PrefsUtil.Flush();
                        Debug.Log("已清空:" + files[i].Name);
                    }
                }
            }
        }
    }
}