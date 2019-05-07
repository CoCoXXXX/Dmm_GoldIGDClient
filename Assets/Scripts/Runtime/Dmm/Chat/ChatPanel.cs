using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Chat
{
    /// <summary>
    /// 聊天面板。
    /// </summary>
    public class ChatPanel : MyDialog
    {
        #region 组件

        public InputField InputContent;

        public Image TxtTabSelected;

        public ChatTextList TextContent;

        public Image EmojiTabSelected;

        public RectTransform EmojiContent;

        public Image JianMengTabSelected;

        public JianMengList JianMengContent;

        public ScrollRect TextContentScroll;

        public Toggle ChatShieldToggle;

        public Text ChatShieldDescription;

        public GameObject FreeChatOn;

        public GameObject FreeChatOff;

        public Image ContentImage;

        public Sprite HasInputBg;

        public Sprite NotInputBg;

        public GameObject SendBtn;

        #endregion

        private IDataContainer<bool> _shieldChat;

        private IDataContainer<Table> _currentTable;

        private void OnEnable()
        {
            _shieldChat = GetContainer<bool>(DataKey.ShieldChat);
            _currentTable = GetContainer<Table>(DataKey.CurrentTable);
        }

        public override void BeforeShow()
        {
            ChatShieldToggle.isOn = _shieldChat.Read();
            FreeChatOn.SetActive(ChatShieldToggle.isOn);
            FreeChatOff.SetActive(!ChatShieldToggle.isOn);
            var table = _currentTable.Read();
            var enableFreeInput = table != null && table.enable_free_chat;
            InputContent.gameObject.SetActive(enableFreeInput);
            SendBtn.SetActive(enableFreeInput);
            ContentImage.sprite = enableFreeInput ? HasInputBg : NotInputBg;
            ContentImage.SetNativeSize();

            var featureSwitchContainer = GetDataRepository().GetContainer<FeatureSwitch>(DataKey.FeatureSwitch);
            var feature = featureSwitchContainer.Read();
            if (feature == null || feature.interaction)
            {
                ChatShieldDescription.text = "屏蔽本局所有聊天及动画表情";
            }
            else
            {
                ChatShieldDescription.text = "屏蔽本局所有聊天";
            }
        }

        public override void AfterShow()
        {
            // 默认切换到贱萌表情。
            SwitchToJianMengTab();
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }

        public void SwitchToTxtTab()
        {
            SelectChatText(true);
            SelectEmoji(false);
            SelectJianMeng(false);
        }

        private void SelectChatText(bool selected)
        {
            if (TxtTabSelected.gameObject.activeSelf != selected)
                TxtTabSelected.gameObject.SetActive(selected);

            if (TextContent.gameObject.activeSelf != selected)
                TextContent.gameObject.SetActive(selected);
        }

        public void SwitchToEmojiTab()
        {
            SelectChatText(false);
            SelectEmoji(true);
            SelectJianMeng(false);
        }

        private void SelectEmoji(bool selected)
        {
            if (EmojiTabSelected.gameObject.activeSelf != selected)
                EmojiTabSelected.gameObject.SetActive(selected);

            if (EmojiContent.gameObject.activeSelf != selected)
                EmojiContent.gameObject.SetActive(selected);
        }

        public void SwitchToJianMengTab()
        {
            SelectChatText(false);
            SelectEmoji(false);
            SelectJianMeng(true);
        }

        private void SelectJianMeng(bool selected)
        {
            if (JianMengTabSelected.gameObject.activeSelf != selected)
                JianMengTabSelected.gameObject.SetActive(selected);

            if (JianMengContent.gameObject.activeSelf != selected)
                JianMengContent.gameObject.SetActive(selected);
        }

        public int TextMsgLengthLimit = 20;

        public void SendCustomMsg()
        {
            var content = InputContent.text;
            if (string.IsNullOrEmpty(content))
            {
                Toast("不能发送空消息！", true);
                return;
            }

            if (content.Length > TextMsgLengthLimit)
            {
                Toast("消息过长！\n消息字数不能超过" + TextMsgLengthLimit);
                return;
            }

            var remoteAPI = GetRemoteAPI();
            remoteAPI.SendTextMsg(content);
            Hide();
        }

        public void ChatShieldValueChanged()
        {
            _shieldChat.Write(ChatShieldToggle.isOn, Time.time);
            FreeChatOn.SetActive(ChatShieldToggle.isOn);
            FreeChatOff.SetActive(!ChatShieldToggle.isOn);
        }

        private void Toast(string content, bool error = false)
        {
            var dialogManager = GetDialogManager();
            dialogManager.ShowToast(content, 2, error);
        }
    }
}