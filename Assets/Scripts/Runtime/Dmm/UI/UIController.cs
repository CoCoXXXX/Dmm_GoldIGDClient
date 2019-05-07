using System;
using System.Collections;
using DG.Tweening;
using Dmm.Common;
using Dmm.Dialog;
using Dmm.Game;
using Dmm.Hall;
using Dmm.Log;
using Dmm.Login;
using Dmm.Res;
using Dmm.Task;
using UnityEngine;
using Zenject;

namespace Dmm.UI
{
    /// <summary>
    /// 整个界面UI的控制器。
    /// 爱掼蛋实际上有三个大的Window：
    /// 1、LoginWindow。
    /// 2、HallWindow。
    /// 3、GameWindow。
    /// </summary>
    public class UIController : MonoBehaviour, IUIController
    {
        #region Inject

        private IGameCanvas _gameCanvas;

        private LoginModeWindow.Factory _loginModeWindowFactory;

        private PortalWindow.Factory _portalWindowFactory;

        private RoomWindow.Factory _roomWindowFactory;

        private SeatWindow.Factory _seatWindowFactory;

        private GameWindow.Factory _gameWindowFactory;

        private IDialogManager _dialogManager;

        private IResourceManager _resource;

        [Inject]
        public void Initialize(
            IGameCanvas gameCanvas,
            IDialogManager dialogManager,
            IResourceManager resourceManager,
            LoginModeWindow.Factory loginModeWindowFactory,
            PortalWindow.Factory portalWindowFactory,
            RoomWindow.Factory roomWindowFactory,
            SeatWindow.Factory seatWindowFactory,
            GameWindow.Factory gameWindowFactory)
        {
            _gameCanvas = gameCanvas;
            _dialogManager = dialogManager;
            _resource = resourceManager;

            _loginModeWindowFactory = loginModeWindowFactory;
            _portalWindowFactory = portalWindowFactory;
            _roomWindowFactory = roomWindowFactory;
            _seatWindowFactory = seatWindowFactory;

            _gameWindowFactory = gameWindowFactory;
        }

        #endregion

        #region Unity方法

        public void Start()
        {
            var loginModeWindow = _gameCanvas.GetLoginModeWindow();
            SetInstantiatedWindow(UIWindowType.LoginMode, loginModeWindow);
        }

        public void Update()
        {
            UpdateSwitchTask();
        }

        public void LateUpdate()
        {
            CheckResourceState();
        }

        #endregion

        #region 大窗口

        /// <summary>
        /// Resources下Window的Prefab的存储路径。
        /// </summary>
        public const string WindowPath = "Window/";

        #region LoginModeWindow

        public string LoginModeWindowPrefab;

        public string LoginModeWindowPrefabPath
        {
            get { return WindowPath + LoginModeWindowPrefab; }
        }

        public LoginModeWindow GetLoginModeWindow()
        {
            if (!_loginModeWindow)
            {
                return null;
            }

            return _loginModeWindow.GetComponent<LoginModeWindow>();
        }

        private RectTransform _loginModeWindow;

        #endregion

        #region PortalWindow

        public string PortalWindowPrefab;

        public string PortalWindowPrefabPath
        {
            get { return WindowPath + PortalWindowPrefab; }
        }

        public PortalWindow GetPortalWindow()
        {
            if (!_portalWindow)
            {
                return null;
            }

            return _portalWindow.GetComponent<PortalWindow>();
        }

        private RectTransform _portalWindow;

        #endregion

        #region RoomWindow

        public string RoomWindowPrefab;

        public string RoomWindowPrefabPath
        {
            get { return WindowPath + RoomWindowPrefab; }
        }

        public RoomWindow GetRoomWindow()
        {
            if (!_roomWindow)
            {
                return null;
            }

            return _roomWindow.GetComponent<RoomWindow>();
        }

        private RectTransform _roomWindow;

        #endregion

        #region SeatWindow

        public string SeatWindowPrefab;

        public string SeatWindowPrefabPath
        {
            get { return WindowPath + SeatWindowPrefab; }
        }

        public SeatWindow GetSeatWindow()
        {
            if (!_seatWindow)
            {
                return null;
            }

            return _seatWindow.GetComponent<SeatWindow>();
        }

