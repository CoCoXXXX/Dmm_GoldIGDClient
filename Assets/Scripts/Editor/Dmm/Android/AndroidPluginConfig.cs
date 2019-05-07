using UnityEngine;

namespace Dmm.Android
{
    public class AndroidPluginConfig : ScriptableObject
    {
		/// <summary>
		/// 安卓插件的类型：
		/// xiaomi:		小米
		/// yyb:		应用宝
		/// huawei:		华为
		/// oppo:		oppo
		/// vivo:		vivo
		/// </summary>
		public string Type;

		/// <summary>
		/// 显示名称。
		/// </summary>
		public string DisplayName;
		
		/// <summary>
		/// 插件文件的路径。
		/// </summary>
		public string FilePath;
    }
}