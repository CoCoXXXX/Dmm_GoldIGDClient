using Dmm.Constant;
using UnityEditor;
using UnityEngine;

namespace Dmm.Data
{
    [CustomEditor(typeof (ClientConfig))]
    public class ClientConfigInspector : Editor
    {
        private ClientConfig _data;

        private SaleChannelConfig _saleChannelConfig;

        public void OnEnable()
        {
            _data = target as ClientConfig;
            _saleChannelConfig = AssetDatabase.LoadAssetAtPath("Assets/Config/SaleChannel.asset", typeof (SaleChannelConfig)) as SaleChannelConfig;
            
            _platformLabels = new string[Platform.Values.Length];
            for (int i = 0; i < Platform.Values.Length; i++)
            {
                _platformLabels[i] = Platform.LabelOf(Platform.Values[i]);
            }
        }

        private int IndexOfPlatform(int platform)
        {
            for (int i = 0; i < Platform.Values.Length; i++)
            {
                if (Platform.Values[i] == platform)
                {
                    return i;
                }
            }

            return -1;
        }

        private string[] _platformLabels;

        public override void OnInspectorGUI()
        {
            _data.Product = EditorGUILayout.ObjectField("当前产品", _data.Product, typeof(ProductConfig), false) as ProductConfig;
            EditorGUILayout.LabelField("产品名", _data.Product != null? _data.Product.DisplayName : "无");
            
            EditorGUILayout.Separator();
            
            _data.ClientVersion = EditorGUILayout.IntField("客户端版本号", _data.ClientVersion);

            EditorGUILayout.Separator();
            
            _data.VersionTxt = EditorGUILayout.TextField("显示版本号", _data.VersionTxt);
            
            EditorGUILayout.Separator();
            
            if (_saleChannelConfig)
            {
                var saleChannels = _saleChannelConfig.Values();
                var selected = _saleChannelConfig.IndexOf(_data.SaleChannel);
                selected = EditorGUILayout.Popup("渠道", selected, saleChannels);
                _data.SaleChannel = _saleChannelConfig.SaleChannelOf(selected);
            }
            else
            {
                _data.SaleChannel = EditorGUILayout.TextField("渠道", _data.SaleChannel);
            }

            EditorGUILayout.Separator();
            
            var selectedPlatform = IndexOfPlatform(_data.Platform);
            selectedPlatform = EditorGUILayout.Popup("平台", selectedPlatform, _platformLabels);
            _data.Platform = Platform.Values[selectedPlatform];
            
            EditorGUILayout.Separator();
            
            _data.XiaoMiMode = EditorGUILayout.Toggle("小米模式", _data.XiaoMiMode);

            EditorGUILayout.Separator();
            
            if (GUILayout.Button("更新数据"))
            {
                EditorUtility.SetDirty(_data);
            }
        }

    }
}
