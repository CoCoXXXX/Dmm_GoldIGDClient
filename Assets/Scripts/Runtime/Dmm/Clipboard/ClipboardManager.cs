using System.Runtime.InteropServices;
using Dmm.Sdk;
using UnityEngine;
using Zenject;

namespace Dmm.Clipboard
{
    public class ClipboardManager : MonoBehaviour, IClipboardManager
    {
        #region Inject

        private AndroidSDK _android;

        private IosSDK _ios;

        [Inject]
        public void Initialize(
            AndroidSDK android,
            IosSDK ios)
        {
            _android = android;
            _ios = ios;
        }

        #endregion

        public void CopyToClipboard(string input)
        {
#if UNITY_ANDROID
            _android.CopyToClipboard(input);
#elif UNITY_IPHONE
            _ios.CopyToClipboard(input);
#elif UNITY_EDITOR
            TextEditor t = new TextEditor();
            t.content = new GUIContent(input);
            t.OnFocus();
            t.Copy();
#endif
        }
    }
}