        private RectTransform _seatWindow;

        #endregion

        #region GameWindow

        public string GameWindowPrefab;

        public string GameWindowPrefabPath
        {
            get { return WindowPath + GameWindowPrefab; }
        }

        public GameWindow GetGameWindow()
        {
            if (!_gameWindow)
            {
                return null;
            }

            return _gameWindow.GetComponent<GameWindow>();
        }

        private RectTransform _gameWindow;

        #endregion

        #endregion

        #region 界面状态

        public UIWindowType GetCurrentWindowType()
        {
            return _currentWindowType;
        }

        public RectTransform GetCurrentWindow()
        {
            switch (_currentWindowType)
            {
                case UIWindowType.LoginMode:
                    return _loginModeWindow;

                case UIWindowType.Portal:
                    return _portalWindow;

                case UIWindowType.Room:
                    return _roomWindow;

                case UIWindowType.Seat:
                    return _seatWindow;

                case UIWindowType.Game:
                    return _gameWindow;

                default:
                    return null;
            }
        }

        /// <summary>
        /// 当前显示的界面。
        /// </summary>
        private UIWindowType _currentWindowType = UIWindowType.LoginMode;

        #endregion

        #region 白屏效果

        /// <summary>
        /// 白屏效果显现的时间。
        /// </summary>
        public float BlankAppearTime = 0.5f;

        /// <summary>
        /// 白屏效果消失的时间。
        /// </summary>
        public float BlankFadeTime = 0.5f;

        #endregion

        #region 滑屏切换

        /// <summary>
        /// 滑屏动画的时间。
        /// </summary>
        public float SlideTime = 1f;

        #endregion

        #region 切换逻辑

        /// <summary>
        /// 当前的切换状态。
        /// </summary>
        public UISwitchStatus GetSwitchStatus()
        {
            if (_curSwitchTask == null)
            {
                return UISwitchStatus.Idle;
            }

            return _curSwitchTask.Status;
        }

        /// <summary>
        /// 当前切换的动画类型。
        /// </summary>
        public UISwitchType GetSwitchType()
        {
            if (_curSwitchTask == null)
            {
                return UISwitchType.Null;
            }

            return _curSwitchTask.SwitchType;
        }

        public float GetSwitchStartTime()
        {
            if (_curSwitchTask == null)
                return 0;

            return _curSwitchTask.GetStartTime();
        }

