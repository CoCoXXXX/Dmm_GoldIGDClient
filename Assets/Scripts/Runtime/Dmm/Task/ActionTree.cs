using System.Collections.Generic;

namespace Dmm.Task
{
    public class ActionTree
    {
        #region Delegate and Event

        /// <summary>
        /// 执行动作。
        /// </summary>
        public delegate void ExecuteAction();

        /// <summary>
        /// 检查动作的状态。
        /// </summary>
        /// <returns></returns>
        public delegate void CheckActionState(ActionState state);

        /// <summary>
        /// 整个动作树都执行完毕的事件。
        /// </summary>
        public delegate void ActionTreeCompleteDelegate();

        /// <summary>
        /// 整个动作树执行完毕的消息。
        /// </summary>
        public event ActionTreeCompleteDelegate OnActionTreeComplete;

        #endregion

        #region 动作树数据

        /// <summary>
        /// 动作树的根。
        /// </summary>
        private readonly ActionPair _head = new ActionPair(null, null);

        /// <summary>
        /// 添加根节点。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="checker"></param>
        /// <returns></returns>
        public ActionPair AddRoot(ExecuteAction action, CheckActionState checker)
        {
            var root = new ActionPair(action, checker);
            _head.AddDefaultChild(root);
            return root;
        }

        /// <summary>
        /// 返回动作树的根节点。
        /// </summary>
        public ActionPair Root
        {
            get { return _head.GetChild(ActionState.Default); }
        }

        #endregion

        #region 当前动作树的状态

        /// <summary>
        /// 当前ActionTree的状态。
        /// </summary>
        public ActionTreeState CurrentState { get; private set; }

        /// <summary>
        /// 当前执行的动作。
        /// </summary>
        public ActionPair CurrentAction { get; private set; }

        #endregion

        /// <summary>
        /// 由上层的MonoBehaviour调用。
        /// </summary>
        public void Update()
        {
            if (CurrentState != ActionTreeState.Running) return;

            if (CurrentAction == null)
            {
                // 如果当前没有执行Action，则直接结束所有的动作。
                CompleteActionTree();
                return;
            }

            if (CurrentAction.Checker != null)
            {
                // 存在checker，则检查checker的结果。
                CurrentAction.Checker(CurrentAction.State);
                if (CurrentAction.State.Result != ActionState.Continue)
                {
                    // 不是Contine，则尝试开始下一个动作。
                    var next = CurrentAction.GetChild(CurrentAction.State.Result);
                    CurrentAction = next;

                    if (CurrentAction != null && CurrentAction.Action != null)
                        CurrentAction.Action();
                }
            }
            else
            {
                // 不存在checker，则尝试查找默认的动作。
                var next = CurrentAction.GetChild(ActionState.Default);
                CurrentAction = next;

                if (CurrentAction != null && CurrentAction.Action != null)
                    CurrentAction.Action();
            }
        }

        /// <summary>
        /// 开始执行整个动作树。
        /// </summary>
        public void Start()
        {
            CurrentAction = _head.GetChild(ActionState.Default);
            if (CurrentAction != null && CurrentAction.Action != null)
                // 立即执行第一个动作。
                CurrentAction.Action();

            CurrentState = ActionTreeState.Running;
        }

        /// <summary>
        /// 停止执行动作树。
        /// </summary>
        public void Stop()
        {
            CurrentState = ActionTreeState.Stopped;
        }

        /// <summary>
        /// 完成整个ActionTree。
        /// </summary>
        public void CompleteActionTree()
        {
            CurrentState = ActionTreeState.Complete;

            if (OnActionTreeComplete != null)
                OnActionTreeComplete();
        }

        /// <summary>
        /// 重置状态。
        /// </summary>
        public void Reset()
        {
            CurrentAction = _head;
            CurrentState = ActionTreeState.Idle;
        }

        /// <summary>
        /// 清空所有的动作。
        /// </summary>
        public void Clear()
        {
            if (_head.Children != null)
                _head.Children.Clear();
        }
    }

    /// <summary>
    /// 动作树的状态。
    /// </summary>
    public enum ActionTreeState
    {
        /// <summary>
        /// 未运行状态。
        /// </summary>
        Idle,

        /// <summary>
        /// 正在运行中。
        /// </summary>
        Running,

        /// <summary>
        /// 已经完成了。
        /// </summary>
        Complete,

        /// <summary>
        /// 被手动停止了。
        /// </summary>
        Stopped
    }

    /// <summary>
    /// 动作单元。
    /// </summary>
    public class ActionPair
    {
        /// <summary>
        /// 执行动作。
        /// </summary>
        public ActionTree.ExecuteAction Action { get; private set; }

        /// <summary>
        /// 检查器。
        /// </summary>
        public ActionTree.CheckActionState Checker { get; private set; }

        /// <summary>
        /// 动作的状态。
        /// </summary>
        public readonly ActionState State = new ActionState();

        /// <summary>
        /// 所有的子节点。
        /// </summary>
        public Dictionary<int, ActionPair> Children { get; private set; }

        public ActionPair(ActionTree.ExecuteAction action, ActionTree.CheckActionState checker)
        {
            Action = action;
            Checker = checker;
        }

        /// <summary>
        /// 获取child。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionPair GetChild(int key)
        {
            if (Children == null || Children.Count <= 0)
                return null;

            ActionPair result;
            return Children.TryGetValue(key, out result) ? result : null;
        }

        /// <summary>
        /// 添加默认的子节点。
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public ActionPair AddDefaultChild(ActionPair child)
        {
            if (child == null)
                return this;

            if (Children == null)
                Children = new Dictionary<int, ActionPair>();

            if (Children.ContainsKey(ActionState.Default))
                Children[ActionState.Default] = child;
            else
                Children.Add(ActionState.Default, child);

            return this;
        }

        /// <summary>
        /// 添加子节点。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="child"></param>
        /// <returns>返回当前节点，一遍重复添加。</returns>
        public ActionPair AddChild(int key, ActionPair child)
        {
            if (child == null)
                return this;

            if (Children == null)
                Children = new Dictionary<int, ActionPair>();

            if (Children.ContainsKey(key))
                Children[key] = child;
            else
                Children.Add(key, child);

            return this;
        }
    }

    /// <summary>
    /// 动作的状态。
    /// </summary>
    public class ActionState
    {
        #region 默认返回值

        public const int Continue = -100000;

        /// <summary>
        /// 默认的Child的Key。
        /// </summary>
        public const int Default = -400000;

        #endregion

        public int Result = Continue;

        public void Reset()
        {
            Result = Continue;
        }
    }
}