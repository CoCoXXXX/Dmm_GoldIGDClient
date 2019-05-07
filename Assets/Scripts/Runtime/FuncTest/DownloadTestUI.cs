using System;
using System.Collections;
using System.IO;
using Dmm.Base;
using Dmm.Log;
using UnityEngine;
using UnityEngine.UI;

namespace FuncTest
{
    public class DownloadTestUI : MonoBehaviour
    {
        public InputField UrlEdt;

        public Text Text;

        public Image Image;

        public void Start()
        {
            UrlEdt.text = "http://iguandan.qiniudn.com/2014springbg.png";
        }

        public void StartDownload()
        {
            StartCoroutine(DownloadCoroutine());
        }

        public string Path
        {
            get { return Application.persistentDataPath + "/test.png"; }
        }

        private IEnumerator DownloadCoroutine()
        {
            var url = UrlEdt.text;
            MyLog.InfoWithFrame(name, "new www.");

            WWW www = null;
            try
            {
                www = new WWW(url);
            }
            catch (Exception e)
            {
                MyLog.InfoWithFrame(name, "exception: " + e.Message);
            }

            if (www != null)
            {
                MyLog.InfoWithFrame(name, "after new www");

                while (!www.isDone)
                {
                    Text.text = www.progress + "";
                    MyLog.InfoWithFrame(name, "progress: " + www.progress);
                    yield return null;
                }

                MyLog.InfoWithFrame(name, "finish download.");

                try
                {
                    var res = www.texture.EncodeToPNG();
                    File.WriteAllBytes(Path, res);
                    www.Dispose();
                }
                catch (Exception e)
                {
                    MyLog.InfoWithFrame(name, "exception: " + e.Message);
                }
            }
        }

        public void ShowImage()
        {
            var res = File.ReadAllBytes(Path);

            var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);
            texture.LoadImage(res);

            Image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                Vector2.zero, 1);
            Image.SetNativeSize();
        }
    }
}