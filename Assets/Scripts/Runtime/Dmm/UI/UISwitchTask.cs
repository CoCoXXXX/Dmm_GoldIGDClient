using System;
using DG.Tweening;
using Dmm.Task;
using UnityEngine;

namespace Dmm.UI
{
    public class UISwitchTask
    {
        #region 数据

        public IUIController Parent;
        public UIWindowType FromWindowType;
        public UIWindowType ToWindowType;
        public UISwitchType SwitchType;

        public RectTransform FromWindow;
        public RectTransform ToWindow;
        public Coroutine InstantiateCoroutine;

        #endregion

        #region 动画

        public Tweener FromTweener;
        public Tweener ToTweener;
        public Sequence BlankSequence;

        public ActionSequenceGeneric<UISwitchTask> SwitchAction;

        public void StopAllAnimation()
        {
            if (FromTweener != null)
            {
                FromTweener.Kill();
                FromTweener = null;
            }

            if (ToTweener != null)
            {
                ToTweener.Kill();
                ToTweener = null;
            }

            if (BlankSequence != null)
            {
                BlankSequence.Kill();
                BlankSequence = null;
            }
        }

        #endregion

        #region 状态

        public Action<UISwitchTask> OnUISwitchComplete;

        public Action<UISwitchTask> OnEnableTarget;

        public UISwitchStatus Status
        {
            get
            {
                if (SwitchAction == null)
                {
                    return UISwitchStatus.Idle;
                }

                switch (SwitchAction.GetStatus())
                {
                    case ActionSequenceStatus.Idle:
                        return UISwitchStatus.Idle;

                    case ActionSequenceStatus.Running:
                        return UISwitchStatus.Switching;

                    case ActionSequenceStatus.Finished:
                        return UISwitchStatus.Finish;

                    case ActionSequenceStatus.Canceled:
                        return UISwitchStatus.Canceled;

                    default:
                        return UISwitchStatus.Idle;
                }
            }
        }

        public void Start()
        {
            if (SwitchAction != null)
            {
                SwitchAction.Start();
            }
        }

        public void Cancel(bool destroyTarget = true)
        {
            StopAllAnimation();

            if (Parent != null)
            {
                if (ToWindow != null && destroyTarget)
                {
                    Parent.DestroyWindow(ToWindow.gameObject);
                    ToWindow = null;
                }

                if (InstantiateCoroutine != null)
                {
                    Parent.StopInstantiateCoroutine(InstantiateCoroutine);
                    InstantiateCoroutine = null;
                }
            }

            if (SwitchAction != null)
            {
                SwitchAction.Cancel();
            }
        }

        public void Process()
        {
            if (Status != UISwitchStatus.Switching)
                return;

            if (SwitchAction != null)
            {
                SwitchAction.Process();
            }
        }

        public float GetStartTime()
        {
            if (SwitchAction == null)
            {
                return 0;
            }

            return SwitchAction.GetStartTime();
        }

        #endregion
    }
}