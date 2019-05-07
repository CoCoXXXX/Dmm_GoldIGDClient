using Dmm.Sound;
using UnityEngine;
using Zenject;

namespace Dmm.Common
{
    public class AwardMoneyObject : MonoBehaviour
    {
        #region Inject

        private ISoundController _soundController;

        [Inject]
        public void Initialize(ISoundController soundController)
        {
            _soundController = soundController;
        }

        public class Factory : PrefabFactory<AwardMoneyObject>
        {
        }

        #endregion

        public void OnCollisionEnter2D(Collision2D col)
        {
            if (col.relativeVelocity.magnitude > 200)
                _soundController.PlayGoldDingSound();
        }
    }
}