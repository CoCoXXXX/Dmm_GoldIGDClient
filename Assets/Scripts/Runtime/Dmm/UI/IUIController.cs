using System;
using Dmm.Game;
using Dmm.Hall;
using Dmm.Login;
using UnityEngine;

namespace Dmm.UI
{
    public interface IUIController
    {
        void StopInstantiateCoroutine(Coroutine coroutine);
        void DestroyWindow(GameObject window);

        void NeedUnloadAsset();

        UISwitchStatus GetSwitchStatus();
        UISwitchType GetSwitchType();
        float GetSwitchStartTime();

        void SwitchTo(
            UIWindowType targetWindow,
            Action<UISwitchTask> onEnableTarget = null,
            Action<UISwitchTask> onUISwitchComplete = null);

        UIWindowType GetCurrentWindowType();
        RectTransform GetCurrentWindow();

        LoginModeWindow GetLoginModeWindow();
        PortalWindow GetPortalWindow();
        RoomWindow GetRoomWindow();
        SeatWindow GetSeatWindow();
        GameWindow GetGameWindow();
    }
}