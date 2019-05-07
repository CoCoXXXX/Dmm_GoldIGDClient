using Dmm.Dialog;
using Zenject;

namespace Dmm.YuanBao
{
    public class YuanBaoRecordDialog : MyDialog
    {
        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}