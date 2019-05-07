using System.Collections.Generic;
using DG.Tweening;
using Dmm.Constant;
using Dmm.PokerLogic;
using Dmm.Util;
using UnityEngine;

namespace Dmm.Sound
{
    public class SoundController : MonoBehaviour, ISoundController
    {
        public void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            #region 播放器

            if (BgmPlayer)
            {
                var bgmEnable = PrefsUtil.GetBool(PrefsKeys.BgmEnable, true);
                if (bgmEnable)
                    PlayBgm();
                else
                    StopBgm();

                var bgmVolume = Mathf.Clamp01(PrefsUtil.GetFloat(PrefsKeys.BgmVolumeKey, 1));
                BgmPlayer.volume = bgmVolume;
            }

            if (EffectPlayer)
            {
                var effectEnable = PrefsUtil.GetBool(PrefsKeys.EffectEnable, true);
                EffectPlayer.mute = !effectEnable;

                var effectVolume = Mathf.Clamp01(PrefsUtil.GetFloat(PrefsKeys.EffectVolumeKey, 1));
                EffectPlayer.volume = effectVolume;
            }

            #endregion

            #region 初始化女声

            _femaleASounds.Clear();

            if (Female2)
                _femaleASounds.Add(PokerNumType.P2, Female2);
            if (Female3)
                _femaleASounds.Add(PokerNumType.P3, Female3);
            if (Female4)
                _femaleASounds.Add(PokerNumType.P4, Female4);
            if (Female5)
                _femaleASounds.Add(PokerNumType.P5, Female5);
            if (Female6)
                _femaleASounds.Add(PokerNumType.P6, Female6);
            if (Female7)
                _femaleASounds.Add(PokerNumType.P7, Female7);
            if (Female8)
                _femaleASounds.Add(PokerNumType.P8, Female8);
            if (Female9)
                _femaleASounds.Add(PokerNumType.P9, Female9);
            if (Female10)
                _femaleASounds.Add(PokerNumType.P10, Female10);
            if (FemaleJ)
                _femaleASounds.Add(PokerNumType.PJ, FemaleJ);
            if (FemaleQ)
                _femaleASounds.Add(PokerNumType.PQ, FemaleQ);
            if (FemaleK)
                _femaleASounds.Add(PokerNumType.PK, FemaleK);
            if (FemaleA)
                _femaleASounds.Add(PokerNumType.PA, FemaleA);
            if (FemaleWX)
                _femaleASounds.Add(PokerNumType.PX, FemaleWX);
            if (FemaleWD)
                _femaleASounds.Add(PokerNumType.PD, FemaleWD);

            _femalePatternSounds.Clear();

            if (FemaleBuChu)
                _femalePatternSounds.Add(PatternType.BUCHU, FemaleBuChu);
            if (FemaleDuiZi)
                _femalePatternSounds.Add(PatternType.AA, FemaleDuiZi);
            if (FemaleSanDaiEr)
                _femalePatternSounds.Add(PatternType.AAAXX, FemaleSanDaiEr);
            if (FemaleSanLianDui)
                _femalePatternSounds.Add(PatternType.AABBCC, FemaleSanLianDui);
            if (FemaleSanZhang)
                _femalePatternSounds.Add(PatternType.AAA, FemaleSanZhang);
            if (FemaleShunZi)
                _femalePatternSounds.Add(PatternType.ABCDE, FemaleShunZi);
            if (FemaleFeiJi)
                _femalePatternSounds.Add(PatternType.AAABBB, FemaleFeiJi);
            if (FemaleZhaDan)
                _femalePatternSounds.Add(PatternType.XXXX, FemaleZhaDan);
            if (FemaleTongHuaShun)
                _femalePatternSounds.Add(PatternType.SuperABCDE, FemaleTongHuaShun);
            if (FemaleWangZha)
                _femalePatternSounds.Add(PatternType.XXDD, FemaleWangZha);

            #endregion

            #region 初始化男声

            _maleASounds.Clear();

