using Dmm.Widget;
using UnityEngine;

namespace Dmm.Base
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorWindow : UIWindow
    {
        public Animator Animator { get; private set; }

        public void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        public const string DisplayKey = "Display";

        public override void Show()
        {
            Animator.SetBool(DisplayKey, true);
        }

        public override void Hide()
        {
            Animator.SetBool(DisplayKey, false);
        }
    }
}