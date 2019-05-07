using UnityEngine;

namespace Dmm.Widget
{
    public class RotateTransform : MonoBehaviour
    {
        /// <summary>
        /// 1秒钟旋转多少度。
        /// </summary>
        public float RotateSpeed = 360;

        /// <summary>
        /// 是否在运行中。
        /// </summary>
        public bool Running = false;

        private void Update()
        {
            if (!Running)
            {
                return;
            }

            var r = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, 0, r.z - RotateSpeed * Time.deltaTime);
        }
    }
}