            if (Male2)
                _maleASounds.Add(PokerNumType.P2, Male2);
            if (Male3)
                _maleASounds.Add(PokerNumType.P3, Male3);
            if (Male4)
                _maleASounds.Add(PokerNumType.P4, Male4);
            if (Male5)
                _maleASounds.Add(PokerNumType.P5, Male5);
            if (Male6)
                _maleASounds.Add(PokerNumType.P6, Male6);
            if (Male7)
                _maleASounds.Add(PokerNumType.P7, Male7);
            if (Male8)
                _maleASounds.Add(PokerNumType.P8, Male8);
            if (Male9)
                _maleASounds.Add(PokerNumType.P9, Male9);
            if (Male10)
                _maleASounds.Add(PokerNumType.P10, Male10);
            if (MaleJ)
                _maleASounds.Add(PokerNumType.PJ, MaleJ);
            if (MaleQ)
                _maleASounds.Add(PokerNumType.PQ, MaleQ);
            if (MaleK)
                _maleASounds.Add(PokerNumType.PK, MaleK);
            if (MaleA)
                _maleASounds.Add(PokerNumType.PA, MaleA);
            if (MaleWX)
                _maleASounds.Add(PokerNumType.PX, MaleWX);
            if (MaleWD)
                _maleASounds.Add(PokerNumType.PD, MaleWD);

            _malePatternSounds.Clear();

            if (MaleBuChu)
                _malePatternSounds.Add(PatternType.BUCHU, MaleBuChu);
            if (MaleDuiZi)
                _malePatternSounds.Add(PatternType.AA, MaleDuiZi);
            if (MaleSanDaiEr)
                _malePatternSounds.Add(PatternType.AAAXX, MaleSanDaiEr);
            if (MaleSanLianDui)
                _malePatternSounds.Add(PatternType.AABBCC, MaleSanLianDui);
            if (MaleSanZhang)
                _malePatternSounds.Add(PatternType.AAA, MaleSanZhang);
            if (MaleShunZi)
                _malePatternSounds.Add(PatternType.ABCDE, MaleShunZi);
            if (MaleFeiJi)
                _malePatternSounds.Add(PatternType.AAABBB, MaleFeiJi);
            if (MaleZhaDan)
                _malePatternSounds.Add(PatternType.XXXX, MaleZhaDan);
            if (MaleTongHuaShun)
                _malePatternSounds.Add(PatternType.SuperABCDE, MaleTongHuaShun);
            if (MaleWangZha)
                _malePatternSounds.Add(PatternType.XXDD, MaleWangZha);

            #endregion

            #region 特殊牌型音效

            _specPatternSounds.Clear();

            if (Airplane)
                _specPatternSounds.Add(PatternType.AAABBB, Airplane);
            if (HuoJian)
                _specPatternSounds.Add(PatternType.XXDD, HuoJian);
            if (TongHuaShun)
                _specPatternSounds.Add(PatternType.SuperABCDE, TongHuaShun);

            #endregion
        }

        #region 背景音乐

        public AudioSource BgmPlayer;

        public AudioClip Bgm;

        public float FadeTime = 1f;

        private Tweener _bgmTweener;

        public void SetBgmMute(bool mute)
        {
            if (_bgmTweener != null)
            {
                _bgmTweener.Kill();
                _bgmTweener = null;
            }

            if (BgmPlayer)
            {
                if (mute)
                {
                    _bgmTweener = BgmPlayer.DOFade(0f, FadeTime).SetEase(Ease.Linear);
                }
                else
                {
                    var volume = Mathf.Clamp01(PrefsUtil.GetFloat(PrefsKeys.BgmVolumeKey, 1));
                    _bgmTweener = BgmPlayer.DOFade(volume, FadeTime).SetEase(Ease.Linear);
                }
            }
        }

        public void SetBgmVolume(float volume)
        {
            var vol = Mathf.Clamp01(volume);
            PrefsUtil.SetFloat(PrefsKeys.BgmVolumeKey, vol);
            PrefsUtil.Flush();

            if (BgmPlayer)
                BgmPlayer.volume = vol;
        }

        public void SetBgmEnable(bool enable)
        {
            PrefsUtil.SetBool(PrefsKeys.BgmEnable, enable);
            PrefsUtil.Flush();

            if (BgmPlayer)
            {
                if (enable) PlayBgm();
                else StopBgm();
            }
        }

        /// <summary>
        /// 播放背景音乐。
        /// </summary>
        public void PlayBgm()
        {
            if (BgmPlayer)
            {
                if (BgmPlayer.isPlaying)
                    BgmPlayer.Stop();

                var volume = Mathf.Clamp01(PrefsUtil.GetFloat(PrefsKeys.BgmVolumeKey, 1));
                BgmPlayer.volume = volume;
                BgmPlayer.loop = true;
                BgmPlayer.mute = false;

                if (Bgm)
                {
                    BgmPlayer.clip = Bgm;
                    BgmPlayer.Play();
                }
            }
        }

