namespace Dmm.Res
{
    public class DownloadAssetBundleInfo
    {
        public enum DownloadType
        {
            FirstBuild = 1,

            Download
        }

        public int TotalCount;

        public int CompleteCount;

        public float DownloadProgress;

        public DownloadType LoadType;
    }
}