        public void SwitchTo(
            UIWindowType targetWindow,
            Action<UISwitchTask> onEnableTarget = null,
            Action<UISwitchTask> onUISwitchComplete = null)
        {
            MyLog.InfoWithFrame(
                name,
                string.Format("try switch {0}-->{1}", _currentWindowType, targetWindow)
            );

            _dialogManager.ShowWaitingDialog(false);
            if (_curSwitchTask != null && _curSwitchTask.Status == UISwitchStatus.Switching)
            {
                MyLog.WarnWithFrame(
                    name,
                    string.Format("curTask: {0}-->{1}", _curSwitchTask.FromWindowType, _curSwitchTask.ToWindowType)
                );

                if (targetWindow == _curSwitchTask.ToWindowType)
                {
                    MyLog.InfoWithFrame(name, "same target contine switch.");
                    // 继续上一个任务的执行。
                    _curSwitchTask.OnEnableTarget = onEnableTarget;
                    _curSwitchTask.OnUISwitchComplete = onUISwitchComplete;
                    return;
                }

                if (targetWindow == _currentWindowType)
                {
                    MyLog.WarnWithFrame(name, "switch to current window, stop switch.");
                    // 如果新的目标与当前窗口类型相同。
                    // 则直接将当前窗口恢复正常位置。
                    // TODO 实现更牛叉的动画方式，就是反向切换回来。
                    var destroyToWindow = _curSwitchTask.ToWindowType != _currentWindowType;
                    _curSwitchTask.Cancel(destroyToWindow);

                    ResetCurrentWindow();

                    if (onEnableTarget != null) onEnableTarget(_curSwitchTask);
                    if (onUISwitchComplete != null) onUISwitchComplete(_curSwitchTask);
                    return;
                }

                MyLog.WarnWithFrame(
                    name,
                    string.Format("cancel curTask: {0}-->{1}", _curSwitchTask.FromWindowType,
                        _curSwitchTask.ToWindowType)
                );

                _curSwitchTask.Cancel();
                _curSwitchTask = null;

                ResetCurrentWindow();
            }

            if (_currentWindowType == targetWindow)
            {
                MyLog.WarnWithFrame(name, "switch to current window, finish.");
                if (onEnableTarget != null)
                {
                    onEnableTarget(_curSwitchTask);
                }

                if (onUISwitchComplete != null)
                {
                    onUISwitchComplete(_curSwitchTask);
                }

                return;
            }

            var type = DetermineSwitchType(_currentWindowType, targetWindow);

            _curSwitchTask = new UISwitchTask();
            _curSwitchTask.Parent = this;

            switch (type)
            {
                case UISwitchType.Blank:
                    _curSwitchTask.SwitchAction = CreateBlankSwitchSequence(_curSwitchTask);
                    break;

                case UISwitchType.SlideLeft:
                case UISwitchType.SlideRight:
                    _curSwitchTask.SwitchAction = CreateSlideLeftRightSwitchSequence(_curSwitchTask);
                    break;

                case UISwitchType.SlideUp:
                case UISwitchType.SlideDown:
                    _curSwitchTask.SwitchAction = CreateSlideUpDownSwitchSequence(_curSwitchTask);
                    break;

                case UISwitchType.Null:
                    _curSwitchTask.SwitchAction = CreateNullSwitchSequence(_curSwitchTask);
                    break;
            }

            _curSwitchTask.FromWindowType = _currentWindowType;
            _curSwitchTask.FromWindow = GetWindow(_currentWindowType);

            _curSwitchTask.ToWindowType = targetWindow;
            _curSwitchTask.ToWindow = GetWindow(targetWindow);

            _curSwitchTask.SwitchType = type;

            _curSwitchTask.OnEnableTarget = onEnableTarget;
            _curSwitchTask.OnUISwitchComplete = onUISwitchComplete;

            _curSwitchTask.Start();
        }

        /// <summary>
        /// 生成窗口的时候，窗口放置的位置。
        /// </summary>
        public Vector2 BornPosition = new Vector2(5000, 5000);

        private ActionSequenceGeneric<UISwitchTask> CreateBlankSwitchSequence(UISwitchTask task)
        {
            var seq = new ActionSequenceGeneric<UISwitchTask>(task);
            seq.Append(InstantiateTarget, CheckTargetStatus);
            seq.Append(StartBlankAnimation, CheckBlankAnimationStatus);
            seq.CompleteListener = CompleteLogic;

            return seq;
        }

        private ActionSequenceGeneric<UISwitchTask> CreateSlideLeftRightSwitchSequence(UISwitchTask task)
        {
            var seq = new ActionSequenceGeneric<UISwitchTask>(task);
            seq.Append(InstantiateTarget, CheckTargetStatus);
            seq.Append(EnableTarget);
            seq.Append(StartSlideLeftRightAnimation, CheckSlideAnimationStatus);
            seq.CompleteListener = CompleteLogic;

            return seq;
        }

        private ActionSequenceGeneric<UISwitchTask> CreateSlideUpDownSwitchSequence(UISwitchTask task)
        {
            var seq = new ActionSequenceGeneric<UISwitchTask>(task);
            seq.Append(InstantiateTarget, CheckTargetStatus);
            seq.Append(EnableTarget);
            seq.Append(StartSlideUpDownAnimation, CheckSlideAnimationStatus);
            seq.CompleteListener = CompleteLogic;

            return seq;
        }

        private ActionSequenceGeneric<UISwitchTask> CreateNullSwitchSequence(UISwitchTask task)
        {
            var seq = new ActionSequenceGeneric<UISwitchTask>(task);
            seq.Append(InstantiateTarget, CheckTargetStatus);
            seq.Append(SwitchToTargetDirectly);
            seq.CompleteListener = CompleteLogic;

            return seq;
        }

        /// <summary>
        /// 当前执行的切换任务。
        /// </summary>
        private UISwitchTask _curSwitchTask;

