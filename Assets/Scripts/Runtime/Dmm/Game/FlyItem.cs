using DG.Tweening;
using Dmm.Sound;
using UnityEngine;
using Zenject;

namespace Dmm.Game
{
    [RequireComponent(typeof(Animator))]
    public class FlyItem : MonoBehaviour
    {
        #region Inject

        private ISoundController _soundController;

        [Inject]
        public void Initialize(ISoundController soundController)
        {
            _soundController = soundController;
        }

        public class Factory : PrefabFactory<FlyItem>
        {
        }

        #endregion

        private Animator _animator;

        /// <summary>
        /// 向后返回的时间。
        /// </summary>
        public float ThrowBackLength = 1f / 15;

        /// <summary>
        /// 向后返回的时间。
        /// </summary>
        public float ThrowBackTime = 0.3f;

        /// <summary>
        /// 出现之后开始飞行的延迟时间。
        /// </summary>
        public float JumpDelay = 0.5f;

        /// <summary>
        /// 飞行的时候播放的音效。
        /// </summary>
        public AudioClip FlySound;

        /// <summary>
        /// 集中目标的之后播放的音效。
        /// </summary>
        public AudioClip HitSound;

        public void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private Sequence _curTweener;

        public void StartFly(Vector2 fromPos, Vector2 toPos, float fromScale, float toScale)
        {
            if (_curTweener != null)
            {
                // 如有，则先停止之前的动作。
                _curTweener.Kill();
            }

            transform.position = fromPos;
            transform.localScale = new Vector3(fromScale, fromScale, 1);

            var backPos = fromPos + (fromPos - toPos) * ThrowBackLength;

            _curTweener = DOTween.Sequence();
            // 先出现。
            _curTweener.AppendCallback(() => _animator.SetTrigger("Appear"));
            _curTweener.AppendInterval(JumpDelay);
            // 向后退一下。
            _curTweener.Append(transform.DOMove(backPos, ThrowBackTime));
            // 同时执行跳跃和缩放两个动作。
            _curTweener.AppendCallback(() =>
            {
                _animator.SetTrigger("Fly");
                _soundController.PlayEffect(FlySound);
            });
            _curTweener.Append(transform.DOJump(toPos, JumpPower, 1, JumpTime));
            _curTweener.Insert(JumpDelay + ThrowBackTime,
                transform.DOScale(new Vector3(toScale, toScale, 1), JumpTime));
            // 跳跃到目标之后，爆炸。
            _curTweener.AppendCallback(() =>
            {
                _animator.SetTrigger("Blast");
                _soundController.PlayEffect(HitSound);
            });
            _curTweener.AppendInterval(BlastTime);
            _curTweener.OnComplete(() => { _curTweener = null; });
            _curTweener.Play();
        }

        public float JumpPower = 100f;

        public float JumpTime = 0.5f;

        public float BlastTime = 1f;

        /// <summary>
        /// 当前是否在运行中。
        /// </summary>
        public bool IsRunning
        {
            get { return _curTweener != null; }
        }
    }
}