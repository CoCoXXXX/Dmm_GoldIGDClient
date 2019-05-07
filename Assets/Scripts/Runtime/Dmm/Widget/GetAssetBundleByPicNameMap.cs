using System.Collections.Generic;
using Dmm.Res;

namespace Dmm.Widget
{
    public class GetAssetBundleByPicNameMap
    {
        private static readonly Dictionary<string, string> _map = new Dictionary<string, string>();

        static GetAssetBundleByPicNameMap()
        {
            _map.Add(ResourcePicName.ActivityOuterPic, ResourcePath.PortalWindow);
            _map.Add(ResourcePicName.ActivityContentPic, ResourcePath.PortalWindow);
            _map.Add(ResourcePicName.ActivityChargePic, ResourcePath.PayChannelDialog);
            _map.Add(ResourcePicName.ChuJiChang, ResourcePath.RoomPath);
            _map.Add(ResourcePicName.DaShiChang, ResourcePath.RoomPath);
            _map.Add(ResourcePicName.DiBeiChang, ResourcePath.RoomPath);
            _map.Add(ResourcePicName.GaoBeiChang, ResourcePath.RoomPath);
            _map.Add(ResourcePicName.JiaBeiChang, ResourcePath.RoomPath);
            _map.Add(ResourcePicName.JinJieChang, ResourcePath.RoomPath);
            _map.Add(ResourcePicName.LianXiChang, ResourcePath.RoomPath);
            _map.Add(ResourcePicName.Pengyouchang1, ResourcePath.RoomPath);
            _map.Add(ResourcePicName.PengYouChang2, ResourcePath.RoomPath);
            _map.Add(ResourcePicName.TtzChuJiChang, ResourcePath.RoomPath);
            _map.Add(ResourcePicName.TtzGaoJiCChang, ResourcePath.RoomPath);
            _map.Add(ResourcePicName.ZhongJiChang, ResourcePath.RoomPath);
        }

        public static string GetAssetBundleName(string picName)
        {
            if (string.IsNullOrEmpty(picName))
            {
                return null;
            }

            if (!_map.ContainsKey(picName))
            {
                return null;
            }

            return _map[picName];
        }
    }
}