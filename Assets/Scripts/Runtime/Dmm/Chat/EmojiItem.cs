using Dmm.Dialog;
using Dmm.Network;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Chat
{
    public class EmojiItem : MonoBehaviour
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        [Inject]
        public void Initialize(
            IDialogManager dialogManager,
            RemoteAPI remoteAPI)
        {
            _dialogManager = dialogManager;
            _remoteAPI = remoteAPI;
        }

        public class Factory : Factory<EmojiItem>
        {
        }

        #endregion

        public Button Button;

        public Image Image;

        public void Awake()
        {
            if (Button)
            {
                Button.onClick.AddListener(OnClick);
            }
        }

        public void OnClick()
        {
            if (Image && Image.sprite)
            {
                var data = Image.sprite.name;
                _remoteAPI.SendTextMsg(data);
                _dialogManager.HideDialog(DialogName.ChatPanel);
            }
        }
    }
}