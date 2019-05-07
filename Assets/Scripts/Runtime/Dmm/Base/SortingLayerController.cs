using UnityEngine;

namespace Dmm.Base
{
    [RequireComponent(typeof(Renderer))]
    public class SortingLayerController : MonoBehaviour
    {
        public string SortingLayer
        {
            get { return GetComponent<Renderer>().sortingLayerName; }
            set { GetComponent<Renderer>().sortingLayerName = value; }
        }

        public int SortingOrder
        {
            get { return GetComponent<Renderer>().sortingOrder; }
            set { GetComponent<Renderer>().sortingOrder = value; }
        }
    }
}