        /// <summary>
        /// 停止背景音乐。
        /// </summary>
        public void StopBgm()
        {
            if (BgmPlayer && BgmPlayer.isPlaying)
                BgmPlayer.Stop();
        }

        #endregion

        #region 音效

        public AudioSource EffectPlayer;

        public void SetEffectVolume(float volume)
        {
            var vol = Mathf.Clamp01(volume);
            PrefsUtil.SetFloat(PrefsKeys.EffectVolumeKey, vol);
            PrefsUtil.Flush();

            if (EffectPlayer)
                EffectPlayer.volume = vol;
        }

        public void SetEffectEnable(bool enable)
        {
            PrefsUtil.SetInt(PrefsKeys.EffectEnable, enable ? 1 : 0);
            PrefsUtil.Flush();

            if (EffectPlayer)
                EffectPlayer.mute = !enable;
        }

        public AudioClip EndRoundWin;

        public void PlayEndRoundWinSound()
        {
            if (EndRoundWin) PlayEffect(EndRoundWin);
        }

        public AudioClip EndRoundLose;

        public void PlayEndRoundLoseSound()
        {
            if (EndRoundLose) PlayEffect(EndRoundLose);
        }

        public AudioClip BuChu;

        /// <summary>
        /// 播放玩家不出的音效。
        /// </summary>
        public void PlayBuChuSound()
        {
            if (BuChu) PlayEffect(BuChu);
        }

        public AudioClip ChuPai;

        /// <summary>
        /// 播放玩家出牌的音效。
        /// </summary>
        public void PlayChuPaiSound()
        {
            if (ChuPai) PlayEffect(ChuPai);
        }

        public AudioClip FaPai;

        public void PlayFaPaiSound()
        {
            if (FaPai) PlayEffect(FaPai);
        }

        public AudioClip HurryChuPaiTick;

        public void PlayHurrySound()
        {
            if (HurryChuPaiTick) PlayEffect(HurryChuPaiTick);
        }

        public void StartHurryTick()
        {
            if (HurryChuPaiTick && EffectPlayer)
            {
                EffectPlayer.loop = true;
                EffectPlayer.clip = HurryChuPaiTick;
                EffectPlayer.Play();
            }
        }

        public void StopHurryTick()
        {
            if (EffectPlayer && EffectPlayer.isPlaying)
                EffectPlayer.Stop();
        }

        public AudioClip PleaseChuPai;

        public void PlayPleaseChuPaiSound()
        {
            if (PleaseChuPai) PlayEffect(PleaseChuPai);
        }

        public AudioClip CameraShot;

        public void PlayCameraShotSound()
        {
            if (CameraShot) PlayEffect(CameraShot);
        }

        public AudioClip ChatBubble;

        public void PlayChatBubbleSound()
        {
            if (ChatBubble) PlayEffect(ChatBubble);
        }

        public AudioClip GiftPackExplode;

        public void PlayGiftPackExplodeSound()
        {
            if (GiftPackExplode) PlayEffect(GiftPackExplode);
        }

        public AudioClip GetGold;

        public void PlayGetGoldSound()
        {
            if (GetGold) PlayEffect(GetGold);
        }

        public AudioClip UseGold;

        public void PlayUseGoldSound()
        {
            if (UseGold) PlayEffect(UseGold);
        }

        public AudioClip GoldDing;

        public void PlayGoldDingSound()
        {
            if (GoldDing) PlayEffect(GoldDing);
        }

        private float _lastPlayTime;

        public float EffectInterval = 0.01f;

        public void PlayEffect(AudioClip sound)
        {
            if (!sound || !EffectPlayer) return;

            if (Time.time - _lastPlayTime > EffectInterval)
            {
                _lastPlayTime = Time.time;
                EffectPlayer.PlayOneShot(sound);
            }
        }

        #endregion

        #region 出牌

        #region 女性出牌声

        public AudioClip Female2;

        public AudioClip Female3;

        public AudioClip Female4;

        public AudioClip Female5;

        public AudioClip Female6;

        public AudioClip Female7;

        public AudioClip Female8;

        public AudioClip Female9;

        public AudioClip Female10;

        public AudioClip FemaleJ;

        public AudioClip FemaleQ;

        public AudioClip FemaleK;

        public AudioClip FemaleA;

        public AudioClip FemaleWX;

        public AudioClip FemaleWD;

        public AudioClip FemaleBuChu;

        public AudioClip FemaleDuiZi;

