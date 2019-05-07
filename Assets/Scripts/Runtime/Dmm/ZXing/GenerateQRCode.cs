using UnityEngine;
using ZXing;
using ZXing.QrCode;

namespace Dmm.ZXing
{
    public class GenerateQRCode
    {
        public static Sprite GenerateQRCodeSpriteFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            var encoded = new Texture2D(256, 256, TextureFormat.ARGB32, false);
            var color32 = Encode(url, encoded.width, encoded.height);
            encoded.SetPixels32(color32);
            encoded.Apply();
            var sprite = Sprite.Create(
                encoded,
                new Rect(0, 0, encoded.width, encoded.height),
                Vector2.zero, 1, 0, SpriteMeshType.FullRect);

            return sprite;
        }

        public static Texture2D GenerateQRCodeTexture2DFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            var encoded = new Texture2D(256, 256, TextureFormat.ARGB32, false);
            var color32 = Encode(url, encoded.width, encoded.height);
            encoded.SetPixels32(color32);
            encoded.Apply();

            return encoded;
        }

        private static Color32[] Encode(string textForEncoding, int width, int height)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width
                }
            };
            return writer.Write(textForEncoding);
        }
    }
}