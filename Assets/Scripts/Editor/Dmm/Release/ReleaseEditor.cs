using System.IO;
using Dmm.Constant;
using Dmm.Data;
using UnityEditor;
using UnityEngine;

namespace Dmm.Release
{
    public class ReleaseEditor : EditorWindow
    {
        #region 版本

        private ClientConfig _client;

        private SaleChannelConfig _saleChannelConfig;

        #endregion

        #region 产品

        private ProductConfig[] _products;

        private string[] _productLabels;

        private int IndexOfProduct(string product)
        {
            if (string.IsNullOrEmpty(product))
            {
                return -1;
            }

            if (_products == null)
            {
                return -1;
            }

            for (int i = 0; i < _products.Length; i++)
            {
                if (product.Equals(_products[i].ProductName))
                {
                    return i;
                }
            }

            return -1;
        }

        private ProductConfig ProductOf(int index)
        {
            if (index < 0 || index >= _products.Length)
            {
                return null;
            }

            return _products[index];
        }

        #endregion

        #region 安卓插件

        private const int XiaoMi = 1;

        private const int YingYongBao = 2;

        private void ReplaceAndroidPlugin(int target)
        {
            // 先删除插件文件夹中的文件。
            DeleteAndroidPluginFile("AndroidManifest.xml");
            DeleteAndroidPluginFolder("bin");
            DeleteAndroidPluginFolder("assets");
            DeleteAndroidPluginFolder("res");

            string saleChannel = null;
            switch (target)
            {
                case XiaoMi:
                    saleChannel = "XiaoMi";
                    break;
                case YingYongBao:
                    saleChannel = "YingYongBao";
                    break;
            }

            CopyAndroidPluginFile(saleChannel, "AndroidManifest.xml");
            CopyAndroidPluginDirectory(saleChannel, "bin");
            CopyAndroidPluginDirectory(saleChannel, "res");
            CopyAndroidPluginDirectory(saleChannel, "assets");
        }

        #region 复制文件

        private void CopyAndroidPluginDirectory(string saleChannel, string dir)
        {
            if (string.IsNullOrEmpty(saleChannel) || string.IsNullOrEmpty(dir))
            {
                return;
            }

            string sourcePath = Application.dataPath + "/AndroidSDK/" + saleChannel + "/" + dir;
            string destinationPath = Application.dataPath + "/Plugins/Android/" + dir;

            CopyDirectory(sourcePath, destinationPath);
        }

        private void CopyDirectory(string sourcePath, string destinationPath)
        {
            if (string.IsNullOrEmpty(sourcePath) ||
                !Directory.Exists(sourcePath) ||
                string.IsNullOrEmpty(destinationPath))
            {
                return;
            }

            DirectoryInfo info = new DirectoryInfo(sourcePath);
            Directory.CreateDirectory(destinationPath);
            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                string destName = Path.Combine(destinationPath, fsi.Name);
                if (fsi is System.IO.FileInfo)
                    File.Copy(fsi.FullName, destName, true);
                else
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectory(fsi.FullName, destName);
                }
            }
        }

        private void CopyAndroidPluginFile(string saleChannel, string file)
        {
            if (string.IsNullOrEmpty(saleChannel) || string.IsNullOrEmpty(file))
            {
                return;
            }

            string sourcePath = Application.dataPath + "/AndroidSDK/" + saleChannel + "/" + file;
            if (!File.Exists(sourcePath))
            {
                return;
            }

            File.Copy(sourcePath, Application.dataPath + "/Plugins/Android/" + file, true);
        }

        #endregion

        #region 删除文件

        private void DeleteAndroidPluginFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            string path = Application.dataPath + "/Plugins/Android/" + fileName;
            if (!File.Exists(path))
            {
                return;
            }

            File.Delete(path);
        }

        private void DeleteAndroidPluginFolder(string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                return;
            }

            string path = Application.dataPath + "/Plugins/Android/" + dir;
            DeleteFolder(path);
        }

        private void DeleteFolder(string dir)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }

            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);
                }
                else
                {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0)
                    {
                        DeleteFolder(d1.FullName);
                    }

                    Directory.Delete(d);
                }
            }
        }

        #endregion

        #endregion

        public void OnEnable()
        {
            titleContent = new GUIContent("版本发布配置");

            _client = AssetDatabase.LoadAssetAtPath("Assets/Config/ClientVersion.asset", typeof(ClientConfig)) as ClientConfig;
            _saleChannelConfig = AssetDatabase.LoadAssetAtPath("Assets/Config/SaleChannel.asset", typeof(SaleChannelConfig)) as SaleChannelConfig;

            var files = Directory.GetFiles(Application.dataPath + "/Config/Product/", "*.asset");
            if (files != null && files.Length > 0)
            {
                _products = new ProductConfig[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    var fileName = Path.GetFileName(files[i]);
                    _products[i] = AssetDatabase.LoadAssetAtPath("Assets/Config/Product/" + fileName, typeof(ProductConfig)) as ProductConfig;
                }
            }

            if (_products != null && _products.Length > 0)
            {
                _productLabels = new string[_products.Length];
                for (int i = 0; i < _products.Length; i++)
                {
                    var p = _products[i];
                    _productLabels[i] = p.DisplayName;
                }
            }
        }

        public void OnGUI()
        {
            var selectedProduct = IndexOfProduct(_client.Product.ProductName);
            selectedProduct = EditorGUILayout.Popup("产品", selectedProduct, _productLabels);
            var product = ProductOf(selectedProduct);
            _client.Product = product;

            _client.ClientVersion = EditorGUILayout.IntField("客户端版本号", _client.ClientVersion);

            if (_saleChannelConfig)
            {
                var saleChannels = _saleChannelConfig.Values();
                var selected = _saleChannelConfig.IndexOf(_client.SaleChannel);
                selected = EditorGUILayout.Popup("渠道", selected, saleChannels);
                _client.SaleChannel = _saleChannelConfig.SaleChannelOf(selected);
            }
            else
            {
                _client.SaleChannel = EditorGUILayout.TextField("渠道", _client.SaleChannel);
            }

            EditorGUILayout.LabelField("平台", Platform.LabelOf(_client.Platform));
            _client.XiaoMiMode = EditorGUILayout.Toggle("小米模式", _client.XiaoMiMode);

            if (GUILayout.Button("更新数据"))
            {
                EditorUtility.SetDirty(_client);
            }

            // TODO 完成更牛叉的分块显示。
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("安卓插件");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("应用宝"))
            {
                ReplaceAndroidPlugin(YingYongBao);
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("小米"))
            {
                ReplaceAndroidPlugin(XiaoMi);
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();
        }

    }
}