using Dmm.Util;

namespace Dmm.Chat
{
    public class Emoji
    {
        public static readonly string[] Values =
        {
            "[亲亲]",
            "[呲牙]",
            "[坏笑]",
            "[怒火]",
            "[流汗]",
            "[流泪]",
            "[炸弹]",
            "[窘迫]",
            "[贪财]",
            "[邪恶]"
        };

        public static bool IsEmoji(string content)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (StringUtil.AreEqual(content, Values[i]))
                    return true;
            }

            return false;
        }
    }
}