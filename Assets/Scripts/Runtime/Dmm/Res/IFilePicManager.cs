using UnityEngine;

namespace Dmm.Res
{
    public interface IFilePicManager
    {
        Sprite GetSprite(string picName);
        bool ContainsSprite(string picName);

        Texture2D CreateTextureFromFile(string path);
        string SavePic(byte[] bytes, string picName);
    }
}