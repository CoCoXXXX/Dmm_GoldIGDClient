namespace Dmm.Constant
{
    public class Platform
    {
        public const int Ios = 1;

        public const int Android = 2;

        public const int Pc = 3;

        public const int Mac = 4;

        public const int Web = 5;

        public const int Editor = 100;

        public const int Default = 0;

        public static int[] Values =
        {
            Ios,
            Android,
            Pc,
            Mac,
            Web,
            Editor,
            Default
        };

        public static string LabelOf(int platform)
        {
            switch (platform)
            {
                case Ios:
                    return "Ios";

                case Android:
                    return "Android";

                case Pc:
                    return "Pc";

                case Mac:
                    return "Mac";

                case Web:
                    return "Web";

                case Editor:
                    return "Editor";

                default:
                    return "Default";
            }
        }
    }
}