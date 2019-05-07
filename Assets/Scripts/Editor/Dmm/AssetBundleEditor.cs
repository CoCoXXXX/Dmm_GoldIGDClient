using System.IO;
using Dmm.Util;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Dmm
{
    public class AssetBundleEditor : EditorWindow
    {
        private const string LastAssetBundleTargetKey = "LastBuildAssetBundleTarget";
        private const string LastAssetBundleOptionsKey = "LastBuildAssetBundleOptions";

        private BuildTarget _target;
        private BuildAssetBundleOptions _options;

        private void OnEnable()
        {
            titleContent = new GUIContent("ABEditor");
            _target = (BuildTarget) PlayerPrefs.GetInt(LastAssetBundleTargetKey, (int) BuildTarget.iOS);
            _options = (BuildAssetBundleOptions) PlayerPrefs.GetInt(LastAssetBundleOptionsKey, (int) BuildAssetBundleOptions.None);
        }

        private void OnGUI()
        {
            var outputPath = Path.Combine(Application.dataPath, "StreamingAssets");
            _target = (BuildTarget) EditorGUILayout.EnumPopup("目标平台", _target);
            _options = (BuildAssetBundleOptions) EditorGUILayout.EnumPopup("选项", _options);

            if (GUILayout.Button("构建AssetBundle"))
            {
                PlayerPrefs.SetInt(LastAssetBundleTargetKey, (int) _target);
                PlayerPrefs.SetInt(LastAssetBundleOptionsKey, (int) _options);
                PlayerPrefs.Save();
                
                BuildPipeline.BuildAssetBundles(outputPath, _options, _target);
            }
        }
    }
}