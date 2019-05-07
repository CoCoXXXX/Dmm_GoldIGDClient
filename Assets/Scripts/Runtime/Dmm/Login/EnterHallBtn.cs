using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Login
{
    public class EnterHallBtn : MonoBehaviour
    {
        public Button Button;

        public GameObject Light1;

        public GameObject Light2;

        public bool Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;

                if (Button) Button.interactable = value;

                if (Light1 && Light1.activeSelf != value)
                    Light1.SetActive(value);

                if (Light2 && Light2.activeSelf != value)
                    Light2.SetActive(value);
            }
        }

        private bool _enable;
    }
}