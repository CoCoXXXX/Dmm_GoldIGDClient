using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Game
{
    public class TableWatch : MonoBehaviour
    {
        public Text CurrentTime;

        private void Update()
        {
            var date = DateTime.Now;
            var div = date.Second % 2 == 0 ? ":" : " ";
            CurrentTime.text = string.Format("{0:00}{1}{2:00}", date.Hour, div, date.Minute);
        }
    }
}