        public AudioClip FemaleSanDaiEr;

        public AudioClip FemaleSanLianDui;

        public AudioClip FemaleSanZhang;

        public AudioClip FemaleShunZi;

        public AudioClip FemaleFeiJi;

        public AudioClip FemaleZhaDan;

        public AudioClip FemaleTongHuaShun;

        public AudioClip FemaleWangZha;

        /// <summary>
        /// 单张的女声缓存。
        /// </summary>
        private readonly Dictionary<int, AudioClip> _femaleASounds = new Dictionary<int, AudioClip>();

        /// <summary>
        /// BUCHU、AA、AAA、AAAXX、AABBCC、ABCDE
        /// 女声缓存。
        /// </summary>
        private readonly Dictionary<int, AudioClip> _femalePatternSounds = new Dictionary<int, AudioClip>();

        #endregion

        #region 男性出牌声

        public AudioClip Male2;

        public AudioClip Male3;

        public AudioClip Male4;

        public AudioClip Male5;

        public AudioClip Male6;

        public AudioClip Male7;

        public AudioClip Male8;

        public AudioClip Male9;

        public AudioClip Male10;

        public AudioClip MaleJ;

        public AudioClip MaleQ;

        public AudioClip MaleK;

        public AudioClip MaleA;

        public AudioClip MaleWX;

        public AudioClip MaleWD;

        public AudioClip MaleBuChu;

        public AudioClip MaleDuiZi;

        public AudioClip MaleSanDaiEr;

        public AudioClip MaleSanLianDui;

        public AudioClip MaleSanZhang;

        public AudioClip MaleShunZi;

        public AudioClip MaleFeiJi;

        public AudioClip MaleZhaDan;

        public AudioClip MaleTongHuaShun;

        public AudioClip MaleWangZha;

        /// <summary>
        /// 单张的男声缓存
        /// </summary>
        private readonly Dictionary<int, AudioClip> _maleASounds = new Dictionary<int, AudioClip>();

        /// <summary>
        /// BUCHU、AA、AAA、AAAXX、AABBCC、ABCDE
        /// 男声缓存。
        /// </summary>
        private readonly Dictionary<int, AudioClip> _malePatternSounds = new Dictionary<int, AudioClip>();

        #endregion

        #region 特殊牌型音效

        public AudioClip Airplane;

        public AudioClip HuoJian;

        public AudioClip TongHuaShun;

        /// <summary>
        /// AAABBB、XXXX、XXDD、SuperABCDE
        /// </summary>
        private readonly Dictionary<int, AudioClip> _specPatternSounds = new Dictionary<int, AudioClip>();

        #endregion

        public AudioClip BombSmall;

        public AudioClip BombBig;

        #region 炸弹

        #endregion

        /// <summary>
        /// 播放出牌的音效。
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="sex"></param>
        public void PlayChuPaiSound(PokerPattern pattern, int sex)
        {
            if (pattern == null)
                return;

            AudioClip sound = null;
            AudioClip effectSound = null;

            switch (pattern.Type)
            {
                case PatternType.XXXX:
                case PatternType.AAABBB:
                case PatternType.XXDD:
                case PatternType.SuperABCDE:
                case PatternType.BUCHU:
                case PatternType.AA:
                case PatternType.AAAXX:
                case PatternType.AABBCC:
                case PatternType.AAA:
                case PatternType.ABCDE:
                    if (sex == 0)
                        _femalePatternSounds.TryGetValue(pattern.Type, out sound);
                    else
                        _malePatternSounds.TryGetValue(pattern.Type, out sound);
                    break;

                case PatternType.A:
                    if (sex == 0)
                        _femaleASounds.TryGetValue(pattern.MajorNumType, out sound);
                    else
                        _maleASounds.TryGetValue(pattern.MajorNumType, out sound);
                    break;
            }

            switch (pattern.Type)
            {
                case PatternType.XXXX:
                    if (PatternType.IsBigXXXX(pattern))
                        effectSound = BombBig;
                    else
                        effectSound = BombSmall;
                    break;

                case PatternType.AAABBB:
                case PatternType.XXDD:
                case PatternType.SuperABCDE:
                    _specPatternSounds.TryGetValue(pattern.Type, out effectSound);
                    break;
            }

            if (sound && EffectPlayer)
                EffectPlayer.PlayOneShot(sound);

            if (effectSound && EffectPlayer)
                EffectPlayer.PlayOneShot(effectSound);
        }

        #endregion
    }
}