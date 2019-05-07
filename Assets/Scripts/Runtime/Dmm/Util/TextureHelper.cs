using Dmm.Log;
using UnityEngine;

namespace Dmm.Util
{
    public class TextureHelper
    {
        /// <summary>
        /// 截取Rext所有像素点，返回Texture2D
        /// </summary>
        /// <param name="mRect"></param>
        /// <returns></returns>
        public static Texture2D CaptureByRect(Rect mRect)
        {
            //初始化Texture2D  
            Texture2D mTexture = new Texture2D((int) mRect.width, (int) mRect.height, TextureFormat.RGB24, false);
            //读取屏幕像素信息并存储为纹理数据  
            mTexture.ReadPixels(mRect, 0, 0);
            //应用  
            mTexture.Apply();

            //如果需要可以返回截图  
            return mTexture;
        }

        /// <summary>
        /// 将第二张图融合到第一张图
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="startCorX"></param>
        /// <param name="startCorY"></param>
        public static void ComposeTwoTexture(Texture2D first, Texture2D second, int startCorX, int startCorY)
        {
            //读取第二张图片像素
            //混合第一张像素
            //写入
            for (int i = 0; i < second.height; i++)
            {
                for (int j = 0; j < second.width; j++)
                {
                    var color = second.GetPixel(i, j);
                    var colorFirst = first.GetPixel(startCorX + i, startCorY + j);
                    var r = (color.r * color.a) + colorFirst.r * colorFirst.a * (1 - color.a);
                    var g = (color.g * color.a) + colorFirst.g * colorFirst.a * (1 - color.a);
                    var b = (color.b * color.a) + colorFirst.b * colorFirst.a * (1 - color.a);
                    var a = color.a + colorFirst.a;

                    var x = startCorX + i;
                    var y = startCorY + j;
                    if (x > first.width || y > first.height)
                    {
                        MyLog.ErrorWithFrame("TextureHelper", "像素点越界");
                        continue;
                    }

                    first.SetPixel(startCorX + i, startCorY + j, new Color(r, g, b, a));
                }
            }

            first.Apply();
        }
    }
}