using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.DataRelation;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Widget
{
    public class NicknameGroup : MonoBehaviour
    {
        #region Inject

        private SpriteHolder _spriteHolder;

        private IDataContainer<FeatureSwitch> _featureSwitch;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            SpriteHolder spriteHolder)
        {
            _spriteHolder = spriteHolder;
            _featureSwitch = dataRepository.GetContainer<FeatureSwitch>(DataKey.FeatureSwitch);
        }

        #endregion

        public Image VipIcon;
        public Text NicknameText;

        public void SetData(User user)
        {
            if (user == null)
            {
                Clear();
                return;
            }

            if (VipIcon)
            {
                var sprite = _spriteHolder.VipIcon(user.vip);
                if (sprite)
                {
                    if (!VipIcon.gameObject.activeSelf)
                        VipIcon.gameObject.SetActive(true);

                    VipIcon.sprite = sprite;
                }
                else
                {
                    if (VipIcon.gameObject.activeSelf)
                        VipIcon.gameObject.SetActive(false);
                }

#if UNITY_IOS
                var featureSwitch = _featureSwitch.Read();
                if (!(featureSwitch != null && featureSwitch.vip))
                {
                    if (VipIcon.gameObject.activeSelf)
                        VipIcon.gameObject.SetActive(false);
                }
#endif
            }

            if (NicknameText)
            {
                if (!NicknameText.gameObject.activeSelf)
                    NicknameText.gameObject.SetActive(true);

                NicknameText.text = user.nickname;
            }
        }

        public void SetData(string nickname, int vip)
        {
            if (VipIcon)
            {
                var sprite = _spriteHolder.VipIcon(vip);
                if (sprite)
                {
                    if (!VipIcon.gameObject.activeSelf)
                        VipIcon.gameObject.SetActive(true);

                    VipIcon.sprite = sprite;
                }
                else
                {
                    if (VipIcon.gameObject.activeSelf)
                        VipIcon.gameObject.SetActive(false);
                }

#if UNITY_IOS
                var featureSwitch = _featureSwitch.Read();
                if (!(featureSwitch != null && featureSwitch.vip))
                {
                    if (VipIcon.gameObject.activeSelf)
                        VipIcon.gameObject.SetActive(false);
                }
#endif
            }

            if (NicknameText)
            {
                if (!NicknameText.gameObject.activeSelf)
                    NicknameText.gameObject.SetActive(true);

                NicknameText.text = nickname;
            }
        }

        public void Clear()
        {
            if (VipIcon)
            {
                VipIcon.sprite = null;
                if (VipIcon.gameObject.activeSelf)
                    VipIcon.gameObject.SetActive(false);
            }

            if (NicknameText && NicknameText.gameObject.activeSelf)
                NicknameText.gameObject.SetActive(false);
        }
    }
}