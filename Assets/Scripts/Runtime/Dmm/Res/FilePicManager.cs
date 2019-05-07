using System;
using System.IO;
using Dmm.Constant;
using Dmm.Log;
using UnityEngine;

namespace Dmm.Res
{
    /// <summary>
    /// 统一处理所有的图片文件管理。
    /// 同时缓存一些本地的图片。
    /// </summary>
    public class FilePicManager : IFilePicManager
    {
        #region GetSprite

        public Sprite GetSprite(string picName)
        {
            if (string.IsNullOrEmpty(picName))
                return null;

            var texture = CreateTextureFromFile(FilePath.PicPath() + picName);
            if (texture)
            {
                // 默认情况下会使用SpriteMeshType.Tight模式。
                // 这种模式在创建Sprite的时候，因为要侦测图片的形状，会非常的耗时。
                var sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    Vector2.zero, 1, 0, SpriteMeshType.FullRect);

                return sprite;
            }

            return null;
        }

        public bool ContainsSprite(string picName)
        {
            if (string.IsNullOrEmpty(picName))
                return false;

            if (File.Exists(FilePath.PicPath() + picName))
                return true;

            return false;
        }

        #endregion

        #region Create & Save

        public Texture2D CreateTextureFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            try
            {
                var bytes = File.ReadAllBytes(path);
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Bilinear;
                texture.LoadImage(bytes);
                return texture;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string SavePic(byte[] bytes, string picName)
        {
            if (bytes == null || string.IsNullOrEmpty(picName))
                return null;

            if (!Directory.Exists(FilePath.PicPath()))
            {
                Directory.CreateDirectory(FilePath.PicPath());
            }

            try
            {
                File.WriteAllBytes(FilePath.PicPath() + picName, bytes);
                return FilePath.PicPath() + picName;
            }
            catch (Exception e)
            {
                MyLog.ErrorWithFrame("FilePicManager",
                    string.Format("Save file {0} failed, error: {1}", picName, e.Message));

                return null;
            }
        }

        #endregion
    }
}