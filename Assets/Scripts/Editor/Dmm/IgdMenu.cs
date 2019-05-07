using Dmm.Data;
using Dmm.Game;
using Dmm.Log;
using Dmm.Release;
using UnityEditor;
using UnityEngine;

namespace Dmm
{
	public class IgdMenu : MonoBehaviour
	{

		[MenuItem("IGD/Release Editor")]
		public static void ShowReleaseConfig()
		{
			EditorWindow.CreateInstance<ReleaseEditor>().Show();
		}

		[MenuItem("IGD/System Msg Test")]
		public static void SystemMsgTest()
		{
			EditorWindow.CreateInstance<SystemMsgTest>().Show();
		}
		
		[MenuItem("IGD/Config Data/Create ClientVersion")]
		public static void CreateClientVersionData()
		{
			var clientVersion = ScriptableObject.CreateInstance<ClientConfig>();
			AssetDatabase.CreateAsset(clientVersion, "Assets/Config/ClientVersion.asset");
			AssetDatabase.SaveAssets();
		}

		[MenuItem("IGD/Config Data/Create SaleChannel Config")]
		public static void CreateSaleChannelData()
		{
			var saleChannel = ScriptableObject.CreateInstance<SaleChannelConfig>();
			AssetDatabase.CreateAsset(saleChannel, "Assets/Config/SaleChannel.asset");
			AssetDatabase.SaveAssets();
		}

		[MenuItem("IGD/Config Data/Create MyLog Config")]
		public static void CreateMyLogData()
		{
			var myLog = ScriptableObject.CreateInstance<MyLogConfig>();
			AssetDatabase.CreateAsset(myLog, "Assets/Resources/MyLogConfig.asset");
			AssetDatabase.SaveAssets();
		}

		[MenuItem("IGD/Config Data/Create Product Config")]
		public static void CreateProductConfig()
		{
			var config = ScriptableObject.CreateInstance<ProductConfig>();
			AssetDatabase.CreateAsset(config, "Assets/Config/Product/ProductConfig.asset");
			AssetDatabase.SaveAssets();
		}

		[MenuItem("IGD/PreferencesEditor")]
		public static void OpenWindow()
		{
			EditorWindow.CreateInstance<PreferencesEditor>().Show();
		}

		[MenuItem("IGD/Test/Award")]
		public static void TestAward()
		{
			EditorWindow.CreateInstance<AwardTestWindow>().Show();
		}

		[MenuItem("IGD/AssetBundleEditor")]
		public static void AssetBundleEditor()
		{
			EditorWindow.CreateInstance<AssetBundleEditor>().Show();
		}

	}
}