        /// <summary>
        /// 更新切换任务。
        /// </summary>
        private void UpdateSwitchTask()
        {
            if (_curSwitchTask == null)
                return;

            _curSwitchTask.Process();
        }

        private void ResetCurrentWindow()
        {
            MyLog.InfoWithFrame(
                name,
                string.Format("reset current window {0}", _currentWindowType)
            );

            var curWindow = GetCurrentWindow();
            if (curWindow)
            {
                curWindow.anchoredPosition = Vector2.zero;
            }

            var blankCover = _gameCanvas.GetWindowSwitchCover();
            blankCover.alpha = 0;
            if (blankCover.gameObject.activeSelf)
            {
                blankCover.gameObject.SetActive(false);
            }

            DestroyOtherWindow();
        }

        /// <summary>
        /// 生成目标窗口。
        /// </summary>
        private void InstantiateTarget(UISwitchTask curTask)
        {
            if (curTask == null)
            {
                return;
            }

            if (curTask.ToWindow)
            {
                curTask.ToWindow.anchoredPosition = BornPosition;
                return;
            }

            var toType = curTask.ToWindowType;

            var to = GetWindow(toType);
            if (to)
            {
                curTask.ToWindow = to;
                curTask.ToWindow.anchoredPosition = BornPosition;
            }
            else
            {
                curTask.InstantiateCoroutine = StartCoroutine(InstantiateWindowCoroutine(toType, curTask));
            }
        }

        /// <summary>
        /// 检查目标窗口是否完成生成过程。
        /// </summary>
        /// <param name="curTask"></param>
        /// <returns></returns>
        private bool CheckTargetStatus(UISwitchTask curTask)
        {
            if (curTask == null)
            {
                return false;
            }

            return curTask.ToWindow != null;
        }

        /// <summary>
        /// Enable目标窗口。
        /// 单独使用一个方法开启是为了防止在开始动画那一帧造成卡顿。
        /// </summary>
        /// <param name="curTask"></param>
        private void EnableTarget(UISwitchTask curTask)
        {
            if (curTask == null)
            {
                return;
            }

            var target = curTask.ToWindow;
            if (target && !target.gameObject.activeSelf)
            {
                target.gameObject.SetActive(true);
            }

            if (curTask.OnEnableTarget != null)
            {
                curTask.OnEnableTarget(curTask);
            }
        }

        /// <summary>
        /// 启动Blank动画。
        /// </summary>
        /// <param name="curTask"></param>
        private void StartBlankAnimation(UISwitchTask curTask)
        {
            if (curTask == null)
            {
                return;
            }

            StopAllAnimationAndDisableBlankCover(curTask);

            var from = curTask.FromWindow;
            var to = curTask.ToWindow;

            var blankCover = _gameCanvas.GetWindowSwitchCover();
            if (!blankCover.gameObject.activeSelf)
            {
                blankCover.gameObject.SetActive(true);
            }

            curTask.StopAllAnimation();

            var blank = DOTween.Sequence();
            blank
                .Append(blankCover.DOFade(1, BlankAppearTime))
                .AppendCallback(() =>
                {
                    if (from)
                    {
                        from.anchoredPosition = BornPosition;
                    }

                    if (to)
                    {
                        if (!to.gameObject.activeSelf)
                            to.gameObject.SetActive(true);

                        to.anchoredPosition = Vector2.zero;

                        if (curTask.OnEnableTarget != null)
                            curTask.OnEnableTarget(curTask);
                    }
                })
                .Append(blankCover.DOFade(0, BlankFadeTime))
                .OnComplete(() =>
                {
                    if (blankCover.gameObject.activeSelf)
                        blankCover.gameObject.SetActive(false);
                });

            blank.Play();
            curTask.BlankSequence = blank;
        }

        private bool CheckBlankAnimationStatus(UISwitchTask curTask)
        {
            if (curTask == null)
            {
                return true;
            }

            var seq = curTask.BlankSequence;
            if (seq == null)
            {
                return true;
            }

            return !seq.IsPlaying();
        }

