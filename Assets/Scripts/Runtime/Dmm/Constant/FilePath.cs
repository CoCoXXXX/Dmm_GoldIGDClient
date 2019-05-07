using UnityEngine;

namespace Dmm.Constant
{
    public class FilePath
    {
        public static string PicPath()
        {
            return Application.persistentDataPath + "/img/";
        }

        public static string BinaryFilePath()
        {
            return Application.persistentDataPath + "/file/";
        }

        public static string TextPath()
        {
            return Application.persistentDataPath + "/text/";
        }
    }
}