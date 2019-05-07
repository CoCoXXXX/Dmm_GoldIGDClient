using UnityEngine;

namespace Dmm.Base
{
    [RequireComponent(typeof(Camera))]
    public class MainCamera : MonoBehaviour
    {
        public static Camera Current { get; private set; }

        public void Awake()
        {
            Current = GetComponent<Camera>();
        }
    }
}