        /// <summary>
        /// 启动左右滑动的动画。
        /// </summary>
        /// <param name="curTask"></param>
        private void StartSlideLeftRightAnimation(UISwitchTask curTask)
        {
            if (curTask == null)
            {
                return;
            }

            StopAllAnimationAndDisableBlankCover(curTask);

            var from = curTask.FromWindow;
            var to = curTask.ToWindow;
            var type = curTask.SwitchType;
            var canvasWidth = _gameCanvas.GetCanvasWidth();

            if (type != UISwitchType.SlideLeft && type != UISwitchType.SlideRight)
            {
                return;
            }

            if (from)
            {
                from.anchoredPosition = Vector2.zero;
                curTask.FromTweener = @from.DOAnchorPos(
                        new Vector2(
                            type == UISwitchType.SlideLeft ? -canvasWidth : canvasWidth,
                            0),
                        SlideTime)
                    .SetEase(Ease.OutBack, 1.1f)
                    .Play();
            }

            if (to)
            {
                to.anchoredPosition = new Vector2(
                    type == UISwitchType.SlideLeft ? canvasWidth : -canvasWidth,
                    0);
                curTask.ToTweener = to
                    .DOAnchorPos(Vector2.zero, SlideTime)
                    .SetEase(Ease.OutBack, 1.1f)
                    .Play();
            }
        }

        private void StartSlideUpDownAnimation(UISwitchTask curTask)
        {
            if (curTask == null)
            {
                return;
            }

            StopAllAnimationAndDisableBlankCover(curTask);

            var from = curTask.FromWindow;
            var to = curTask.ToWindow;
            var type = curTask.SwitchType;
            var canvasHeight = _gameCanvas.GetCanvasHeight();

            if (type != UISwitchType.SlideUp && type != UISwitchType.SlideDown)
            {
                return;
            }

            if (type == UISwitchType.SlideUp)
            {
                if (from)
                {
                    from.anchoredPosition = Vector2.zero;
                    curTask.FromTweener = @from.DOAnchorPos(
                            new Vector2(0, canvasHeight),
                            SlideTime)
                        .SetEase(Ease.OutBack, 1.1f)
                        .Play();
                }

                if (to)
                {
                    to.anchoredPosition = Vector2.zero;
                }

                if (from && to)
                {
                    from.SetSiblingIndex(to.GetSiblingIndex() + 1);
                }
            }
            else if (type == UISwitchType.SlideDown)
            {
                if (to)
                {
                    to.anchoredPosition = new Vector2(0, canvasHeight);
                    curTask.ToTweener = to
                        .DOAnchorPos(Vector2.zero, SlideTime)
                        .SetEase(Ease.OutBack, 1.1f)
                        .Play();
                }
            }
        }

        private bool CheckSlideAnimationStatus(UISwitchTask task)
        {
            if (task == null)
            {
                return true;
            }

            var from = task.FromTweener;
            var to = task.ToTweener;

            if (from != null && from.IsPlaying())
            {
                return false;
            }

            if (to != null && to.IsPlaying())
            {
                return false;
            }

            return true;
        }

        private void SwitchToTargetDirectly(UISwitchTask curTask)
        {
            if (curTask == null)
            {
                return;
            }

            StopAllAnimationAndDisableBlankCover(curTask);

            var from = curTask.FromWindow;
            var to = curTask.ToWindow;

            if (from && from.gameObject.activeSelf)
            {
                from.gameObject.SetActive(false);
            }

            if (to)
            {
                if (!to.gameObject.activeSelf)
                {
                    to.gameObject.SetActive(true);
                }

                to.anchoredPosition = Vector2.zero;
            }
        }

        private void CompleteLogic(UISwitchTask curTask)
        {
            if (curTask == null)
            {
                return;
            }

            _currentWindowType = curTask.ToWindowType;

            SetInstantiatedWindow(curTask.ToWindowType, curTask.ToWindow);
            DestroyOtherWindow();

            var complete = curTask.OnUISwitchComplete;
            if (complete != null)
            {
                complete(curTask);
            }
        }

        public static UIWindowType[] AllWindowTypes =
        {
            UIWindowType.LoginMode,
            UIWindowType.Portal,
            UIWindowType.Room,
            UIWindowType.Seat,
            UIWindowType.Game
        };

