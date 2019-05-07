using Dmm.DataContainer;
using Dmm.Dialog;
using UnityEngine.UI;

namespace Dmm.Login
{
    public class OnlineServiceDialog : MyDialog
    {
        public Text ContentTxt;

        public override void BeforeShow()
        {
            var dataRepository = GetDataRepository();
            var serviceQQContainer = dataRepository.GetContainer<string>(DataKey.ServiceQQ);
            var serviceQQ = serviceQQContainer.Read();
            var serviceQQGroupContainer = dataRepository.GetContainer<string>(DataKey.ServiceQQGroup);
            var serviceQQGroup = serviceQQGroupContainer.Read();

            ContentTxt.text = string.Format(
                "微信公众号：{0}\n客服QQ：{1}\n粉丝QQ群：{2}",
                "全民掼蛋",
                serviceQQ,
                serviceQQGroup);

            GetAnalyticManager().Event("online_service_show");
        }
    }
}