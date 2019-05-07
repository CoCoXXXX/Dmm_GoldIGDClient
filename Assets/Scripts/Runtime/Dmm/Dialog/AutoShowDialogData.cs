using System;
using Dmm.Widget;

namespace Dmm.Dialog
{
    public enum DialogDataState
    {
        Null = 0,

        Wait = 1,

        Ok = 2,

        Failed = 3
    }

    public class AutoShowDialogData
    {
        public string DialogName;

        public bool Trigger = false;

        public int DialogShow = 0;

        public Action<AutoShowDialogData> Action;

        public UIWindow CurrentShowDialog;

        public void SetDialogDataState(DialogDataState state)
        {
            DialogShow = (int) state;
        }

        public void SetCurrentShowDialog(UIWindow dialog)
        {
            CurrentShowDialog = dialog;
        }
    }
}