        private void DestroyOtherWindow()
        {
            var current = _currentWindowType;
            for (int i = 0; i < AllWindowTypes.Length; i++)
            {
                var w = AllWindowTypes[i];
                if (w != current)
                {
                    DestroyWindow(w);
                }
            }
        }

        private void StopAllAnimationAndDisableBlankCover(UISwitchTask curTask)
        {
            if (curTask != null)
            {
                curTask.StopAllAnimation();
            }

            var cover = _gameCanvas.GetWindowSwitchCover();
            if (cover && cover.gameObject.activeSelf)
            {
                cover.gameObject.SetActive(false);
            }
        }

        private UISwitchType DetermineSwitchType(UIWindowType from, UIWindowType to)
        {
            // 如果有一个窗口为空，或者两个窗口相同，则不进行任何切换。
            if (from == UIWindowType.Null ||
                to == UIWindowType.Null ||
                from == to)
            {
                return UISwitchType.Null;
            }

            if (from == UIWindowType.LoginMode || to == UIWindowType.LoginMode)
            {
                return UISwitchType.Null;
            }

            // 如果有一个窗口是GameWindow，则进行白屏切换。
            if (from == UIWindowType.Game || to == UIWindowType.Game)
            {
                return UISwitchType.Blank;
            }

            if (from == UIWindowType.Portal && to == UIWindowType.Room)
            {
                return UISwitchType.SlideLeft;
            }

            if (from == UIWindowType.Room && to == UIWindowType.Portal)
            {
                return UISwitchType.SlideRight;
            }

            if (from == UIWindowType.Room && to == UIWindowType.Seat)
            {
                return UISwitchType.SlideLeft;
            }

            if (from == UIWindowType.Seat && to == UIWindowType.Room)
            {
                return UISwitchType.SlideRight;
            }

            return UISwitchType.Null;
        }

        private RectTransform GetWindow(UIWindowType windowType)
        {
            switch (windowType)
            {
                case UIWindowType.LoginMode:
                    return _loginModeWindow;

                case UIWindowType.Portal:
                    return _portalWindow;

                case UIWindowType.Room:
                    return _roomWindow;

                case UIWindowType.Seat:
                    return _seatWindow;

                case UIWindowType.Game:
                    return _gameWindow;

                default:
                    return null;
            }
        }

        private void SetInstantiatedWindow(UIWindowType windowType, RectTransform window)
        {
            switch (windowType)
            {
                case UIWindowType.LoginMode:
                    _loginModeWindow = window;
                    break;

                case UIWindowType.Portal:
                    _portalWindow = window;
                    break;

                case UIWindowType.Room:
                    _roomWindow = window;
                    break;

                case UIWindowType.Seat:
                    _seatWindow = window;
                    break;

                case UIWindowType.Game:
                    _gameWindow = window;
                    break;
            }
        }

        private IEnumerator InstantiateWindowCoroutine(UIWindowType windowType, UISwitchTask curTask)
        {
            if (curTask == null)
            {
                yield break;
            }

            if (windowType == UIWindowType.Null)
            {
                yield break;
            }

            if (!NeedInstantiate(windowType))
            {
                yield break;
            }

            var prefabPath = GetPrefabPath(windowType);
            var prefabName = GetPrefabName(windowType);
            if (string.IsNullOrEmpty(prefabPath) || string.IsNullOrEmpty(prefabName))
            {
                yield break;
            }

            _dialogManager.ShowWaitingDialog(true, 10);

            _resource.StartLoadResource(prefabPath, prefabName);

            while (!_resource.HasCached(prefabPath))
            {
                yield return null;
            }

            var prefab = _resource.GetResource<GameObject>(prefabPath, prefabName);
            RectTransform target = null;
            switch (windowType)
            {
                case UIWindowType.Game:
                    var game = _gameWindowFactory.Create(prefab);
                    target = game.GetComponent<RectTransform>();
                    break;

                case UIWindowType.LoginMode:
                    var loginMode = _loginModeWindowFactory.Create(prefab);
                    target = loginMode.GetComponent<RectTransform>();
                    break;

                case UIWindowType.Portal:
                    var portal = _portalWindowFactory.Create(prefab);
                    target = portal.GetComponent<RectTransform>();
                    break;

                case UIWindowType.Room:
                    var room = _roomWindowFactory.Create(prefab);
                    target = room.GetComponent<RectTransform>();
                    break;

                case UIWindowType.Seat:
                    var seat = _seatWindowFactory.Create(prefab);
                    target = seat.GetComponent<RectTransform>();
                    break;
            }

            _dialogManager.ShowWaitingDialog(false);

            if (target == null)
            {
                yield break;
            }

            target.SetParent(_gameCanvas.GetWindowParent(), false);
            target.anchoredPosition = BornPosition;

            curTask.ToWindow = target;

            yield return null;
        }

