namespace Dmm.Constant
{
    public class LevelTitle
    {
        public static string[] Titles =
        {
            "蜗牛蛋", // 1
            "壁虎蛋", // 2
            "蜂鸟蛋", // 3
            "麻雀蛋", // 4
            "喜鹊蛋", // 5
            "猫头鹰蛋", // 6
            "鹌鹑蛋", // 7
            "鸽子蛋", // 8
            "鸡蛋", // 9
            "鸭蛋", // 10
            "鹅蛋", // 11
            "龟蛋", // 12
            "企鹅蛋", // 13
            "鸵鸟蛋", // 14
            "象鸟蛋", // 15
            "恐龙蛋", // 16
            "哥斯拉蛋", // 17
            "宇宙巨蛋"
        };

        public static string TitleOf(int level)
        {
            if (level < 1 || level > Titles.Length)
                return null;

            return Titles[level - 1];
        }
    }
}