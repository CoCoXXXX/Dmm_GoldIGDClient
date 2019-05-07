using Dmm.PokerLogic;
using UnityEngine;

namespace Dmm.Sound
{
    public interface ISoundController
    {
        void PlayChatBubbleSound();
        void PlayCameraShotSound();
        void PlayPleaseChuPaiSound();

        void PlayHurrySound();
        void StartHurryTick();
        void StopHurryTick();

        void PlayChuPaiSound(PokerPattern pattern, int sex);
        void PlayFaPaiSound();
        void PlayChuPaiSound();
        void PlayBuChuSound();
        void PlayEndRoundLoseSound();
        void PlayEndRoundWinSound();

        void PlayGetGoldSound();
        void PlayUseGoldSound();
        void PlayGoldDingSound();
        void PlayGiftPackExplodeSound();

        void PlayBgm();
        void StopBgm();
        void SetBgmEnable(bool enable);
        void SetBgmVolume(float volume);
        void SetBgmMute(bool mute);

        void PlayEffect(AudioClip sound);
        void SetEffectEnable(bool enable);
        void SetEffectVolume(float volume);
    }
}