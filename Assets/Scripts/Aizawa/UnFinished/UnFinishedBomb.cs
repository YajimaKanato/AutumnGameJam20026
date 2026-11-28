using DG.Tweening;
using Kato;
using UnityEngine;
using Yajima;

namespace Aizawa
{
    public class UnFinishedBomb : Unfinished
    {
        [SerializeField] GameObject _bombVisual;
        [SerializeField] SpriteRenderer _front;
        [SerializeField] SpriteRenderer _back;
        [SerializeField] SpriteRenderer _debris;
        [SerializeField] Sprite[] _cutSprites;
        [SerializeField] Sprite[] _explosionFrames;
        [SerializeField, Min(1f)] float _explosionFramesPerSecond = 24f;

        Sprite _intactFront;
        Sprite _intactBack;
        Tween _effect;

        void Awake()
        {
            _intactFront = _front.sprite;
            _intactBack = _back.sprite;
        }

        public override void Entry()
        {
            _front.sprite = _intactFront;
            _back.sprite = _intactBack;
            _bombVisual.SetActive(true);
            _debris.gameObject.SetActive(false);
            base.Entry();
        }

        public override void InteractExit(Player player)
        {
            if (IsStopped || !_entered || _interacted) return;
            _entered = false;

            var turn = Turn(player);
            if (turn == null) PlayOutcome();
            else turn.OnComplete(PlayOutcome);
        }

        void PlayOutcome()
        {
            if (IsStopped || !isActiveAndEnabled) return;

            bool exploded = Random.Range(0, 3) == 1;
            AudioManager.instance?.Play(exploded ? "BombMissSE" : "BombCutSE");

            if (exploded && _explosionFrames is { Length: > 0 })
            {
                _bombVisual.SetActive(false);
                _debris.sprite = _explosionFrames[0];
                _debris.gameObject.SetActive(true);
                _effect = DOVirtual.Float(0f, _explosionFrames.Length,
                        _explosionFrames.Length / _explosionFramesPerSecond,
                        frame => _debris.sprite = _explosionFrames[Mathf.Min((int)frame, _explosionFrames.Length - 1)])
                    .SetEase(Ease.Linear).OnComplete(FinishInteraction);
                return;
            }

            if (!exploded && _cutSprites is { Length: > 0 })
            {
                var cutSprite = _cutSprites[Random.Range(0, _cutSprites.Length)];
                _front.sprite = cutSprite;
                _back.sprite = cutSprite;
            }

            _effect = DOVirtual.DelayedCall(0.3f, FinishInteraction, ignoreTimeScale: false);
        }

        void FinishInteraction()
        {
            _effect = null;
            NotifyInteraction();
            if (isActiveAndEnabled) Exit();
        }

        protected override void OnPaused() => _effect?.Pause();
        protected override void OnResumed() => _effect?.Play();
        protected override void OnFinished() => _effect?.Kill();

        protected override void OnDisable()
        {
            _effect?.Kill();
            _effect = null;
            base.OnDisable();
        }
    }
}
