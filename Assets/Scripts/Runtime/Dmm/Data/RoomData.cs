namespace Dmm.Data
{
    /// <summary>
    /// 房间的数据对象。
    /// </summary>
    public class RoomData
    {
        /// <summary>
        /// 房间的ID。
        /// </summary>
        public int RoomId;

        /// <summary>
        /// 房间的类型。
        /// </summary>
        public int Type;

        /// <summary>
        /// 当前房间中的玩家人数。
        /// </summary>
        public int CurrentPlayerNum;

        /// <summary>
        /// 底注。
        /// </summary>
        public int BaseMoney;

        /// <summary>
        /// 房间的图片。
        /// </summary>
        public string RoomPic;

        /// <summary>
        /// Tag0
        /// </summary>
        public string Tag;

        /// <summary>
        /// Tag1
        /// </summary>
        public string Tag1;

        /// <summary>
        /// 附加信息。
        /// </summary>
        public string Extra;

        /// <summary>
        /// 房间打到几。
        /// </summary>
        public int TargetHost;
    }
}