        private bool NeedInstantiate(UIWindowType windowType)
        {
            switch (windowType)
            {
                case UIWindowType.LoginMode:
                    return !_loginModeWindow;

                case UIWindowType.Portal:
                    return !_portalWindow;

                case UIWindowType.Room:
                    return !_roomWindow;

                case UIWindowType.Seat:
                    return !_seatWindow;

                case UIWindowType.Game:
                    return !_gameWindow;

                default:
                    return false;
            }
        }

        private string GetPrefabPath(UIWindowType windowType)
        {
            switch (windowType)
            {
                case UIWindowType.LoginMode:
                    return "login-mode-window";

                case UIWindowType.Portal:
                    return "portal-window";

                case UIWindowType.Room:
                    return "room-window";

                case UIWindowType.Seat:
                    return "seat-window";

                case UIWindowType.Game:
                    return "game-window";

                default:
                    return null;
            }
        }

        private string GetPrefabName(UIWindowType windowType)
        {
            switch (windowType)
            {
                case UIWindowType.LoginMode:
                    return "LoginModeWindow";

                case UIWindowType.Portal:
                    return "PortalWindow";

                case UIWindowType.Room:
                    return "RoomWindow";

                case UIWindowType.Seat:
                    return "SeatWindow";

                case UIWindowType.Game:
                    return "GameWindow";

                default:
                    return null;
            }
        }

        #endregion

        #region 接口方法

        public void StopInstantiateCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        public void DestroyWindow(GameObject window)
        {
            if (window)
            {
                Destroy(window);
                NeedUnloadAsset();
            }
        }

        #endregion

        #region 卸载资源

        private void DestroyWindow(UIWindowType type)
        {
            MyLog.DebugWithFrame(name, string.Format("Destroy window: {0}", type));
            switch (type)
            {
                case UIWindowType.LoginMode:
                    if (_loginModeWindow)
                    {
                        Destroy(_loginModeWindow.gameObject);
                        _loginModeWindow = null;
                        NeedUnloadAsset();
                    }

                    break;

                case UIWindowType.Portal:
                    if (_portalWindow)
                    {
                        Destroy(_portalWindow.gameObject);
                        _portalWindow = null;
                        NeedUnloadAsset();
                    }

                    break;

                case UIWindowType.Room:
                    if (_roomWindow)
                    {
                        Destroy(_roomWindow.gameObject);
                        _roomWindow = null;
                        NeedUnloadAsset();
                    }

                    break;

                case UIWindowType.Seat:
                    if (_seatWindow)
                    {
                        Destroy(_seatWindow.gameObject);
                        _seatWindow = null;
                        NeedUnloadAsset();
                    }

                    break;

                case UIWindowType.Game:
                    if (_gameWindow)
                    {
                        Destroy(_gameWindow.gameObject);
                        _gameWindow = null;
                        NeedUnloadAsset();
                    }

                    break;
            }
        }

        public float UnloadAssetInterval = 1;

        private float _lastUnloadTime;

        private bool _needUnload;

        public void NeedUnloadAsset()
        {
            _needUnload = true;
        }

        private void CheckResourceState()
        {
            if (Time.time - _lastUnloadTime < UnloadAssetInterval)
            {
                return;
            }

            if (!_needUnload)
            {
                return;
            }

            Resources.UnloadUnusedAssets();

            _lastUnloadTime = Time.time;
            _needUnload = false;
        }

        #endregion
    }
}