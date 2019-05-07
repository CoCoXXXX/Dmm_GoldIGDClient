#if UNITY_IOS
using System.IO;
using Dmm.Data;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

#endif

namespace Dmm.Release
{
    public class IosPostBuildProcess
    {
#if UNITY_IOS
        [PostProcessBuild]
        public static void PostBuildProcess(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            var clientConfig =
                AssetDatabase.LoadAssetAtPath("Assets/Config/ClientConfig.asset", typeof(ClientConfig)) as
                    ClientConfig;

            var product = clientConfig.Product;

            var target = PBXProject.GetUnityTargetName();
            EditProject(path, target);
            EditPlist(path);
            EditCapacity(path, target, product);
        }

        #region Project

        private static void EditProject(string path, string targetName)
        {
            var project = new PBXProject();
            var projPath = PBXProject.GetPBXProjectPath(path);
            project.ReadFromFile(projPath);
            var targetGuid = project.TargetGuidByName(targetName);

            AddFrameworks(project, targetGuid);
            project.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
            project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            File.WriteAllText(projPath, project.WriteToString());
        }

        private static void AddFrameworks(PBXProject project, string targetGuid)
        {
            project.AddFrameworkToProject(targetGuid, "libstdc++.6.0.9.tbd", false);
            project.AddFrameworkToProject(targetGuid, "libz.dylib", false);
            project.AddFrameworkToProject(targetGuid, "libsqlite3.0.dylib", false);
            project.AddFrameworkToProject(targetGuid, "libc++.dylib", false);
            if (project.ContainsFramework(targetGuid, "MetalKit.framework"))
            {
                project.RemoveFrameworkFromProject(targetGuid, "MetalKit.framework");
            }
            project.AddFrameworkToProject(targetGuid, "MetalKit.framework", true);
        }

        private static void EditCapacity(string path, string targetName, ProductConfig product)
        {
            var projPath = PBXProject.GetPBXProjectPath(path);
            var cap = new ProjectCapabilityManager(projPath, product.EntitlementFile, targetName);

            cap.AddInAppPurchase();

            cap.AddKeychainSharing(new[]
            {
                product.KeyChainGroup
            });

            var links = product.UniversalLinks;
            if (links != null && links.Count > 0)
            {
                cap.AddAssociatedDomains(links.ToArray());
            }

            cap.WriteToFile();
        }

        #endregion

        #region Plist

        private static void EditPlist(string projectPath)
        {
            var clientConfig = AssetDatabase.LoadAssetAtPath(
                "Assets/Config/ClientConfig.asset",
                typeof(ClientConfig)) as ClientConfig;

            var product =  clientConfig.Product;

            var plistFile = Path.Combine(projectPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistFile);

            plist.root.SetString("CFBundleName", product.ProductName);
            plist.root.SetString("CFBundleDisplayName", product.DisplayName);
            plist.root.SetString("CFBundleDevelopmentRegion", "zh_CN");

            var schemes = plist.root.CreateArray("LSApplicationQueriesSchemes");
            schemes.AddString("weixin");
            schemes.AddString("wechat");
            schemes.AddString("dmmttigd");

            plist.WriteToFile(plistFile);
        }

        #endregion

#endif
    }
}